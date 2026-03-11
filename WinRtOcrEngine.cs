using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace OcrPro;

static class WinRtOcrEngine
{
    private static readonly Dictionary<string, OcrEngine> _engines = new();

    private static readonly GrayscaleParams _defaultGray    = new(true);
    private static readonly ClaheParams     _defaultClahe   = new(true, Clip: 2.0, TileSize: 8);
    private static readonly SharpenParams   _defaultSharpen = new(true, Sigma: 2.5, Strength: 1.5);

    public static void WarmUpInBackground()
    {
        Task.Run(() =>
        {
            try
            {
                using var bmp = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
                RunAsync(bmp).GetAwaiter().GetResult();
            }
            catch { }
        });
    }

    public static async Task<OcrResult> RunAsync(Bitmap bmp, string? languageTag = null,
        int padding = 0, SharpenParams? sharpen = null, bool skipPreprocess = false)
    {
        var engine = GetOrCreateEngine(languageTag);

        var sw = Stopwatch.StartNew();

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
            preprocessed = ImagePreprocessor.ApplySharpen(bmp, sharpen);
            bmp = preprocessed;
        }

        using var softBitmap = BitmapToSoftwareBitmap(bmp, padding);

        var winResult = await engine.RecognizeAsync(softBitmap);
        sw.Stop();

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

        string correctedText = RoiExtractor.BuildCorrectedText(rawText, words);

        preprocessed?.Dispose();
        return new OcrResult(correctedText, sw.ElapsedMilliseconds, rois, words);
    }

    public static bool IsAvailable =>
        OcrEngine.AvailableRecognizerLanguages.Count > 0;

    private static OcrEngine GetOrCreateEngine(string? languageTag)
    {
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
            engine ??= OcrEngine.TryCreateFromUserProfileLanguages();
        }

        if (engine == null)
            throw new InvalidOperationException(
                "Windows OCR: no language pack is installed. " +
                "Go to Settings → Time & Language → Language and add an OCR language.");

        _engines[languageTag] = engine;
        return engine;
    }

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

    private const int MinOcrDimension = 800;

    private static SoftwareBitmap BitmapToSoftwareBitmap(Bitmap bmp, int padding = 0)
    {
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
