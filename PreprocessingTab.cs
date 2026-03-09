namespace OcrTesseract;

public partial class PreprocessingTab : UserControl
{
    private Bitmap? _source;
    private Bitmap? _preview;

    public event EventHandler<Bitmap>? UseAsOcrInput;

    public PreprocessingTab()
    {
        InitializeComponent();

        // 1. Upscale
        trkUpscale.ValueChanged += (_, _) => lblUpscaleVal.Text = $"×{trkUpscale.Value / 10.0:F1}";
        btnApplyUpscale.Click += (_, _) => ApplyStep(0);

        // 2. Grayscale
        btnApplyGray.Click += (_, _) => ApplyStep(1);

        // 3. CLAHE
        trkClaheClip.ValueChanged += (_, _) => lblClaheClip.Text = $"{trkClaheClip.Value}.0";
        trkClaheTile.ValueChanged += (_, _) => lblClaheTile.Text = $"{trkClaheTile.Value}";
        btnApplyClahe.Click += (_, _) => ApplyStep(2);

        // 4. Deskew
        trkDeskewMax.ValueChanged += (_, _) => lblDeskewMax.Text = $"{trkDeskewMax.Value}°";
        btnApplyDeskew.Click += (_, _) => ApplyStep(3);

        // 5. Sharpen
        trkSharpenSig.ValueChanged += (_, _) => lblSharpenSig.Text = $"{trkSharpenSig.Value / 10.0:F1}";
        trkSharpenStr.ValueChanged += (_, _) => lblSharpenStr.Text = $"{trkSharpenStr.Value / 10.0:F1}";
        btnApplySharpen.Click += (_, _) => ApplyStep(4);

        // 6. Binarise
        trkBinThr.ValueChanged += (_, _) => lblBinThr.Text = trkBinThr.Value == 0 ? "Otsu" : $"{trkBinThr.Value}";
        btnApplyBinarise.Click += (_, _) => ApplyStep(5);

        // 7. Brightness / Contrast
        trkBrightness.ValueChanged += (_, _) => lblBrightnessVal.Text = $"{trkBrightness.Value:+#;-#;0}";
        trkContrast.ValueChanged   += (_, _) => lblContrastVal.Text   = $"{trkContrast.Value:+#;-#;0}";
        btnApplyBrightnessContrast.Click += (_, _) => ApplyStep(6);

        // Buttons
        btnApplyAll.Click += (_, _) => ApplyAll();
        btnUseAsOcr.Click += (_, _) => OnUseAsOcrInput();
        btnReset.Click += (_, _) => { if (_source != null) ShowPreview(_source); };
    }

    public void SetSource(Bitmap bmp)
    {
        _source?.Dispose();
        _source = (Bitmap)bmp.Clone();
        ShowPreview(_source);
        lblInfo.Text = $"Source: {bmp.Width}×{bmp.Height}";
    }

    private void ApplyStep(int step)
    {
        var src = _preview ?? _source;
        if (src == null) return;

        Bitmap? result = null;
        try
        {
            result = step switch
            {
                0 => ImagePreprocessor.ApplyUpscale(src, GetUpscaleParams()),
                1 => ImagePreprocessor.ApplyGrayscale(src, GetGrayscaleParams()),
                2 => ImagePreprocessor.ApplyClahe(src, GetClaheParams()),
                3 => ImagePreprocessor.ApplyDeskew(src, GetDeskewParams()),
                4 => ImagePreprocessor.ApplySharpen(src, GetSharpenParams()),
                5 => ImagePreprocessor.ApplyBinarise(src, GetBinariseParams()),
                6 => ImagePreprocessor.ApplyBrightnessContrast(src, GetBrightnessContrastParams()),
                _ => null,
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Step failed:\n{ex.Message}", "Preprocessing Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (result != null) ShowPreview(result);
    }

    private void ApplyAll()
    {
        if (_source == null) return;
        Bitmap? result = null;
        try
        {
            result = ImagePreprocessor.Process(_source,
                GetUpscaleParams(),
                GetGrayscaleParams(),
                GetClaheParams(),
                GetDeskewParams(),
                GetSharpenParams(),
                GetBinariseParams(),
                GetBrightnessContrastParams());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Pipeline failed:\n{ex.Message}", "Preprocessing Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        ShowPreview(result);
    }

    private void OnUseAsOcrInput()
    {
        if (_source == null) return;
        Bitmap? result = null;
        try
        {
        result = ImagePreprocessor.Process(_source,
            GetUpscaleParams(), GetGrayscaleParams(), GetClaheParams(),
            GetDeskewParams(), GetSharpenParams(), GetBinariseParams(),
            GetBrightnessContrastParams());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Pipeline failed:\n{ex.Message}", "Preprocessing Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        ShowPreview(result);
        UseAsOcrInput?.Invoke(this, (Bitmap)result.Clone());
    }

    private void ShowPreview(Bitmap bmp)
    {
        var old = _preview;
        _preview = (Bitmap)bmp.Clone();

        var oldBox = previewBox.Image;
        previewBox.Image = _preview;  // share the same instance; PictureBox doesn't modify it
        oldBox?.Dispose();

        if (old != null && !ReferenceEquals(old, _source)) old.Dispose();

        lblInfo.Text = $"{bmp.Width}×{bmp.Height} px";
    }

    private UpscaleParams GetUpscaleParams() => new(chkUpscale.Checked, trkUpscale.Value / 10.0);
    private GrayscaleParams GetGrayscaleParams() => new(chkGray.Checked);
    private ClaheParams GetClaheParams() => new(chkClahe.Checked, trkClaheClip.Value, trkClaheTile.Value);
    private DeskewParams GetDeskewParams() => new(chkDeskew.Checked, trkDeskewMax.Value);
    private SharpenParams GetSharpenParams() => new(chkSharpen.Checked, trkSharpenSig.Value / 10.0, trkSharpenStr.Value / 10.0);
    private BinariseParams GetBinariseParams() => new(chkBin.Checked, trkBinThr.Value);
    private BrightnessContrastParams GetBrightnessContrastParams() => new(chkBrightnessContrast.Checked, trkBrightness.Value, trkContrast.Value);
}
