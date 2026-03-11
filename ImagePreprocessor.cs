using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace OcrPro;

record GrayscaleParams (bool Enabled);
record ClaheParams     (bool Enabled, double Clip = 3.0, int TileSize = 8);
record SharpenParams   (bool Enabled, double Sigma = 1.0, double Strength = 1.2);

internal static class ImagePreprocessor
{
    public static Bitmap ApplySharpen(Bitmap src, SharpenParams p)
    {
        using var mat  = BitmapToMat(src);
        using var gray = EnsureGray(mat);
        using var out_ = StepSharpen(gray, p);
        return MatToBitmap(out_);
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

    internal static Mat StepSharpen(Mat gray, SharpenParams p)
    {
        if (!p.Enabled) return gray.Clone();
        using var blurred = new Mat();
        CvInvoke.GaussianBlur(gray, blurred, new Size(0, 0), p.Sigma);
        var dst = new Mat();
        CvInvoke.AddWeighted(gray, 1.0 + p.Strength, blurred, -p.Strength, 0, dst);
        return dst;
    }

    private static Mat EnsureGray(Mat src)
    {
        if (src.NumberOfChannels == 1) return src.Clone();
        var dst = new Mat();
        CvInvoke.CvtColor(src, dst, ColorConversion.Bgr2Gray);
        return dst;
    }

    public static Mat BitmapToMat(Bitmap bmp)
    {
        if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
            return bmp.ToMat();
        using var canvas = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
        using (var g = Graphics.FromImage(canvas))
            g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
        return canvas.ToMat();
    }

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
