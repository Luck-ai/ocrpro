using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace OcrPro;

/// <summary>
    /// OCR engine backed by the Windows.Media.Ocr WinRT API.
    /// The engine instance is created once and reused across calls (~5–20 ms per image).
    ///
    /// Default preprocessing pipeline: V01c — Gray+CLAHE(2,t8)+Sharp(2.5,1.5) = 50/53 All-3.
    /// </summary>
    static class WinRtOcrEngine
    {
    // Cache one engine per language tag (e.g. "en-US", "fr-FR").
    private static readonly Dictionary<string, OcrEngine> _engines = new();

    // ── V01c default preprocessing (Gray + CLAHE(2,t8) + Sharpen(2.5,1.5)) ───
    private static readonly GrayscaleParams _defaultGray    = new(true);
    private static readonly ClaheParams     _defaultClahe   = new(true, Clip: 2.0, TileSize: 8);
    private static readonly SharpenParams   _defaultSharpen = new(true, Sigma: 2.5, Strength: 1.5);

    // ── Public entry point ────────────────────────────────────────────────────

    /// <summary>
    /// Run OCR on <paramref name="bmp"/> using Windows.Media.Ocr.
    /// Applies V01c preprocessing by default (gray + CLAHE + sharpen).
    /// Pass <paramref name="skipPreprocess"/> = true to bypass preprocessing.
    /// </summary>
    /// <param name="bmp">Source bitmap (any pixel format; converted internally).</param>
    /// <param name="languageTag">
    ///   BCP-47 language tag, e.g. "en-US" or "eng". Pass null / empty to use
    ///   the user's profile languages. "eng" is normalised to "en-US".
    /// </param>
    /// <param name="skipPreprocess">
    ///   If true, skip the default V01c preprocessing (useful when caller
    ///   has already preprocessed or wants raw WinRT output).
    /// </param>
    public static async Task<OcrResult> RunAsync(Bitmap bmp, string? languageTag = null,
        int padding = 0, SharpenParams? sharpen = null, bool skipPreprocess = false)
    {
        var engine = GetOrCreateEngine(languageTag);

        // Apply V01c preprocessing by default: Gray + CLAHE(2,t8) + Sharpen(2.5,1.5)
        // Can be overridden by caller via sharpen parameter or skipped with skipPreprocess=true.
        Bitmap? preprocessed = null;
        if (!skipPreprocess)
        {
            using var bgr  = ImagePreprocessor.BitmapToMat(bmp);
            using var gray = ImagePreprocessor.StepGrayscale(bgr, _defaultGray);
            using var eq   = ImagePreprocessor.StepClahe(gray, _defaultClahe);
            var sharpParams = (sharpen is { Enabled: true }) ? sharpen : _defaultSharpen;
            using var shp  = ImagePreprocessor.StepSharpen(eq, sharpParams);
            preprocessed = ImagePreprocessor.MatToBitmap(shp);
            bmp = preprocessed;
        }
        else if (sharpen is { Enabled: true })
        {
            // Legacy: sharpen-only path when skipPreprocess=true but sharpen supplied
            preprocessed = ImagePreprocessor.ApplySharpen(bmp, sharpen);
            bmp = preprocessed;
        }

        // Convert System.Drawing.Bitmap → SoftwareBitmap via direct pixel copy (no encode/decode)
        using var softBitmap = BitmapToSoftwareBitmap(bmp, padding);

        // Time only the WinRT recognition call itself
        var sw = Stopwatch.StartNew();
        var winResult = await engine.RecognizeAsync(softBitmap);
        sw.Stop();

        // ── Flatten WinRT result into our WordEntry list ──────────────────────
        var words = new List<WordEntry>();
        var sb = new System.Text.StringBuilder();

        foreach (var line in winResult.Lines)
        {
            if (sb.Length > 0) sb.Append(' ');
            bool firstWordInLine = true;
            foreach (var word in line.Words)
            {
                var r = word.BoundingRect;
                words.Add(new WordEntry(
                    word.Text,
                    new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height)));

                if (!firstWordInLine) sb.Append(' ');
                sb.Append(word.Text);
                firstWordInLine = false;
            }
        }

        string rawText = sb.ToString();

        var rois = RoiExtractor.Extract(words);

        // Rebuild raw text with OCR noise corrected in ROI-matched spans
        // (e.g. leading tick on serial → '1', O→0/l→1 in part number)
        string correctedText = RoiExtractor.BuildCorrectedText(rawText, words);

        preprocessed?.Dispose();
        return new OcrResult(correctedText, sw.ElapsedMilliseconds, rois, words);
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

    // WinRT OCR accuracy improves significantly when text height is adequate.
    // Upscale images whose shorter side is below this threshold so small UI text
    // is large enough for the recogniser to handle reliably.
    private const int MinOcrDimension = 800;

    /// <summary>
    /// Converts a System.Drawing.Bitmap to a SoftwareBitmap via a single direct pixel copy.
    /// Upscales small images so the shorter side is at least <see cref="MinOcrDimension"/> px.
    /// Optionally adds <paramref name="padding"/> px of white border on all sides.
    /// </summary>
    private static SoftwareBitmap BitmapToSoftwareBitmap(Bitmap bmp, int padding = 0)
    {
        // Upscale if the shorter side is below MinOcrDimension.
        Bitmap? scaled = null;
        int shortSide = Math.Min(bmp.Width, bmp.Height);
        if (shortSide < MinOcrDimension)
        {
            float scale = (float)MinOcrDimension / shortSide;
            int newW = Math.Max(1, (int)(bmp.Width  * scale));
            int newH = Math.Max(1, (int)(bmp.Height * scale));
            scaled = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
            using var gs = Graphics.FromImage(scaled);
            gs.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            gs.DrawImage(bmp, 0, 0, newW, newH);
            bmp = scaled;
        }

        // Optionally add white padding around the image.
        Bitmap? padded = null;
        if (padding > 0)
        {
            padded = new Bitmap(bmp.Width + padding * 2, bmp.Height + padding * 2, PixelFormat.Format32bppArgb);
            using var gp = Graphics.FromImage(padded);
            gp.Clear(Color.White);
            gp.DrawImage(bmp, padding, padding, bmp.Width, bmp.Height);
            scaled?.Dispose();
            scaled = null;
            bmp = padded;
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
            padded?.Dispose();
        }
    }
}
