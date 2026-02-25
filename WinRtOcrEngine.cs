using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace OcrTesseract;

/// <summary>
/// OCR engine backed by the Windows.Media.Ocr WinRT API.
/// The engine instance is created once and reused across calls (~5–20 ms per image).
/// </summary>
static class WinRtOcrEngine
{
    // Cache one engine per language tag (e.g. "en-US", "fr-FR").
    private static readonly Dictionary<string, OcrEngine> _engines = new();

    // ── Public entry point ────────────────────────────────────────────────────

    /// <summary>
    /// Run OCR on <paramref name="bmp"/> using Windows.Media.Ocr.
    /// </summary>
    /// <param name="bmp">Source bitmap (any pixel format; converted internally).</param>
    /// <param name="languageTag">
    ///   BCP-47 language tag, e.g. "en-US" or "eng". Pass null / empty to use
    ///   the user's profile languages. "eng" is normalised to "en-US".
    /// </param>
    /// <returns>An <see cref="OcrResult"/> compatible with the rest of the app.</returns>
    public static async Task<OcrResult> RunAsync(Bitmap bmp, string? languageTag = null)
    {
        var sw = Stopwatch.StartNew();

        var engine = GetOrCreateEngine(languageTag);

        // Convert System.Drawing.Bitmap → SoftwareBitmap via direct pixel copy (no encode/decode)
        using var softBitmap = BitmapToSoftwareBitmap(bmp);

        // The WinRT call itself — this is what takes ~5–20 ms
        var winResult = await engine.RecognizeAsync(softBitmap);

        sw.Stop();

        // ── Flatten WinRT result into our WordEntry list ──────────────────────
        var words = new List<WordEntry>();
        float totalConf = 0f;
        int   wordCount = 0;
        var sb = new System.Text.StringBuilder();

        foreach (var line in winResult.Lines)
        {
            if (sb.Length > 0) sb.Append(' ');
            bool firstWordInLine = true;
            foreach (var word in line.Words)
            {
                var r = word.BoundingRect;
                // Windows.Media.Ocr does not expose per-word confidence; use 1.0
                // as a placeholder so existing ROI confidence logic still works.
                const float winRtConf = 1.0f;
                words.Add(new WordEntry(
                    word.Text,
                    winRtConf,
                    new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height)));
                totalConf += winRtConf;
                wordCount++;

                if (!firstWordInLine) sb.Append(' ');
                sb.Append(word.Text);
                firstWordInLine = false;
            }
        }

        float meanConf = wordCount > 0 ? totalConf / wordCount : 0f;
        string rawText = sb.ToString();

        var rois = RoiExtractor.Extract(words);

        return new OcrResult(rawText, meanConf, sw.ElapsedMilliseconds, rois, words);
    }

    // ── Availability check ────────────────────────────────────────────────────

    /// <summary>
    /// Returns true if at least one OCR language is available on this machine.
    /// </summary>
    public static bool IsAvailable =>
        OcrEngine.AvailableRecognizerLanguages.Count > 0;

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static OcrEngine GetOrCreateEngine(string? languageTag)
    {
        // Normalise Tesseract-style codes → BCP-47
        languageTag = NormaliseTag(languageTag);

        if (_engines.TryGetValue(languageTag, out var cached))
            return cached;

        OcrEngine? engine;
        if (string.IsNullOrEmpty(languageTag))
        {
            engine = OcrEngine.TryCreateFromUserProfileLanguages();
        }
        else
        {
            engine = OcrEngine.TryCreateFromLanguage(new Language(languageTag));
            // Fall back to user profile if the specific language isn't installed
            engine ??= OcrEngine.TryCreateFromUserProfileLanguages();
        }

        if (engine == null)
            throw new InvalidOperationException(
                "Windows OCR: no language pack is installed. " +
                "Go to Settings → Time & Language → Language and add an OCR language.");

        _engines[languageTag] = engine;
        return engine;
    }

    /// <summary>
    /// Maps Tesseract 3-letter codes to BCP-47 where needed.
    /// Unknown codes are passed through as-is (the Language ctor accepts both).
    /// </summary>
    private static string NormaliseTag(string? tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return "";
        return tag.ToLowerInvariant() switch
        {
            "eng" => "en-US",
            "fra" => "fr-FR",
            "deu" => "de-DE",
            "spa" => "es-ES",
            "ita" => "it-IT",
            "por" => "pt-PT",
            "rus" => "ru-RU",
            "chi_sim" => "zh-Hans-CN",
            "chi_tra" => "zh-Hant-TW",
            "jpn" => "ja-JP",
            "kor" => "ko-KR",
            "ara" => "ar-SA",
            _    => tag,
        };
    }

    // Maximum dimension (width or height) passed to the OCR engine.
    // WinRT OCR is accurate down to ~40px text height; 1400px is safe for large-text label images.
    // Reduce further (e.g. 1000) if text is consistently large and you need more speed.
    private const int MaxOcrDimension = 1400;

    /// <summary>
    /// Converts a System.Drawing.Bitmap to a SoftwareBitmap via a single direct pixel copy.
    /// Downscales to <see cref="MaxOcrDimension"/> on the longest side before conversion,
    /// reducing the pixel count fed to RecognizeAsync (~proportional speedup).
    /// </summary>
    private static SoftwareBitmap BitmapToSoftwareBitmap(Bitmap bmp)
    {
        // Downscale if the largest dimension exceeds MaxOcrDimension.
        Bitmap? scaled = null;
        if (bmp.Width > MaxOcrDimension || bmp.Height > MaxOcrDimension)
        {
            float scale = Math.Min((float)MaxOcrDimension / bmp.Width,
                                   (float)MaxOcrDimension / bmp.Height);
            int newW = Math.Max(1, (int)(bmp.Width  * scale));
            int newH = Math.Max(1, (int)(bmp.Height * scale));
            scaled = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
            using var gs = Graphics.FromImage(scaled);
            gs.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gs.DrawImage(bmp, 0, 0, newW, newH);
            bmp = scaled;
        }

        // Convert to Bgra32 if needed so the pixel layout matches BitmapPixelFormat.Bgra8.
        Bitmap? converted = null;
        if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
        {
            converted = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
            using var g = Graphics.FromImage(converted);
            g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
            bmp = converted;
        }

        try
        {
            int byteCount = bmp.Width * bmp.Height * 4;
            var pixels = new byte[byteCount];

            // Copy pixels out of the locked GDI+ bitmap into a managed byte[]
            var srcData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            try
            {
                Marshal.Copy(srcData.Scan0, pixels, 0, byteCount);
            }
            finally
            {
                bmp.UnlockBits(srcData);
            }

            // CopyFromBuffer copies the managed byte[] directly into the SoftwareBitmap pixel buffer
            var softBmp = new SoftwareBitmap(
                BitmapPixelFormat.Bgra8, bmp.Width, bmp.Height, BitmapAlphaMode.Ignore);
            softBmp.CopyFromBuffer(pixels.AsBuffer());
            return softBmp;
        }
        finally
        {
            converted?.Dispose();
            scaled?.Dispose();
        }
    }
}
