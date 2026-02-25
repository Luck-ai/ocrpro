namespace OcrTesseract;

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
        lblCameraSep = new Label();
        cmbCamera = new ComboBox();
        btnCamera = new Button();
        btnCapture = new Button();
        EngineCard = new Panel();
        lblTitleEngineCard = new Label();
        cmbEngine = new ComboBox();
        chkEnablePrep = new CheckBox();
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
        lblFps = new Label();
        statusBar = new Panel();
        lblProcStatus = new Label();
        lblLighting = new Label();
        lblCamStatus = new Label();
        mainTabs = new TabControl();
        tabOcr = new TabPage();
        tabPrep = new TabPage();
        prepTab = new PreprocessingTab();
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
        mainTabs.SuspendLayout();
        tabOcr.SuspendLayout();
        tabPrep.SuspendLayout();
        SuspendLayout();
        // 
        // leftScroll
        // 
        leftScroll.AutoScroll = true;
        leftScroll.BackColor = Color.FromArgb(28, 30, 36);
        leftScroll.Controls.Add(leftFlow);
        leftScroll.Dock = DockStyle.Left;
        leftScroll.Location = new Point(0, 0);
        leftScroll.Margin = new Padding(6, 6, 6, 6);
        leftScroll.Name = "leftScroll";
        leftScroll.Padding = new Padding(0, 0, 7, 0);
        leftScroll.Size = new Size(631, 1231);
        leftScroll.TabIndex = 0;
        // 
        // leftFlow
        // 
        leftFlow.AutoSize = true;
        leftFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        leftFlow.BackColor = Color.FromArgb(28, 30, 36);
        leftFlow.Controls.Add(InputCard);
        leftFlow.Controls.Add(EngineCard);
        leftFlow.Controls.Add(TimingCard);
        leftFlow.Controls.Add(resultsScroll);
        leftFlow.Controls.Add(RawOcrCard);
        leftFlow.Controls.Add(btnRunOcr);
        leftFlow.Dock = DockStyle.Top;
        leftFlow.FlowDirection = FlowDirection.TopDown;
        leftFlow.Location = new Point(0, 0);
        leftFlow.Margin = new Padding(6, 6, 6, 6);
        leftFlow.Name = "leftFlow";
        leftFlow.Size = new Size(590, 1659);
        leftFlow.TabIndex = 0;
        leftFlow.WrapContents = false;
        // 
        // InputCard
        // 
        InputCard.BackColor = Color.FromArgb(36, 39, 46);
        InputCard.Controls.Add(lblTitleInputCard);
        InputCard.Controls.Add(btnLoadImage);
        InputCard.Controls.Add(lblCameraSep);
        InputCard.Controls.Add(cmbCamera);
        InputCard.Controls.Add(btnCamera);
        InputCard.Controls.Add(btnCapture);
        InputCard.Location = new Point(0, 0);
        InputCard.Margin = new Padding(0, 0, 0, 13);
        InputCard.Name = "InputCard";
        InputCard.Size = new Size(594, 320);
        InputCard.TabIndex = 0;
        InputCard.Paint += this.CardBorderPaint;
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
        btnLoadImage.Margin = new Padding(6, 6, 6, 6);
        btnLoadImage.Name = "btnLoadImage";
        btnLoadImage.Size = new Size(557, 55);
        btnLoadImage.TabIndex = 1;
        btnLoadImage.Text = "Load Image";
        btnLoadImage.UseVisualStyleBackColor = false;
        btnLoadImage.Click += btnLoadImage_Click;
        // 
        // lblCameraSep
        // 
        lblCameraSep.AutoSize = true;
        lblCameraSep.Font = new Font("Consolas", 8F);
        lblCameraSep.ForeColor = Color.FromArgb(140, 145, 155);
        lblCameraSep.Location = new Point(19, 137);
        lblCameraSep.Margin = new Padding(6, 0, 6, 0);
        lblCameraSep.Name = "lblCameraSep";
        lblCameraSep.Size = new Size(84, 26);
        lblCameraSep.TabIndex = 2;
        lblCameraSep.Text = "Camera";
        // 
        // cmbCamera
        // 
        cmbCamera.BackColor = Color.FromArgb(36, 39, 46);
        cmbCamera.DrawMode = DrawMode.OwnerDrawFixed;
        cmbCamera.DropDownHeight = 200;
        cmbCamera.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCamera.FlatStyle = FlatStyle.Flat;
        cmbCamera.ForeColor = Color.FromArgb(220, 220, 220);
        cmbCamera.IntegralHeight = false;
        cmbCamera.ItemHeight = 32;
        cmbCamera.Location = new Point(19, 175);
        cmbCamera.Margin = new Padding(6, 6, 6, 6);
        cmbCamera.Name = "cmbCamera";
        cmbCamera.Size = new Size(554, 26);
        cmbCamera.TabIndex = 3;
        cmbCamera.DrawItem += cmb_DrawItem;
        // 
        // btnCamera
        // 
        btnCamera.BackColor = Color.FromArgb(48, 52, 62);
        btnCamera.Cursor = Cursors.Hand;
        btnCamera.FlatAppearance.BorderColor = Color.FromArgb(55, 60, 70);
        btnCamera.FlatStyle = FlatStyle.Flat;
        btnCamera.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCamera.ForeColor = Color.FromArgb(220, 220, 220);
        btnCamera.Location = new Point(19, 247);
        btnCamera.Margin = new Padding(6, 6, 6, 6);
        btnCamera.Name = "btnCamera";
        btnCamera.Size = new Size(271, 55);
        btnCamera.TabIndex = 4;
        btnCamera.Text = "Start";
        btnCamera.UseVisualStyleBackColor = false;
        btnCamera.Click += btnCamera_Click;
        // 
        // btnCapture
        // 
        btnCapture.BackColor = Color.FromArgb(0, 120, 200);
        btnCapture.Cursor = Cursors.Hand;
        btnCapture.Enabled = false;
        btnCapture.FlatAppearance.BorderSize = 0;
        btnCapture.FlatStyle = FlatStyle.Flat;
        btnCapture.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCapture.ForeColor = Color.White;
        btnCapture.Location = new Point(305, 247);
        btnCapture.Margin = new Padding(6, 6, 6, 6);
        btnCapture.Name = "btnCapture";
        btnCapture.Size = new Size(271, 55);
        btnCapture.TabIndex = 5;
        btnCapture.Text = "Capture & OCR";
        btnCapture.UseVisualStyleBackColor = false;
        btnCapture.Click += btnCapture_Click;
        // 
        // EngineCard
        // 
        EngineCard.BackColor = Color.FromArgb(36, 39, 46);
        EngineCard.Controls.Add(lblTitleEngineCard);
        EngineCard.Controls.Add(cmbEngine);
        EngineCard.Controls.Add(chkEnablePrep);
        EngineCard.Location = new Point(0, 333);
        EngineCard.Margin = new Padding(0, 0, 0, 13);
        EngineCard.Name = "EngineCard";
        EngineCard.Size = new Size(594, 203);
        EngineCard.TabIndex = 0;
        EngineCard.Paint += this.CardBorderPaint;
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
        cmbEngine.Items.AddRange(new object[] { "WinRT  (~5–20 ms)", "PaddleOCR  (~200–500 ms)", "Tesseract  (~700 ms)" });
        cmbEngine.Location = new Point(19, 64);
        cmbEngine.Margin = new Padding(6, 6, 6, 6);
        cmbEngine.Name = "cmbEngine";
        cmbEngine.Size = new Size(554, 26);
        cmbEngine.TabIndex = 1;
        cmbEngine.DrawItem += cmb_DrawItem;
        // 
        // chkEnablePrep
        // 
        chkEnablePrep.AutoSize = true;
        chkEnablePrep.BackColor = Color.FromArgb(36, 39, 46);
        chkEnablePrep.Checked = true;
        chkEnablePrep.CheckState = CheckState.Checked;
        chkEnablePrep.Font = new Font("Segoe UI", 9F);
        chkEnablePrep.ForeColor = Color.FromArgb(220, 220, 220);
        chkEnablePrep.Location = new Point(19, 137);
        chkEnablePrep.Margin = new Padding(6, 6, 6, 6);
        chkEnablePrep.Name = "chkEnablePrep";
        chkEnablePrep.Size = new Size(272, 36);
        chkEnablePrep.TabIndex = 2;
        chkEnablePrep.Text = "Enable Preprocessing";
        chkEnablePrep.UseVisualStyleBackColor = false;
        // 
        // TimingCard
        // 
        TimingCard.BackColor = Color.FromArgb(36, 39, 46);
        TimingCard.Controls.Add(lblProcLabel);
        TimingCard.Controls.Add(lblProcTime);
        TimingCard.Location = new Point(0, 549);
        TimingCard.Margin = new Padding(0, 0, 0, 13);
        TimingCard.Name = "TimingCard";
        TimingCard.Size = new Size(594, 192);
        TimingCard.TabIndex = 0;
        TimingCard.Paint += this.CardBorderPaint;
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
        resultsScroll.Location = new Point(0, 754);
        resultsScroll.Margin = new Padding(0, 0, 0, 13);
        resultsScroll.Name = "resultsScroll";
        resultsScroll.Size = new Size(594, 427);
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
        resultsFlow.Margin = new Padding(6, 6, 6, 6);
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
        RawOcrCard.Location = new Point(0, 1194);
        RawOcrCard.Margin = new Padding(0, 0, 0, 13);
        RawOcrCard.Name = "RawOcrCard";
        RawOcrCard.Size = new Size(594, 341);
        RawOcrCard.TabIndex = 0;
        RawOcrCard.Paint += this.CardBorderPaint;
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
        lblTitleRawOcrCard.Text = "4 · Raw OCR Output";
        // 
        // txtRawOcr
        // 
        txtRawOcr.BackColor = Color.FromArgb(24, 26, 32);
        txtRawOcr.BorderStyle = BorderStyle.None;
        txtRawOcr.Font = new Font("Consolas", 8F);
        txtRawOcr.ForeColor = Color.FromArgb(220, 220, 220);
        txtRawOcr.Location = new Point(19, 64);
        txtRawOcr.Margin = new Padding(6, 6, 6, 6);
        txtRawOcr.Multiline = true;
        txtRawOcr.Name = "txtRawOcr";
        txtRawOcr.ReadOnly = true;
        txtRawOcr.ScrollBars = ScrollBars.Vertical;
        txtRawOcr.Size = new Size(557, 256);
        txtRawOcr.TabIndex = 1;
        txtRawOcr.Text = "(run OCR to see raw output)";
        // 
        // btnRunOcr
        // 
        btnRunOcr.BackColor = Color.FromArgb(57, 255, 20);
        btnRunOcr.Cursor = Cursors.Hand;
        btnRunOcr.FlatAppearance.BorderSize = 0;
        btnRunOcr.FlatStyle = FlatStyle.Flat;
        btnRunOcr.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnRunOcr.ForeColor = Color.Black;
        btnRunOcr.Location = new Point(0, 1565);
        btnRunOcr.Margin = new Padding(0, 17, 0, 0);
        btnRunOcr.Name = "btnRunOcr";
        btnRunOcr.Size = new Size(594, 77);
        btnRunOcr.TabIndex = 0;
        btnRunOcr.Text = "Run OCR";
        btnRunOcr.UseVisualStyleBackColor = false;
        btnRunOcr.Click += btnRunOcr_Click;
        // 
        // rightPanel
        // 
        rightPanel.BackColor = Color.FromArgb(36, 39, 46);
        rightPanel.Controls.Add(pictureBox);
        rightPanel.Controls.Add(lblFps);
        rightPanel.Dock = DockStyle.Fill;
        rightPanel.Location = new Point(631, 0);
        rightPanel.Margin = new Padding(6, 6, 6, 6);
        rightPanel.Name = "rightPanel";
        rightPanel.Padding = new Padding(4, 4, 4, 4);
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
        pictureBox.Margin = new Padding(6, 6, 6, 6);
        pictureBox.Name = "pictureBox";
        pictureBox.Size = new Size(1348, 1223);
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        // 
        // lblFps
        // 
        lblFps.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblFps.AutoSize = true;
        lblFps.BackColor = Color.FromArgb(57, 255, 20);
        lblFps.Font = new Font("Consolas", 9F, FontStyle.Bold);
        lblFps.ForeColor = Color.Black;
        lblFps.Location = new Point(520, 21);
        lblFps.Margin = new Padding(6, 0, 6, 0);
        lblFps.Name = "lblFps";
        lblFps.Padding = new Padding(11, 6, 11, 6);
        lblFps.Size = new Size(125, 40);
        lblFps.TabIndex = 1;
        lblFps.Text = "FPS: --";
        // 
        // statusBar
        // 
        statusBar.BackColor = Color.FromArgb(20, 22, 27);
        statusBar.Controls.Add(lblProcStatus);
        statusBar.Controls.Add(lblLighting);
        statusBar.Controls.Add(lblCamStatus);
        statusBar.Dock = DockStyle.Bottom;
        statusBar.Location = new Point(0, 1278);
        statusBar.Margin = new Padding(6, 6, 6, 6);
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
        lblLighting.Location = new Point(579, 0);
        lblLighting.Margin = new Padding(6, 0, 6, 0);
        lblLighting.Name = "lblLighting";
        lblLighting.Size = new Size(241, 64);
        lblLighting.TabIndex = 1;
        lblLighting.Text = "✓ Lighting OK";
        lblLighting.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblCamStatus
        // 
        lblCamStatus.Dock = DockStyle.Left;
        lblCamStatus.Font = new Font("Segoe UI", 9F);
        lblCamStatus.ForeColor = Color.FromArgb(140, 145, 155);
        lblCamStatus.Location = new Point(22, 0);
        lblCamStatus.Margin = new Padding(6, 0, 6, 0);
        lblCamStatus.Name = "lblCamStatus";
        lblCamStatus.Size = new Size(557, 64);
        lblCamStatus.TabIndex = 0;
        lblCamStatus.Text = "● Camera: Disconnected";
        lblCamStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // mainTabs
        // 
        mainTabs.Controls.Add(tabOcr);
        mainTabs.Controls.Add(tabPrep);
        mainTabs.Dock = DockStyle.Fill;
        mainTabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        mainTabs.Location = new Point(0, 0);
        mainTabs.Margin = new Padding(6, 6, 6, 6);
        mainTabs.Name = "mainTabs";
        mainTabs.Padding = new Point(12, 4);
        mainTabs.SelectedIndex = 0;
        mainTabs.Size = new Size(1995, 1278);
        mainTabs.TabIndex = 0;
        mainTabs.DrawItem += MainTabs_DrawItem;
        // 
        // tabOcr
        // 
        tabOcr.BackColor = Color.FromArgb(28, 30, 36);
        tabOcr.Controls.Add(rightPanel);
        tabOcr.Controls.Add(leftScroll);
        tabOcr.Location = new Point(4, 43);
        tabOcr.Margin = new Padding(6, 6, 6, 6);
        tabOcr.Name = "tabOcr";
        tabOcr.Size = new Size(1987, 1231);
        tabOcr.TabIndex = 0;
        tabOcr.Text = "  OCR  ";
        // 
        // tabPrep
        // 
        tabPrep.BackColor = Color.FromArgb(28, 30, 36);
        tabPrep.Controls.Add(prepTab);
        tabPrep.Location = new Point(4, 43);
        tabPrep.Margin = new Padding(6, 6, 6, 6);
        tabPrep.Name = "tabPrep";
        tabPrep.Size = new Size(363, 166);
        tabPrep.TabIndex = 1;
        tabPrep.Text = "  Preprocessing  ";
        // 
        // prepTab
        // 
        prepTab.BackColor = Color.FromArgb(28, 30, 36);
        prepTab.Dock = DockStyle.Fill;
        prepTab.Location = new Point(0, 0);
        prepTab.Margin = new Padding(6, 6, 6, 6);
        prepTab.Name = "prepTab";
        prepTab.Size = new Size(363, 166);
        prepTab.TabIndex = 0;
        prepTab.UseAsOcrInput += PrepTab_UseAsOcrInput;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(28, 30, 36);
        ClientSize = new Size(1995, 1342);
        Controls.Add(mainTabs);
        Controls.Add(statusBar);
        Font = new Font("Segoe UI", 9F);
        ForeColor = Color.FromArgb(220, 220, 220);
        Margin = new Padding(6, 6, 6, 6);
        MinimumSize = new Size(1835, 1285);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TesseractOCR Pro - v1.0";
        leftScroll.ResumeLayout(false);
        leftScroll.PerformLayout();
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
        rightPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
        statusBar.ResumeLayout(false);
        mainTabs.ResumeLayout(false);
        tabOcr.ResumeLayout(false);
        tabPrep.ResumeLayout(false);
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
        lblFps.Location = new System.Drawing.Point(rightPanel.ClientSize.Width - lblFps.Width - 10, 10);
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
    private System.Windows.Forms.TabControl mainTabs = null!;
    private PreprocessingTab prepTab = null!;
    private System.Windows.Forms.Panel leftScroll = null!;
    private System.Windows.Forms.FlowLayoutPanel leftFlow = null!;
    private System.Windows.Forms.Panel InputCard = null!;
    private System.Windows.Forms.Label lblTitleInputCard = null!;
    private System.Windows.Forms.Label lblCameraSep = null!;
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
    private System.Windows.Forms.Button btnCamera = null!;
    private System.Windows.Forms.Button btnCapture = null!;
    private System.Windows.Forms.ComboBox cmbEngine = null!;
    private System.Windows.Forms.ComboBox cmbCamera = null!;
    private System.Windows.Forms.CheckBox chkEnablePrep = null!;
    private System.Windows.Forms.Panel rightPanel = null!;
    private System.Windows.Forms.PictureBox pictureBox = null!;
    private System.Windows.Forms.Label lblFps = null!;
    private System.Windows.Forms.Label lblCamStatus = null!;
    private System.Windows.Forms.Label lblLighting = null!;
    private System.Windows.Forms.Label lblProcStatus = null!;
    private System.Windows.Forms.TextBox txtRawOcr = null!;
    private TabPage tabOcr;
    private TabPage tabPrep;
}
