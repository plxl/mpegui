﻿namespace mpegui
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listFiles = new System.Windows.Forms.ListBox();
            this.textCommand = new System.Windows.Forms.TextBox();
            this.numAudioGain = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numAudioDelay = new System.Windows.Forms.NumericUpDown();
            this.dropEncoder = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnSpeedInfo = new System.Windows.Forms.Button();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.trkSpeed = new System.Windows.Forms.TrackBar();
            this.btnCRFinfo = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbFPS = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtAdditionalOptions = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbPreset = new System.Windows.Forms.ComboBox();
            this.lblCRF = new System.Windows.Forms.Label();
            this.trkCRF = new System.Windows.Forms.TrackBar();
            this.chkHvc1 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGetEnd = new System.Windows.Forms.Button();
            this.lblOutputFilename = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtOutName = new System.Windows.Forms.TextBox();
            this.chkOverwrite = new System.Windows.Forms.CheckBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.lblFilename = new System.Windows.Forms.Label();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCropPresets = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCrop = new System.Windows.Forms.TextBox();
            this.menuCropPresets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnRun = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnCopyCommand = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptionsEncoder = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptionsEncoderDrop = new System.Windows.Forms.ToolStripComboBox();
            this.menuOptionsHvc1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.defaultCRFCQPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptionsCRF = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.defaultPresetCPUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptionsPresetCPU = new System.Windows.Forms.ToolStripComboBox();
            this.defaultPresetGPUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptionsPresetGPU = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuOptionsCopyOverwrite = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.additionalOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptionsAdditionalOptions = new System.Windows.Forms.ToolStripTextBox();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.splitList = new System.Windows.Forms.SplitContainer();
            this.splitOutput = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioDelay)).BeginInit();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCRF)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitList)).BeginInit();
            this.splitList.Panel1.SuspendLayout();
            this.splitList.Panel2.SuspendLayout();
            this.splitList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitOutput)).BeginInit();
            this.splitOutput.Panel1.SuspendLayout();
            this.splitOutput.Panel2.SuspendLayout();
            this.splitOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // listFiles
            // 
            this.listFiles.AllowDrop = true;
            this.listFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listFiles.FormattingEnabled = true;
            this.listFiles.IntegralHeight = false;
            this.listFiles.Location = new System.Drawing.Point(0, 0);
            this.listFiles.Name = "listFiles";
            this.listFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listFiles.Size = new System.Drawing.Size(120, 438);
            this.listFiles.TabIndex = 0;
            this.listFiles.SelectedIndexChanged += new System.EventHandler(this.listFiles_SelectedIndexChanged);
            this.listFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.listFiles_DragDrop);
            this.listFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.listFiles_DragEnter);
            this.listFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listFiles_KeyDown);
            // 
            // textCommand
            // 
            this.textCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCommand.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textCommand.Location = new System.Drawing.Point(0, 2);
            this.textCommand.Name = "textCommand";
            this.textCommand.ReadOnly = true;
            this.textCommand.Size = new System.Drawing.Size(479, 20);
            this.textCommand.TabIndex = 1;
            // 
            // numAudioGain
            // 
            this.numAudioGain.DecimalPlaces = 2;
            this.numAudioGain.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numAudioGain.Location = new System.Drawing.Point(0, 184);
            this.numAudioGain.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            65536});
            this.numAudioGain.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147418112});
            this.numAudioGain.Name = "numAudioGain";
            this.numAudioGain.Size = new System.Drawing.Size(56, 20);
            this.numAudioGain.TabIndex = 2;
            this.numAudioGain.ValueChanged += new System.EventHandler(this.numAudioGain_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Audio Gain";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Audio Delay Seconds";
            // 
            // numAudioDelay
            // 
            this.numAudioDelay.DecimalPlaces = 2;
            this.numAudioDelay.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numAudioDelay.Location = new System.Drawing.Point(65, 184);
            this.numAudioDelay.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            65536});
            this.numAudioDelay.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147418112});
            this.numAudioDelay.Name = "numAudioDelay";
            this.numAudioDelay.Size = new System.Drawing.Size(106, 20);
            this.numAudioDelay.TabIndex = 4;
            this.numAudioDelay.ValueChanged += new System.EventHandler(this.numAudioDelay_ValueChanged);
            // 
            // dropEncoder
            // 
            this.dropEncoder.FormattingEnabled = true;
            this.dropEncoder.Items.AddRange(new object[] {
            "NONE",
            "av1_nvenc",
            "hevc_nvenc",
            "h264_nvenc",
            "libx264",
            "libx265"});
            this.dropEncoder.Location = new System.Drawing.Point(0, 79);
            this.dropEncoder.Name = "dropEncoder";
            this.dropEncoder.Size = new System.Drawing.Size(121, 21);
            this.dropEncoder.TabIndex = 6;
            this.dropEncoder.TextChanged += new System.EventHandler(this.dropEncoder_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Encoder";
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnSpeedInfo);
            this.panelControls.Controls.Add(this.lblSpeed);
            this.panelControls.Controls.Add(this.trkSpeed);
            this.panelControls.Controls.Add(this.btnCRFinfo);
            this.panelControls.Controls.Add(this.label13);
            this.panelControls.Controls.Add(this.cmbFPS);
            this.panelControls.Controls.Add(this.label12);
            this.panelControls.Controls.Add(this.txtAdditionalOptions);
            this.panelControls.Controls.Add(this.label10);
            this.panelControls.Controls.Add(this.label11);
            this.panelControls.Controls.Add(this.label7);
            this.panelControls.Controls.Add(this.cmbPreset);
            this.panelControls.Controls.Add(this.lblCRF);
            this.panelControls.Controls.Add(this.trkCRF);
            this.panelControls.Controls.Add(this.chkHvc1);
            this.panelControls.Controls.Add(this.label8);
            this.panelControls.Controls.Add(this.btnGetEnd);
            this.panelControls.Controls.Add(this.lblOutputFilename);
            this.panelControls.Controls.Add(this.label9);
            this.panelControls.Controls.Add(this.txtOutName);
            this.panelControls.Controls.Add(this.chkOverwrite);
            this.panelControls.Controls.Add(this.btnPaste);
            this.panelControls.Controls.Add(this.btnCopy);
            this.panelControls.Controls.Add(this.lblFilename);
            this.panelControls.Controls.Add(this.txtEnd);
            this.panelControls.Controls.Add(this.label6);
            this.panelControls.Controls.Add(this.txtStart);
            this.panelControls.Controls.Add(this.label5);
            this.panelControls.Controls.Add(this.btnCropPresets);
            this.panelControls.Controls.Add(this.label4);
            this.panelControls.Controls.Add(this.txtCrop);
            this.panelControls.Controls.Add(this.label1);
            this.panelControls.Controls.Add(this.label3);
            this.panelControls.Controls.Add(this.numAudioGain);
            this.panelControls.Controls.Add(this.dropEncoder);
            this.panelControls.Controls.Add(this.numAudioDelay);
            this.panelControls.Controls.Add(this.label2);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControls.Location = new System.Drawing.Point(0, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(496, 438);
            this.panelControls.TabIndex = 8;
            this.panelControls.Visible = false;
            // 
            // btnSpeedInfo
            // 
            this.btnSpeedInfo.Location = new System.Drawing.Point(385, 193);
            this.btnSpeedInfo.Name = "btnSpeedInfo";
            this.btnSpeedInfo.Size = new System.Drawing.Size(35, 23);
            this.btnSpeedInfo.TabIndex = 46;
            this.btnSpeedInfo.Text = "Info";
            this.btnSpeedInfo.UseVisualStyleBackColor = true;
            this.btnSpeedInfo.Click += new System.EventHandler(this.btnSpeedInfo_Click);
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(249, 198);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(130, 13);
            this.lblSpeed.TabIndex = 45;
            this.lblSpeed.Text = "Speed (Video only) | 1.00x";
            // 
            // trkSpeed
            // 
            this.trkSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trkSpeed.LargeChange = 300;
            this.trkSpeed.Location = new System.Drawing.Point(252, 214);
            this.trkSpeed.Maximum = 800;
            this.trkSpeed.Name = "trkSpeed";
            this.trkSpeed.Size = new System.Drawing.Size(232, 45);
            this.trkSpeed.SmallChange = 50;
            this.trkSpeed.TabIndex = 44;
            this.trkSpeed.TickFrequency = 100;
            this.trkSpeed.Value = 100;
            this.trkSpeed.ValueChanged += new System.EventHandler(this.trkSpeed_ValueChanged);
            this.trkSpeed.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trkSpeed_KeyDown);
            this.trkSpeed.KeyUp += new System.Windows.Forms.KeyEventHandler(this.trkSpeed_KeyUp);
            // 
            // btnCRFinfo
            // 
            this.btnCRFinfo.Location = new System.Drawing.Point(298, 19);
            this.btnCRFinfo.Name = "btnCRFinfo";
            this.btnCRFinfo.Size = new System.Drawing.Size(35, 23);
            this.btnCRFinfo.TabIndex = 43;
            this.btnCRFinfo.Text = "Info";
            this.btnCRFinfo.UseVisualStyleBackColor = true;
            this.btnCRFinfo.Click += new System.EventHandler(this.btnCRFinfo_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(249, 148);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(97, 13);
            this.label13.TabIndex = 42;
            this.label13.Text = "Frames per second";
            // 
            // cmbFPS
            // 
            this.cmbFPS.FormattingEnabled = true;
            this.cmbFPS.Items.AddRange(new object[] {
            "Same as source",
            "5",
            "10",
            "12",
            "15",
            "20",
            "23.976",
            "24",
            "25",
            "29.97",
            "30",
            "48",
            "50",
            "59.94",
            "60",
            "72",
            "75",
            "90",
            "100",
            "120"});
            this.cmbFPS.Location = new System.Drawing.Point(252, 164);
            this.cmbFPS.Name = "cmbFPS";
            this.cmbFPS.Size = new System.Drawing.Size(121, 21);
            this.cmbFPS.TabIndex = 41;
            this.cmbFPS.TextChanged += new System.EventHandler(this.cmbFPS_TextChanged);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(-3, 246);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 40;
            this.label12.Text = "Additional Options";
            this.label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtAdditionalOptions
            // 
            this.txtAdditionalOptions.AcceptsTab = true;
            this.txtAdditionalOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAdditionalOptions.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAdditionalOptions.Location = new System.Drawing.Point(0, 262);
            this.txtAdditionalOptions.Multiline = true;
            this.txtAdditionalOptions.Name = "txtAdditionalOptions";
            this.txtAdditionalOptions.Size = new System.Drawing.Size(484, 51);
            this.txtAdditionalOptions.TabIndex = 39;
            this.txtAdditionalOptions.TextChanged += new System.EventHandler(this.txtAdditionalOptions_TextChanged);
            this.txtAdditionalOptions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(250, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 37;
            this.label10.Text = "Placebo";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.Location = new System.Drawing.Point(293, 72);
            this.label11.Name = "label11";
            this.label11.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label11.Size = new System.Drawing.Size(191, 13);
            this.label11.TabIndex = 38;
            this.label11.Text = "Cooked";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(249, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 36;
            this.label7.Text = "Preset";
            // 
            // cmbPreset
            // 
            this.cmbPreset.FormattingEnabled = true;
            this.cmbPreset.Items.AddRange(new object[] {
            "ultrafast",
            "superfast",
            "veryfast",
            "faster",
            "fast",
            "medium",
            "slow",
            "slower",
            "veryslow",
            "placebo"});
            this.cmbPreset.Location = new System.Drawing.Point(252, 119);
            this.cmbPreset.Name = "cmbPreset";
            this.cmbPreset.Size = new System.Drawing.Size(121, 21);
            this.cmbPreset.TabIndex = 35;
            this.cmbPreset.TextChanged += new System.EventHandler(this.cmbPreset_TextChanged);
            // 
            // lblCRF
            // 
            this.lblCRF.AutoSize = true;
            this.lblCRF.Location = new System.Drawing.Point(249, 24);
            this.lblCRF.Name = "lblCRF";
            this.lblCRF.Size = new System.Drawing.Size(46, 13);
            this.lblCRF.TabIndex = 34;
            this.lblCRF.Text = "CRF: 22";
            // 
            // trkCRF
            // 
            this.trkCRF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trkCRF.Location = new System.Drawing.Point(252, 40);
            this.trkCRF.Maximum = 51;
            this.trkCRF.Name = "trkCRF";
            this.trkCRF.Size = new System.Drawing.Size(232, 45);
            this.trkCRF.TabIndex = 33;
            this.trkCRF.TickFrequency = 3;
            this.trkCRF.Value = 22;
            this.trkCRF.Scroll += new System.EventHandler(this.trkCRF_Scroll);
            // 
            // chkHvc1
            // 
            this.chkHvc1.AutoSize = true;
            this.chkHvc1.Location = new System.Drawing.Point(0, 103);
            this.chkHvc1.Name = "chkHvc1";
            this.chkHvc1.Size = new System.Drawing.Size(181, 17);
            this.chkHvc1.TabIndex = 32;
            this.chkHvc1.Text = "hvc1 Tag (Apple device support)";
            this.chkHvc1.UseVisualStyleBackColor = true;
            this.chkHvc1.Visible = false;
            this.chkHvc1.CheckedChanged += new System.EventHandler(this.chkHvc1_CheckedChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(-2, 392);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(236, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "Copy/Paste File Settings (pasting works on mulit)";
            // 
            // btnGetEnd
            // 
            this.btnGetEnd.Location = new System.Drawing.Point(164, 39);
            this.btnGetEnd.Name = "btnGetEnd";
            this.btnGetEnd.Size = new System.Drawing.Size(65, 23);
            this.btnGetEnd.TabIndex = 26;
            this.btnGetEnd.Text = "Get End";
            this.btnGetEnd.UseVisualStyleBackColor = true;
            this.btnGetEnd.Click += new System.EventHandler(this.btnGetEnd_Click);
            // 
            // lblOutputFilename
            // 
            this.lblOutputFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOutputFilename.AutoSize = true;
            this.lblOutputFilename.Location = new System.Drawing.Point(-2, 362);
            this.lblOutputFilename.Name = "lblOutputFilename";
            this.lblOutputFilename.Size = new System.Drawing.Size(69, 13);
            this.lblOutputFilename.TabIndex = 25;
            this.lblOutputFilename.Text = "Out Filename";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(-3, 323);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(123, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Output name | {filename}";
            // 
            // txtOutName
            // 
            this.txtOutName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutName.Location = new System.Drawing.Point(0, 339);
            this.txtOutName.Name = "txtOutName";
            this.txtOutName.Size = new System.Drawing.Size(484, 20);
            this.txtOutName.TabIndex = 23;
            this.txtOutName.TextChanged += new System.EventHandler(this.txtOutName_TextChanged);
            // 
            // chkOverwrite
            // 
            this.chkOverwrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkOverwrite.AutoSize = true;
            this.chkOverwrite.Location = new System.Drawing.Point(121, 322);
            this.chkOverwrite.Name = "chkOverwrite";
            this.chkOverwrite.Size = new System.Drawing.Size(108, 17);
            this.chkOverwrite.TabIndex = 18;
            this.chkOverwrite.Text = "Overwrite if exists";
            this.chkOverwrite.UseVisualStyleBackColor = true;
            this.chkOverwrite.CheckedChanged += new System.EventHandler(this.chkOverwrite_CheckedChanged);
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaste.Location = new System.Drawing.Point(58, 406);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(52, 23);
            this.btnPaste.TabIndex = 17;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(0, 406);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(52, 23);
            this.btnCopy.TabIndex = 16;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(-3, 0);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(49, 13);
            this.lblFilename.TabIndex = 15;
            this.lblFilename.Text = "Filename";
            // 
            // txtEnd
            // 
            this.txtEnd.Location = new System.Drawing.Point(85, 40);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(79, 20);
            this.txtEnd.TabIndex = 14;
            this.txtEnd.TextChanged += new System.EventHandler(this.txtEnd_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(82, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "End at";
            // 
            // txtStart
            // 
            this.txtStart.Location = new System.Drawing.Point(0, 40);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(79, 20);
            this.txtStart.TabIndex = 12;
            this.txtStart.TextChanged += new System.EventHandler(this.txtStart_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-3, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Start from";
            // 
            // btnCropPresets
            // 
            this.btnCropPresets.Location = new System.Drawing.Point(127, 138);
            this.btnCropPresets.Name = "btnCropPresets";
            this.btnCropPresets.Size = new System.Drawing.Size(52, 23);
            this.btnCropPresets.TabIndex = 10;
            this.btnCropPresets.Text = "Presets";
            this.btnCropPresets.UseVisualStyleBackColor = true;
            this.btnCropPresets.Click += new System.EventHandler(this.btnCropPresets_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-3, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Crop";
            // 
            // txtCrop
            // 
            this.txtCrop.Location = new System.Drawing.Point(0, 139);
            this.txtCrop.Name = "txtCrop";
            this.txtCrop.Size = new System.Drawing.Size(128, 20);
            this.txtCrop.TabIndex = 8;
            this.txtCrop.TextChanged += new System.EventHandler(this.txtCrop_TextChanged);
            // 
            // menuCropPresets
            // 
            this.menuCropPresets.Name = "menuCropPresets";
            this.menuCropPresets.Size = new System.Drawing.Size(61, 4);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(543, 1);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(77, 23);
            this.btnRun.TabIndex = 18;
            this.btnRun.Text = "Run Queue";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(0, 28);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(620, 86);
            this.txtOutput.TabIndex = 19;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(0, 114);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(620, 10);
            this.progressBar1.TabIndex = 20;
            // 
            // btnCopyCommand
            // 
            this.btnCopyCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyCommand.Location = new System.Drawing.Point(485, 1);
            this.btnCopyCommand.Name = "btnCopyCommand";
            this.btnCopyCommand.Size = new System.Drawing.Size(52, 23);
            this.btnCopyCommand.TabIndex = 27;
            this.btnCopyCommand.Text = "Copy";
            this.btnCopyCommand.UseVisualStyleBackColor = true;
            this.btnCopyCommand.Click += new System.EventHandler(this.btnCopyCommand_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(620, 24);
            this.menuStrip1.TabIndex = 28;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptionsEncoder,
            this.menuOptionsHvc1,
            this.toolStripSeparator1,
            this.defaultCRFCQPToolStripMenuItem,
            this.toolStripSeparator2,
            this.defaultPresetCPUToolStripMenuItem,
            this.defaultPresetGPUToolStripMenuItem,
            this.toolStripSeparator3,
            this.menuOptionsCopyOverwrite,
            this.toolStripSeparator4,
            this.additionalOptionsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // menuOptionsEncoder
            // 
            this.menuOptionsEncoder.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptionsEncoderDrop});
            this.menuOptionsEncoder.Name = "menuOptionsEncoder";
            this.menuOptionsEncoder.Size = new System.Drawing.Size(196, 22);
            this.menuOptionsEncoder.Text = "Default Encoder";
            // 
            // menuOptionsEncoderDrop
            // 
            this.menuOptionsEncoderDrop.Name = "menuOptionsEncoderDrop";
            this.menuOptionsEncoderDrop.Size = new System.Drawing.Size(121, 23);
            this.menuOptionsEncoderDrop.TextChanged += new System.EventHandler(this.menuOptionsEncoderDrop_TextChanged);
            // 
            // menuOptionsHvc1
            // 
            this.menuOptionsHvc1.Name = "menuOptionsHvc1";
            this.menuOptionsHvc1.Size = new System.Drawing.Size(196, 22);
            this.menuOptionsHvc1.Text = "Always Add \'hvc1\' Tag";
            this.menuOptionsHvc1.Click += new System.EventHandler(this.menuOptionsHvc1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // defaultCRFCQPToolStripMenuItem
            // 
            this.defaultCRFCQPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptionsCRF});
            this.defaultCRFCQPToolStripMenuItem.Name = "defaultCRFCQPToolStripMenuItem";
            this.defaultCRFCQPToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.defaultCRFCQPToolStripMenuItem.Text = "Default CRF / CQP";
            // 
            // menuOptionsCRF
            // 
            this.menuOptionsCRF.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuOptionsCRF.Name = "menuOptionsCRF";
            this.menuOptionsCRF.Size = new System.Drawing.Size(100, 23);
            this.menuOptionsCRF.TextChanged += new System.EventHandler(this.menuOptionsCRF_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(193, 6);
            // 
            // defaultPresetCPUToolStripMenuItem
            // 
            this.defaultPresetCPUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptionsPresetCPU});
            this.defaultPresetCPUToolStripMenuItem.Name = "defaultPresetCPUToolStripMenuItem";
            this.defaultPresetCPUToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.defaultPresetCPUToolStripMenuItem.Text = "Default Preset (CPU)";
            // 
            // menuOptionsPresetCPU
            // 
            this.menuOptionsPresetCPU.Items.AddRange(new object[] {
            "ultrafast",
            "superfast",
            "veryfast",
            "faster",
            "fast",
            "medium",
            "slow",
            "slower",
            "veryslow",
            "placebo"});
            this.menuOptionsPresetCPU.Name = "menuOptionsPresetCPU";
            this.menuOptionsPresetCPU.Size = new System.Drawing.Size(121, 23);
            this.menuOptionsPresetCPU.TextChanged += new System.EventHandler(this.menuOptionsPresetCPU_TextChanged);
            // 
            // defaultPresetGPUToolStripMenuItem
            // 
            this.defaultPresetGPUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptionsPresetGPU});
            this.defaultPresetGPUToolStripMenuItem.Name = "defaultPresetGPUToolStripMenuItem";
            this.defaultPresetGPUToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.defaultPresetGPUToolStripMenuItem.Text = "Default Preset (GPU)";
            // 
            // menuOptionsPresetGPU
            // 
            this.menuOptionsPresetGPU.Items.AddRange(new object[] {
            "p1",
            "p2",
            "p3",
            "p4",
            "p5",
            "p6",
            "p7"});
            this.menuOptionsPresetGPU.Name = "menuOptionsPresetGPU";
            this.menuOptionsPresetGPU.Size = new System.Drawing.Size(121, 23);
            this.menuOptionsPresetGPU.TextChanged += new System.EventHandler(this.menuOptionsPresetGPU_TextChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
            // 
            // menuOptionsCopyOverwrite
            // 
            this.menuOptionsCopyOverwrite.Name = "menuOptionsCopyOverwrite";
            this.menuOptionsCopyOverwrite.Size = new System.Drawing.Size(196, 22);
            this.menuOptionsCopyOverwrite.Text = "Copy Overwrite Setting";
            this.menuOptionsCopyOverwrite.Click += new System.EventHandler(this.menuOptionsCopyOverwrite_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(193, 6);
            // 
            // additionalOptionsToolStripMenuItem
            // 
            this.additionalOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOptionsAdditionalOptions});
            this.additionalOptionsToolStripMenuItem.Name = "additionalOptionsToolStripMenuItem";
            this.additionalOptionsToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.additionalOptionsToolStripMenuItem.Text = "Additional Options";
            // 
            // menuOptionsAdditionalOptions
            // 
            this.menuOptionsAdditionalOptions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuOptionsAdditionalOptions.Name = "menuOptionsAdditionalOptions";
            this.menuOptionsAdditionalOptions.Size = new System.Drawing.Size(100, 23);
            this.menuOptionsAdditionalOptions.TextChanged += new System.EventHandler(this.menuOptionsAdditionalOptions_TextChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(107, 22);
            this.menuHelpAbout.Text = "About";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // splitList
            // 
            this.splitList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitList.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitList.Location = new System.Drawing.Point(0, 0);
            this.splitList.Name = "splitList";
            // 
            // splitList.Panel1
            // 
            this.splitList.Panel1.Controls.Add(this.listFiles);
            // 
            // splitList.Panel2
            // 
            this.splitList.Panel2.Controls.Add(this.panelControls);
            this.splitList.Size = new System.Drawing.Size(620, 438);
            this.splitList.SplitterDistance = 120;
            this.splitList.TabIndex = 29;
            // 
            // splitOutput
            // 
            this.splitOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitOutput.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitOutput.Location = new System.Drawing.Point(0, 24);
            this.splitOutput.Name = "splitOutput";
            this.splitOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitOutput.Panel1
            // 
            this.splitOutput.Panel1.Controls.Add(this.splitList);
            // 
            // splitOutput.Panel2
            // 
            this.splitOutput.Panel2.Controls.Add(this.textCommand);
            this.splitOutput.Panel2.Controls.Add(this.btnRun);
            this.splitOutput.Panel2.Controls.Add(this.btnCopyCommand);
            this.splitOutput.Panel2.Controls.Add(this.txtOutput);
            this.splitOutput.Panel2.Controls.Add(this.progressBar1);
            this.splitOutput.Size = new System.Drawing.Size(620, 566);
            this.splitOutput.SplitterDistance = 438;
            this.splitOutput.TabIndex = 30;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 590);
            this.Controls.Add(this.splitOutput);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "mpegui";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numAudioGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioDelay)).EndInit();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkCRF)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitList.Panel1.ResumeLayout(false);
            this.splitList.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitList)).EndInit();
            this.splitList.ResumeLayout(false);
            this.splitOutput.Panel1.ResumeLayout(false);
            this.splitOutput.Panel2.ResumeLayout(false);
            this.splitOutput.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitOutput)).EndInit();
            this.splitOutput.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listFiles;
        private System.Windows.Forms.TextBox textCommand;
        private System.Windows.Forms.NumericUpDown numAudioGain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numAudioDelay;
        private System.Windows.Forms.ComboBox dropEncoder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.TextBox txtEnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCropPresets;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCrop;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ContextMenuStrip menuCropPresets;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtOutName;
        private System.Windows.Forms.CheckBox chkOverwrite;
        private System.Windows.Forms.Label lblOutputFilename;
        private System.Windows.Forms.Button btnGetEnd;
        private System.Windows.Forms.Button btnCopyCommand;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.CheckBox chkHvc1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuOptionsEncoder;
        private System.Windows.Forms.ToolStripMenuItem menuOptionsHvc1;
        private System.Windows.Forms.ToolStripComboBox menuOptionsEncoderDrop;
        private System.Windows.Forms.Label lblCRF;
        private System.Windows.Forms.TrackBar trkCRF;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbPreset;
        private System.Windows.Forms.ToolStripMenuItem defaultCRFCQPToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox menuOptionsCRF;
        private System.Windows.Forms.ToolStripMenuItem defaultPresetCPUToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox menuOptionsPresetCPU;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem defaultPresetGPUToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox menuOptionsPresetGPU;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuOptionsCopyOverwrite;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem additionalOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox menuOptionsAdditionalOptions;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAdditionalOptions;
        private System.Windows.Forms.SplitContainer splitList;
        private System.Windows.Forms.SplitContainer splitOutput;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbFPS;
        private System.Windows.Forms.Button btnCRFinfo;
        private System.Windows.Forms.Button btnSpeedInfo;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TrackBar trkSpeed;
    }
}

