import sys
import os

form1_ds_path = r'Form1.Designer.cs'
with open(form1_ds_path, 'w', encoding='utf-8') as f:
    f.write('''namespace OcrTesseract
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            // Dispose Emgu CV video capture and other resources if any
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.statusBar = new System.Windows.Forms.Panel();
            this.lblCamStatus = new System.Windows.Forms.Label();
            this.lblLighting = new System.Windows.Forms.Label();
            this.lblProcStatus = new System.Windows.Forms.Label();
            this.rootTable = new System.Windows.Forms.TableLayoutPanel();
            this.mainTabs = new System.Windows.Forms.TabControl();
            this.tabOcr = new System.Windows.Forms.TabPage();
            this.bodyTable = new System.Windows.Forms.TableLayoutPanel();
            this.leftTable = new System.Windows.Forms.TableLayoutPanel();
            this.pnlProcessTime = new System.Windows.Forms.Panel();
            this.lblProcTime = new System.Windows.Forms.Label();
            this.lblProcLabel = new System.Windows.Forms.Label();
            this.lblDetResults = new System.Windows.Forms.Label();
            this.resultsScroll = new System.Windows.Forms.Panel();
            this.resultsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblEngineLabel = new System.Windows.Forms.Label();
            this.cmbEngine = new System.Windows.Forms.ComboBox();
            this.lblCameraLabel = new System.Windows.Forms.Label();
            this.cmbCamera = new System.Windows.Forms.ComboBox();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnCamera = new System.Windows.Forms.Button();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnRunOcr = new System.Windows.Forms.Button();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblFps = new System.Windows.Forms.Label();
            this.tabPrep = new System.Windows.Forms.TabPage();
            this.prepTab = new OcrTesseract.PreprocessingTab();
            this.statusBar.SuspendLayout();
            this.rootTable.SuspendLayout();
            this.mainTabs.SuspendLayout();
            this.tabOcr.SuspendLayout();
            this.bodyTable.SuspendLayout();
            this.leftTable.SuspendLayout();
            this.pnlProcessTime.SuspendLayout();
            this.resultsScroll.SuspendLayout();
            this.rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.tabPrep.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.statusBar.Controls.Add(this.lblProcStatus);
            this.statusBar.Controls.Add(this.lblLighting);
            this.statusBar.Controls.Add(this.lblCamStatus);
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusBar.Location = new System.Drawing.Point(0, 670);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.statusBar.Size = new System.Drawing.Size(1100, 30);
            this.statusBar.TabIndex = 1;
            // 
            // lblCamStatus
            // 
            this.lblCamStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCamStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCamStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblCamStatus.Location = new System.Drawing.Point(12, 0);
            this.lblCamStatus.Name = "lblCamStatus";
            this.lblCamStatus.Size = new System.Drawing.Size(300, 30);
            this.lblCamStatus.TabIndex = 0;
            this.lblCamStatus.Text = "● Camera: Disconnected";
            this.lblCamStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLighting
            // 
            this.lblLighting.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblLighting.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblLighting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(83)))));
            this.lblLighting.Location = new System.Drawing.Point(312, 0);
            this.lblLighting.Name = "lblLighting";
            this.lblLighting.Size = new System.Drawing.Size(130, 30);
            this.lblLighting.TabIndex = 1;
            this.lblLighting.Text = "✓ Lighting OK";
            this.lblLighting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblProcStatus
            // 
            this.lblProcStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblProcStatus.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProcStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProcStatus.Location = new System.Drawing.Point(878, 0);
            this.lblProcStatus.Name = "lblProcStatus";
            this.lblProcStatus.Size = new System.Drawing.Size(210, 30);
            this.lblProcStatus.TabIndex = 2;
            this.lblProcStatus.Text = "Processing: Ready";
            this.lblProcStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rootTable
            // 
            this.rootTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.rootTable.ColumnCount = 1;
            this.rootTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootTable.Controls.Add(this.mainTabs, 0, 0);
            this.rootTable.Controls.Add(this.statusBar, 0, 1);
            this.rootTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootTable.Location = new System.Drawing.Point(0, 0);
            this.rootTable.Margin = new System.Windows.Forms.Padding(0);
            this.rootTable.Name = "rootTable";
            this.rootTable.RowCount = 2;
            this.rootTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootTable.Size = new System.Drawing.Size(1100, 700);
            this.rootTable.TabIndex = 0;
            // 
            // mainTabs
            // 
            this.mainTabs.Controls.Add(this.tabOcr);
            this.mainTabs.Controls.Add(this.tabPrep);
            this.mainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.mainTabs.Location = new System.Drawing.Point(0, 0);
            this.mainTabs.Margin = new System.Windows.Forms.Padding(0);
            this.mainTabs.Name = "mainTabs";
            this.mainTabs.Padding = new System.Drawing.Point(12, 4);
            this.mainTabs.SelectedIndex = 0;
            this.mainTabs.Size = new System.Drawing.Size(1100, 670);
            this.mainTabs.TabIndex = 0;
            this.mainTabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.MainTabs_DrawItem);
            // 
            // tabOcr
            // 
            this.tabOcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.tabOcr.Controls.Add(this.bodyTable);
            this.tabOcr.Location = new System.Drawing.Point(4, 27);
            this.tabOcr.Margin = new System.Windows.Forms.Padding(0);
            this.tabOcr.Name = "tabOcr";
            this.tabOcr.Size = new System.Drawing.Size(1092, 639);
            this.tabOcr.TabIndex = 0;
            this.tabOcr.Text = "  OCR  ";
            // 
            // bodyTable
            // 
            this.bodyTable.ColumnCount = 2;
            this.bodyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            this.bodyTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyTable.Controls.Add(this.leftTable, 0, 0);
            this.bodyTable.Controls.Add(this.rightPanel, 1, 0);
            this.bodyTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyTable.Location = new System.Drawing.Point(0, 0);
            this.bodyTable.Margin = new System.Windows.Forms.Padding(0);
            this.bodyTable.Name = "bodyTable";
            this.bodyTable.Padding = new System.Windows.Forms.Padding(10, 10, 10, 6);
            this.bodyTable.RowCount = 1;
            this.bodyTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyTable.Size = new System.Drawing.Size(1092, 639);
            this.bodyTable.TabIndex = 0;
            // 
            // leftTable
            // 
            this.leftTable.ColumnCount = 1;
            this.leftTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftTable.Controls.Add(this.pnlProcessTime, 0, 0);
            this.leftTable.Controls.Add(this.lblDetResults, 0, 1);
            this.leftTable.Controls.Add(this.resultsScroll, 0, 2);
            this.leftTable.Controls.Add(this.lblEngineLabel, 0, 3);
            this.leftTable.Controls.Add(this.cmbEngine, 0, 4);
            this.leftTable.Controls.Add(this.lblCameraLabel, 0, 5);
            this.leftTable.Controls.Add(this.cmbCamera, 0, 6);
            this.leftTable.Controls.Add(this.btnLoadImage, 0, 7);
            this.leftTable.Controls.Add(this.btnCamera, 0, 8);
            this.leftTable.Controls.Add(this.btnCapture, 0, 9);
            this.leftTable.Controls.Add(this.btnRunOcr, 0, 10);
            this.leftTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftTable.Location = new System.Drawing.Point(10, 10);
            this.leftTable.Margin = new System.Windows.Forms.Padding(0);
            this.leftTable.Name = "leftTable";
            this.leftTable.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.leftTable.RowCount = 11;
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.leftTable.Size = new System.Drawing.Size(360, 623);
            this.leftTable.TabIndex = 0;
            // 
            // pnlProcessTime
            // 
            this.pnlProcessTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.pnlProcessTime.Controls.Add(this.lblProcTime);
            this.pnlProcessTime.Controls.Add(this.lblProcLabel);
            this.pnlProcessTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProcessTime.Location = new System.Drawing.Point(0, 0);
            this.pnlProcessTime.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.pnlProcessTime.Name = "pnlProcessTime";
            this.pnlProcessTime.Padding = new System.Windows.Forms.Padding(14, 6, 14, 6);
            this.pnlProcessTime.Size = new System.Drawing.Size(352, 104);
            this.pnlProcessTime.TabIndex = 0;
            this.pnlProcessTime.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlProcessTime_Paint);
            // 
            // lblProcLabel
            // 
            this.lblProcLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProcLabel.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProcLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProcLabel.Location = new System.Drawing.Point(14, 6);
            this.lblProcLabel.Name = "lblProcLabel";
            this.lblProcLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblProcLabel.Size = new System.Drawing.Size(324, 26);
            this.lblProcLabel.TabIndex = 1;
            this.lblProcLabel.Text = "TOTAL PROCESSING TIME";
            // 
            // lblProcTime
            // 
            this.lblProcTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProcTime.Font = new System.Drawing.Font("Consolas", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblProcTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblProcTime.Location = new System.Drawing.Point(14, 32);
            this.lblProcTime.Name = "lblProcTime";
            this.lblProcTime.Size = new System.Drawing.Size(324, 66);
            this.lblProcTime.TabIndex = 0;
            this.lblProcTime.Text = "—";
            this.lblProcTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDetResults
            // 
            this.lblDetResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDetResults.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDetResults.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblDetResults.Location = new System.Drawing.Point(0, 110);
            this.lblDetResults.Name = "lblDetResults";
            this.lblDetResults.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.lblDetResults.Size = new System.Drawing.Size(352, 32);
            this.lblDetResults.TabIndex = 1;
            this.lblDetResults.Text = "Detection Results";
            this.lblDetResults.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // resultsScroll
            // 
            this.resultsScroll.AutoScroll = true;
            this.resultsScroll.Controls.Add(this.resultsFlow);
            this.resultsScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsScroll.Location = new System.Drawing.Point(0, 142);
            this.resultsScroll.Margin = new System.Windows.Forms.Padding(0);
            this.resultsScroll.Name = "resultsScroll";
            this.resultsScroll.Size = new System.Drawing.Size(352, 171);
            this.resultsScroll.TabIndex = 2;
            // 
            // resultsFlow
            // 
            this.resultsFlow.AutoSize = true;
            this.resultsFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resultsFlow.Dock = System.Windows.Forms.DockStyle.Top;
            this.resultsFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.resultsFlow.Location = new System.Drawing.Point(0, 0);
            this.resultsFlow.Name = "resultsFlow";
            this.resultsFlow.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.resultsFlow.Size = new System.Drawing.Size(352, 2);
            this.resultsFlow.TabIndex = 0;
            this.resultsFlow.WrapContents = false;
            // 
            // lblEngineLabel
            // 
            this.lblEngineLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEngineLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblEngineLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblEngineLabel.Location = new System.Drawing.Point(0, 313);
            this.lblEngineLabel.Name = "lblEngineLabel";
            this.lblEngineLabel.Size = new System.Drawing.Size(352, 24);
            this.lblEngineLabel.TabIndex = 3;
            this.lblEngineLabel.Text = "OCR Engine";
            this.lblEngineLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cmbEngine
            // 
            this.cmbEngine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.cmbEngine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbEngine.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEngine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEngine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.cmbEngine.ItemHeight = 28;
            this.cmbEngine.Items.AddRange(new object[] {
            "WinRT  (~5–20 ms)",
            "RapidOCR  (~30–100 ms)",
            "Tesseract  (~700 ms)"});
            this.cmbEngine.Location = new System.Drawing.Point(0, 337);
            this.cmbEngine.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.cmbEngine.Name = "cmbEngine";
            this.cmbEngine.Size = new System.Drawing.Size(352, 34);
            this.cmbEngine.TabIndex = 4;
            this.cmbEngine.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmb_DrawItem);
            // 
            // lblCameraLabel
            // 
            this.lblCameraLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCameraLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCameraLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblCameraLabel.Location = new System.Drawing.Point(0, 379);
            this.lblCameraLabel.Name = "lblCameraLabel";
            this.lblCameraLabel.Size = new System.Drawing.Size(352, 24);
            this.lblCameraLabel.TabIndex = 5;
            this.lblCameraLabel.Text = "Camera";
            this.lblCameraLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cmbCamera
            // 
            this.cmbCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.cmbCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCamera.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.cmbCamera.ItemHeight = 28;
            this.cmbCamera.Location = new System.Drawing.Point(0, 403);
            this.cmbCamera.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.cmbCamera.Name = "cmbCamera";
            this.cmbCamera.Size = new System.Drawing.Size(352, 34);
            this.cmbCamera.TabIndex = 6;
            this.cmbCamera.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmb_DrawItem);
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnLoadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadImage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnLoadImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadImage.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnLoadImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnLoadImage.Location = new System.Drawing.Point(0, 445);
            this.btnLoadImage.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(352, 40);
            this.btnLoadImage.TabIndex = 7;
            this.btnLoadImage.Text = "📂  Load Image";
            this.btnLoadImage.UseVisualStyleBackColor = false;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // btnCamera
            // 
            this.btnCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnCamera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCamera.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCamera.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnCamera.Location = new System.Drawing.Point(0, 487);
            this.btnCamera.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnCamera.Name = "btnCamera";
            this.btnCamera.Size = new System.Drawing.Size(352, 40);
            this.btnCamera.TabIndex = 8;
            this.btnCamera.Text = "📷  Start Camera";
            this.btnCamera.UseVisualStyleBackColor = false;
            this.btnCamera.Click += new System.EventHandler(this.btnCamera_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.btnCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCapture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCapture.Enabled = false;
            this.btnCapture.FlatAppearance.BorderSize = 0;
            this.btnCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCapture.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCapture.ForeColor = System.Drawing.Color.White;
            this.btnCapture.Location = new System.Drawing.Point(0, 529);
            this.btnCapture.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(352, 40);
            this.btnCapture.TabIndex = 9;
            this.btnCapture.Text = "⚡  Capture && OCR";
            this.btnCapture.UseVisualStyleBackColor = false;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnRunOcr
            // 
            this.btnRunOcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.btnRunOcr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRunOcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRunOcr.FlatAppearance.BorderSize = 0;
            this.btnRunOcr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunOcr.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRunOcr.ForeColor = System.Drawing.Color.Black;
            this.btnRunOcr.Location = new System.Drawing.Point(0, 571);
            this.btnRunOcr.Margin = new System.Windows.Forms.Padding(0);
            this.btnRunOcr.Name = "btnRunOcr";
            this.btnRunOcr.Size = new System.Drawing.Size(352, 52);
            this.btnRunOcr.TabIndex = 10;
            this.btnRunOcr.Text = "▶  Run Inference";
            this.btnRunOcr.UseVisualStyleBackColor = false;
            this.btnRunOcr.Click += new System.EventHandler(this.btnRunOcr_Click);
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.rightPanel.Controls.Add(this.lblFps);
            this.rightPanel.Controls.Add(this.pictureBox);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(370, 10);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(2);
            this.rightPanel.Size = new System.Drawing.Size(712, 623);
            this.rightPanel.TabIndex = 1;
            this.rightPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.RightPanel_Paint);
            this.rightPanel.Resize += new System.EventHandler(this.rightPanel_Resize);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(2, 2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(708, 619);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // lblFps
            // 
            this.lblFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFps.AutoSize = true;
            this.lblFps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.lblFps.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFps.ForeColor = System.Drawing.Color.Black;
            this.lblFps.Location = new System.Drawing.Point(645, 10);
            this.lblFps.Name = "lblFps";
            this.lblFps.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.lblFps.Size = new System.Drawing.Size(55, 20);
            this.lblFps.TabIndex = 1;
            this.lblFps.Text = "FPS: --";
            // 
            // tabPrep
            // 
            this.tabPrep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.tabPrep.Controls.Add(this.prepTab);
            this.tabPrep.Location = new System.Drawing.Point(4, 27);
            this.tabPrep.Margin = new System.Windows.Forms.Padding(0);
            this.tabPrep.Name = "tabPrep";
            this.tabPrep.Size = new System.Drawing.Size(1092, 639);
            this.tabPrep.TabIndex = 1;
            this.tabPrep.Text = "  Preprocessing  ";
            // 
            // prepTab
            // 
            this.prepTab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.prepTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prepTab.Location = new System.Drawing.Point(0, 0);
            this.prepTab.Name = "prepTab";
            this.prepTab.Size = new System.Drawing.Size(1092, 639);
            this.prepTab.TabIndex = 0;
            this.prepTab.UseAsOcrInput += new System.EventHandler<System.Drawing.Bitmap>(this.PrepTab_UseAsOcrInput);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.rootTable);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TesseractOCR Pro - v1.0";
            this.statusBar.ResumeLayout(false);
            this.rootTable.ResumeLayout(false);
            this.mainTabs.ResumeLayout(false);
            this.tabOcr.ResumeLayout(false);
            this.bodyTable.ResumeLayout(false);
            this.leftTable.ResumeLayout(false);
            this.pnlProcessTime.ResumeLayout(false);
            this.resultsScroll.ResumeLayout(false);
            this.resultsScroll.PerformLayout();
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.tabPrep.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel statusBar;
        private System.Windows.Forms.Label lblCamStatus;
        private System.Windows.Forms.Label lblLighting;
        private System.Windows.Forms.Label lblProcStatus;
        private System.Windows.Forms.TableLayoutPanel rootTable;
        private System.Windows.Forms.TabControl mainTabs;
        private System.Windows.Forms.TabPage tabOcr;
        private System.Windows.Forms.TableLayoutPanel bodyTable;
        private System.Windows.Forms.TableLayoutPanel leftTable;
        private System.Windows.Forms.Panel pnlProcessTime;
        private System.Windows.Forms.Label lblProcLabel;
        private System.Windows.Forms.Label lblProcTime;
        private System.Windows.Forms.Label lblDetResults;
        private System.Windows.Forms.Panel resultsScroll;
        private System.Windows.Forms.FlowLayoutPanel resultsFlow;
        private System.Windows.Forms.Label lblEngineLabel;
        private System.Windows.Forms.ComboBox cmbEngine;
        private System.Windows.Forms.Label lblCameraLabel;
        private System.Windows.Forms.ComboBox cmbCamera;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnCamera;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnRunOcr;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.TabPage tabPrep;
        private OcrTesseract.PreprocessingTab prepTab;
    }
}
''')
    print('wrote Form1.Designer.cs successfully.')
