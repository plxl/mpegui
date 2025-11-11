namespace mpegui
{
    partial class formPasteEdits
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPasteEdits));
            this.listEdits = new mpegui.CustomCheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lblIncompatibility = new System.Windows.Forms.Label();
            this.infoIncompatibility = new mpegui.InfoButton();
            this.lnkAll = new System.Windows.Forms.LinkLabel();
            this.lnkNone = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // listEdits
            // 
            this.listEdits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listEdits.CheckOnClick = true;
            this.listEdits.FormattingEnabled = true;
            this.listEdits.Items.AddRange(new object[] {
            "Trim Settings (Start, End/Duration): none",
            "Encoder: nvenc_hevc",
            "hvc1 Tag: YES",
            "Crop Filter: none",
            "Audio Gain: 0.00",
            "Audio Delay Seconds: 0.00",
            "Audio Normalisation: False",
            "CRF/CQP: 22",
            "Encoder Preset: p7",
            "Frames per second: Same as source",
            "Video Speed: 1.00x",
            "Additional Options:",
            "Output Name: {filename}_out.mp4",
            "Overwrite if exists: NO"});
            this.listEdits.Location = new System.Drawing.Point(12, 27);
            this.listEdits.Name = "listEdits";
            this.listEdits.RedIndices = ((System.Collections.Generic.List<int>)(resources.GetObject("listEdits.RedIndices")));
            this.listEdits.Size = new System.Drawing.Size(347, 244);
            this.listEdits.TabIndex = 0;
            this.listEdits.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listEdits_ItemCheck);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(284, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(203, 278);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Paste Edits";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lblIncompatibility
            // 
            this.lblIncompatibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIncompatibility.AutoSize = true;
            this.lblIncompatibility.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIncompatibility.ForeColor = System.Drawing.Color.Red;
            this.lblIncompatibility.Location = new System.Drawing.Point(12, 283);
            this.lblIncompatibility.Name = "lblIncompatibility";
            this.lblIncompatibility.Size = new System.Drawing.Size(146, 13);
            this.lblIncompatibility.TabIndex = 3;
            this.lblIncompatibility.Text = "Potential Incompatibility!";
            // 
            // infoIncompatibility
            // 
            this.infoIncompatibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.infoIncompatibility.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.infoIncompatibility.InfoText = resources.GetString("infoIncompatibility.InfoText");
            this.infoIncompatibility.InfoTitle = "Potential Incompatibility";
            this.infoIncompatibility.Location = new System.Drawing.Point(158, 281);
            this.infoIncompatibility.Name = "infoIncompatibility";
            this.infoIncompatibility.Size = new System.Drawing.Size(20, 20);
            this.infoIncompatibility.TabIndex = 4;
            // 
            // lnkAll
            // 
            this.lnkAll.AutoSize = true;
            this.lnkAll.Location = new System.Drawing.Point(9, 11);
            this.lnkAll.Name = "lnkAll";
            this.lnkAll.Size = new System.Drawing.Size(18, 13);
            this.lnkAll.TabIndex = 5;
            this.lnkAll.TabStop = true;
            this.lnkAll.Text = "All";
            this.lnkAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAll_LinkClicked);
            // 
            // lnkNone
            // 
            this.lnkNone.AutoSize = true;
            this.lnkNone.Location = new System.Drawing.Point(33, 11);
            this.lnkNone.Name = "lnkNone";
            this.lnkNone.Size = new System.Drawing.Size(33, 13);
            this.lnkNone.TabIndex = 6;
            this.lnkNone.TabStop = true;
            this.lnkNone.Text = "None";
            this.lnkNone.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkNone_LinkClicked);
            // 
            // formPasteEdits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 313);
            this.Controls.Add(this.lnkNone);
            this.Controls.Add(this.lnkAll);
            this.Controls.Add(this.infoIncompatibility);
            this.Controls.Add(this.lblIncompatibility);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.listEdits);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formPasteEdits";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Paste Edits";
            this.Load += new System.EventHandler(this.formPasteEdits_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomCheckedListBox listEdits;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Label lblIncompatibility;
        private InfoButton infoIncompatibility;
        private System.Windows.Forms.LinkLabel lnkAll;
        private System.Windows.Forms.LinkLabel lnkNone;
    }
}