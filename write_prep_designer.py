import sys

prep_ds_path = r'PreprocessingTab.Designer.cs'

header = """namespace OcrTesseract
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
"""

cards_data = [
    {
        "name": "UpscaleCard", "title": "1 · Upscale", "color": "255, 152, 0",
        "chk": "chkUpscale", "chk_txt": "Upscale", "chk_def": "true",
        "btn_apply": "btnApplyUpscale",
        "rows": [
            {"label": "Factor", "trk": "trkUpscale", "val": 20, "val_lbl": "lblUpscaleVal", "val_txt": "×2.0"},
        ]
    },
    {
        "name": "GrayCard", "title": "2 · Grayscale", "color": "140, 145, 155",
        "chk": "chkGray", "chk_txt": "Convert to Grayscale", "chk_def": "true",
        "btn_apply": "btnApplyGray",
        "rows": []
    },
    {
        "name": "ClaheCard", "title": "3 · CLAHE", "color": "0, 180, 255",
        "chk": "chkClahe", "chk_txt": "CLAHE", "chk_def": "true",
        "btn_apply": "btnApplyClahe",
        "rows": [
            {"label": "Clip limit", "trk": "trkClaheClip", "val": 3, "val_lbl": "lblClaheClip", "val_txt": "3.0"},
            {"label": "Tile size", "trk": "trkClaheTile", "val": 8, "val_lbl": "lblClaheTile", "val_txt": "8"},
        ]
    },
    {
        "name": "DeskewCard", "title": "4 · Deskew", "color": "180, 80, 255",
        "chk": "chkDeskew", "chk_txt": "Deskew", "chk_def": "true",
        "btn_apply": "btnApplyDeskew",
        "rows": [
            {"label": "Max angle", "trk": "trkDeskewMax", "val": 15, "val_lbl": "lblDeskewMax", "val_txt": "15°"},
        ]
    },
    {
        "name": "SharpenCard", "title": "5 · Sharpen", "color": "255, 200, 0",
        "chk": "chkSharpen", "chk_txt": "Sharpen", "chk_def": "true",
        "btn_apply": "btnApplySharpen",
        "rows": [
            {"label": "Sigma", "trk": "trkSharpenSig", "val": 10, "val_lbl": "lblSharpenSig", "val_txt": "1.0"},
            {"label": "Strength", "trk": "trkSharpenStr", "val": 12, "val_lbl": "lblSharpenStr", "val_txt": "1.2"},
        ]
    },
    {
        "name": "BinariseCard", "title": "6 · Binarise", "color": "57, 255, 20",
        "chk": "chkBin", "chk_txt": "Binarise (Otsu when 0)", "chk_def": "true",
        "btn_apply": "btnApplyBinarise",
        "rows": [
            {"label": "Threshold", "trk": "trkBinThr", "val": 0, "val_lbl": "lblBinThr", "val_txt": "Otsu"},
        ]
    },
]

init_code = ""
fields_code = ""

init_code += "            // Instantiate dynamic panels\n"
for c in cards_data:
    fields_code += f"        private System.Windows.Forms.Panel {c['name']};\n"
    fields_code += f"        private System.Windows.Forms.Label lblTitle{c['name']};\n"
    fields_code += f"        private System.Windows.Forms.CheckBox {c['chk']};\n"
    fields_code += f"        private System.Windows.Forms.Button {c['btn_apply']};\n"

    init_code += f"            this.{c['name']} = new System.Windows.Forms.Panel();\n"
    init_code += f"            this.lblTitle{c['name']} = new System.Windows.Forms.Label();\n"
    init_code += f"            this.{c['chk']} = new System.Windows.Forms.CheckBox();\n"
    init_code += f"            this.{c['btn_apply']} = new System.Windows.Forms.Button();\n"
    for r in c['rows']:
        fields_code += f"        private System.Windows.Forms.TrackBar {r['trk']};\n"
        fields_code += f"        private System.Windows.Forms.Label {r['val_lbl']};\n"
        fields_code += f"        private System.Windows.Forms.Label lblProp{r['trk']};\n"
        init_code += f"            this.{r['trk']} = new System.Windows.Forms.TrackBar();\n"
        init_code += f"            this.{r['val_lbl']} = new System.Windows.Forms.Label();\n"
        init_code += f"            this.lblProp{r['trk']} = new System.Windows.Forms.Label();\n"

init_code += """            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            this.leftScroll.SuspendLayout();
            this.stepsFlow.SuspendLayout();
"""
for c in cards_data:
    init_code += f"            this.{c['name']}.SuspendLayout();\n"
    for r in c['rows']:
        init_code += f"            ((System.ComponentModel.ISupportInitialize)(this.{r['trk']})).BeginInit();\n"

init_code += "            this.SuspendLayout();\n"

# Setup stepsFlow
init_code += f"""
            // stepsFlow
            this.stepsFlow.AutoSize = true;
            this.stepsFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stepsFlow.Dock = System.Windows.Forms.DockStyle.Top;
            this.stepsFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.stepsFlow.Location = new System.Drawing.Point(0, 0);
            this.stepsFlow.Name = "stepsFlow";
            this.stepsFlow.Padding = new System.Windows.Forms.Padding(0);
            this.stepsFlow.Size = new System.Drawing.Size(360, 100);
            this.stepsFlow.TabIndex = 0;
            this.stepsFlow.WrapContents = false;
"""
for c in cards_data:
    init_code += f"            this.stepsFlow.Controls.Add(this.{c['name']});\n"
init_code += """            this.stepsFlow.Controls.Add(this.btnApplyAll);
            this.stepsFlow.Controls.Add(this.btnUseAsOcr);
            this.stepsFlow.Controls.Add(this.btnReset);
"""

# Setup Cards
for c in cards_data:
    init_code += f"""
            // {c['name']}
            this.{c['name']}.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(39)))), ((int)(((byte)(46)))));
            this.{c['name']}.Location = new System.Drawing.Point(0, 0);
            this.{c['name']}.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.{c['name']}.Name = "{c['name']}";
            this.{c['name']}.Size = new System.Drawing.Size(330, 200);
            this.{c['name']}.TabIndex = 0;
            this.{c['name']}.Controls.Add(this.lblTitle{c['name']});
            this.{c['name']}.Controls.Add(this.{c['chk']});
            this.{c['name']}.Controls.Add(this.{c['btn_apply']});
"""
    for r in c['rows']:
        init_code += f"            this.{c['name']}.Controls.Add(this.lblProp{r['trk']});\n"
        init_code += f"            this.{c['name']}.Controls.Add(this.{r['trk']});\n"
        init_code += f"            this.{c['name']}.Controls.Add(this.{r['val_lbl']});\n"

    init_code += f"""
            // lblTitle{c['name']}
            this.lblTitle{c['name']}.AutoSize = true;
            this.lblTitle{c['name']}.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle{c['name']}.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblTitle{c['name']}.Location = new System.Drawing.Point(10, 10);
            this.lblTitle{c['name']}.Name = "lblTitle{c['name']}";
            this.lblTitle{c['name']}.Size = new System.Drawing.Size(100, 15);
            this.lblTitle{c['name']}.TabIndex = 0;
            this.lblTitle{c['name']}.Text = "{c['title']}";

            // {c['chk']}
            this.{c['chk']}.AutoSize = true;
            this.{c['chk']}.Checked = {c['chk_def'].lower()};
            this.{c['chk']}.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.{c['chk']}.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.{c['chk']}.Location = new System.Drawing.Point(10, 30);
            this.{c['chk']}.Name = "{c['chk']}";
            this.{c['chk']}.Size = new System.Drawing.Size(200, 19);
            this.{c['chk']}.TabIndex = 1;
            this.{c['chk']}.Text = "{c['chk_txt']}";
            this.{c['chk']}.UseVisualStyleBackColor = true;

            // {c['btn_apply']}
            this.{c['btn_apply']}.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.{c['btn_apply']}.Cursor = System.Windows.Forms.Cursors.Hand;
            this.{c['btn_apply']}.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.{c['btn_apply']}.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.{c['btn_apply']}.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.{c['btn_apply']}.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.{c['btn_apply']}.Location = new System.Drawing.Point(10, 150);
            this.{c['btn_apply']}.Name = "{c['btn_apply']}";
            this.{c['btn_apply']}.Size = new System.Drawing.Size(300, 26);
            this.{c['btn_apply']}.TabIndex = 2;
            this.{c['btn_apply']}.Text = "Apply";
            this.{c['btn_apply']}.UseVisualStyleBackColor = false;
"""
    y_off = 55
    for r in c['rows']:
        init_code += f"""
            // lblProp{r['trk']}
            this.lblProp{r['trk']}.AutoSize = true;
            this.lblProp{r['trk']}.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProp{r['trk']}.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblProp{r['trk']}.Location = new System.Drawing.Point(10, {y_off});
            this.lblProp{r['trk']}.Name = "lblProp{r['trk']}";
            this.lblProp{r['trk']}.Size = new System.Drawing.Size(60, 13);
            this.lblProp{r['trk']}.TabIndex = 3;
            this.lblProp{r['trk']}.Text = "{r['label']}";

            // {r['trk']}
            this.{r['trk']}.AutoSize = false;
            this.{r['trk']}.Location = new System.Drawing.Point(70, {y_off});
            this.{r['trk']}.Name = "{r['trk']}";
            this.{r['trk']}.Size = new System.Drawing.Size(130, 20);
            this.{r['trk']}.TabIndex = 4;
            this.{r['trk']}.TickStyle = System.Windows.Forms.TickStyle.None;
            this.{r['trk']}.Value = {r['val']};

            // {r['val_lbl']}
            this.{r['val_lbl']}.AutoSize = true;
            this.{r['val_lbl']}.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.{r['val_lbl']}.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.{r['val_lbl']}.Location = new System.Drawing.Point(210, {y_off});
            this.{r['val_lbl']}.Name = "{r['val_lbl']}";
            this.{r['val_lbl']}.Size = new System.Drawing.Size(40, 13);
            this.{r['val_lbl']}.TabIndex = 5;
            this.{r['val_lbl']}.Text = "{r['val_txt']}";
"""
        y_off += 25

init_code += """
            // leftScroll
            this.leftScroll.AutoScroll = true;
            this.leftScroll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(30)))), ((int)(((byte)(36)))));
            this.leftScroll.Controls.Add(this.stepsFlow);
            this.leftScroll.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftScroll.Location = new System.Drawing.Point(0, 0);
            this.leftScroll.Name = "leftScroll";
            this.leftScroll.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.leftScroll.Size = new System.Drawing.Size(360, 600);
            this.leftScroll.TabIndex = 0;

            // previewBox
            this.previewBox.BackColor = System.Drawing.Color.Transparent;
            this.previewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewBox.Location = new System.Drawing.Point(360, 0);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(440, 578);
            this.previewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.previewBox.TabIndex = 1;
            this.previewBox.TabStop = false;

            // lblInfo
            this.lblInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(27)))));
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblInfo.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(145)))), ((int)(((byte)(155)))));
            this.lblInfo.Location = new System.Drawing.Point(360, 578);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblInfo.Size = new System.Drawing.Size(440, 22);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "No source loaded — load an image or capture a frame first";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // btnApplyAll
            this.btnApplyAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(57)))), ((int)(((byte)(255)))), ((int)(((byte)(20)))));
            this.btnApplyAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApplyAll.FlatAppearance.BorderSize = 0;
            this.btnApplyAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnApplyAll.ForeColor = System.Drawing.Color.Black;
            this.btnApplyAll.Location = new System.Drawing.Point(0, 300);
            this.btnApplyAll.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.btnApplyAll.Name = "btnApplyAll";
            this.btnApplyAll.Size = new System.Drawing.Size(330, 34);
            this.btnApplyAll.TabIndex = 3;
            this.btnApplyAll.Text = "▶  Apply All Steps";
            this.btnApplyAll.UseVisualStyleBackColor = false;

            // btnUseAsOcr
            this.btnUseAsOcr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(200)))));
            this.btnUseAsOcr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUseAsOcr.FlatAppearance.BorderSize = 0;
            this.btnUseAsOcr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUseAsOcr.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnUseAsOcr.ForeColor = System.Drawing.Color.White;
            this.btnUseAsOcr.Location = new System.Drawing.Point(0, 340);
            this.btnUseAsOcr.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.btnUseAsOcr.Name = "btnUseAsOcr";
            this.btnUseAsOcr.Size = new System.Drawing.Size(330, 34);
            this.btnUseAsOcr.TabIndex = 4;
            this.btnUseAsOcr.Text = "⚡  Use as OCR Input";
            this.btnUseAsOcr.UseVisualStyleBackColor = false;

            // btnReset
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(52)))), ((int)(((byte)(62)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnReset.Location = new System.Drawing.Point(0, 380);
            this.btnReset.Margin = new System.Windows.Forms.Padding(0);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(330, 34);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "↺  Reset to Source";
            this.btnReset.UseVisualStyleBackColor = false;

            // PreprocessingTab
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
"""
for c in cards_data:
    init_code += f"            this.{c['name']}.ResumeLayout(false);\n"
    init_code += f"            this.{c['name']}.PerformLayout();\n"
    for r in c['rows']:
        init_code += f"            ((System.ComponentModel.ISupportInitialize)(this.{r['trk']})).EndInit();\n"
init_code += """            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel leftScroll;
        private System.Windows.Forms.FlowLayoutPanel stepsFlow;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnApplyAll;
        private System.Windows.Forms.Button btnUseAsOcr;
        private System.Windows.Forms.Button btnReset;
"""

fields_code += """    }
}
"""

with open(prep_ds_path, 'w', encoding='utf-8') as f:
    f.write(header + init_code + fields_code)
print('wrote PreprocessingTab.Designer.cs successfully.')
