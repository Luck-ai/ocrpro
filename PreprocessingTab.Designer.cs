namespace OcrTesseract
{
    partial class PreprocessingTab
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.leftScroll = new System.Windows.Forms.Panel();
            this.stepsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.previewBox = new System.Windows.Forms.PictureBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnApplyAll = new System.Windows.Forms.Button();
            this.btnUseAsOcr = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.UpscaleCard = new System.Windows.Forms.Panel();
            this.lblTitleUpscaleCard = new System.Windows.Forms.Label();
            this.chkUpscale = new System.Windows.Forms.CheckBox();
            this.btnApplyUpscale = new System.Windows.Forms.Button();
            this.trkUpscale = new System.Windows.Forms.TrackBar();
            this.lblUpscaleVal = new System.Windows.Forms.Label();
            this.lblProptrkUpscale = new System.Windows.Forms.Label();
            this.GrayCard = new System.Windows.Forms.Panel();
            this.lblTitleGrayCard = new System.Windows.Forms.Label();
            this.chkGray = new System.Windows.Forms.CheckBox();
            this.btnApplyGray = new System.Windows.Forms.Button();
            this.ClaheCard = new System.Windows.Forms.Panel();
            this.lblTitleClaheCard = new System.Windows.Forms.Label();
            this.chkClahe = new System.Windows.Forms.CheckBox();
            this.btnApplyClahe = new System.Windows.Forms.Button();
            this.trkClaheClip = new System.Windows.Forms.TrackBar();
            this.lblClaheClip = new System.Windows.Forms.Label();
            this.lblProptrkClaheClip = new System.Windows.Forms.Label();
            this.trkClaheTile = new System.Windows.Forms.TrackBar();
            this.lblClaheTile = new System.Windows.Forms.Label();
            this.lblProptrkClaheTile = new System.Windows.Forms.Label();
            this.DeskewCard = new System.Windows.Forms.Panel();
            this.lblTitleDeskewCard = new System.Windows.Forms.Label();
            this.chkDeskew = new System.Windows.Forms.CheckBox();
            this.btnApplyDeskew = new System.Windows.Forms.Button();
            this.trkDeskewMax = new System.Windows.Forms.TrackBar();
            this.lblDeskewMax = new System.Windows.Forms.Label();
            this.lblProptrkDeskewMax = new System.Windows.Forms.Label();
            this.SharpenCard = new System.Windows.Forms.Panel();
            this.lblTitleSharpenCard = new System.Windows.Forms.Label();
            this.chkSharpen = new System.Windows.Forms.CheckBox();
            this.btnApplySharpen = new System.Windows.Forms.Button();
            this.trkSharpenSig = new System.Windows.Forms.TrackBar();
            this.lblSharpenSig = new System.Windows.Forms.Label();
            this.lblProptrkSharpenSig = new System.Windows.Forms.Label();
            this.trkSharpenStr = new System.Windows.Forms.TrackBar();
            this.lblSharpenStr = new System.Windows.Forms.Label();
            this.lblProptrkSharpenStr = new System.Windows.Forms.Label();
            this.BinariseCard = new System.Windows.Forms.Panel();
            this.lblTitleBinariseCard = new System.Windows.Forms.Label();
            this.chkBin = new System.Windows.Forms.CheckBox();
            this.btnApplyBinarise = new System.Windows.Forms.Button();
            this.trkBinThr = new System.Windows.Forms.TrackBar();
            this.lblBinThr = new System.Windows.Forms.Label();
            this.lblProptrkBinThr = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            this.leftScroll.SuspendLayout();
            this.stepsFlow.SuspendLayout();
            this.UpscaleCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkUpscale)).BeginInit();
            this.GrayCard.SuspendLayout();
            this.ClaheCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkClaheClip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkClaheTile)).BeginInit();
            this.DeskewCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkDeskewMax)).BeginInit();
            this.SharpenCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSharpenSig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSharpenStr)).BeginInit();
            this.BinariseCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBinThr)).BeginInit();
            this.SuspendLayout();

            // stepsFlow
            this.stepsFlow.AutoSize = true;
            this.stepsFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stepsFlow.Dock = System.Windows.Forms.DockStyle.Top;
            this.stepsFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.stepsFlow.Location = new System.Drawing.Point(0, 0);
            this.stepsFlow.Name = "stepsFlow";
            this.stepsFlow.Padding = new System.Windows.Forms.Padding(0);
            this.stepsFlow.Size = new System.Drawing.Size(320, 100);
            this.stepsFlow.TabIndex = 0;
            this.stepsFlow.WrapContents = false;
            this.stepsFlow.Controls.Add(this.UpscaleCard);
            this.stepsFlow.Controls.Add(this.GrayCard);
            this.stepsFlow.Controls.Add(this.ClaheCard);
            this.stepsFlow.Controls.Add(this.DeskewCard);
            this.stepsFlow.Controls.Add(this.SharpenCard);
            this.stepsFlow.Controls.Add(this.BinariseCard);
            this.stepsFlow.Controls.Add(this.btnApplyAll);
            this.stepsFlow.Controls.Add(this.btnUseAsOcr);
            this.stepsFlow.Controls.Add(this.btnReset);

            // ── UpscaleCard (1 slider, height=115, Apply at Y=80) ─────────────
            this.UpscaleCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.UpscaleCard.Location = new System.Drawing.Point(0, 0);
            this.UpscaleCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.UpscaleCard.Name = "UpscaleCard";
            this.UpscaleCard.Size = new System.Drawing.Size(320, 115);
            this.UpscaleCard.TabIndex = 0;
            this.UpscaleCard.Controls.Add(this.lblTitleUpscaleCard);
            this.UpscaleCard.Controls.Add(this.chkUpscale);
            this.UpscaleCard.Controls.Add(this.btnApplyUpscale);
            this.UpscaleCard.Controls.Add(this.lblProptrkUpscale);
            this.UpscaleCard.Controls.Add(this.trkUpscale);
            this.UpscaleCard.Controls.Add(this.lblUpscaleVal);

            // lblTitleUpscaleCard
            this.lblTitleUpscaleCard.AutoSize = true;
            this.lblTitleUpscaleCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleUpscaleCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitleUpscaleCard.Location = new System.Drawing.Point(10, 10);
            this.lblTitleUpscaleCard.Name = "lblTitleUpscaleCard";
            this.lblTitleUpscaleCard.Size = new System.Drawing.Size(100, 15);
            this.lblTitleUpscaleCard.TabIndex = 0;
            this.lblTitleUpscaleCard.Text = "1 · Upscale";

            // chkUpscale
            this.chkUpscale.AutoSize = true;
            this.chkUpscale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.chkUpscale.Checked = true;
            this.chkUpscale.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkUpscale.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkUpscale.Location = new System.Drawing.Point(10, 30);
            this.chkUpscale.Name = "chkUpscale";
            this.chkUpscale.Size = new System.Drawing.Size(200, 19);
            this.chkUpscale.TabIndex = 1;
            this.chkUpscale.Text = "Upscale";
            this.chkUpscale.UseVisualStyleBackColor = false;

            // btnApplyUpscale
            this.btnApplyUpscale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnApplyUpscale.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyUpscale.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnApplyUpscale.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyUpscale.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApplyUpscale.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnApplyUpscale.Location = new System.Drawing.Point(10, 80);
            this.btnApplyUpscale.Name = "btnApplyUpscale";
            this.btnApplyUpscale.Size = new System.Drawing.Size(300, 26);
            this.btnApplyUpscale.TabIndex = 2;
            this.btnApplyUpscale.Text = "Apply";
            this.btnApplyUpscale.UseVisualStyleBackColor = false;

            // lblProptrkUpscale (row Y=50)
            this.lblProptrkUpscale.AutoSize = true;
            this.lblProptrkUpscale.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkUpscale.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkUpscale.Location = new System.Drawing.Point(10, 50);
            this.lblProptrkUpscale.Name = "lblProptrkUpscale";
            this.lblProptrkUpscale.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkUpscale.TabIndex = 3;
            this.lblProptrkUpscale.Text = "Factor";

            // trkUpscale
            this.trkUpscale.AutoSize = false;
            this.trkUpscale.Location = new System.Drawing.Point(78, 49);
            this.trkUpscale.Name = "trkUpscale";
            this.trkUpscale.Size = new System.Drawing.Size(194, 22);
            this.trkUpscale.TabIndex = 4;
            this.trkUpscale.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkUpscale.Minimum = 1;
            this.trkUpscale.Maximum = 40;
            this.trkUpscale.Value = 20;

            // lblUpscaleVal
            this.lblUpscaleVal.AutoSize = true;
            this.lblUpscaleVal.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblUpscaleVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblUpscaleVal.Location = new System.Drawing.Point(276, 50);
            this.lblUpscaleVal.Name = "lblUpscaleVal";
            this.lblUpscaleVal.Size = new System.Drawing.Size(42, 13);
            this.lblUpscaleVal.TabIndex = 5;
            this.lblUpscaleVal.Text = "×2.0";

            // ── GrayCard (0 sliders, height=75, Apply at Y=53) ───────────────
            this.GrayCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.GrayCard.Location = new System.Drawing.Point(0, 0);
            this.GrayCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.GrayCard.Name = "GrayCard";
            this.GrayCard.Size = new System.Drawing.Size(320, 75);
            this.GrayCard.TabIndex = 0;
            this.GrayCard.Controls.Add(this.lblTitleGrayCard);
            this.GrayCard.Controls.Add(this.chkGray);
            this.GrayCard.Controls.Add(this.btnApplyGray);

            // lblTitleGrayCard
            this.lblTitleGrayCard.AutoSize = true;
            this.lblTitleGrayCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleGrayCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitleGrayCard.Location = new System.Drawing.Point(10, 10);
            this.lblTitleGrayCard.Name = "lblTitleGrayCard";
            this.lblTitleGrayCard.Size = new System.Drawing.Size(100, 15);
            this.lblTitleGrayCard.TabIndex = 0;
            this.lblTitleGrayCard.Text = "2 · Grayscale";

            // chkGray
            this.chkGray.AutoSize = true;
            this.chkGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.chkGray.Checked = true;
            this.chkGray.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkGray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkGray.Location = new System.Drawing.Point(10, 30);
            this.chkGray.Name = "chkGray";
            this.chkGray.Size = new System.Drawing.Size(200, 19);
            this.chkGray.TabIndex = 1;
            this.chkGray.Text = "Convert to Grayscale";
            this.chkGray.UseVisualStyleBackColor = false;

            // btnApplyGray
            this.btnApplyGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnApplyGray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyGray.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnApplyGray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyGray.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApplyGray.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnApplyGray.Location = new System.Drawing.Point(10, 53);
            this.btnApplyGray.Name = "btnApplyGray";
            this.btnApplyGray.Size = new System.Drawing.Size(300, 20);
            this.btnApplyGray.TabIndex = 2;
            this.btnApplyGray.Text = "Apply";
            this.btnApplyGray.UseVisualStyleBackColor = false;

            // ── ClaheCard (2 sliders, height=145, Apply at Y=106) ────────────
            this.ClaheCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.ClaheCard.Location = new System.Drawing.Point(0, 0);
            this.ClaheCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.ClaheCard.Name = "ClaheCard";
            this.ClaheCard.Size = new System.Drawing.Size(320, 145);
            this.ClaheCard.TabIndex = 0;
            this.ClaheCard.Controls.Add(this.lblTitleClaheCard);
            this.ClaheCard.Controls.Add(this.chkClahe);
            this.ClaheCard.Controls.Add(this.btnApplyClahe);
            this.ClaheCard.Controls.Add(this.lblProptrkClaheClip);
            this.ClaheCard.Controls.Add(this.trkClaheClip);
            this.ClaheCard.Controls.Add(this.lblClaheClip);
            this.ClaheCard.Controls.Add(this.lblProptrkClaheTile);
            this.ClaheCard.Controls.Add(this.trkClaheTile);
            this.ClaheCard.Controls.Add(this.lblClaheTile);

            // lblTitleClaheCard
            this.lblTitleClaheCard.AutoSize = true;
            this.lblTitleClaheCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleClaheCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitleClaheCard.Location = new System.Drawing.Point(10, 10);
            this.lblTitleClaheCard.Name = "lblTitleClaheCard";
            this.lblTitleClaheCard.Size = new System.Drawing.Size(100, 15);
            this.lblTitleClaheCard.TabIndex = 0;
            this.lblTitleClaheCard.Text = "3 · CLAHE";

            // chkClahe
            this.chkClahe.AutoSize = true;
            this.chkClahe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.chkClahe.Checked = true;
            this.chkClahe.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkClahe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkClahe.Location = new System.Drawing.Point(10, 30);
            this.chkClahe.Name = "chkClahe";
            this.chkClahe.Size = new System.Drawing.Size(200, 19);
            this.chkClahe.TabIndex = 1;
            this.chkClahe.Text = "CLAHE";
            this.chkClahe.UseVisualStyleBackColor = false;

            // btnApplyClahe
            this.btnApplyClahe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnApplyClahe.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyClahe.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnApplyClahe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyClahe.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApplyClahe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnApplyClahe.Location = new System.Drawing.Point(10, 106);
            this.btnApplyClahe.Name = "btnApplyClahe";
            this.btnApplyClahe.Size = new System.Drawing.Size(300, 26);
            this.btnApplyClahe.TabIndex = 2;
            this.btnApplyClahe.Text = "Apply";
            this.btnApplyClahe.UseVisualStyleBackColor = false;

            // lblProptrkClaheClip (row 1, Y=50)
            this.lblProptrkClaheClip.AutoSize = true;
            this.lblProptrkClaheClip.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkClaheClip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkClaheClip.Location = new System.Drawing.Point(10, 50);
            this.lblProptrkClaheClip.Name = "lblProptrkClaheClip";
            this.lblProptrkClaheClip.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkClaheClip.TabIndex = 3;
            this.lblProptrkClaheClip.Text = "Clip limit";

            // trkClaheClip (row 1, Y=49)
            this.trkClaheClip.AutoSize = false;
            this.trkClaheClip.Location = new System.Drawing.Point(78, 49);
            this.trkClaheClip.Name = "trkClaheClip";
            this.trkClaheClip.Size = new System.Drawing.Size(194, 22);
            this.trkClaheClip.TabIndex = 4;
            this.trkClaheClip.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkClaheClip.Minimum = 1;
            this.trkClaheClip.Maximum = 10;
            this.trkClaheClip.Value = 3;

            // lblClaheClip (row 1)
            this.lblClaheClip.AutoSize = true;
            this.lblClaheClip.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblClaheClip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblClaheClip.Location = new System.Drawing.Point(276, 50);
            this.lblClaheClip.Name = "lblClaheClip";
            this.lblClaheClip.Size = new System.Drawing.Size(42, 13);
            this.lblClaheClip.TabIndex = 5;
            this.lblClaheClip.Text = "3.0";

            // lblProptrkClaheTile (row 2, Y=76)
            this.lblProptrkClaheTile.AutoSize = true;
            this.lblProptrkClaheTile.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkClaheTile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkClaheTile.Location = new System.Drawing.Point(10, 76);
            this.lblProptrkClaheTile.Name = "lblProptrkClaheTile";
            this.lblProptrkClaheTile.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkClaheTile.TabIndex = 3;
            this.lblProptrkClaheTile.Text = "Tile size";

            // trkClaheTile (row 2, Y=75)
            this.trkClaheTile.AutoSize = false;
            this.trkClaheTile.Location = new System.Drawing.Point(78, 75);
            this.trkClaheTile.Name = "trkClaheTile";
            this.trkClaheTile.Size = new System.Drawing.Size(194, 22);
            this.trkClaheTile.TabIndex = 4;
            this.trkClaheTile.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkClaheTile.Minimum = 2;
            this.trkClaheTile.Maximum = 16;
            this.trkClaheTile.Value = 8;

            // lblClaheTile (row 2)
            this.lblClaheTile.AutoSize = true;
            this.lblClaheTile.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblClaheTile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblClaheTile.Location = new System.Drawing.Point(276, 76);
            this.lblClaheTile.Name = "lblClaheTile";
            this.lblClaheTile.Size = new System.Drawing.Size(42, 13);
            this.lblClaheTile.TabIndex = 5;
            this.lblClaheTile.Text = "8";

            // ── DeskewCard (1 slider, height=115, Apply at Y=80) ─────────────
            this.DeskewCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.DeskewCard.Location = new System.Drawing.Point(0, 0);
            this.DeskewCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.DeskewCard.Name = "DeskewCard";
            this.DeskewCard.Size = new System.Drawing.Size(320, 115);
            this.DeskewCard.TabIndex = 0;
            this.DeskewCard.Controls.Add(this.lblTitleDeskewCard);
            this.DeskewCard.Controls.Add(this.chkDeskew);
            this.DeskewCard.Controls.Add(this.btnApplyDeskew);
            this.DeskewCard.Controls.Add(this.lblProptrkDeskewMax);
            this.DeskewCard.Controls.Add(this.trkDeskewMax);
            this.DeskewCard.Controls.Add(this.lblDeskewMax);

            // lblTitleDeskewCard
            this.lblTitleDeskewCard.AutoSize = true;
            this.lblTitleDeskewCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleDeskewCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitleDeskewCard.Location = new System.Drawing.Point(10, 10);
            this.lblTitleDeskewCard.Name = "lblTitleDeskewCard";
            this.lblTitleDeskewCard.Size = new System.Drawing.Size(100, 15);
            this.lblTitleDeskewCard.TabIndex = 0;
            this.lblTitleDeskewCard.Text = "4 · Deskew";

            // chkDeskew
            this.chkDeskew.AutoSize = true;
            this.chkDeskew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.chkDeskew.Checked = true;
            this.chkDeskew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkDeskew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkDeskew.Location = new System.Drawing.Point(10, 30);
            this.chkDeskew.Name = "chkDeskew";
            this.chkDeskew.Size = new System.Drawing.Size(200, 19);
            this.chkDeskew.TabIndex = 1;
            this.chkDeskew.Text = "Deskew";
            this.chkDeskew.UseVisualStyleBackColor = false;

            // btnApplyDeskew
            this.btnApplyDeskew.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnApplyDeskew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyDeskew.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnApplyDeskew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyDeskew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApplyDeskew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnApplyDeskew.Location = new System.Drawing.Point(10, 80);
            this.btnApplyDeskew.Name = "btnApplyDeskew";
            this.btnApplyDeskew.Size = new System.Drawing.Size(300, 26);
            this.btnApplyDeskew.TabIndex = 2;
            this.btnApplyDeskew.Text = "Apply";
            this.btnApplyDeskew.UseVisualStyleBackColor = false;

            // lblProptrkDeskewMax (row 1, Y=50)
            this.lblProptrkDeskewMax.AutoSize = true;
            this.lblProptrkDeskewMax.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkDeskewMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkDeskewMax.Location = new System.Drawing.Point(10, 50);
            this.lblProptrkDeskewMax.Name = "lblProptrkDeskewMax";
            this.lblProptrkDeskewMax.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkDeskewMax.TabIndex = 3;
            this.lblProptrkDeskewMax.Text = "Max angle";

            // trkDeskewMax (row 1, Y=49)
            this.trkDeskewMax.AutoSize = false;
            this.trkDeskewMax.Location = new System.Drawing.Point(78, 49);
            this.trkDeskewMax.Name = "trkDeskewMax";
            this.trkDeskewMax.Size = new System.Drawing.Size(194, 22);
            this.trkDeskewMax.TabIndex = 4;
            this.trkDeskewMax.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkDeskewMax.Minimum = 1;
            this.trkDeskewMax.Maximum = 45;
            this.trkDeskewMax.Value = 15;

            // lblDeskewMax
            this.lblDeskewMax.AutoSize = true;
            this.lblDeskewMax.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblDeskewMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblDeskewMax.Location = new System.Drawing.Point(276, 50);
            this.lblDeskewMax.Name = "lblDeskewMax";
            this.lblDeskewMax.Size = new System.Drawing.Size(42, 13);
            this.lblDeskewMax.TabIndex = 5;
            this.lblDeskewMax.Text = "15°";

            // ── SharpenCard (2 sliders, height=145, Apply at Y=106) ──────────
            this.SharpenCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.SharpenCard.Location = new System.Drawing.Point(0, 0);
            this.SharpenCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.SharpenCard.Name = "SharpenCard";
            this.SharpenCard.Size = new System.Drawing.Size(320, 145);
            this.SharpenCard.TabIndex = 0;
            this.SharpenCard.Controls.Add(this.lblTitleSharpenCard);
            this.SharpenCard.Controls.Add(this.chkSharpen);
            this.SharpenCard.Controls.Add(this.btnApplySharpen);
            this.SharpenCard.Controls.Add(this.lblProptrkSharpenSig);
            this.SharpenCard.Controls.Add(this.trkSharpenSig);
            this.SharpenCard.Controls.Add(this.lblSharpenSig);
            this.SharpenCard.Controls.Add(this.lblProptrkSharpenStr);
            this.SharpenCard.Controls.Add(this.trkSharpenStr);
            this.SharpenCard.Controls.Add(this.lblSharpenStr);

            // lblTitleSharpenCard
            this.lblTitleSharpenCard.AutoSize = true;
            this.lblTitleSharpenCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleSharpenCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitleSharpenCard.Location = new System.Drawing.Point(10, 10);
            this.lblTitleSharpenCard.Name = "lblTitleSharpenCard";
            this.lblTitleSharpenCard.Size = new System.Drawing.Size(100, 15);
            this.lblTitleSharpenCard.TabIndex = 0;
            this.lblTitleSharpenCard.Text = "5 · Sharpen";

            // chkSharpen
            this.chkSharpen.AutoSize = true;
            this.chkSharpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.chkSharpen.Checked = true;
            this.chkSharpen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkSharpen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkSharpen.Location = new System.Drawing.Point(10, 30);
            this.chkSharpen.Name = "chkSharpen";
            this.chkSharpen.Size = new System.Drawing.Size(200, 19);
            this.chkSharpen.TabIndex = 1;
            this.chkSharpen.Text = "Sharpen";
            this.chkSharpen.UseVisualStyleBackColor = false;

            // btnApplySharpen
            this.btnApplySharpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnApplySharpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplySharpen.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnApplySharpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplySharpen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApplySharpen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnApplySharpen.Location = new System.Drawing.Point(10, 106);
            this.btnApplySharpen.Name = "btnApplySharpen";
            this.btnApplySharpen.Size = new System.Drawing.Size(300, 26);
            this.btnApplySharpen.TabIndex = 2;
            this.btnApplySharpen.Text = "Apply";
            this.btnApplySharpen.UseVisualStyleBackColor = false;

            // lblProptrkSharpenSig (row 1, Y=50)
            this.lblProptrkSharpenSig.AutoSize = true;
            this.lblProptrkSharpenSig.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkSharpenSig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkSharpenSig.Location = new System.Drawing.Point(10, 50);
            this.lblProptrkSharpenSig.Name = "lblProptrkSharpenSig";
            this.lblProptrkSharpenSig.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkSharpenSig.TabIndex = 3;
            this.lblProptrkSharpenSig.Text = "Sigma";

            // trkSharpenSig (row 1, Y=49)
            this.trkSharpenSig.AutoSize = false;
            this.trkSharpenSig.Location = new System.Drawing.Point(78, 49);
            this.trkSharpenSig.Name = "trkSharpenSig";
            this.trkSharpenSig.Size = new System.Drawing.Size(194, 22);
            this.trkSharpenSig.TabIndex = 4;
            this.trkSharpenSig.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkSharpenSig.Minimum = 1;
            this.trkSharpenSig.Maximum = 30;
            this.trkSharpenSig.Value = 10;

            // lblSharpenSig (row 1)
            this.lblSharpenSig.AutoSize = true;
            this.lblSharpenSig.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSharpenSig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblSharpenSig.Location = new System.Drawing.Point(276, 50);
            this.lblSharpenSig.Name = "lblSharpenSig";
            this.lblSharpenSig.Size = new System.Drawing.Size(42, 13);
            this.lblSharpenSig.TabIndex = 5;
            this.lblSharpenSig.Text = "1.0";

            // lblProptrkSharpenStr (row 2, Y=76)
            this.lblProptrkSharpenStr.AutoSize = true;
            this.lblProptrkSharpenStr.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkSharpenStr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkSharpenStr.Location = new System.Drawing.Point(10, 76);
            this.lblProptrkSharpenStr.Name = "lblProptrkSharpenStr";
            this.lblProptrkSharpenStr.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkSharpenStr.TabIndex = 3;
            this.lblProptrkSharpenStr.Text = "Strength";

            // trkSharpenStr (row 2, Y=75)
            this.trkSharpenStr.AutoSize = false;
            this.trkSharpenStr.Location = new System.Drawing.Point(78, 75);
            this.trkSharpenStr.Name = "trkSharpenStr";
            this.trkSharpenStr.Size = new System.Drawing.Size(194, 22);
            this.trkSharpenStr.TabIndex = 4;
            this.trkSharpenStr.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkSharpenStr.Minimum = 1;
            this.trkSharpenStr.Maximum = 20;
            this.trkSharpenStr.Value = 12;

            // lblSharpenStr (row 2)
            this.lblSharpenStr.AutoSize = true;
            this.lblSharpenStr.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblSharpenStr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblSharpenStr.Location = new System.Drawing.Point(276, 76);
            this.lblSharpenStr.Name = "lblSharpenStr";
            this.lblSharpenStr.Size = new System.Drawing.Size(42, 13);
            this.lblSharpenStr.TabIndex = 5;
            this.lblSharpenStr.Text = "1.2";

            // ── BinariseCard (1 slider, height=115, Apply at Y=80) ───────────
            this.BinariseCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.BinariseCard.Location = new System.Drawing.Point(0, 0);
            this.BinariseCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.BinariseCard.Name = "BinariseCard";
            this.BinariseCard.Size = new System.Drawing.Size(320, 115);
            this.BinariseCard.TabIndex = 0;
            this.BinariseCard.Controls.Add(this.lblTitleBinariseCard);
            this.BinariseCard.Controls.Add(this.chkBin);
            this.BinariseCard.Controls.Add(this.btnApplyBinarise);
            this.BinariseCard.Controls.Add(this.lblProptrkBinThr);
            this.BinariseCard.Controls.Add(this.trkBinThr);
            this.BinariseCard.Controls.Add(this.lblBinThr);

            // lblTitleBinariseCard
            this.lblTitleBinariseCard.AutoSize = true;
            this.lblTitleBinariseCard.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitleBinariseCard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitleBinariseCard.Location = new System.Drawing.Point(10, 10);
            this.lblTitleBinariseCard.Name = "lblTitleBinariseCard";
            this.lblTitleBinariseCard.Size = new System.Drawing.Size(100, 15);
            this.lblTitleBinariseCard.TabIndex = 0;
            this.lblTitleBinariseCard.Text = "6 · Binarise";

            // chkBin
            this.chkBin.AutoSize = true;
            this.chkBin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.chkBin.Checked = true;
            this.chkBin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkBin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkBin.Location = new System.Drawing.Point(10, 30);
            this.chkBin.Name = "chkBin";
            this.chkBin.Size = new System.Drawing.Size(200, 19);
            this.chkBin.TabIndex = 1;
            this.chkBin.Text = "Binarise (Otsu when 0)";
            this.chkBin.UseVisualStyleBackColor = false;

            // btnApplyBinarise
            this.btnApplyBinarise.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnApplyBinarise.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyBinarise.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnApplyBinarise.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyBinarise.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnApplyBinarise.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnApplyBinarise.Location = new System.Drawing.Point(10, 80);
            this.btnApplyBinarise.Name = "btnApplyBinarise";
            this.btnApplyBinarise.Size = new System.Drawing.Size(300, 26);
            this.btnApplyBinarise.TabIndex = 2;
            this.btnApplyBinarise.Text = "Apply";
            this.btnApplyBinarise.UseVisualStyleBackColor = false;

            // lblProptrkBinThr (row 1, Y=50)
            this.lblProptrkBinThr.AutoSize = true;
            this.lblProptrkBinThr.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProptrkBinThr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProptrkBinThr.Location = new System.Drawing.Point(10, 50);
            this.lblProptrkBinThr.Name = "lblProptrkBinThr";
            this.lblProptrkBinThr.Size = new System.Drawing.Size(65, 13);
            this.lblProptrkBinThr.TabIndex = 3;
            this.lblProptrkBinThr.Text = "Threshold";

            // trkBinThr (row 1, Y=49)
            this.trkBinThr.AutoSize = false;
            this.trkBinThr.Location = new System.Drawing.Point(78, 49);
            this.trkBinThr.Name = "trkBinThr";
            this.trkBinThr.Size = new System.Drawing.Size(194, 22);
            this.trkBinThr.TabIndex = 4;
            this.trkBinThr.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkBinThr.Minimum = 0;
            this.trkBinThr.Maximum = 255;
            this.trkBinThr.Value = 0;

            // lblBinThr
            this.lblBinThr.AutoSize = true;
            this.lblBinThr.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblBinThr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblBinThr.Location = new System.Drawing.Point(276, 50);
            this.lblBinThr.Name = "lblBinThr";
            this.lblBinThr.Size = new System.Drawing.Size(42, 13);
            this.lblBinThr.TabIndex = 5;
            this.lblBinThr.Text = "Otsu";

            // ── leftScroll ────────────────────────────────────────────────────
            this.leftScroll.AutoScroll = true;
            this.leftScroll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.leftScroll.Controls.Add(this.stepsFlow);
            this.leftScroll.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftScroll.Location = new System.Drawing.Point(0, 0);
            this.leftScroll.Name = "leftScroll";
            this.leftScroll.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.leftScroll.Size = new System.Drawing.Size(340, 600);
            this.leftScroll.TabIndex = 0;

            // ── previewBox ────────────────────────────────────────────────────
            this.previewBox.BackColor = System.Drawing.Color.Transparent;
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBox.Location = new System.Drawing.Point(340, 0);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(460, 578);
            this.previewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.previewBox.TabIndex = 1;
            this.previewBox.TabStop = false;

            // ── lblInfo ───────────────────────────────────────────────────────
            this.lblInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblInfo.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblInfo.Location = new System.Drawing.Point(340, 578);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblInfo.Size = new System.Drawing.Size(460, 22);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "No source loaded — load an image or capture a frame first";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── btnApplyAll ───────────────────────────────────────────────────
            this.btnApplyAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.btnApplyAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyAll.FlatAppearance.BorderSize = 0;
            this.btnApplyAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnApplyAll.ForeColor = System.Drawing.Color.Black;
            this.btnApplyAll.Location = new System.Drawing.Point(0, 300);
            this.btnApplyAll.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.btnApplyAll.Name = "btnApplyAll";
            this.btnApplyAll.Size = new System.Drawing.Size(320, 36);
            this.btnApplyAll.TabIndex = 3;
            this.btnApplyAll.Text = "▶  Apply All Steps";
            this.btnApplyAll.UseVisualStyleBackColor = false;

            // ── btnUseAsOcr ───────────────────────────────────────────────────
            this.btnUseAsOcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.btnUseAsOcr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUseAsOcr.FlatAppearance.BorderSize = 0;
            this.btnUseAsOcr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUseAsOcr.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnUseAsOcr.ForeColor = System.Drawing.Color.White;
            this.btnUseAsOcr.Location = new System.Drawing.Point(0, 340);
            this.btnUseAsOcr.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnUseAsOcr.Name = "btnUseAsOcr";
            this.btnUseAsOcr.Size = new System.Drawing.Size(320, 36);
            this.btnUseAsOcr.TabIndex = 4;
            this.btnUseAsOcr.Text = "⚡  Use as OCR Input (run selected steps)";
            this.btnUseAsOcr.UseVisualStyleBackColor = false;

            // ── btnReset ──────────────────────────────────────────────────────
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnReset.Location = new System.Drawing.Point(0, 380);
            this.btnReset.Margin = new System.Windows.Forms.Padding(0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(320, 36);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "↺  Reset to Source";
            this.btnReset.UseVisualStyleBackColor = false;

            // ── PreprocessingTab ──────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.leftScroll);
            this.Name = "PreprocessingTab";
            this.Size = new System.Drawing.Size(800, 600);
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            this.leftScroll.ResumeLayout(false);
            this.stepsFlow.ResumeLayout(false);
            this.UpscaleCard.ResumeLayout(false);
            this.UpscaleCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkUpscale)).EndInit();
            this.GrayCard.ResumeLayout(false);
            this.GrayCard.PerformLayout();
            this.ClaheCard.ResumeLayout(false);
            this.ClaheCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkClaheClip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkClaheTile)).EndInit();
            this.DeskewCard.ResumeLayout(false);
            this.DeskewCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkDeskewMax)).EndInit();
            this.SharpenCard.ResumeLayout(false);
            this.SharpenCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSharpenSig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkSharpenStr)).EndInit();
            this.BinariseCard.ResumeLayout(false);
            this.BinariseCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkBinThr)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel leftScroll;
        private System.Windows.Forms.FlowLayoutPanel stepsFlow;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnApplyAll;
        private System.Windows.Forms.Button btnUseAsOcr;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Panel UpscaleCard;
        private System.Windows.Forms.Label lblTitleUpscaleCard;
        private System.Windows.Forms.CheckBox chkUpscale;
        private System.Windows.Forms.Button btnApplyUpscale;
        private System.Windows.Forms.TrackBar trkUpscale;
        private System.Windows.Forms.Label lblUpscaleVal;
        private System.Windows.Forms.Label lblProptrkUpscale;
        private System.Windows.Forms.Panel GrayCard;
        private System.Windows.Forms.Label lblTitleGrayCard;
        private System.Windows.Forms.CheckBox chkGray;
        private System.Windows.Forms.Button btnApplyGray;
        private System.Windows.Forms.Panel ClaheCard;
        private System.Windows.Forms.Label lblTitleClaheCard;
        private System.Windows.Forms.CheckBox chkClahe;
        private System.Windows.Forms.Button btnApplyClahe;
        private System.Windows.Forms.TrackBar trkClaheClip;
        private System.Windows.Forms.Label lblClaheClip;
        private System.Windows.Forms.Label lblProptrkClaheClip;
        private System.Windows.Forms.TrackBar trkClaheTile;
        private System.Windows.Forms.Label lblClaheTile;
        private System.Windows.Forms.Label lblProptrkClaheTile;
        private System.Windows.Forms.Panel DeskewCard;
        private System.Windows.Forms.Label lblTitleDeskewCard;
        private System.Windows.Forms.CheckBox chkDeskew;
        private System.Windows.Forms.Button btnApplyDeskew;
        private System.Windows.Forms.TrackBar trkDeskewMax;
        private System.Windows.Forms.Label lblDeskewMax;
        private System.Windows.Forms.Label lblProptrkDeskewMax;
        private System.Windows.Forms.Panel SharpenCard;
        private System.Windows.Forms.Label lblTitleSharpenCard;
        private System.Windows.Forms.CheckBox chkSharpen;
        private System.Windows.Forms.Button btnApplySharpen;
        private System.Windows.Forms.TrackBar trkSharpenSig;
        private System.Windows.Forms.Label lblSharpenSig;
        private System.Windows.Forms.Label lblProptrkSharpenSig;
        private System.Windows.Forms.TrackBar trkSharpenStr;
        private System.Windows.Forms.Label lblSharpenStr;
        private System.Windows.Forms.Label lblProptrkSharpenStr;
        private System.Windows.Forms.Panel BinariseCard;
        private System.Windows.Forms.Label lblTitleBinariseCard;
        private System.Windows.Forms.CheckBox chkBin;
        private System.Windows.Forms.Button btnApplyBinarise;
        private System.Windows.Forms.TrackBar trkBinThr;
        private System.Windows.Forms.Label lblBinThr;
        private System.Windows.Forms.Label lblProptrkBinThr;
    }
}
