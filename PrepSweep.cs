using System.Text;
using System.Text.RegularExpressions;

namespace OcrTesseract;

/// <summary>
/// Sweeps preprocessing parameters on PaddleOcr V4 to find the combo
/// that maximises detection of all 3 ROIs (UPC, Serial, Part).
/// </summary>
static class PrepSweep
{
    private static readonly string[] DatasetFolders =
    {
        @"C:\Users\gamin\Downloads\Project\Project",
        @"C:\Users\gamin\Downloads\Project\datset2",
    };

    private const string ExpectedSerial = "1421323031448";
    // Fuzzy: accept 1+ zeros (OCR sometimes merges the "0000" run)
    private static readonly Regex _partGtRx = new(@"945-13450-0+-100", RegexOptions.Compiled);

    record PrepConfig(string Name,
        SharpenParams Sharpen,
        ClaheParams Clahe,
        UpscaleParams Upscale,
        BinariseParams Binarise,
        BrightnessContrastParams BrightContrast);

    record RunResult(string ConfigName, int Images, int UpcHits, int SerialHits, int PartHits,
        int AllThreeHits, double AvgMs);

    public static async Task RunAsync()
    {
        // Collect images
        var images = new List<string>();
        foreach (var folder in DatasetFolders)
            if (Directory.Exists(folder))
                images.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.TopDirectoryOnly));
        images.Sort(StringComparer.OrdinalIgnoreCase);

        // Define configs to sweep
        var configs = new List<PrepConfig>
        {
            // Current best
            C("sharp1.8/1.3",       sh(1.8, 1.3)),
            // Higher sharpening
            C("sharp2.0/1.5",       sh(2.0, 1.5)),
            C("sharp2.5/1.5",       sh(2.5, 1.5)),
            C("sharp2.5/2.0",       sh(2.5, 2.0)),
            C("sharp3.0/2.0",       sh(3.0, 2.0)),
            C("sharp1.5/1.0",       sh(1.5, 1.0)),
            // Different CLAHE clip
            C("sharp1.8/1.3+clahe5", sh(1.8, 1.3), clahe: new ClaheParams(true, 5.0, 8)),
            C("sharp1.8/1.3+clahe8", sh(1.8, 1.3), clahe: new ClaheParams(true, 8.0, 8)),
            // Upscale factor
            C("sharp1.8/1.3+up3x",  sh(1.8, 1.3), upscale: new UpscaleParams(true, 3.0)),
            C("sharp2.5/1.5+up3x",  sh(2.5, 1.5), upscale: new UpscaleParams(true, 3.0)),
            // No binarise
            C("sharp1.8/1.3+nobin", sh(1.8, 1.3), binarise: new BinariseParams(false)),
            C("sharp2.5/1.5+nobin", sh(2.5, 1.5), binarise: new BinariseParams(false)),
            // Higher threshold binarise
            C("sharp1.8/1.3+bin128", sh(1.8, 1.3), binarise: new BinariseParams(true, 128)),
            C("sharp1.8/1.3+bin160", sh(1.8, 1.3), binarise: new BinariseParams(true, 160)),
            // Brightness/contrast boost
            C("sharp1.8/1.3+cont20", sh(1.8, 1.3), bc: new BrightnessContrastParams(true, 0, 20)),
            C("sharp1.8/1.3+cont40", sh(1.8, 1.3), bc: new BrightnessContrastParams(true, 0, 40)),
            C("sharp2.5/1.5+cont20", sh(2.5, 1.5), bc: new BrightnessContrastParams(true, 0, 20)),
            // No preprocessing at all
            C("raw(no-prep)", sh(0, 0, false),
              clahe: new ClaheParams(false),
              upscale: new UpscaleParams(false),
              binarise: new BinariseParams(false)),
        };

        // Warm up engine
        PaddleOcrEngine.WarmUpInBackground();
        await Task.Delay(1000);

        var results = new List<RunResult>();

        foreach (var cfg in configs)
        {
            int upc = 0, serial = 0, part = 0, allThree = 0, count = 0;
            long totalMs = 0;

            foreach (var imgPath in images)
            {
                try
                {
                    using var original = new Bitmap(Image.FromFile(imgPath));
                    using var prep = ImagePreprocessor.Process(original,
                        sharpen: cfg.Sharpen,
                        clahe: cfg.Clahe,
                        upscale: cfg.Upscale,
                        binarise: cfg.Binarise,
                        brightnessContrast: cfg.BrightContrast);

                    var r = PaddleOcrEngine.Run(prep);
                    count++;
                    totalMs += r.ElapsedMs;

                    bool u = r.Rois.Any(x => x.FieldName.Equals("UPC", StringComparison.OrdinalIgnoreCase));
                    bool s = r.Rois.Any(x => x.FieldName.Equals("Serial Number", StringComparison.OrdinalIgnoreCase)
                                             && x.Value.Contains(ExpectedSerial));
                    bool p = r.Rois.Any(x => x.FieldName.Equals("Part Number", StringComparison.OrdinalIgnoreCase)
                                             && _partGtRx.IsMatch(x.Value));
                    if (u) upc++;
                    if (s) serial++;
                    if (p) part++;
                    if (u && s && p) allThree++;
                }
                catch { /* skip */ }
            }

            double avgMs = count > 0 ? (double)totalMs / count : 0;
            results.Add(new RunResult(cfg.Name, count, upc, serial, part, allThree, avgMs));
        }

        // Build report
        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
        sb.AppendLine("         PREPROCESSING SWEEP — PaddleOcr V4 English");
        sb.AppendLine($"         {DateTime.Now:yyyy-MM-dd HH:mm:ss}    Images: {images.Count}");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"{"Config",-28} {"N",3} {"AvgMs",6} {"UPC%",7} {"Ser%",7} {"Part%",7} {"All3%",7} {"All3",5}");
        sb.AppendLine(new string('-', 78));

        foreach (var r in results.OrderByDescending(x => x.AllThreeHits)
                                  .ThenByDescending(x => x.UpcHits + x.SerialHits + x.PartHits))
        {
            double uP = r.Images > 0 ? 100.0 * r.UpcHits / r.Images : 0;
            double sP = r.Images > 0 ? 100.0 * r.SerialHits / r.Images : 0;
            double pP = r.Images > 0 ? 100.0 * r.PartHits / r.Images : 0;
            double aP = r.Images > 0 ? 100.0 * r.AllThreeHits / r.Images : 0;
            sb.AppendLine($"{r.ConfigName,-28} {r.Images,3} {r.AvgMs,6:F0} {uP,6:F1}% {sP,6:F1}% {pP,6:F1}% {aP,6:F1}% {r.AllThreeHits,5}");
        }

        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════");

        string outPath = @"D:\ocr_tesserack\prep_sweep.txt";
        await File.WriteAllTextAsync(outPath, sb.ToString());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    static SharpenParams sh(double sigma, double strength, bool enabled = true) =>
        new(enabled, sigma, strength);

    static PrepConfig C(string name, SharpenParams sharpen,
        ClaheParams? clahe = null, UpscaleParams? upscale = null,
        BinariseParams? binarise = null, BrightnessContrastParams? bc = null) =>
        new(name, sharpen,
            clahe    ?? ImagePreprocessor.DefaultClahe,
            upscale  ?? ImagePreprocessor.DefaultUpscale,
            binarise ?? ImagePreprocessor.DefaultBinarise,
            bc       ?? ImagePreprocessor.DefaultBrightnessContrast);
}
