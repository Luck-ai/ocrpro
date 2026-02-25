using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using Basler.Pylon;
using Emgu.CV;
using WinDE = Windows.Devices.Enumeration;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Tesseract;

namespace OcrTesseract;

// ── OCR result data ───────────────────────────────────────────────────────────
record OcrResult(string RawText, float Confidence, long ElapsedMs,
                 IReadOnlyList<RoiMatch> Rois, IReadOnlyList<WordEntry> Words);

public partial class Form1 : Form
{
    // ── Colours reused for result cards ──────────────────────────────────────
    private static readonly Color ClrPanel     = Color.FromArgb(36, 39, 46);
    private static readonly Color ClrBorder    = Color.FromArgb(55, 60, 70);
    private static readonly Color ClrGreen     = Color.FromArgb(57, 255, 20);
    private static readonly Color ClrGreenDim  = Color.FromArgb(0, 200, 83);
    private static readonly Color ClrOrange    = Color.FromArgb(255, 152, 0);
    private static readonly Color ClrText      = Color.FromArgb(220, 220, 220);
    private static readonly Color ClrSubText   = Color.FromArgb(140, 145, 155);
    private static readonly Color ClrAccent    = Color.FromArgb(48, 52, 62);

    // ROI accent colours (one per slot)
    private static readonly Color[] RoiAccentColors =
    {
        Color.FromArgb(57, 255, 20),   // ROI A – green
        Color.FromArgb(0,  180, 255),  // ROI B – cyan
        Color.FromArgb(255, 152,  0),  // ROI C – orange
    };

    // ── Camera source type ────────────────────────────────────────────────────
    private enum CameraSourceType { Webcam, Pylon }
    private record CameraEntry(CameraSourceType Type, int WebcamIndex, ICameraInfo? PylonInfo);

    // ── State ─────────────────────────────────────────────────────────────────
    private string?          _imagePath;
    private Bitmap?          _currentFrame;
    private readonly List<CameraEntry> _cameraList = new();
    private readonly List<int> _cameraDeviceIndices = new();  // kept for StartCamera compat
    private VideoCapture?    _videoCapture;
    private Camera?          _pylonCamera;
    private readonly PixelDataConverter _pylonConverter = new();
    private Thread?          _captureThread;
    private bool             _cameraRunning;
    private int              _frameCount;
    private OcrResult?       _lastResult;
    private readonly System.Windows.Forms.Timer _fpsTimer = new() { Interval = 1000 };
    private readonly object  _frameLock = new();

    // Which engine index maps to which type (matches cmbEngine order)
    private static readonly OcrEngineType[] EngineOrder =
    {
        OcrEngineType.WinRt,
        OcrEngineType.PaddleOcr,
        OcrEngineType.Tesseract,
    };

    public Form1()
    {
        InitializeComponent();
        _fpsTimer.Tick    += FpsTimer_Tick;
        Load              += Form1_Load;
        FormClosing       += Form1_FormClosing;
        resultsScroll.Resize += (_, _) => { if (_lastResult != null) RenderResultCards(_lastResult); };
        leftScroll.Resize += (_, _) => AdjustResultsScrollHeight();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOAD
    // ═══════════════════════════════════════════════════════════════════════════
    private async void Form1_Load(object? sender, EventArgs e)
    {
        await DetectCamerasAsync();
        PaddleOcrEngine.WarmUpInBackground();   // initialise model while user sets up image/camera
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CAMERA DETECTION
    // ═══════════════════════════════════════════════════════════════════════════
    private async Task DetectCamerasAsync()
    {
        cmbCamera.Items.Clear();
        cmbCamera.Items.Add("Detecting…");
        cmbCamera.SelectedIndex = 0;
        cmbCamera.Enabled = btnCamera.Enabled = false;

        var entries = await Task.Run(async () =>
        {
            var result = new List<CameraEntry>();

            // ── 1. Pylon / GigE cameras ──────────────────────────────────────
            try
            {
                List<ICameraInfo> pylonCameras = CameraFinder.Enumerate();
                foreach (var info in pylonCameras)
                {
                    result.Add(new CameraEntry(CameraSourceType.Pylon, -1, info));
                }
            }
            catch { /* Pylon runtime not available or no cameras */ }

            // ── 2. DirectShow webcams (with friendly names via WinRT) ─────────
            WinDE.DeviceInformationCollection? devices = null;
            try { devices = await WinDE.DeviceInformation.FindAllAsync(WinDE.DeviceClass.VideoCapture); }
            catch { }

            if (devices != null)
            {
                for (int i = 0; i < Math.Min(devices.Count, 8); i++)
                {
                    try
                    {
                        using var probe = new VideoCapture(i, VideoCapture.API.DShow);
                        if (probe.IsOpened)
                            result.Add(new CameraEntry(CameraSourceType.Webcam, i, null));
                    }
                    catch { }
                }
                // Attach friendly names: match by position among opened webcams
                int wi = 0;
                for (int i = result.Count - 1; i >= 0; i--)
                    if (result[i].Type == CameraSourceType.Webcam) wi++;
                // Re-enumerate to pair index with name
                int nameIdx = 0;
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].Type == CameraSourceType.Webcam && nameIdx < devices.Count)
                    {
                        string name = devices[nameIdx].Name;
                        result[i] = result[i] with { WebcamIndex = result[i].WebcamIndex };
                        // Store friendly name alongside – we'll use a parallel list
                        _ = name; // name is stored below via display string
                        nameIdx++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    try
                    {
                        using var probe = new VideoCapture(i, VideoCapture.API.DShow);
                        if (!probe.IsOpened) break;
                        result.Add(new CameraEntry(CameraSourceType.Webcam, i, null));
                    }
                    catch { break; }
                }
            }

            return (result, devices);
        });

        cmbCamera.Items.Clear();
        _cameraList.Clear();
        _cameraDeviceIndices.Clear();

        var (found, winDevices) = entries;

        if (found.Count > 0)
        {
            int webcamDevIdx = 0;
            foreach (var entry in found)
            {
                string displayName;
                if (entry.Type == CameraSourceType.Pylon && entry.PylonInfo != null)
                {
                    string model  = entry.PylonInfo[CameraInfoKey.ModelName]       ?? "Pylon Camera";
                    string serial = entry.PylonInfo[CameraInfoKey.SerialNumber]    ?? "";
                    string devType = entry.PylonInfo[CameraInfoKey.DeviceType]     ?? "";
                    displayName = $"[{devType}] {model} ({serial})";
                }
                else
                {
                    string friendlyName = winDevices != null && webcamDevIdx < winDevices.Count
                        ? winDevices[webcamDevIdx].Name
                        : $"Camera {entry.WebcamIndex}";
                    displayName = friendlyName;
                    webcamDevIdx++;
                }

                cmbCamera.Items.Add(displayName);
                _cameraList.Add(entry);
                if (entry.Type == CameraSourceType.Webcam)
                    _cameraDeviceIndices.Add(entry.WebcamIndex);
            }

            cmbCamera.SelectedIndex = 0;
            cmbCamera.Enabled = btnCamera.Enabled = true;
            SetCameraStatus(connected: false, deviceName: cmbCamera.Items[0]?.ToString());
        }
        else
        {
            cmbCamera.Items.Add("(no camera found)");
            cmbCamera.SelectedIndex = 0;
            SetCameraStatus(connected: false, deviceName: null);
        }
    }

    private void SetCameraStatus(bool connected, string? deviceName)
    {
        if (connected)
        {
            lblCamStatus.Text      = "● Camera Connected";
            lblCamStatus.ForeColor = ClrGreen;
        }
        else if (deviceName != null)
        {
            lblCamStatus.Text      = $"● Camera: {deviceName} (not started)";
            lblCamStatus.ForeColor = ClrOrange;
        }
        else
        {
            lblCamStatus.Text      = "● Camera: No device found";
            lblCamStatus.ForeColor = Color.FromArgb(200, 60, 60);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOAD IMAGE (file)
    // ═══════════════════════════════════════════════════════════════════════════
    private void btnLoadImage_Click(object? sender, EventArgs e)
    {
        if (_cameraRunning) StopCamera();

        using var dlg = new OpenFileDialog
        {
            Title  = "Select an image",
            Filter = "Image Files|*.bmp;*.png;*.jpg;*.jpeg;*.tiff;*.tif;*.gif|All Files|*.*"
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        _imagePath = dlg.FileName;
        try
        {
            var bmp = new Bitmap(Image.FromFile(_imagePath));
            lock (_frameLock) { _currentFrame?.Dispose(); _currentFrame = bmp; }
            pictureBox.Image = bmp;
            prepTab.SetSource(bmp);   // feed into preprocessing tab
            SetProcStatus("Processing: Image loaded");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load image:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // WEBCAM
    // ═══════════════════════════════════════════════════════════════════════════
    private void btnCamera_Click(object? sender, EventArgs e)
    {
        if (_cameraRunning)
            StopCamera();
        else
            StartCamera();
    }

    private void btnCapture_Click(object? sender, EventArgs e)
    {
        if (!_cameraRunning) return;

        Bitmap? snapshot;
        lock (_frameLock)
            snapshot = _currentFrame == null ? null : (Bitmap)_currentFrame.Clone();

        if (snapshot == null)
        {
            MessageBox.Show("No frame available yet.", "No Frame",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        prepTab.SetSource((Bitmap)snapshot.Clone());   // feed captured frame into preprocessing tab
        RunOcrOnBitmap(snapshot);
    }

    private void StartCamera()
    {
        int comboIdx  = Math.Max(cmbCamera.SelectedIndex, 0);
        string cameraName = cmbCamera.SelectedItem?.ToString() ?? "Camera";

        CameraEntry? entry = comboIdx < _cameraList.Count ? _cameraList[comboIdx] : null;

        if (entry?.Type == CameraSourceType.Pylon)
            StartPylonCamera(entry.PylonInfo!, cameraName);
        else
            StartWebcam(entry?.WebcamIndex ?? comboIdx, cameraName);
    }

    private void StartWebcam(int cameraIndex, string cameraName)
    {
        VideoCapture cap;
        try
        {
            cap = new VideoCapture(cameraIndex, VideoCapture.API.DShow);
            if (!cap.IsOpened)
            {
                cap.Dispose();
                MessageBox.Show($"Could not open {cameraName}.", "Camera",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Camera error:\n{ex.Message}", "Camera",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _videoCapture  = cap;
        _cameraRunning = true;
        _frameCount    = 0;
        _fpsTimer.Start();
        SetCameraStatus(connected: true, deviceName: cameraName);
        SetProcStatus("Processing: Live");
        btnCamera.Text     = "⏹  Stop Camera";
        btnCapture.Enabled = true;
        cmbCamera.Enabled  = false;

        _captureThread = new Thread(CaptureLoop)
        {
            IsBackground = true,
            Name         = "EmguCVCapture"
        };
        _captureThread.Start();
    }

    private void StartPylonCamera(ICameraInfo pylonInfo, string cameraName)
    {
        try
        {
            var cam = new Camera(pylonInfo);
            cam.CameraOpened += Configuration.AcquireContinuous;
            cam.Open();
            cam.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(5);
            cam.StreamGrabber.Start();
            _pylonCamera = cam;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Pylon camera error:\n{ex.Message}", "Camera",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _cameraRunning = true;
        _frameCount    = 0;
        _fpsTimer.Start();
        SetCameraStatus(connected: true, deviceName: cameraName);
        SetProcStatus("Processing: Live");
        btnCamera.Text     = "⏹  Stop Camera";
        btnCapture.Enabled = true;
        cmbCamera.Enabled  = false;

        _captureThread = new Thread(PylonCaptureLoop)
        {
            IsBackground = true,
            Name         = "PylonCapture"
        };
        _captureThread.Start();
    }

    private void StopCamera()
    {
        _cameraRunning = false;          // signals capture loops to exit
        _fpsTimer.Stop();

        _captureThread?.Join(3000);
        _captureThread = null;

        _videoCapture?.Dispose();
        _videoCapture = null;

        if (_pylonCamera != null)
        {
            try
            {
                if (_pylonCamera.StreamGrabber.IsGrabbing) _pylonCamera.StreamGrabber.Stop();
                _pylonCamera.Close();
            }
            catch { }
            _pylonCamera.Dispose();
            _pylonCamera = null;
        }

        string selectedName = cmbCamera.SelectedItem?.ToString() ?? "Camera 0";
        lblFps.Text        = "FPS: --";
        btnCamera.Text     = "📷  Start Camera";
        btnCapture.Enabled = false;
        cmbCamera.Enabled  = true;
        SetCameraStatus(connected: false, deviceName: selectedName);
        SetProcStatus("Processing: Ready");
    }

    private void CaptureLoop()
    {
        using var mat = new Emgu.CV.Mat();
        while (_cameraRunning)
        {
            if (_videoCapture == null || !_videoCapture.Read(mat) || mat.IsEmpty)
            {
                Thread.Sleep(10);
                continue;
            }

            Interlocked.Increment(ref _frameCount);

            // mat.ToBitmap() is provided by Emgu.CV.Bitmap extension package
            var forState   = mat.ToBitmap();
            var forDisplay = (Bitmap)forState.Clone();

            lock (_frameLock) { _currentFrame?.Dispose(); _currentFrame = forState; }

            if (pictureBox.IsHandleCreated)
                pictureBox.BeginInvoke(() =>
                {
                    var old = pictureBox.Image;
                    pictureBox.Image = forDisplay;
                    old?.Dispose();
                });
            else
                forDisplay.Dispose();
        }
    }

    private void PylonCaptureLoop()
    {
        while (_cameraRunning)
        {
            var cam = _pylonCamera;
            if (cam == null || !cam.StreamGrabber.IsGrabbing)
            {
                Thread.Sleep(10);
                continue;
            }

            try
            {
                using IGrabResult grabResult = cam.StreamGrabber.RetrieveResult(500, TimeoutHandling.Return);
                if (grabResult == null || !grabResult.GrabSucceeded) continue;

                // Convert to a 32-bit BGRA Bitmap
                var bmp = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
                BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    bmp.PixelFormat);
                _pylonConverter.OutputPixelFormat = PixelType.BGRA8packed;
                _pylonConverter.Convert(bmpData.Scan0, bmpData.Stride * bmp.Height, grabResult);
                bmp.UnlockBits(bmpData);

                Interlocked.Increment(ref _frameCount);

                var forDisplay = (Bitmap)bmp.Clone();
                lock (_frameLock) { _currentFrame?.Dispose(); _currentFrame = bmp; }

                if (pictureBox.IsHandleCreated)
                    pictureBox.BeginInvoke(() =>
                    {
                        var old = pictureBox.Image;
                        pictureBox.Image = forDisplay;
                        old?.Dispose();
                    });
                else
                    forDisplay.Dispose();
            }
            catch (TimeoutException) { /* no frame yet – keep looping */ }
            catch { Thread.Sleep(10); }
        }
    }

    private void FpsTimer_Tick(object? sender, EventArgs e)
    {
        int fps = Interlocked.Exchange(ref _frameCount, 0);
        if (lblFps.IsHandleCreated)
            lblFps.BeginInvoke(() => lblFps.Text = $"FPS: {fps}");
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RUN OCR
    // ═══════════════════════════════════════════════════════════════════════════
    private void btnRunOcr_Click(object? sender, EventArgs e)
    {
        if (_currentFrame == null)
        {
            MessageBox.Show("Load an image or capture a camera frame first.", "No Source",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Bitmap? snapshot;
        lock (_frameLock)
            snapshot = _currentFrame == null ? null : (Bitmap)_currentFrame.Clone();

        if (snapshot == null)
        {
            MessageBox.Show("No frame available yet.", "No Frame",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        RunOcrOnBitmap(snapshot);
    }

    private void RunOcrOnBitmap(Bitmap snapshot, bool alreadyPreprocessed = false)
    {
        var engineType = EngineOrder[Math.Clamp(cmbEngine.SelectedIndex, 0, EngineOrder.Length - 1)];

        // Tesseract needs tessdata path validation up-front
        string? tessDataPath = null;
        if (engineType == OcrEngineType.Tesseract)
        {
            tessDataPath = Path.Combine(Application.StartupPath, "tessdata");
            if (!Directory.Exists(tessDataPath))
            {
                MessageBox.Show(
                    $"tessdata folder not found at:\n{tessDataPath}\n\n" +
                    "Download eng.traineddata from https://github.com/tesseract-ocr/tessdata",
                    "Missing tessdata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                snapshot.Dispose();
                return;
            }
        }

        bool skipPreprocessing = alreadyPreprocessed || !chkEnablePrep.Checked;

        btnRunOcr.Enabled  = false;
        btnCapture.Enabled = false;
        SetProcStatus(skipPreprocessing ? "Processing: Running OCR…" : "Processing: Preprocessing…");
        lblProcTime.Text = "…";
        ClearResultCards();

        // Preprocess on background thread so UI stays responsive
        Bitmap? preprocessed = null;
        var worker = new BackgroundWorker();
        worker.DoWork += (_, args) =>
        {
            preprocessed = skipPreprocessing
                ? (Bitmap)snapshot.Clone()
                : ImagePreprocessor.Process(snapshot);
            args.Result = engineType switch
            {
                OcrEngineType.WinRt    => WinRtOcrEngine.RunAsync(preprocessed).GetAwaiter().GetResult(),
                OcrEngineType.PaddleOcr => PaddleOcrEngine.Run(preprocessed),
                _                      => RunTesseract(preprocessed, tessDataPath!),
            };
        };
        worker.RunWorkerCompleted += (_, args) =>
        {
            snapshot.Dispose();
            btnRunOcr.Enabled  = true;
            btnCapture.Enabled = _cameraRunning;

            if (args.Error != null)
            {
                preprocessed?.Dispose();
                SetProcStatus("Processing: Error");
                lblProcTime.Text = "ERR";
                MessageBox.Show($"OCR failed:\n{args.Error.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show the preprocessed (binarised) image so the user can see what the engine received
            if (preprocessed != null)
            {
                var old = pictureBox.Image;
                pictureBox.Image = preprocessed;
                if (!ReferenceEquals(old, _currentFrame)) old?.Dispose();
            }

            if (args.Result is OcrResult result)
            {
                _lastResult      = result;
                lblProcTime.Text = $"{result.ElapsedMs}ms";
                SetProcStatus("Processing: Done");
                RenderResultCards(result);
                DrawRoiOverlays(result);
                txtRawOcr.Text = string.IsNullOrWhiteSpace(result.RawText)
                    ? "(no text detected)"
                    : result.RawText;
            }
        };
        worker.RunWorkerAsync();
    }

    // ── Bitmap → Pix without PNG round-trip ──────────────────────────────────
    // BMP is uncompressed — encode/decode is a plain memory copy, ~10x faster than PNG.
    private static Pix BitmapToPix(Bitmap bmp)
    {
        using var ms = new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        return Pix.LoadFromMemory(ms.ToArray());
    }

    // ── Cached Tesseract engine (construction costs ~300–500 ms, so reuse it) ──
    private static TesseractEngine? _tessEngine;
    private static readonly object  _tessLock = new();

    private static TesseractEngine GetOrCreateTessEngine(string tessDataPath)
    {
        if (_tessEngine != null) return _tessEngine;
        lock (_tessLock)
        {
            _tessEngine ??= new TesseractEngine(tessDataPath, "eng", EngineMode.LstmOnly);
            return _tessEngine;
        }
    }

    // ── Core Tesseract call ───────────────────────────────────────────────────
    private static OcrResult RunTesseract(Bitmap bmp, string tessDataPath)
    {
        var sw = Stopwatch.StartNew();

        var engine = GetOrCreateTessEngine(tessDataPath);

        // Direct Bitmap → Pix without a PNG encode/decode round-trip (~30–80 ms saved)
        using var pix  = BitmapToPix(bmp);
        using var page = engine.Process(pix, PageSegMode.Auto);
        float  conf = page.GetMeanConfidence();
        string text = page.GetText().Trim();

        var words = new List<WordEntry>();
        using (var iter = page.GetIterator())
        {
            iter.Begin();
            do
            {
                if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                {
                    string wordText = iter.GetText(PageIteratorLevel.Word)?.Trim() ?? "";
                    float  wordConf = iter.GetConfidence(PageIteratorLevel.Word) / 100f;
                    if (wordText.Length > 0)
                        words.Add(new WordEntry(
                            wordText,
                            wordConf,
                            new Rectangle(rect.X1, rect.Y1,
                                          rect.X2 - rect.X1,
                                          rect.Y2 - rect.Y1)));
                }
            }
            while (iter.Next(PageIteratorLevel.Word));
        }

        var rois = RoiExtractor.Extract(words);

        sw.Stop();
        return new OcrResult(text, conf, sw.ElapsedMilliseconds, rois, words);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // ROI OVERLAY
    // ═══════════════════════════════════════════════════════════════════════════
    private void DrawRoiOverlays(OcrResult result)
    {
        if (result.Rois.Count == 0) return;

        var src = pictureBox.Image as Bitmap;
        if (src == null) return;

        var overlay = new Bitmap(src);
        using var g = Graphics.FromImage(overlay);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        for (int i = 0; i < result.Rois.Count; i++)
        {
            var roi    = result.Rois[i];
            var color  = RoiAccentColors[i % RoiAccentColors.Length];
            var rect   = roi.Bounds;
            if (rect.IsEmpty || rect.Width <= 0 || rect.Height <= 0) continue;

            rect.Inflate(4, 4);
            rect.Intersect(new Rectangle(0, 0, overlay.Width, overlay.Height));

            using var pen       = new Pen(color, 2.5f);
            using var tagBrush  = new SolidBrush(Color.FromArgb(210, color));
            using var font      = new Font("Consolas", 9f, FontStyle.Bold);

            g.DrawRectangle(pen, rect);

            string tag = roi.RoiLabel.Length > 0 ? roi.RoiLabel : roi.FieldName;
            var tagSize = g.MeasureString(tag, font);
            var tagRect = new RectangleF(rect.Left, rect.Top - tagSize.Height - 2,
                                         tagSize.Width + 6, tagSize.Height + 2);
            if (tagRect.Top < 0) tagRect.Y = rect.Bottom + 2;

            g.FillRectangle(tagBrush, tagRect);
            g.DrawString(tag, font, Brushes.Black,
                         tagRect.Left + 3, tagRect.Top + 1);
        }

        var old = pictureBox.Image;
        pictureBox.Image = overlay;
        old?.Dispose();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RESULT CARDS
    // ═══════════════════════════════════════════════════════════════════════════
    private void ClearResultCards() => resultsFlow.Controls.Clear();

    private void RenderResultCards(OcrResult result)
    {
        resultsFlow.SuspendLayout();
        resultsFlow.Controls.Clear();

        float confPct = result.Confidence * 100f;

        if (result.Rois.Count > 0)
        {
            for (int i = 0; i < result.Rois.Count; i++)
            {
                var roi      = result.Rois[i];
                float roiPct = roi.Confidence * 100f;
                bool  low    = roiPct < 60f;
                var   accent = RoiAccentColors[i % RoiAccentColors.Length];

                resultsFlow.Controls.Add(BuildResultCard(
                    label:     roi.RoiLabel.Length > 0 ? roi.RoiLabel : $"ROI {(char)('A' + i)} – {roi.FieldName}",
                    value:     roi.Value,
                    confStr:   low ? "Low Conf" : $"{roiPct:F1}%",
                    confCol:   low ? ClrOrange : ClrGreenDim,
                    borderCol: accent
                ));
            }
        }
        else
        {
            var lines = result.RawText
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Take(20)
                .ToArray();

            if (lines.Length == 0) lines = new[] { "(no text detected)" };

            for (int i = 0; i < lines.Length; i++)
            {
                bool low = confPct < 60f;
                resultsFlow.Controls.Add(BuildResultCard(
                    label:     $"LINE {i + 1}",
                    value:     lines[i],
                    confStr:   low ? "Low Conf" : $"{confPct:F1}%",
                    confCol:   low ? ClrOrange : ClrGreenDim,
                    borderCol: low ? ClrOrange : ClrBorder
                ));
            }
        }

        resultsFlow.Controls.Add(BuildResultCard(
            label:     "CONFIDENCE",
            value:     $"{confPct:F1}%  —  {result.ElapsedMs} ms",
            confStr:   confPct >= 80 ? "High" : confPct >= 60 ? "Medium" : "Low",
            confCol:   confPct >= 80 ? ClrGreenDim : confPct >= 60 ? ClrOrange : Color.FromArgb(220, 60, 60),
            borderCol: confPct >= 80 ? ClrGreenDim : ClrOrange
        ));

        resultsFlow.ResumeLayout(true);
    }

    private Panel BuildResultCard(string label, string value, string confStr,
                                  Color confCol, Color borderCol)
    {
        const int cardH    = 82;
        const int leftBar  = 4;
        const int padL     = 12;
        const int padR     = 10;
        const int rowLabel = 10;
        const int rowValue = 34;
        const int valH     = 36;

        var card = new Panel
        {
            Width     = Math.Max(resultsScroll.ClientSize.Width - 4, 50),
            Height    = cardH,
            BackColor = ClrAccent,
            Margin    = new Padding(0, 0, 0, 6),
        };

        void SyncWidth(object? s, EventArgs _)
        {
            int w = Math.Max(resultsScroll.ClientSize.Width - 4, 50);
            if (card.Width != w)
            {
                card.Width = w;
                foreach (Control c in card.Controls)
                {
                    if (c.Tag is "conf")
                        c.Left = w - c.Width - padR;
                    else if (c.Tag is "val")
                        c.Width = w - padL - leftBar - padR;
                }
            }
        }
        resultsScroll.ClientSizeChanged += SyncWidth;
        card.Disposed += (_, _) => resultsScroll.ClientSizeChanged -= SyncWidth;

        card.Paint += (s, e) =>
        {
            using var bar    = new SolidBrush(borderCol);
            using var border = new Pen(ClrBorder, 1);
            e.Graphics.FillRectangle(bar, 0, 0, leftBar, cardH);
            e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, cardH - 1);
        };

        var lblTag = new Label
        {
            Text      = label,
            ForeColor = ClrSubText,
            Font      = new Font("Consolas", 8f, FontStyle.Regular),
            AutoSize  = false,
            Left      = padL + leftBar,
            Top       = rowLabel,
            Width     = card.Width - padL - leftBar - 90,
            Height    = 18,
            AutoEllipsis = true,
        };

        var lblConf = new Label
        {
            Text      = confStr,
            ForeColor = confCol,
            Font      = new Font("Consolas", 8.5f, FontStyle.Bold),
            AutoSize  = true,
            Tag       = "conf",
            Top       = rowLabel,
            Height    = 18,
        };
        lblConf.Left = card.Width - lblConf.PreferredWidth - padR;

        var lblVal = new Label
        {
            Text         = value,
            ForeColor    = ClrText,
            Font         = new Font("Consolas", 10f, FontStyle.Bold),
            AutoSize     = false,
            Left         = padL + leftBar,
            Top          = rowValue,
            Width        = card.Width - padL - leftBar - padR,
            Height       = valH,
            AutoEllipsis = true,
            Tag          = "val",
        };

        card.Controls.Add(lblTag);
        card.Controls.Add(lblConf);
        card.Controls.Add(lblVal);
        return card;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // TAB HANDLERS
    // ═══════════════════════════════════════════════════════════════════════════
    private void MainTabs_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var g   = e.Graphics;
        bool sel = e.Index == mainTabs.SelectedIndex;
        using var bgBrush = new SolidBrush(sel
            ? Color.FromArgb(36, 39, 46)
            : Color.FromArgb(20, 22, 27));
        using var fgBrush = new SolidBrush(sel
            ? Color.FromArgb(57, 255, 20)
            : Color.FromArgb(140, 145, 155));
        g.FillRectangle(bgBrush, e.Bounds);
        var text = mainTabs.TabPages[e.Index].Text;
        var sf   = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString(text, mainTabs.Font, fgBrush, e.Bounds, sf);
    }

    private void PrepTab_UseAsOcrInput(object? sender, Bitmap bmp)
    {
        lock (_frameLock) { _currentFrame?.Dispose(); _currentFrame = (Bitmap)bmp.Clone(); }
        pictureBox.Image = bmp;
        mainTabs.SelectedIndex = 0;   // switch back to OCR tab
        RunOcrOnBitmap((Bitmap)bmp.Clone(), alreadyPreprocessed: true);  // no double-preprocessing
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════════════════
    private void AdjustResultsScrollHeight()
    {
        // Cap resultsScroll height to leave room for other cards
        int available = leftScroll.ClientSize.Height;
        int desired = Math.Clamp(available / 3, 100, 300);
        if (resultsScroll.Height != desired)
            resultsScroll.Height = desired;
    }

    private void SetProcStatus(string text)
    {
        if (lblProcStatus.IsHandleCreated)
            lblProcStatus.BeginInvoke(() => lblProcStatus.Text = text);
        else
            lblProcStatus.Text = text;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // CLEANUP
    // ═══════════════════════════════════════════════════════════════════════════
    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        StopCamera();
        lock (_frameLock) { _currentFrame?.Dispose(); }
        lock (_tessLock)   { _tessEngine?.Dispose(); _tessEngine = null; }
    }
}
