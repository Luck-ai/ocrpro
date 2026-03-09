namespace OcrPro;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        leftScroll = new Panel();
        leftFlow = new FlowLayoutPanel();
        InputCard = new Panel();
        lblTitleInputCard = new Label();
        btnLoadImage = new Button();
        EngineCard = new Panel();
        lblTitleEngineCard = new Label();
        cmbEngine = new ComboBox();
        TimingCard = new Panel();
        lblProcLabel = new Label();
        lblProcTime = new Label();
        resultsScroll = new Panel();
        resultsFlow = new FlowLayoutPanel();
        RawOcrCard = new Panel();
        lblTitleRawOcrCard = new Label();
        txtRawOcr = new TextBox();
        btnRunOcr = new Button();
        rightPanel = new Panel();
        pictureBox = new PictureBox();
        statusBar = new Panel();
        lblProcStatus = new Label();
        lblLighting = new Label();
        leftScroll.SuspendLayout();
        leftFlow.SuspendLayout();
        InputCard.SuspendLayout();
        EngineCard.SuspendLayout();
        TimingCard.SuspendLayout();
        resultsScroll.SuspendLayout();
        RawOcrCard.SuspendLayout();
        rightPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
        statusBar.SuspendLayout();
        SuspendLayout();
        // 
        // leftScroll
        // leftScroll holds btnRunOcr docked to bottom, and leftFlow (the cards) filling the rest
        // 
        leftScroll.AutoScroll = false;
        leftScroll.BackColor = Color.FromArgb(28, 30, 36);
        leftScroll.Controls.Add(leftFlow);
        leftScroll.Controls.Add(btnRunOcr);
        leftScroll.Dock = DockStyle.Left;
        leftScroll.Location = new Point(0, 0);
        leftScroll.Margin = new Padding(6);
        leftScroll.Name = "leftScroll";
        leftScroll.Padding = new Padding(0, 0, 7, 0);
        leftScroll.Size = new Size(631, 1231);
        leftScroll.TabIndex = 0;
        // 
        // btnRunOcr  (docked to bottom of leftScroll)
        // 
        btnRunOcr.BackColor = Color.FromArgb(57, 255, 20);
        btnRunOcr.Cursor = Cursors.Hand;
        btnRunOcr.Dock = DockStyle.Bottom;
        btnRunOcr.FlatAppearance.BorderSize = 0;
        btnRunOcr.FlatStyle = FlatStyle.Flat;
        btnRunOcr.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnRunOcr.ForeColor = Color.Black;
        btnRunOcr.Margin = new Padding(0);
        btnRunOcr.Name = "btnRunOcr";
        btnRunOcr.Size = new Size(624, 77);
        btnRunOcr.TabIndex = 1;
        btnRunOcr.Text = "Run OCR";
        btnRunOcr.UseVisualStyleBackColor = false;
        btnRunOcr.Click += btnRunOcr_Click;
        // 
        // leftFlow  (scrollable cards area above the button)
        // 
        leftFlow.AutoScroll = true;
        leftFlow.AutoSize = false;
        leftFlow.BackColor = Color.FromArgb(28, 30, 36);
        leftFlow.Controls.Add(InputCard);
        leftFlow.Controls.Add(EngineCard);
        leftFlow.Controls.Add(TimingCard);
        leftFlow.Controls.Add(resultsScroll);
        leftFlow.Controls.Add(RawOcrCard);
        leftFlow.Dock = DockStyle.Fill;
        leftFlow.FlowDirection = FlowDirection.TopDown;
        leftFlow.Location = new Point(0, 0);
        leftFlow.Margin = new Padding(6);
        leftFlow.Name = "leftFlow";
        leftFlow.Padding = new Padding(0, 6, 0, 6);
        leftFlow.TabIndex = 0;
        leftFlow.WrapContents = false;
        // 
        // InputCard
        // 
        InputCard.BackColor = Color.FromArgb(36, 39, 46);
        InputCard.Controls.Add(lblTitleInputCard);
        InputCard.Controls.Add(btnLoadImage);
        InputCard.Location = new Point(0, 6);
        InputCard.Margin = new Padding(0, 0, 0, 13);
        InputCard.Name = "InputCard";
        InputCard.Size = new Size(594, 140);
        InputCard.TabIndex = 0;
        InputCard.Paint += CardBorderPaint;
        // 
        // lblTitleInputCard
        // 
        lblTitleInputCard.AutoSize = true;
        lblTitleInputCard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTitleInputCard.ForeColor = Color.FromArgb(220, 220, 220);
        lblTitleInputCard.Location = new Point(19, 21);
        lblTitleInputCard.Margin = new Padding(6, 0, 6, 0);
        lblTitleInputCard.Name = "lblTitleInputCard";
        lblTitleInputCard.Size = new Size(111, 32);
        lblTitleInputCard.TabIndex = 0;
        lblTitleInputCard.Text = "1 · Input";
        // 
        // btnLoadImage
        // 
        btnLoadImage.BackColor = Color.FromArgb(48, 52, 62);
        btnLoadImage.Cursor = Cursors.Hand;
        btnLoadImage.FlatAppearance.BorderColor = Color.FromArgb(55, 60, 70);
        btnLoadImage.FlatStyle = FlatStyle.Flat;
        btnLoadImage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLoadImage.ForeColor = Color.FromArgb(220, 220, 220);
        btnLoadImage.Location = new Point(19, 64);
        btnLoadImage.Margin = new Padding(6);
        btnLoadImage.Name = "btnLoadImage";
        btnLoadImage.Size = new Size(557, 55);
        btnLoadImage.TabIndex = 1;
        btnLoadImage.Text = "Load Image";
        btnLoadImage.UseVisualStyleBackColor = false;
        btnLoadImage.Click += btnLoadImage_Click;
        // 
        // EngineCard
        // 
        EngineCard.BackColor = Color.FromArgb(36, 39, 46);
        EngineCard.Controls.Add(lblTitleEngineCard);
        EngineCard.Controls.Add(cmbEngine);
        EngineCard.Location = new Point(0, 153);
        EngineCard.Margin = new Padding(0, 0, 0, 13);
        EngineCard.Name = "EngineCard";
        EngineCard.Size = new Size(594, 140);
        EngineCard.TabIndex = 0;
        EngineCard.Paint += CardBorderPaint;
        // 
        // lblTitleEngineCard
        // 
        lblTitleEngineCard.AutoSize = true;
        lblTitleEngineCard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTitleEngineCard.ForeColor = Color.FromArgb(220, 220, 220);
        lblTitleEngineCard.Location = new Point(19, 21);
        lblTitleEngineCard.Margin = new Padding(6, 0, 6, 0);
        lblTitleEngineCard.Name = "lblTitleEngineCard";
        lblTitleEngineCard.Size = new Size(183, 32);
        lblTitleEngineCard.TabIndex = 0;
        lblTitleEngineCard.Text = "2 · OCR Engine";
        // 
        // cmbEngine
        // 
        cmbEngine.BackColor = Color.FromArgb(36, 39, 46);
        cmbEngine.DrawMode = DrawMode.OwnerDrawFixed;
        cmbEngine.DropDownHeight = 200;
        cmbEngine.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEngine.FlatStyle = FlatStyle.Flat;
        cmbEngine.ForeColor = Color.FromArgb(220, 220, 220);
        cmbEngine.IntegralHeight = false;
        cmbEngine.ItemHeight = 32;
        cmbEngine.Items.AddRange(new object[] { "1  WinRT  (~5–20 ms)", "2  PaddleOCR V4  (~200–500 ms)" });
        cmbEngine.Location = new Point(19, 64);
        cmbEngine.Margin = new Padding(6);
        cmbEngine.Name = "cmbEngine";
        cmbEngine.Size = new Size(554, 38);
        cmbEngine.TabIndex = 1;
        cmbEngine.SelectedIndex = 0;
        cmbEngine.DrawItem += cmb_DrawItem;
        // 
        // TimingCard
        // 
        TimingCard.BackColor = Color.FromArgb(36, 39, 46);
        TimingCard.Controls.Add(lblProcLabel);
        TimingCard.Controls.Add(lblProcTime);
        TimingCard.Location = new Point(0, 306);
        TimingCard.Margin = new Padding(0, 0, 0, 13);
        TimingCard.Name = "TimingCard";
        TimingCard.Size = new Size(594, 192);
        TimingCard.TabIndex = 0;
        TimingCard.Paint += CardBorderPaint;
        // 
        // lblProcLabel
        // 
        lblProcLabel.Font = new Font("Consolas", 8F);
        lblProcLabel.ForeColor = Color.FromArgb(140, 145, 155);
        lblProcLabel.Location = new Point(26, 21);
        lblProcLabel.Margin = new Padding(6, 0, 6, 0);
        lblProcLabel.Name = "lblProcLabel";
        lblProcLabel.Size = new Size(539, 34);
        lblProcLabel.TabIndex = 0;
        lblProcLabel.Text = "TOTAL PROCESSING TIME";
        // 
        // lblProcTime
        // 
        lblProcTime.Font = new Font("Consolas", 24F, FontStyle.Bold);
        lblProcTime.ForeColor = Color.FromArgb(57, 255, 20);
        lblProcTime.Location = new Point(26, 64);
        lblProcTime.Margin = new Padding(6, 0, 6, 0);
        lblProcTime.Name = "lblProcTime";
        lblProcTime.Size = new Size(539, 107);
        lblProcTime.TabIndex = 1;
        lblProcTime.Text = "—";
        lblProcTime.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // resultsScroll
        // 
        resultsScroll.AutoScroll = true;
        resultsScroll.BackColor = Color.FromArgb(28, 30, 36);
        resultsScroll.Controls.Add(resultsFlow);
        resultsScroll.Location = new Point(0, 511);
        resultsScroll.Margin = new Padding(0, 0, 0, 13);
        resultsScroll.Name = "resultsScroll";
        resultsScroll.Size = new Size(594, 300);
        resultsScroll.TabIndex = 0;
        // 
        // resultsFlow
        // 
        resultsFlow.AutoSize = true;
        resultsFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        resultsFlow.BackColor = Color.FromArgb(28, 30, 36);
        resultsFlow.Dock = DockStyle.Top;
        resultsFlow.FlowDirection = FlowDirection.TopDown;
        resultsFlow.Location = new Point(0, 0);
        resultsFlow.Margin = new Padding(6);
        resultsFlow.Name = "resultsFlow";
        resultsFlow.Padding = new Padding(0, 4, 0, 0);
        resultsFlow.Size = new Size(594, 4);
        resultsFlow.TabIndex = 0;
        resultsFlow.WrapContents = false;
        // 
        // RawOcrCard
        // 
        RawOcrCard.BackColor = Color.FromArgb(36, 39, 46);
        RawOcrCard.Controls.Add(lblTitleRawOcrCard);
        RawOcrCard.Controls.Add(txtRawOcr);
        RawOcrCard.Location = new Point(0, 824);
        RawOcrCard.Margin = new Padding(0, 0, 0, 13);
        RawOcrCard.Name = "RawOcrCard";
        RawOcrCard.Size = new Size(594, 320);
        RawOcrCard.TabIndex = 0;
        RawOcrCard.Paint += CardBorderPaint;
        // 
        // lblTitleRawOcrCard
        // 
        lblTitleRawOcrCard.AutoSize = true;
        lblTitleRawOcrCard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTitleRawOcrCard.ForeColor = Color.FromArgb(220, 220, 220);
        lblTitleRawOcrCard.Location = new Point(19, 21);
        lblTitleRawOcrCard.Margin = new Padding(6, 0, 6, 0);
        lblTitleRawOcrCard.Name = "lblTitleRawOcrCard";
        lblTitleRawOcrCard.Size = new Size(241, 32);
        lblTitleRawOcrCard.TabIndex = 0;
        lblTitleRawOcrCard.Text = "3 · Raw OCR Output";
        // 
        // txtRawOcr
        // 
        txtRawOcr.BackColor = Color.FromArgb(24, 26, 32);
        txtRawOcr.BorderStyle = BorderStyle.None;
        txtRawOcr.Font = new Font("Consolas", 11F);
        txtRawOcr.ForeColor = Color.FromArgb(220, 220, 220);
        txtRawOcr.Location = new Point(19, 64);
        txtRawOcr.Margin = new Padding(6);
        txtRawOcr.Multiline = true;
        txtRawOcr.Name = "txtRawOcr";
        txtRawOcr.ReadOnly = true;
        txtRawOcr.ScrollBars = ScrollBars.None;
        txtRawOcr.Size = new Size(557, 240);
        txtRawOcr.TabIndex = 1;
        txtRawOcr.Text = "(run OCR to see raw output)";
        // 
        // rightPanel
        // 
        rightPanel.BackColor = Color.FromArgb(36, 39, 46);
        rightPanel.Controls.Add(pictureBox);
        rightPanel.Dock = DockStyle.Fill;
        rightPanel.Location = new Point(631, 0);
        rightPanel.Margin = new Padding(6);
        rightPanel.Name = "rightPanel";
        rightPanel.Padding = new Padding(4);
        rightPanel.Size = new Size(1356, 1231);
        rightPanel.TabIndex = 1;
        rightPanel.Paint += RightPanel_Paint;
        rightPanel.Resize += rightPanel_Resize;
        // 
        // pictureBox
        // 
        pictureBox.BackColor = Color.Transparent;
        pictureBox.Dock = DockStyle.Fill;
        pictureBox.Location = new Point(4, 4);
        pictureBox.Margin = new Padding(6);
        pictureBox.Name = "pictureBox";
        pictureBox.Size = new Size(1348, 1223);
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        // 
        // statusBar
        // 
        statusBar.BackColor = Color.FromArgb(20, 22, 27);
        statusBar.Controls.Add(lblProcStatus);
        statusBar.Controls.Add(lblLighting);
        statusBar.Dock = DockStyle.Bottom;
        statusBar.Location = new Point(0, 1278);
        statusBar.Margin = new Padding(6);
        statusBar.Name = "statusBar";
        statusBar.Padding = new Padding(22, 0, 22, 0);
        statusBar.Size = new Size(1995, 64);
        statusBar.TabIndex = 2;
        // 
        // lblProcStatus
        // 
        lblProcStatus.Dock = DockStyle.Right;
        lblProcStatus.Font = new Font("Segoe UI", 9F);
        lblProcStatus.ForeColor = Color.FromArgb(140, 145, 155);
        lblProcStatus.Location = new Point(1583, 0);
        lblProcStatus.Margin = new Padding(6, 0, 6, 0);
        lblProcStatus.Name = "lblProcStatus";
        lblProcStatus.Size = new Size(390, 64);
        lblProcStatus.TabIndex = 2;
        lblProcStatus.Text = "Processing: Ready";
        lblProcStatus.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblLighting
        // 
        lblLighting.Dock = DockStyle.Left;
        lblLighting.Font = new Font("Segoe UI", 9F);
        lblLighting.ForeColor = Color.FromArgb(0, 200, 83);
        lblLighting.Location = new Point(22, 0);
        lblLighting.Margin = new Padding(6, 0, 6, 0);
        lblLighting.Name = "lblLighting";
        lblLighting.Size = new Size(241, 64);
        lblLighting.TabIndex = 1;
        lblLighting.Text = "✓ Lighting OK";
        lblLighting.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(28, 30, 36);
        ClientSize = new Size(1995, 1342);
        Controls.Add(rightPanel);
        Controls.Add(leftScroll);
        Controls.Add(statusBar);
        Font = new Font("Segoe UI", 9F);
        ForeColor = Color.FromArgb(220, 220, 220);
        Margin = new Padding(6);
        MinimumSize = new Size(1835, 1285);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "OCR Pro";
        leftScroll.ResumeLayout(false);
        leftFlow.ResumeLayout(false);
        InputCard.ResumeLayout(false);
        InputCard.PerformLayout();
        EngineCard.ResumeLayout(false);
        EngineCard.PerformLayout();
        TimingCard.ResumeLayout(false);
        resultsScroll.ResumeLayout(false);
        resultsScroll.PerformLayout();
        RawOcrCard.ResumeLayout(false);
        RawOcrCard.PerformLayout();
        rightPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
        statusBar.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void RightPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        if (sender is not System.Windows.Forms.Panel p) return;
        using var dot = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(40, 255, 255, 255));
        const int gap = 20;
        for (int x = gap; x < p.Width; x += gap)
        for (int y = gap; y < p.Height; y += gap)
            e.Graphics.FillRectangle(dot, x, y, 1, 1);
    }

    private void rightPanel_Resize(object sender, System.EventArgs e)
    {
        // no overlay label to reposition
    }

    private void CardBorderPaint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        if (sender is not System.Windows.Forms.Control c) return;
        using var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(55, 60, 70), 1);
        e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
    }

    private void cmb_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
    {
        if (e.Index < 0 || sender is not System.Windows.Forms.ComboBox cmb) return;
        e.DrawBackground();
        bool selected = (e.State & System.Windows.Forms.DrawItemState.Selected) != 0;
        using var bgBrush = new System.Drawing.SolidBrush(
            selected ? System.Drawing.Color.FromArgb(57, 255, 20)
                     : System.Drawing.Color.FromArgb(36, 39, 46));
        using var fgBrush = new System.Drawing.SolidBrush(
            selected ? System.Drawing.Color.Black
                     : System.Drawing.Color.FromArgb(220, 220, 220));
        e.Graphics.FillRectangle(bgBrush, e.Bounds);
        using var sf = new System.Drawing.StringFormat();
        sf.LineAlignment = System.Drawing.StringAlignment.Center;
        sf.Alignment = System.Drawing.StringAlignment.Near;
        sf.FormatFlags = System.Drawing.StringFormatFlags.NoWrap;
        var textRect = new System.Drawing.RectangleF(
            e.Bounds.Left + 6, e.Bounds.Top,
            e.Bounds.Width - 8, e.Bounds.Height);
        e.Graphics.DrawString(
            cmb.Items[e.Index]?.ToString() ?? "",
            e.Font ?? cmb.Font,
            fgBrush,
            textRect,
            sf);
    }

    #endregion

    // ── Field declarations ────────────────────────────────────────────────────
    private System.Windows.Forms.Panel leftScroll = null!;
    private System.Windows.Forms.FlowLayoutPanel leftFlow = null!;
    private System.Windows.Forms.Panel InputCard = null!;
    private System.Windows.Forms.Label lblTitleInputCard = null!;
    private System.Windows.Forms.Panel EngineCard = null!;
    private System.Windows.Forms.Label lblTitleEngineCard = null!;
    private System.Windows.Forms.Panel TimingCard = null!;
    private System.Windows.Forms.Panel RawOcrCard = null!;
    private System.Windows.Forms.Label lblTitleRawOcrCard = null!;
    private System.Windows.Forms.Panel statusBar = null!;
    private System.Windows.Forms.Label lblProcLabel = null!;
    private System.Windows.Forms.Label lblProcTime = null!;
    private System.Windows.Forms.FlowLayoutPanel resultsFlow = null!;
    private System.Windows.Forms.Panel resultsScroll = null!;
    private System.Windows.Forms.Button btnRunOcr = null!;
    private System.Windows.Forms.Button btnLoadImage = null!;
    private System.Windows.Forms.ComboBox cmbEngine = null!;
    private System.Windows.Forms.Panel rightPanel = null!;
    private System.Windows.Forms.PictureBox pictureBox = null!;
    private System.Windows.Forms.Label lblLighting = null!;
    private System.Windows.Forms.Label lblProcStatus = null!;
    private System.Windows.Forms.TextBox txtRawOcr = null!;
}
