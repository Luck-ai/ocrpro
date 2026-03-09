using System.Diagnostics;
using OpenCvSharp;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleInference;

namespace OcrPro;

/// <summary>
/// OCR engine backed by PaddleOCR (PP-OCRv3 English, MKLDNN CPU).
/// Typical throughput: ~130–400 ms on CPU after first-run warm-up.
/// Switched from V4 to V3 based on speed benchmark:
///   V3 local: avg=328ms, min=132ms, 19/19 all-3 PASS (faster AND same accuracy as V4/640)
///   V4/640:   avg=491ms, min=229ms, 19/19 all-3 PASS
/// Models are bundled with Sdcb.PaddleOCR.Models.Local — no download needed.
///
/// A single <see cref="PaddleOcrAll"/> instance is held open and reused.
/// The first call initialises the MKLDNN session (~1–3 s one-time cost).
/// </summary>
static class PaddleOcrEngine
{
    private static PaddleOcrAll? _ocr;
    private static volatile bool _initialised;
    private static string?       _initError;
    private static string        _deviceLabel = "CPU/MKLDNN";
    private static readonly object _lock = new();

    /// <summary>Returns "GPU/DirectML" or "CPU/MKLDNN" after first initialisation.</summary>
    public static string DeviceLabel { get { EnsureInitialised(); return _deviceLabel; } }

    // ── Public entry points ───────────────────────────────────────────────────

    /// <summary>
    /// Starts model initialisation on a background thread so the first
    /// real <see cref="Run"/> call doesn't pay the cold-start penalty.
    /// Safe to call multiple times — subsequent calls are no-ops.
    /// </summary>
    public static void WarmUpInBackground() =>
        Task.Run(EnsureInitialised);

    public static OcrResult Run(Bitmap bmp)
    {
        var sw = Stopwatch.StartNew();

        EnsureInitialised();

        if (_ocr == null)
            throw new InvalidOperationException(
                $"PaddleOCR engine could not be initialised: {_initError}");

        PaddleOcrResult result;
        using (var mat = BitmapToMat(bmp))
        {
            // PaddleOCR's ONNX session is not thread-safe — serialise all Run() calls.
            lock (_lock)
            {
                result = _ocr.Run(mat);
            }
        }

        sw.Stop();

        // ── Build word list from detected regions ─────────────────────────────
        var words = new List<WordEntry>();
        foreach (var region in result.Regions)
        {
            string text = region.Text?.Trim() ?? "";
            if (text.Length == 0) continue;

            // Convert RotatedRect to axis-aligned bounding Rectangle
            var rrect = region.Rect;
            var pts   = Cv2.BoxPoints(rrect);
            int x1 = (int)pts.Min(p => p.X);
            int y1 = (int)pts.Min(p => p.Y);
            int x2 = (int)pts.Max(p => p.X);
            int y2 = (int)pts.Max(p => p.Y);

            words.Add(new WordEntry(text,
                new Rectangle(x1, y1, x2 - x1, y2 - y1)));
        }

        string rawText = result.Text?.Trim() ?? "";
        var rois       = RoiExtractor.Extract(words);

        return new OcrResult(rawText, sw.ElapsedMilliseconds, rois, words);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="Bitmap"/> directly to an OpenCvSharp BGR Mat via
    /// LockBits — no PNG encode/decode, saves ~30–80 ms per call.
    /// </summary>
    private static Mat BitmapToMat(Bitmap bmp)
    {
        const System.Drawing.Imaging.PixelFormat Bgr24 =
            System.Drawing.Imaging.PixelFormat.Format24bppRgb;

        // Ensure 24-bit BGR — the format OpenCV expects natively.
        bool needConvert = bmp.PixelFormat != Bgr24;
        Bitmap? tmp = null;
        if (needConvert)
        {
            tmp = new Bitmap(bmp.Width, bmp.Height, Bgr24);
            using var g = Graphics.FromImage(tmp);
            g.DrawImage(bmp, 0, 0);
        }

        var src = tmp ?? bmp;
        var bd  = src.LockBits(new Rectangle(0, 0, src.Width, src.Height),
                      System.Drawing.Imaging.ImageLockMode.ReadOnly, Bgr24);
        try
        {
            // FromPixelData wraps the existing pointer — Clone() copies into a new Mat
            // so we can unlock the bitmap immediately after.
            return Mat.FromPixelData(src.Height, src.Width, MatType.CV_8UC3,
                                     bd.Scan0, bd.Stride).Clone();
        }
        finally
        {
            src.UnlockBits(bd);
            tmp?.Dispose();
        }
    }

    private static void EnsureInitialised()
    {
        if (_initialised) return;
        lock (_lock)
        {
            if (_initialised) return;
            InitialiseLocked();
        }
    }

    /// <summary>
    /// Does the actual init work. Must be called with <see cref="_lock"/> already held.
    /// Uses CPU/MKLDNN unconditionally — DirectML (ONNX) is unreliable on this hardware
    /// (passes probe but fails on every other real image with a native session error).
    /// </summary>
    private static void InitialiseLocked()
    {
        try
        {
            var model = LocalFullModels.EnglishV3;

            var ocr = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
            {
                AllowRotateDetection    = true,
                Enable180Classification = true,
            };
            ocr.Detector.MaxSize = 640;  // default 1536 — smaller = ~2x faster detection
            _deviceLabel = "CPU/MKLDNN";

            using var warmup = new Mat(720, 1280, MatType.CV_8UC3, Scalar.White);
            ocr.Run(warmup);
            _ocr = ocr;
        }
        catch (Exception ex)
        {
            _initError = ex.Message;
            _ocr       = null;
        }
        _initialised = true;
    }
}
