using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using Emgu.CV;
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

    // ── State ─────────────────────────────────────────────────────────────────
    private string?          _imagePath;
    private Bitmap?          _currentFrame;
    private OcrResult?       _lastResult;
    private readonly object  _frameLock = new();

    // Which engine index maps to which type (matches cmbEngine order)
    private static readonly OcrEngineType[] EngineOrder =
    {
        OcrEngineType.WinRt,
        OcrEngineType.RapidOcr,
        OcrEngineType.PaddleOcr,
        OcrEngineType.Tesseract,
    };

    public Form1()
    {
        InitializeComponent();
        Load              += Form1_Load;
        FormClosing       += Form1_FormClosing;
        resultsScroll.Resize += (_, _) => { if (_lastResult != null) RenderResultCards(_lastResult); };
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOAD
    // ═══════════════════════════════════════════════════════════════════════════
    private void Form1_Load(object? sender, EventArgs e)
    {
        PaddleOcrEngine.WarmUpInBackground();   // initialise model while user sets up image
        RapidOcrEngine.WarmUpInBackground();    // load PP-OCRv4 ONNX models + DirectML session
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOAD IMAGE (file)
    // ═══════════════════════════════════════════════════════════════════════════
    private void btnLoadImage_Click(object? sender, EventArgs e)
    {
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
    // RUN OCR
    // ═══════════════════════════════════════════════════════════════════════════
    private void btnRunOcr_Click(object? sender, EventArgs e)
    {
        if (_currentFrame == null)
        {
            MessageBox.Show("Load an image first.", "No Source",
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
                OcrEngineType.WinRt     => WinRtOcrEngine.RunAsync(preprocessed).GetAwaiter().GetResult(),
                OcrEngineType.RapidOcr  => RapidOcrEngine.Run(preprocessed),
                OcrEngineType.PaddleOcr => PaddleOcrEngine.Run(preprocessed),
                _                       => RunTesseract(preprocessed, tessDataPath!),
            };
        };
        worker.RunWorkerCompleted += (_, args) =>
        {
            snapshot.Dispose();
            btnRunOcr.Enabled  = true;

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
        // ROI overlay drawing disabled – image is shown as-is
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RESULT CARDS
    // ═══════════════════════════════════════════════════════════════════════════
    private void ClearResultCards() => resultsFlow.Controls.Clear();

    private void RenderResultCards(OcrResult result)
    {
        resultsFlow.SuspendLayout();
        resultsFlow.Controls.Clear();

        if (result.Rois.Count > 0)
        {
            for (int i = 0; i < result.Rois.Count; i++)
            {
                var roi    = result.Rois[i];
                var accent = RoiAccentColors[i % RoiAccentColors.Length];
                resultsFlow.Controls.Add(BuildResultCard(
                    label:     roi.RoiLabel.Length > 0 ? roi.RoiLabel : $"ROI {(char)('A' + i)} – {roi.FieldName}",
                    value:     roi.Value,
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
                resultsFlow.Controls.Add(BuildResultCard(
                    label:     $"LINE {i + 1}",
                    value:     lines[i],
                    borderCol: ClrBorder
                ));
        }

        resultsFlow.Controls.Add(BuildResultCard(
            label:     "TIMING",
            value:     $"{result.ElapsedMs} ms",
            borderCol: ClrBorder
        ));

        resultsFlow.ResumeLayout(true);
    }

    private Panel BuildResultCard(string label, string value, Color borderCol)
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
                    if (c.Tag is "val")
                        c.Width = w - padL - leftBar - padR;
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
            Text         = label,
            ForeColor    = ClrSubText,
            Font         = new Font("Consolas", 10f, FontStyle.Regular),
            AutoSize     = false,
            Left         = padL + leftBar,
            Top          = rowLabel,
            Width        = card.Width - padL - leftBar - padR,
            Height       = 18,
            AutoEllipsis = true,
        };

        var lblVal = new Label
        {
            Text         = value,
            ForeColor    = ClrText,
            Font         = new Font("Consolas", 13f, FontStyle.Bold),
            AutoSize     = false,
            Left         = padL + leftBar,
            Top          = rowValue,
            Width        = card.Width - padL - leftBar - padR,
            Height       = valH,
            AutoEllipsis = true,
            Tag          = "val",
        };

        card.Controls.Add(lblTag);
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
        lock (_frameLock) { _currentFrame?.Dispose(); }
        lock (_tessLock)   { _tessEngine?.Dispose(); _tessEngine = null; }
    }
}
