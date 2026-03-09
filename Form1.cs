using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OcrPro;

// ── OCR result data ───────────────────────────────────────────────────────────
record OcrResult(string RawText, long ElapsedMs,
                 IReadOnlyList<RoiMatch> Rois, IReadOnlyList<WordEntry> Words);

public partial class Form1 : Form
{
    [DllImport("user32.dll")] static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
    private const int SB_VERT = 1;
    private const int SB_BOTH = 3;
    private static void HideScrollBars(Control c) =>
        ShowScrollBar(c.Handle, SB_BOTH, false);
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
        OcrEngineType.PaddleOcr,
    };

    public Form1()
    {
        InitializeComponent();
        Load              += Form1_Load;
        Shown             += (_, _) => PaddleOcrEngine.WarmUpInBackground();
        FormClosing       += Form1_FormClosing;
        resultsScroll.Resize += (_, _) => { if (_lastResult != null) RenderResultCards(_lastResult); };
        leftFlow.ClientSizeChanged += (_, _) => SyncRawOcrCardHeight();
        leftScroll.ClientSizeChanged += (_, _) => SyncRawOcrCardHeight();
        // Hide scrollbars whenever layout is recalculated
        resultsScroll.Layout += (_, _) => HideScrollBars(resultsScroll);
        leftFlow.Layout      += (_, _) => HideScrollBars(leftFlow);
        leftScroll.Layout    += (_, _) => HideScrollBars(leftScroll);
    }

    // Resize RawOcrCard to fill all remaining vertical space in leftFlow above the Run OCR button.
    private void SyncRawOcrCardHeight()
    {
        // Sum the heights of all cards above RawOcrCard
        int used = 0;
        foreach (Control c in leftFlow.Controls)
        {
            if (c == RawOcrCard) break;
            used += c.Height + c.Margin.Vertical;
        }
        used += leftFlow.Padding.Vertical;

        int available = leftScroll.ClientSize.Height - btnRunOcr.Height - used;
        int newCardH  = Math.Max(available, 120);

        if (RawOcrCard.Height == newCardH) return;
        RawOcrCard.Height = newCardH;
        // txtRawOcr fills the card below the title label
        txtRawOcr.Height = Math.Max(newCardH - 64 - 16, 40);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOAD
    // ═══════════════════════════════════════════════════════════════════════════
    private void Form1_Load(object? sender, EventArgs e)
    {
        // PaddleOCR warmup is triggered on Shown (after HWND is created) to avoid
        // the MKLDNN background-init deadlock that occurs when a modal dialog is open.
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // LOAD IMAGE (file)
    // ═══════════════════════════════════════════════════════════════════════════
    private async void btnLoadImage_Click(object? sender, EventArgs e)
    {
        btnLoadImage.Enabled = false;
        string? path = null;
        try
        {
            // Run OpenFileDialog on its own STA thread so the Vista-style shell
            // dialog cannot deadlock the main UI message pump.
            var tcs = new TaskCompletionSource<string?>();
            var t = new Thread(() =>
            {
                using var dlg = new OpenFileDialog
                {
                    Title  = "Select an image",
                    Filter = "Image Files|*.bmp;*.png;*.jpg;*.jpeg;*.tiff;*.tif;*.gif|All Files|*.*",
                };
                tcs.SetResult(dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
            path = await tcs.Task;
        }
        finally
        {
            btnLoadImage.Enabled = true;
        }

        if (path == null) return;
        _imagePath = path;

        Bitmap? bmp = null;
        try
        {
            var img = Image.FromFile(_imagePath);
            bmp = new Bitmap(img);
            img.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to decode image:\n{ex.Message}", "Image Load Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        lock (_frameLock) { _currentFrame?.Dispose(); _currentFrame = bmp; }
        pictureBox.Image = bmp;

        SetProcStatus("Processing: Image loaded");
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

    private void RunOcrOnBitmap(Bitmap snapshot)
    {
        var engineType = EngineOrder[Math.Clamp(cmbEngine.SelectedIndex, 0, EngineOrder.Length - 1)];

        btnRunOcr.Enabled  = false;
        SetProcStatus("Processing: Running OCR…");
        lblProcTime.Text = "…";
        ClearResultCards();

        // Run OCR on background thread so UI stays responsive
        var worker = new BackgroundWorker();
        worker.DoWork += (_, args) =>
        {
            args.Result = engineType switch
            {
                OcrEngineType.WinRt    => WinRtOcrEngine.RunAsync(snapshot).GetAwaiter().GetResult(),
                OcrEngineType.PaddleOcr => PaddleOcrEngine.Run(snapshot),
                _ => WinRtOcrEngine.RunAsync(snapshot).GetAwaiter().GetResult(),
            };
        };
        worker.RunWorkerCompleted += (_, args) =>
        {
            snapshot.Dispose();
            btnRunOcr.Enabled  = true;

            if (args.Error != null)
            {
                SetProcStatus("Processing: Error");
                lblProcTime.Text = "ERR";
                MessageBox.Show($"OCR failed:\n{args.Error.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (args.Result is OcrResult result)
            {
                _lastResult      = result;
                lblProcTime.Text = $"{result.ElapsedMs}ms";
                SetProcStatus("Processing: Done");
                RenderResultCards(result);
                txtRawOcr.Text = string.IsNullOrWhiteSpace(result.RawText)
                    ? "(no text detected)"
                    : result.RawText;
            }
        };
        worker.RunWorkerAsync();
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
        const int cardH    = 100;
        const int leftBar  = 4;
        const int padL     = 12;
        const int padR     = 10;
        const int rowLabel = 12;
        const int labelH   = 22;
        const int rowValue = 42;
        const int valH     = 40;

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
            e.Graphics.FillRectangle(bar, 0, 0, leftBar, card.Height);
            e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
        };

        var lblTag = new Label
        {
            Text         = label,
            ForeColor    = ClrSubText,
            Font         = new Font("Consolas", 13f, FontStyle.Regular),
            AutoSize     = false,
            Left         = padL + leftBar,
            Top          = rowLabel,
            Width        = card.Width - padL - leftBar - padR,
            Height       = labelH,
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
    }
}
