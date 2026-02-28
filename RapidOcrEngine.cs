using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace OcrTesseract;

/// <summary>
/// OCR engine backed by PP-OCRv4 mobile ONNX models (RapidAI) via OnnxRuntime + DirectML.
///
/// Pipeline: DBNet detection → angle classification → SVTR-LCNet recognition.
///
/// Model files in &lt;AppDir&gt;\models\ppocr\:
///   ch_PP-OCRv4_det_infer.onnx            — text region detection
///   ch_ppocr_mobile_v2.0_cls_infer.onnx   — 0°/180° angle classification
///   en_PP-OCRv4_rec_infer.onnx            — character recognition (English SVTR-LCNet)
///   en_dict.txt                           — English character dictionary (96 chars)
/// </summary>
static class RapidOcrEngine
{
    // ── ONNX sessions ─────────────────────────────────────────────────────────
    private static InferenceSession? _det;
    private static InferenceSession? _cls;
    private static InferenceSession? _rec;
    private static string[]?         _keys;

    private static string _detInput = "x";
    private static string _clsInput = "x";
    private static string _recInput = "x";

    private static bool    _initialised;
    private static string? _initError;
    private static string  _deviceLabel = "CPU";
    private static readonly object _lock = new();

    // ── Pipeline constants ────────────────────────────────────────────────────
    private const int   DetSize     = 320;   // DBNet input (320 is fast, still accurate for typical labels)
    private const int   ClsWidth    = 192;
    private const int   ClsHeight   = 48;
    private const int   RecHeight   = 48;
    private const float DetThresh   = 0.3f;
    private const float BoxThresh   = 0.5f;
    private const float UnclipRatio = 1.5f;

    // ── Public API ────────────────────────────────────────────────────────────

    public static string DeviceLabel { get { EnsureInitialised(); return _deviceLabel; } }

    public static void WarmUpInBackground() => Task.Run(EnsureInitialised);

    public static OcrResult Run(Bitmap bmp)
    {
        var sw  = Stopwatch.StartNew();
        var swS = Stopwatch.StartNew();

        EnsureInitialised();
        if (_det == null || _cls == null || _rec == null || _keys == null)
            throw new InvalidOperationException($"RapidOCR could not be initialised: {_initError}");

        var boxes = Detect(bmp);
        long msDetect = swS.ElapsedMilliseconds; swS.Restart();

        // ── Classify angle and build crops ───────────────────────────────────
        var crops     = new List<(Bitmap bmp, Rectangle rect)>();
        var tempCrops = new List<Bitmap>();   // track for disposal

        foreach (var box in boxes)
        {
            var crop = CropBox(bmp, box);
            if (crop == null) continue;

            var (angleCrop, _) = ClassifyAngle(crop);
            if (!ReferenceEquals(angleCrop, crop)) { crop.Dispose(); crop = angleCrop; }
            tempCrops.Add(crop);

            int x1 = Math.Clamp((int)box.Min(p => p.X), 0, bmp.Width  - 1);
            int y1 = Math.Clamp((int)box.Min(p => p.Y), 0, bmp.Height - 1);
            int x2 = Math.Clamp((int)box.Max(p => p.X), x1 + 1, bmp.Width);
            int y2 = Math.Clamp((int)box.Max(p => p.Y), y1 + 1, bmp.Height);
            crops.Add((crop, new Rectangle(x1, y1, x2 - x1, y2 - y1)));
        }

        // ── Batch recognise all crops in one GPU call ─────────────────────────
        var words = new List<WordEntry>();
        try
        {
            if (crops.Count > 0)
            {
                var results = RecogniseBatch(crops.Select(c => c.bmp).ToList());
                for (int i = 0; i < results.Count; i++)
                {
                    var (text, conf) = results[i];
                    if (text.Length > 0)
                        words.Add(new WordEntry(text, conf, crops[i].rect));
                }
            }
        }
        finally
        {
            foreach (var b in tempCrops) b.Dispose();
        }

        long msRec = swS.ElapsedMilliseconds;
        sw.Stop();

        for (int i = 0; i < words.Count; i++)
            Console.WriteLine($"  [{i}] \"{words[i].Text}\" {words[i].Bounds}");
        Console.WriteLine($"[RapidOCR] {_deviceLabel} | detect={msDetect}ms rec={msRec}ms ({boxes.Count} boxes) total={sw.ElapsedMilliseconds}ms");

        string rawText  = string.Join("\n", words.Select(w => w.Text));
        float  meanConf = words.Count > 0 ? words.Average(w => w.Confidence) : 0f;
        return new OcrResult(rawText, meanConf, sw.ElapsedMilliseconds,
                             RoiExtractor.Extract(words), words);
    }

    // ── Stage 1 — Detection ───────────────────────────────────────────────────

    private static List<PointF[]> Detect(Bitmap bmp)
    {
        int origW = bmp.Width, origH = bmp.Height;

        float scale = Math.Min((float)DetSize / origW, (float)DetSize / origH);
        int   resW  = (int)(origW * scale);
        int   resH  = (int)(origH * scale);

        // Build DetSize×DetSize input — image in top-left, rest black
        using var canvas = new Bitmap(DetSize, DetSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using (var g = Graphics.FromImage(canvas))
        {
            g.Clear(Color.Black);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.DrawImage(bmp, 0, 0, resW, resH);
        }

        // ImageNet normalisation, RGB order (R=0.485, G=0.456, B=0.406)
        float[] mean = { 0.485f * 255f, 0.456f * 255f, 0.406f * 255f };
        float[] std  = { 0.229f * 255f, 0.224f * 255f, 0.225f * 255f };
        var tensor = BitmapToTensor(canvas, mean, std);

        using var detOut  = _det!.Run(new[] { NamedOnnxValue.CreateFromTensor(_detInput, tensor) });
        var       probTen = detOut.First().AsTensor<float>(); // [1, 1, pH, pW]

        int pH = probTen.Dimensions[2];
        int pW = probTen.Dimensions[3];

        // Copy prob map into a flat float[] and wrap in an OpenCvSharp Mat
        var probFlat = new float[pH * pW];
        for (int r = 0; r < pH; r++)
            for (int c = 0; c < pW; c++)
                probFlat[r * pW + c] = probTen[0, 0, r, c];

        return ExtractBoxes(probFlat, pH, pW, scale, origW, origH);
    }

    private static List<PointF[]> ExtractBoxes(
        float[] probFlat, int pH, int pW, float scale, int origW, int origH)
    {
        // Threshold → binary byte array → byte Mat for ConnectedComponentsWithStats
        var binFlat = new byte[pH * pW];
        for (int i = 0; i < binFlat.Length; i++)
            binFlat[i] = probFlat[i] >= DetThresh ? (byte)255 : (byte)0;

        // Pin the byte array so OpenCvSharp can wrap it without a copy
        var gh = GCHandle.Alloc(binFlat, GCHandleType.Pinned);
        try
        {
            using var bin8      = Mat.FromPixelData(pH, pW, MatType.CV_8UC1, gh.AddrOfPinnedObject());
            using var labelsMat = new Mat();
            using var stats     = new Mat();
            using var centroids = new Mat();
            int numLabels = Cv2.ConnectedComponentsWithStats(bin8, labelsMat, stats, centroids);

            var   boxes = new List<PointF[]>();
            float stepX = (float)DetSize / pW / scale;
            float stepY = (float)DetSize / pH / scale;

            for (int lbl = 1; lbl < numLabels; lbl++)   // skip label 0 = background
            {
                int area = stats.At<int>(lbl, (int)ConnectedComponentsTypes.Area);
                if (area < 16) continue;

                int left   = stats.At<int>(lbl, (int)ConnectedComponentsTypes.Left);
                int top    = stats.At<int>(lbl, (int)ConnectedComponentsTypes.Top);
                int width  = stats.At<int>(lbl, (int)ConnectedComponentsTypes.Width);
                int height = stats.At<int>(lbl, (int)ConnectedComponentsTypes.Height);

                // Mean prob score over the bounding box (fast approximation)
                float scoreSum = 0f;
                int   rEnd = Math.Min(top + height, pH);
                int   cEnd = Math.Min(left + width, pW);
                for (int r = top; r < rEnd; r++)
                    for (int c = left; c < cEnd; c++)
                        scoreSum += probFlat[r * pW + c];
                float score = scoreSum / (width * height);
                if (score < BoxThresh) continue;

                // Unclip
                float perim  = 2f * (width + height);
                int   expand = (int)Math.Ceiling(area * UnclipRatio / perim);

                int r0 = Math.Max(0,      top  + height - 1 + expand < pH ? top    - expand : top);
                int c0 = Math.Max(0,      left - expand);
                int r1 = Math.Min(pH - 1, top  + height - 1 + expand);
                int c1 = Math.Min(pW - 1, left + width  - 1 + expand);
                r0 = Math.Max(0, top - expand);

                float ox0 = Math.Clamp(c0 * stepX, 0, origW - 1);
                float oy0 = Math.Clamp(r0 * stepY, 0, origH - 1);
                float ox1 = Math.Clamp(c1 * stepX, 0, origW);
                float oy1 = Math.Clamp(r1 * stepY, 0, origH);

                if (ox1 - ox0 < 4 || oy1 - oy0 < 2) continue;

                boxes.Add(new[]
                {
                    new PointF(ox0, oy0), new PointF(ox1, oy0),
                    new PointF(ox1, oy1), new PointF(ox0, oy1),
                });
            }

            return boxes;
        }
        finally { gh.Free(); }
    }

    // ── Stage 2 — Angle classification ───────────────────────────────────────

    private static (Bitmap result, int angle) ClassifyAngle(Bitmap crop)
    {
        using var resized = ResizePad(crop, ClsWidth, ClsHeight);
        float[] mean = { 127.5f, 127.5f, 127.5f };
        float[] std  = { 127.5f, 127.5f, 127.5f };
        var tensor = BitmapToTensor(resized, mean, std);

        using var clsOut = _cls!.Run(new[] { NamedOnnxValue.CreateFromTensor(_clsInput, tensor) });
        var scores = clsOut.First().AsTensor<float>(); // [1, 2]

        if (scores[0, 1] > scores[0, 0])
        {
            var rotated = (Bitmap)crop.Clone();
            rotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return (rotated, 180);
        }
        return (crop, 0);
    }

    // Fixed recognition width — all crops padded to this shape so DirectML
    // compiles the kernel once and reuses it for the entire batch.
    private const int RecWidth = 320;

    // ── Stage 3 — Batched recognition ────────────────────────────────────────

    /// <summary>
    /// Runs all crops in a single [N, 3, RecHeight, RecWidth] GPU call
    /// instead of N serial [1, 3, H, W] calls.
    /// </summary>
    private static List<(string text, float conf)> RecogniseBatch(List<Bitmap> cropList)
    {
        int N = cropList.Count;
        var batch = new DenseTensor<float>(new[] { N, 3, RecHeight, RecWidth });

        for (int i = 0; i < N; i++)
        {
            var crop    = cropList[i];
            int scaledW = Math.Clamp(
                (int)Math.Round((float)crop.Width / crop.Height * RecHeight), 1, RecWidth);

            using var resized = ResizePad(crop, scaledW, RecHeight);
            using var canvas  = new Bitmap(RecWidth, RecHeight,
                                           System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Black);
                g.DrawImage(resized, 0, 0, scaledW, RecHeight);
            }

            var bd = canvas.LockBits(new Rectangle(0, 0, RecWidth, RecHeight),
                         System.Drawing.Imaging.ImageLockMode.ReadOnly,
                         System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                int stride = bd.Stride;
                var row    = new byte[stride];
                for (int r = 0; r < RecHeight; r++)
                {
                    Marshal.Copy(bd.Scan0 + r * stride, row, 0, stride);
                    for (int c = 0; c < RecWidth; c++)
                    {
                        float b  = row[c * 3 + 0];
                        float g  = row[c * 3 + 1];
                        float rv = row[c * 3 + 2];
                        batch[i, 0, r, c] = (rv - 127.5f) / 127.5f;
                        batch[i, 1, r, c] = (g  - 127.5f) / 127.5f;
                        batch[i, 2, r, c] = (b  - 127.5f) / 127.5f;
                    }
                }
            }
            finally { canvas.UnlockBits(bd); }
        }

        using var recOut = _rec!.Run(new[] { NamedOnnxValue.CreateFromTensor(_recInput, batch) });
        var logits = recOut.First().AsTensor<float>(); // [N, T, num_classes]

        int T      = logits.Dimensions[1];
        int nClass = logits.Dimensions[2];
        var results = new List<(string, float)>(N);

        for (int i = 0; i < N; i++)
        {
            int   prev    = -1;
            float confSum = 0f;
            int   confN   = 0;
            var   sb      = new System.Text.StringBuilder();

            for (int t = 0; t < T; t++)
            {
                int   best  = 0;
                float bestV = logits[i, t, 0];
                for (int k = 1; k < nClass; k++)
                {
                    float v = logits[i, t, k];
                    if (v > bestV) { bestV = v; best = k; }
                }
                if (best != 0 && best != prev)
                {
                    int ki = best - 1;
                    if (ki < _keys!.Length) sb.Append(_keys[ki]);
                    confSum += bestV;
                    confN++;
                }
                prev = best;
            }
            results.Add((sb.ToString().Trim(), confN > 0 ? confSum / confN : 0f));
        }

        return results;
    }

    // ── Image helpers ─────────────────────────────────────────────────────────

    /// <summary>Resize to exactly w×h (stretches if needed — caller controls aspect).</summary>
    private static Bitmap ResizePad(Bitmap src, int w, int h)
    {
        var dst = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using var g = Graphics.FromImage(dst);
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
        g.DrawImage(src, 0, 0, w, h);
        return dst;
    }

    /// <summary>
    /// Converts a 24-bpp GDI+ Bitmap (BGR byte order) to a [1,3,H,W] float tensor
    /// in RGB channel order with per-channel normalisation.
    /// </summary>
    private static DenseTensor<float> BitmapToTensor(Bitmap bmp, float[] mean, float[] std)
    {
        int h = bmp.Height, w = bmp.Width;
        var bd = bmp.LockBits(new Rectangle(0, 0, w, h),
                     System.Drawing.Imaging.ImageLockMode.ReadOnly,
                     System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        try
        {
            int stride = bd.Stride;
            var row    = new byte[stride];
            var tensor = new DenseTensor<float>(new[] { 1, 3, h, w });

            for (int r = 0; r < h; r++)
            {
                Marshal.Copy(bd.Scan0 + r * stride, row, 0, stride);
                for (int c = 0; c < w; c++)
                {
                    // GDI+ BGR → model RGB
                    float b  = row[c * 3 + 0];
                    float g  = row[c * 3 + 1];
                    float rv = row[c * 3 + 2];
                    tensor[0, 0, r, c] = (rv - mean[0]) / std[0];  // R → ch0
                    tensor[0, 1, r, c] = (g  - mean[1]) / std[1];  // G → ch1
                    tensor[0, 2, r, c] = (b  - mean[2]) / std[2];  // B → ch2
                }
            }
            return tensor;
        }
        finally { bmp.UnlockBits(bd); }
    }

    private static Bitmap? CropBox(Bitmap src, PointF[] box)
    {
        int x = Math.Clamp((int)box.Min(p => p.X), 0, src.Width  - 1);
        int y = Math.Clamp((int)box.Min(p => p.Y), 0, src.Height - 1);
        int w = Math.Clamp((int)(box.Max(p => p.X) - x), 1, src.Width  - x);
        int h = Math.Clamp((int)(box.Max(p => p.Y) - y), 1, src.Height - y);
        if (w < 2 || h < 2) return null;

        var crop = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        using var g = Graphics.FromImage(crop);
        g.DrawImage(src, new Rectangle(0, 0, w, h), new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
        return crop;
    }

    // ── Initialisation ────────────────────────────────────────────────────────

    private static void EnsureInitialised()
    {
        if (_initialised) return;
        lock (_lock)
        {
            if (_initialised) return;
            try
            {
                string modelDir = Path.Combine(AppContext.BaseDirectory, "models", "ppocr");
                string detPath  = Path.Combine(modelDir, "ch_PP-OCRv4_det_infer.onnx");
                string clsPath  = Path.Combine(modelDir, "ch_ppocr_mobile_v2.0_cls_infer.onnx");
                string recPath  = Path.Combine(modelDir, "en_PP-OCRv4_rec_infer.onnx");
                string keysPath = Path.Combine(modelDir, "en_dict.txt");

                foreach (string p in new[] { detPath, clsPath, recPath, keysPath })
                    if (!File.Exists(p))
                        throw new FileNotFoundException($"RapidOCR model file not found: {p}");

                _keys = File.ReadAllLines(keysPath).Where(l => l.Length > 0).ToArray();

                // Try DirectML (GPU); fall back to CPU if session creation fails
                SessionOptions opts = MakeOpts(useDirectML: true);
                try
                {
                    _det = new InferenceSession(detPath, opts);
                    _cls = new InferenceSession(clsPath, opts);
                    _rec = new InferenceSession(recPath, opts);
                    _deviceLabel = "GPU/DirectML";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RapidOCR] DirectML failed ({ex.Message}), falling back to CPU");
                    _det?.Dispose(); _det = null;
                    _cls?.Dispose(); _cls = null;
                    _rec?.Dispose(); _rec = null;
                    opts.Dispose();

                    opts = MakeOpts(useDirectML: false);
                    _det = new InferenceSession(detPath, opts);
                    _cls = new InferenceSession(clsPath, opts);
                    _rec = new InferenceSession(recPath, opts);
                    _deviceLabel = "CPU";
                }

                _detInput = _det.InputNames[0];
                _clsInput = _cls.InputNames[0];
                _recInput = _rec.InputNames[0];

                Console.WriteLine($"[RapidOCR] Ready on {_deviceLabel}. Inputs: det={_detInput} cls={_clsInput} rec={_recInput}");

                // Warm-up: fire one dummy inference per session to compile GPU shaders / JIT
                WarmUpSession(_det, _detInput, new[] { 1, 3, DetSize, DetSize });
                WarmUpSession(_cls, _clsInput, new[] { 1, 3, ClsHeight, ClsWidth });
                WarmUpSession(_rec, _recInput, new[] { 1, 3, RecHeight, 320 });

                Console.WriteLine("[RapidOCR] Warm-up complete.");
            }
            catch (Exception ex)
            {
                _initError = ex.Message;
                Console.WriteLine($"[RapidOCR] Init FAILED: {ex}");
                _det?.Dispose(); _det = null;
                _cls?.Dispose(); _cls = null;
                _rec?.Dispose(); _rec = null;
            }
            _initialised = true;
        }
    }

    private static void WarmUpSession(InferenceSession session, string inputName, int[] shape)
    {
        var t = new DenseTensor<float>(shape);
        using var _ = session.Run(new[] { NamedOnnxValue.CreateFromTensor(inputName, t) });
    }

    private static SessionOptions MakeOpts(bool useDirectML)
    {
        var opts = new SessionOptions
        {
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
            ExecutionMode          = ExecutionMode.ORT_SEQUENTIAL,
        };
        if (useDirectML)
            opts.AppendExecutionProvider_DML(0);
        return opts;
    }
}
