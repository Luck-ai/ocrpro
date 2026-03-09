using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace OcrPro;

// ── Per-step parameter bags (used by both the auto pipeline and the manual tab) ──
record UpscaleParams   (bool Enabled, double Factor    = 2.0);
record GrayscaleParams (bool Enabled);
record ClaheParams     (bool Enabled, double Clip = 3.0, int TileSize = 8);
record DeskewParams    (bool Enabled, double MaxDeg = 15.0);
record SharpenParams   (bool Enabled, double Sigma = 1.0, double Strength = 1.2);
record BinariseParams  (bool Enabled, int Threshold = 0);  // 0 = Otsu
record BrightnessContrastParams (bool Enabled, int Brightness = 0, int Contrast = 0);  // -100..+100

/// <summary>
/// Emgu.CV preprocessing pipeline.
/// Call <see cref="Process"/> for the full automatic pipeline,
/// or call each static step method individually from the Preprocessing tab.
/// </summary>
internal static class ImagePreprocessor
{
    // ── Default auto-pipeline params ─────────────────────────────────────────
    public static readonly UpscaleParams   DefaultUpscale   = new(true,  2.0);
    public static readonly GrayscaleParams DefaultGrayscale = new(true);
    public static readonly ClaheParams     DefaultClahe     = new(true,  3.0, 8);
    public static readonly DeskewParams    DefaultDeskew    = new(true,  15.0);
    public static readonly SharpenParams   DefaultSharpen   = new(true,  1.0, 1.2);
    public static readonly BinariseParams  DefaultBinarise  = new(true,  0);
    public static readonly BrightnessContrastParams DefaultBrightnessContrast = new(false, 0, 0);

    // ─────────────────────────────────────────────────────────────────────────
    /// <summary>Full automatic pipeline — returns a binarised Bitmap ready for OCR.</summary>
    public static Bitmap Process(Bitmap src,
        UpscaleParams?   upscale   = null,
        GrayscaleParams? grayscale = null,
        ClaheParams?     clahe     = null,
        DeskewParams?    deskew    = null,
        SharpenParams?   sharpen   = null,
        BinariseParams?  binarise  = null,
        BrightnessContrastParams? brightnessContrast = null)
    {
        upscale             ??= DefaultUpscale;
        grayscale           ??= DefaultGrayscale;
        clahe               ??= DefaultClahe;
        deskew              ??= DefaultDeskew;
        sharpen             ??= DefaultSharpen;
        binarise            ??= DefaultBinarise;
        brightnessContrast  ??= DefaultBrightnessContrast;

        using var bgr  = BitmapToMat(src);
        using var up   = StepUpscale(bgr,   upscale);
        using var gray = StepGrayscale(up,  grayscale);
        using var eq   = StepClahe(gray,    clahe);
        using var desk = StepDeskew(eq,     deskew);
        using var shp  = StepSharpen(desk,  sharpen);
        using var bin  = StepBinarise(shp,  binarise);
        using var bc   = StepBrightnessContrast(bin, brightnessContrast);
        return MatToBitmap(bc);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // PUBLIC STEP METHODS  (callable individually from the Preprocessing tab)
    // ═════════════════════════════════════════════════════════════════════════

    public static Bitmap ApplyUpscale(Bitmap src, UpscaleParams p)
    {
        using var mat = BitmapToMat(src);
        using var out_ = StepUpscale(mat, p);
        return MatToBitmap(out_);
    }

    public static Bitmap ApplyGrayscale(Bitmap src, GrayscaleParams p)
    {
        using var mat  = BitmapToMat(src);
        using var out_ = StepGrayscale(mat, p);
        return MatToBitmap(out_);
    }

    public static Bitmap ApplyClahe(Bitmap src, ClaheParams p)
    {
        using var mat  = BitmapToMat(src);
        // CLAHE needs grayscale; convert if needed
        using var gray = EnsureGray(mat);
        using var out_ = StepClahe(gray, p);
        return MatToBitmap(out_);
    }

    public static Bitmap ApplyDeskew(Bitmap src, DeskewParams p)
    {
        using var mat  = BitmapToMat(src);
        using var gray = EnsureGray(mat);
        using var out_ = StepDeskew(gray, p);
        return MatToBitmap(out_);
    }

    public static Bitmap ApplySharpen(Bitmap src, SharpenParams p)
    {
        using var mat  = BitmapToMat(src);
        using var gray = EnsureGray(mat);
        using var out_ = StepSharpen(gray, p);
        return MatToBitmap(out_);
    }

    public static Bitmap ApplyBinarise(Bitmap src, BinariseParams p)
    {
        using var mat  = BitmapToMat(src);
        using var gray = EnsureGray(mat);
        using var out_ = StepBinarise(gray, p);
        return MatToBitmap(out_);
    }

    public static Bitmap ApplyBrightnessContrast(Bitmap src, BrightnessContrastParams p)
    {
        using var mat  = BitmapToMat(src);
        using var out_ = StepBrightnessContrast(mat, p);
        return MatToBitmap(out_);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // INTERNAL STEP IMPLEMENTATIONS  (operate on Mat, owned by caller)
    // ═════════════════════════════════════════════════════════════════════════

    internal static Mat StepUpscale(Mat src, UpscaleParams p)
    {
        if (!p.Enabled || Math.Min(src.Width, src.Height) >= 800)
            return src.Clone();
        var dst = new Mat();
        CvInvoke.Resize(src, dst, Size.Empty, p.Factor, p.Factor, Inter.Linear);
        return dst;
    }

    internal static Mat StepGrayscale(Mat src, GrayscaleParams p)
    {
        if (!p.Enabled) return src.Clone();
        if (src.NumberOfChannels == 1) return src.Clone();
        var dst = new Mat();
        CvInvoke.CvtColor(src, dst, ColorConversion.Bgr2Gray);
        return dst;
    }

    internal static Mat StepClahe(Mat src, ClaheParams p)
    {
        if (!p.Enabled) return src.Clone();
        var dst = new Mat();
        CvInvoke.CLAHE(src, p.Clip, new Size(p.TileSize, p.TileSize), dst);
        return dst;
    }

    internal static Mat StepDeskew(Mat gray, DeskewParams p)
    {
        if (!p.Enabled) return gray.Clone();

        using var edges = new Mat();
        CvInvoke.Canny(gray, edges, 50, 150);

        using var lines = new Mat();
        CvInvoke.HoughLinesP(edges, lines, 1, Math.PI / 180.0, 80, 60, 10);

        double angle = EstimateDominantAngle(lines, p.MaxDeg);
        if (Math.Abs(angle) < 0.5) return gray.Clone();

        var center = new PointF(gray.Width / 2f, gray.Height / 2f);
        using var rot = new Mat();
        CvInvoke.GetRotationMatrix2D(center, angle, 1.0, rot);

        var dst = new Mat();
        CvInvoke.WarpAffine(gray, dst, rot,
            new Size(gray.Width, gray.Height),
            Inter.Linear, Warp.Default, BorderType.Replicate);
        return dst;
    }

    internal static Mat StepSharpen(Mat gray, SharpenParams p)
    {
        if (!p.Enabled) return gray.Clone();
        using var blurred = new Mat();
        CvInvoke.GaussianBlur(gray, blurred, new Size(0, 0), p.Sigma);
        var dst = new Mat();
        CvInvoke.AddWeighted(gray, 1.0 + p.Strength, blurred, -p.Strength, 0, dst);
        return dst;
    }

    internal static Mat StepBinarise(Mat gray, BinariseParams p)
    {
        if (!p.Enabled) return gray.Clone();
        var dst = new Mat();
        var type = p.Threshold == 0
            ? ThresholdType.Binary | ThresholdType.Otsu
            : ThresholdType.Binary;
        CvInvoke.Threshold(gray, dst, p.Threshold, 255, type);
        return dst;
    }

    internal static Mat StepBrightnessContrast(Mat src, BrightnessContrastParams p)
    {
        if (!p.Enabled) return src.Clone();
        // alpha = contrast factor: 0 = grey slab, 1 = no change, >1 = more contrast
        // beta  = brightness offset in [0,255] space
        double alpha = (p.Contrast + 100.0) / 100.0;   // range 0.0 .. 2.0
        double beta  = p.Brightness;                    // range -100 .. +100
        var dst = new Mat();
        src.ConvertTo(dst, Emgu.CV.CvEnum.DepthType.Cv8U, alpha, beta);
        return dst;
    }

    // ═════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ═════════════════════════════════════════════════════════════════════════

    private static double EstimateDominantAngle(Mat lines, double maxDeg)
    {
        if (lines.IsEmpty || lines.Rows == 0) return 0.0;

        // HoughLinesP returns CV_32SC4 shaped [N, 1, 4] in Emgu.CV 4.x
        // GetData() gives a 3-D Array: [row, col=0, channel] → int[N, 1, 4]
        var raw    = lines.GetData();
        int rank   = raw.Rank;
        int count  = raw.GetLength(0);
        var angles = new List<double>(count);

        for (int i = 0; i < count; i++)
        {
            int x1, y1, x2, y2;
            if (rank == 3)
            {
                x1 = (int)raw.GetValue(i, 0, 0)!;
                y1 = (int)raw.GetValue(i, 0, 1)!;
                x2 = (int)raw.GetValue(i, 0, 2)!;
                y2 = (int)raw.GetValue(i, 0, 3)!;
            }
            else // rank == 2 fallback
            {
                x1 = (int)raw.GetValue(i, 0)!;
                y1 = (int)raw.GetValue(i, 1)!;
                x2 = (int)raw.GetValue(i, 2)!;
                y2 = (int)raw.GetValue(i, 3)!;
            }

            double dx = x2 - x1;
            double dy = y2 - y1;
            if (Math.Abs(dx) < 1e-6) continue;

            double deg = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            if (deg >  90) deg -= 180;
            if (deg < -90) deg += 180;
            if (Math.Abs(deg) > maxDeg) continue;
            angles.Add(deg);
        }

        if (angles.Count == 0) return 0.0;
        angles.Sort();
        return -angles[angles.Count / 2];
    }

    /// <summary>Ensure Mat is single-channel grayscale; returns a new Mat.</summary>
    private static Mat EnsureGray(Mat src)
    {
        if (src.NumberOfChannels == 1) return src.Clone();
        var dst = new Mat();
        CvInvoke.CvtColor(src, dst, ColorConversion.Bgr2Gray);
        return dst;
    }

    /// <summary>Convert any Bitmap to a 24-bpp BGR Mat.</summary>
    public static Mat BitmapToMat(Bitmap bmp)
    {
        if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
            return bmp.ToMat();
        using var canvas = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
        using (var g = Graphics.FromImage(canvas))
            g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
        return canvas.ToMat();
    }

    /// <summary>Convert a Mat (any channels) to a 24-bpp Bitmap.</summary>
    public static Bitmap MatToBitmap(Mat mat)
    {
        if (mat.NumberOfChannels == 1)
        {
            using var color = new Mat();
            CvInvoke.CvtColor(mat, color, ColorConversion.Gray2Bgr);
            return color.ToBitmap();
        }
        return mat.ToBitmap();
    }
}
