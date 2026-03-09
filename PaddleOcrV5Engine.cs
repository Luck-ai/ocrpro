using System.Diagnostics;
using OpenCvSharp;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using Sdcb.PaddleInference;

namespace OcrTesseract;

/// <summary>
/// PP-OCRv5 engine via Sdcb.PaddleOCR — Chinese V5 models handle English
/// text well due to the unified multilingual architecture.
/// Provides both Mobile (fast) and Server (accurate) variants.
/// Models are downloaded on first use via the Online package.
/// </summary>
static class PaddleOcrV5Engine
{
    // ── Mobile instance ──────────────────────────────────────────────────────
    private static PaddleOcrAll? _ocrMobile;
    private static bool          _mobileInitialised;
    private static string?       _mobileInitError;
    private static string        _mobileDeviceLabel = "CPU/MKLDNN";
    private static readonly object _mobileLock = new();

    // ── Server instance ──────────────────────────────────────────────────────
    private static PaddleOcrAll? _ocrServer;
    private static bool          _serverInitialised;
    private static string?       _serverInitError;
    private static string        _serverDeviceLabel = "CPU/MKLDNN";
    private static readonly object _serverLock = new();

    // ── Public API ───────────────────────────────────────────────────────────

    public static void WarmUpMobileInBackground() => Task.Run(EnsureMobileInitialised);
    public static void WarmUpServerInBackground() => Task.Run(EnsureServerInitialised);

    public static OcrResult RunMobile(Bitmap bmp)
    {
        EnsureMobileInitialised();
        if (_ocrMobile == null)
            throw new InvalidOperationException(
                $"PaddleOCR V5 Mobile could not be initialised: {_mobileInitError}");
        return RunOcr(_ocrMobile, bmp);
    }

    public static OcrResult RunServer(Bitmap bmp)
    {
        EnsureServerInitialised();
        if (_ocrServer == null)
            throw new InvalidOperationException(
                $"PaddleOCR V5 Server could not be initialised: {_serverInitError}");
        return RunOcr(_ocrServer, bmp);
    }

    // ── Shared OCR runner ────────────────────────────────────────────────────
    private static OcrResult RunOcr(PaddleOcrAll ocr, Bitmap bmp)
    {
        var sw = Stopwatch.StartNew();

        PaddleOcrResult result;
        using (var mat = BitmapToMat(bmp))
        {
            result = ocr.Run(mat);
        }

        sw.Stop();

        var words = new List<WordEntry>();
        foreach (var region in result.Regions)
        {
            string text = region.Text?.Trim() ?? "";
            if (text.Length == 0) continue;

            float conf = (float)region.Score;
            var rrect = region.Rect;
            var pts   = Cv2.BoxPoints(rrect);
            int x1 = (int)pts.Min(p => p.X);
            int y1 = (int)pts.Min(p => p.Y);
            int x2 = (int)pts.Max(p => p.X);
            int y2 = (int)pts.Max(p => p.Y);

            words.Add(new WordEntry(text, conf,
                new Rectangle(x1, y1, x2 - x1, y2 - y1)));
        }

        float meanConf = words.Count > 0 ? words.Average(w => w.Confidence) : 0f;
        string rawText = result.Text?.Trim() ?? "";
        var rois       = RoiExtractor.Extract(words);

        return new OcrResult(rawText, meanConf, sw.ElapsedMilliseconds, rois, words);
    }

    // ── Initialisation — Mobile ──────────────────────────────────────────────
    private static void EnsureMobileInitialised()
    {
        if (_mobileInitialised) return;
        lock (_mobileLock)
        {
            if (_mobileInitialised) return;
            try
            {
                // Chinese V5 mobile — handles English via unified multilingual arch
                var model = OnlineFullModels.ChineseV5;
                var fullModel = model.DownloadAsync().GetAwaiter().GetResult();

                PaddleOcrAll? ocr = null;
                try
                {
                    ocr = new PaddleOcrAll(fullModel, PaddleDevice.Onnx(0))
                    {
                        AllowRotateDetection    = false,
                        Enable180Classification = false,
                    };
                    using var probe = new Mat(32, 128, MatType.CV_8UC3, Scalar.White);
                    ocr.Run(probe);
                    _mobileDeviceLabel = "GPU/DirectML";
                }
                catch
                {
                    ocr?.Dispose();
                    ocr = new PaddleOcrAll(fullModel, PaddleDevice.Mkldnn())
                    {
                        AllowRotateDetection    = false,
                        Enable180Classification = false,
                    };
                    _mobileDeviceLabel = "CPU/MKLDNN";
                }

                using var warmup = new Mat(64, 256, MatType.CV_8UC3, Scalar.White);
                ocr.Run(warmup);
                _ocrMobile = ocr;
            }
            catch (Exception ex)
            {
                _mobileInitError = ex.Message;
                _ocrMobile       = null;
            }
            _mobileInitialised = true;
        }
    }

    // ── Initialisation — Server ──────────────────────────────────────────────
    private static void EnsureServerInitialised()
    {
        if (_serverInitialised) return;
        lock (_serverLock)
        {
            if (_serverInitialised) return;
            try
            {
                // Chinese V5 server — highest accuracy models
                var model = OnlineFullModels.ChineseServerV5;
                var fullModel = model.DownloadAsync().GetAwaiter().GetResult();

                PaddleOcrAll? ocr = null;
                try
                {
                    ocr = new PaddleOcrAll(fullModel, PaddleDevice.Onnx(0))
                    {
                        AllowRotateDetection    = false,
                        Enable180Classification = false,
                    };
                    using var probe = new Mat(32, 128, MatType.CV_8UC3, Scalar.White);
                    ocr.Run(probe);
                    _serverDeviceLabel = "GPU/DirectML";
                }
                catch
                {
                    ocr?.Dispose();
                    ocr = new PaddleOcrAll(fullModel, PaddleDevice.Mkldnn())
                    {
                        AllowRotateDetection    = false,
                        Enable180Classification = false,
                    };
                    _serverDeviceLabel = "CPU/MKLDNN";
                }

                using var warmup = new Mat(64, 256, MatType.CV_8UC3, Scalar.White);
                ocr.Run(warmup);
                _ocrServer = ocr;
            }
            catch (Exception ex)
            {
                _serverInitError = ex.Message;
                _ocrServer       = null;
            }
            _serverInitialised = true;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static Mat BitmapToMat(Bitmap bmp)
    {
        const System.Drawing.Imaging.PixelFormat Bgr24 =
            System.Drawing.Imaging.PixelFormat.Format24bppRgb;

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
            return Mat.FromPixelData(src.Height, src.Width, MatType.CV_8UC3,
                                     bd.Scan0, bd.Stride).Clone();
        }
        finally
        {
            src.UnlockBits(bd);
            tmp?.Dispose();
        }
    }
}
