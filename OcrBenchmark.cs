using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Tesseract;

namespace OcrTesseract;

/// <summary>
/// Runs all OCR engines against every .bmp image in the two dataset folders
/// and writes a detailed accuracy/speed report to the Desktop.
/// </summary>
static class OcrBenchmark
{
    private static readonly string[] DatasetFolders =
    {
        @"C:\Users\gamin\Downloads\Project\Project",
        @"C:\Users\gamin\Downloads\Project\datset2",
    };

    private static readonly OcrEngineType[] Engines =
    {
        OcrEngineType.WinRt,
        OcrEngineType.RapidOcr,
        OcrEngineType.PaddleOcr,
        OcrEngineType.Tesseract,
        OcrEngineType.TesseractBest,
        OcrEngineType.PaddleOcrV5Mobile,
        // V5 Server excluded: crashes PaddleInference with SIGSEGV on MKLDNN fallback
    };

    // Ground-truth values
    private const string ExpectedSerial = "1421323031448";
    // Fuzzy: OCR sometimes merges the run of zeros in "0000", accept 1+ zeros
    private static readonly Regex _partGtRx =
        new(@"945-13450-0+-100", RegexOptions.Compiled);

    // ── Result record for one engine × one image ─────────────────────────────
    private record ImageResult(
        string FileName,
        string EngineName,
        long   ElapsedMs,
        bool   UpcDetected,
        bool   SerialDetected,
        bool   PartDetected,
        string? Error);

    // ── Public entry point ───────────────────────────────────────────────────
    public static async Task<string> RunBenchmarkAsync(IProgress<string>? progress = null)
    {
        // Collect all .bmp files
        var images = new List<string>();
        foreach (var folder in DatasetFolders)
        {
            if (Directory.Exists(folder))
                images.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.TopDirectoryOnly));
        }
        images.Sort(StringComparer.OrdinalIgnoreCase);

        if (images.Count == 0)
            throw new InvalidOperationException(
                "No .bmp images found in dataset folders:\n" +
                string.Join("\n", DatasetFolders));

        progress?.Report($"Found {images.Count} images. Starting benchmark...");

        // Ensure engines are warm
        progress?.Report("Warming up engines...");
        PaddleOcrEngine.WarmUpInBackground();
        RapidOcrEngine.WarmUpInBackground();
        PaddleOcrV5Engine.WarmUpMobileInBackground();
        await Task.Delay(500);

        // Tesseract data paths
        string tessDataPath     = Path.Combine(Application.StartupPath, "tessdata");
        string tessFastDataPath = Path.Combine(Application.StartupPath, "tessdata_fast");
        string tessBestDataPath = Path.Combine(Application.StartupPath, "tessdata_best");

        var allResults = new List<ImageResult>();
        int total = images.Count * Engines.Length;
        int done  = 0;

        foreach (var imagePath in images)
        {
            string fileName = Path.GetFileName(imagePath);

            Bitmap? original = null;
            try
            {
                original = new Bitmap(Image.FromFile(imagePath));
            }
            catch (Exception ex)
            {
                foreach (var engine in Engines)
                {
                    allResults.Add(new ImageResult(fileName, engine.ToString(),
                        0, false, false, false, $"Load error: {ex.Message}"));
                    done++;
                }
                progress?.Report($"[{done}/{total}] {fileName} - LOAD ERROR");
                continue;
            }

            foreach (var engineType in Engines)
            {
                done++;
                string engineName = engineType.ToString();
                progress?.Report($"[{done}/{total}] {engineName} on {fileName}");

                ImageResult result;
                try
                {
                    // Preprocess (with custom sharpening: sigma=1.8, strength=1.3)
                    using var preprocessed = ImagePreprocessor.Process(original,
                        sharpen: new SharpenParams(true, Sigma: 1.8, Strength: 1.3));

                    var ocrResult = await RunEngine(engineType, preprocessed,
                        tessDataPath, tessFastDataPath, tessBestDataPath);

                    bool upc    = ocrResult.Rois.Any(r =>
                        r.FieldName.Equals("UPC", StringComparison.OrdinalIgnoreCase));
                    bool serial = ocrResult.Rois.Any(r =>
                        r.FieldName.Equals("Serial Number", StringComparison.OrdinalIgnoreCase) &&
                        r.Value.Contains(ExpectedSerial));
                    bool part   = ocrResult.Rois.Any(r =>
                        r.FieldName.Equals("Part Number", StringComparison.OrdinalIgnoreCase) &&
                        _partGtRx.IsMatch(r.Value));

                    result = new ImageResult(fileName, engineName,
                        ocrResult.ElapsedMs, upc, serial, part, null);
                }
                catch (Exception ex)
                {
                    result = new ImageResult(fileName, engineName,
                        0, false, false, false, ex.Message);
                }

                allResults.Add(result);
            }

            original.Dispose();
        }

        progress?.Report("Generating report...");
        string report = BuildReport(allResults, images.Count);

        string reportPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "benchmark_report.txt");
        await File.WriteAllTextAsync(reportPath, report);

        progress?.Report($"Report saved to {reportPath}");
        return reportPath;
    }

    // ── Engine dispatch ──────────────────────────────────────────────────────
    private static async Task<OcrResult> RunEngine(
        OcrEngineType engineType, Bitmap preprocessed,
        string tessDataPath, string tessFastDataPath, string tessBestDataPath)
    {
        return engineType switch
        {
            OcrEngineType.WinRt => await WinRtOcrEngine.RunAsync(preprocessed),

            OcrEngineType.RapidOcr => await Task.Run(() => RapidOcrEngine.Run(preprocessed)),

            OcrEngineType.PaddleOcr => await Task.Run(() => PaddleOcrEngine.Run(preprocessed)),

            OcrEngineType.Tesseract => await Task.Run(() =>
                RunTesseract(preprocessed, tessDataPath, TessSlot.Standard)),

            OcrEngineType.TesseractFast => await Task.Run(() =>
                RunTesseract(preprocessed, tessFastDataPath, TessSlot.Fast)),

            OcrEngineType.TesseractBest => await Task.Run(() =>
                RunTesseract(preprocessed, tessBestDataPath, TessSlot.Best)),

            OcrEngineType.PaddleOcrV5Mobile => await Task.Run(() =>
                PaddleOcrV5Engine.RunMobile(preprocessed)),

            OcrEngineType.PaddleOcrV5Server => await Task.Run(() =>
                PaddleOcrV5Engine.RunServer(preprocessed)),

            _ => throw new ArgumentOutOfRangeException(nameof(engineType)),
        };
    }

    // ── Tesseract runner ─────────────────────────────────────────────────────
    private enum TessSlot { Standard, Fast, Best }

    private static TesseractEngine? _benchTessStandard;
    private static TesseractEngine? _benchTessFast;
    private static TesseractEngine? _benchTessBest;
    private static readonly object  _benchTessLock = new();

    private static TesseractEngine GetOrCreateBenchTessEngine(string tessDataPath, TessSlot slot)
    {
        lock (_benchTessLock)
        {
            switch (slot)
            {
                case TessSlot.Fast:
                    return _benchTessFast ??= new TesseractEngine(tessDataPath, "eng", EngineMode.LstmOnly);
                case TessSlot.Best:
                    return _benchTessBest ??= new TesseractEngine(tessDataPath, "eng", EngineMode.LstmOnly);
                default:
                    return _benchTessStandard ??= new TesseractEngine(tessDataPath, "eng", EngineMode.LstmOnly);
            }
        }
    }

    private static OcrResult RunTesseract(Bitmap bmp, string tessDataPath, TessSlot slot)
    {
        if (!Directory.Exists(tessDataPath))
            throw new DirectoryNotFoundException($"Tesseract data not found: {tessDataPath}");

        var sw = Stopwatch.StartNew();
        var engine = GetOrCreateBenchTessEngine(tessDataPath, slot);

        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        using var pix  = Pix.LoadFromMemory(ms.ToArray());
        using var page = engine.Process(pix, PageSegMode.Auto);
        float  conf = page.GetMeanConfidence();
        string text = page.GetText().Trim();

        var words = new List<WordEntry>();
        using (var iter = page.GetIterator())
        {
            iter.Begin();
            do
            {
                if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                {
                    string wordText = iter.GetText(PageIteratorLevel.Word)?.Trim() ?? "";
                    float  wordConf = iter.GetConfidence(PageIteratorLevel.Word) / 100f;
                    if (wordText.Length > 0)
                        words.Add(new WordEntry(wordText, wordConf,
                            new Rectangle(rect.X1, rect.Y1,
                                          rect.X2 - rect.X1, rect.Y2 - rect.Y1)));
                }
            }
            while (iter.Next(PageIteratorLevel.Word));
        }

        var rois = RoiExtractor.Extract(words);
        sw.Stop();
        return new OcrResult(text, conf, sw.ElapsedMilliseconds, rois, words);
    }

    // ── Report builder ───────────────────────────────────────────────────────
    private static string BuildReport(List<ImageResult> results, int imageCount)
    {
        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
        sb.AppendLine("                    OCR ENGINE BENCHMARK REPORT");
        sb.AppendLine($"                    {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"Images: {imageCount}   Engines: {Engines.Length}   Total runs: {results.Count}");
        sb.AppendLine($"Ground truth: UPC (word match), Serial={ExpectedSerial}, Part=945-13450-0+-100 (fuzzy)");
        sb.AppendLine($"Preprocessing: sharpen sigma=1.8, strength=1.3");
        sb.AppendLine();

        // ── Per-image detail table ───────────────────────────────────────────
        sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
        sb.AppendLine("DETAILED RESULTS (per image x per engine)");
        sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine(
            $"{"Image",-40} {"Engine",-20} {"Time(ms)",8} {"UPC",5} {"Serial",8} {"Part",6} {"Error"}");
        sb.AppendLine(new string('-', 120));

        foreach (var r in results)
        {
            string upc    = r.UpcDetected    ? "YES" : "no";
            string serial = r.SerialDetected ? "YES" : "no";
            string part   = r.PartDetected   ? "YES" : "no";
            string error  = r.Error != null  ? $" ERR: {Truncate(r.Error, 40)}" : "";
            sb.AppendLine(
                $"{Truncate(r.FileName, 39),-40} {r.EngineName,-20} {r.ElapsedMs,8} {upc,5} {serial,8} {part,6}{error}");
        }

        sb.AppendLine();

        // ── Summary table ────────────────────────────────────────────────────
        sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
        sb.AppendLine("SUMMARY (per engine)");
        sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine(
            $"{"Engine",-20} {"Runs",5} {"Errors",7} {"AvgMs",7} {"UPC%",7} {"Serial%",9} {"Part%",7}");
        sb.AppendLine(new string('-', 68));

        foreach (var engine in Engines)
        {
            var engineResults = results.Where(r => r.EngineName == engine.ToString()).ToList();
            int runs   = engineResults.Count;
            int errors = engineResults.Count(r => r.Error != null);
            var valid  = engineResults.Where(r => r.Error == null).ToList();

            double avgMs   = valid.Count > 0 ? valid.Average(r => r.ElapsedMs) : 0;
            double upcPct  = valid.Count > 0 ? 100.0 * valid.Count(r => r.UpcDetected)    / valid.Count : 0;
            double serPct  = valid.Count > 0 ? 100.0 * valid.Count(r => r.SerialDetected)  / valid.Count : 0;
            double partPct = valid.Count > 0 ? 100.0 * valid.Count(r => r.PartDetected)    / valid.Count : 0;

            sb.AppendLine(
                $"{engine,-20} {runs,5} {errors,7} {avgMs,7:F1} {upcPct,6:F1}% {serPct,8:F1}% {partPct,6:F1}%");
        }

        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
        sb.AppendLine("END OF REPORT");
        sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

        return sb.ToString();
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..(max - 3)] + "...";
}
