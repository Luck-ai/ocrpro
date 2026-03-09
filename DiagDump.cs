using System.Text;
using System.Text.RegularExpressions;

namespace OcrTesseract;

static class DiagDump
{
    private static readonly string[] DatasetFolders =
    {
        @"C:\Users\gamin\Downloads\Project\Project",
        @"C:\Users\gamin\Downloads\Project\datset2",
    };

    private const string ExpectedSerial = "1421323031448";
    // Fuzzy: accept 1+ zeros (OCR sometimes merges the "0000" run)
    private static readonly Regex _partGtRx = new(@"945-13450-0+-100", RegexOptions.Compiled);

    private const string OutPath = @"D:\ocr_tesserack\ocr_diag.txt";

    public static async Task RunAsync()
    {
        var images = new List<string>();
        foreach (var folder in DatasetFolders)
            if (Directory.Exists(folder))
                images.AddRange(Directory.GetFiles(folder, "*.bmp", SearchOption.TopDirectoryOnly));
        images.Sort(StringComparer.OrdinalIgnoreCase);

        PaddleOcrEngine.WarmUpInBackground();
        await Task.Delay(2000);

        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("  FAILURE ANALYSIS — PaddleOcr V4 (sharp2.5/1.5 + no binarise)");
        sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        int pass = 0, fail = 0;

        foreach (var imgPath in images)
        {
            string name = Path.GetFileName(imgPath);
            try
            {
                using var original = new Bitmap(Image.FromFile(imgPath));

                // Best config: sharp 2.5/1.5, no binarise — run on background thread
                // (PaddleInference must NOT be called on the STA thread)
                var r = await Task.Run(() =>
                {
                    using var prep = ImagePreprocessor.Process(original,
                        sharpen: new SharpenParams(true, 2.5, 1.5),
                        binarise: new BinariseParams(false));
                    return PaddleOcrEngine.Run(prep);
                });

                bool upc    = r.Rois.Any(x => x.FieldName.Equals("UPC", StringComparison.OrdinalIgnoreCase));
                bool serial = r.Rois.Any(x => x.FieldName.Equals("Serial Number", StringComparison.OrdinalIgnoreCase)
                                              && x.Value.Contains(ExpectedSerial));
                bool part   = r.Rois.Any(x => x.FieldName.Equals("Part Number", StringComparison.OrdinalIgnoreCase)
                                              && _partGtRx.IsMatch(x.Value));

                if (upc && serial && part)
                {
                    pass++;
                    continue;
                }

                fail++;
                sb.AppendLine($"──── FAIL #{fail}: {name} ({original.Width}x{original.Height}) ────");
                sb.AppendLine($"  UPC={upc}  Serial={serial}  Part={part}");
                sb.AppendLine($"  Time: {r.ElapsedMs}ms   Words: {r.Words.Count}");
                sb.AppendLine($"  ROIs found: {string.Join(", ", r.Rois.Select(roi => $"{roi.FieldName}=\"{roi.Value}\""))}");
                sb.AppendLine($"  RawText:");
                foreach (var line in r.RawText.Split('\n'))
                    sb.AppendLine($"    | {line}");
                sb.AppendLine($"  All words: {string.Join(" | ", r.Words.Select(w => $"\"{w.Text}\""))}");
                sb.AppendLine();
            }
            catch (Exception ex)
            {
                fail++;
                sb.AppendLine($"──── FAIL #{fail}: {name} — EXCEPTION: {ex.Message}");
                sb.AppendLine();
            }
        }

        sb.AppendLine($"═══════════════════════════════════════════════════════════════");
        sb.AppendLine($"  PASS: {pass}/{images.Count}    FAIL: {fail}/{images.Count}");
        sb.AppendLine($"═══════════════════════════════════════════════════════════════");

        await File.WriteAllTextAsync(OutPath, sb.ToString());
    }
}
