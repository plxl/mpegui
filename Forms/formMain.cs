using mpegui.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.IO;
using Microsoft.VisualBasic;

namespace mpegui
{
    public partial class formMain : Form
    {
        private readonly List<FileConversionInfo> queue = new List<FileConversionInfo>();
        private readonly Dictionary<String, String> cropPresets = new Dictionary<String, String>()
        {
            { "1:1 Square", "'min(iw\\,ih)':'min(iw\\,ih)':(iw-min(iw\\,ih))/2:(ih-min(iw\\,ih))/2" }, // dynamic for any resolution for a centered square crop
            { "16:9 to 9:16 Vertical", "ih*9/16:ih:(iw-ih*9/16)/2:0" }, // only works with a 16:9 video (i use this for making beat saber reels)
        };
        private readonly List<string> CPUEncoderPresets = new List<string>()
        {
            "ultrafast",
            "superfast",
            "veryfast",
            "faster",
            "fast",
            "medium",
            "slow",
            "slower",
            "veryslow",
            "placebo"
        };
        private readonly List<string> GPUEncoderPresets = new List<string>()
        {
            "p1",
            "p2",
            "p3",
            "p4",
            "p5",
            "p6",
            "p7"
        };
        private FileConversionInfo copiedEdits = null;


        public static BalloonToolTip toolTipInfo2;
        public formMain()
        {
            InitializeComponent();

            toolTipInfo2 = new BalloonToolTip(this);
        }

        void AddFiles(string[] files)
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    // Initialise the file as a FileConversionInfo class
                    FileConversionInfo f = new FileConversionInfo(file);
                    // set default encoder according to settings
                    f.Encoder = Settings.Default.DefaultEncoder;
                    // If the default encoder is libx265 or hevc_nvenc, and the default settings want to support Apple devices, add the hvc1 tag
                    if (requiresHvc1(f.Encoder) && Settings.Default.AlwaysHvc1)
                        f.Tags = "hvc1";
                    f.CRF = Settings.Default.DefaultCRF;
                    f.Preset = FileConversionInfo.isCPUEncoder(f.Encoder) ? Settings.Default.DefaultPresetCPU : Settings.Default.DefaultPresetGPU;
                    f.AdditionalOptions = Settings.Default.AdditionalOptions;
                    f.OutputName = Settings.Default.DefaultOutput;
                    queue.Add(f);
                    listFiles.Items.Add(Path.GetFileName(file));
                    string presetName = Settings.Default.DefaultPreset;
                    if (!CheckPresets(presetName))
                    {
                        Preset preset = GetPreset(presetName);
                        if (preset != null) f.SetPreset(preset);
                    }
                }
            }
            listFiles.ClearSelected();
            // this will just select -1 if no files were added, which is fine
            listFiles.SelectedIndex = listFiles.Items.Count - 1;
        }

        private void listFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    AddFiles(files);
                }
            }
        }

        void RemoveSelected()
        {
            if (listFiles.SelectedIndex != -1)
            {
                queue.RemoveAt(listFiles.SelectedIndex);
                listFiles.Items.RemoveAt(listFiles.SelectedIndex);
            }
        }

        private void listFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) RemoveSelected();
        }

        private void listFiles_MouseDown(object sender, MouseEventArgs e)
        {
            // this breaks multi-selection so i only want it to work if:
            // - the user is left-clicking
            // - and is selecting -1
            // OR:
            // - the user is right-clicking
            // - 1 or less items are currently selected
            // - the user is not selecting -1
            int index = listFiles.IndexFromPoint(e.Location);
            bool leftClickOOB = e.Button == MouseButtons.Left && index == -1;
            bool rightClickOneItem = e.Button == MouseButtons.Right && index > -1 && listFiles.SelectedItems.Count <= 1;
            if (leftClickOOB || rightClickOneItem)
            {
                // must clear selected with multi-select listbox, otherwise
                // right-click will only /add/ to selected indicies instead of reselect
                listFiles.ClearSelected();
                listFiles.SelectedIndex = index;
            }
        }

        void UpdateUI()
        {
            if (listFiles.SelectedIndex != -1)
            {
                FileConversionInfo f = queue[listFiles.SelectedIndex];
                UpdateCommand();

                panelControls.Tag = 1;

                if (listFiles.SelectedIndices.Count > 1)
                    lblFilename.Text = "Multiple Selected, showing the first selected settings, edits affect ALL";
                else lblFilename.Text = f.Filename;
                lblOutputFilename.Text = f.GetOutput(true);

                txtOutName.Text = f.OutputName;

                // Important: update Audio Gain Mode before setting audio gain
                // to avoid setting value below minimum
                cmbAudioMode.SelectedIndex = f.AudioUseDb ? 1 : 0;
                numAudioGain.Value = f.AudioGain;
                numAudioDelay.Value = f.AudioDelaySeconds;
                txtCrop.Text = f.CropFilter;
                txtStart.Text = f.TrimStart.ToString(@"hh\:mm\:ss\.fff");
                txtEnd.Text = f.TrimEnd.ToString(@"hh\:mm\:ss\.fff");
                if (f.TrimUseDuration) optDuration.Checked = true;
                else optEndAt.Checked = true;
                UpdateEncoderPresets(f.Encoder);
                dropEncoder.Text = f.Encoder;
                chkHvc1.Visible = requiresHvc1(f.Encoder);
                chkHvc1.Checked = f.Tags == "hvc1";
                cmbPreset.Text = f.Preset;
                trkCRF.Value = f.CRF;
                txtAdditionalOptions.Text = f.AdditionalOptions;
                UpdateCRFLabel();
                UpdateFPS(f.FPS);
                trkSpeed.Value = (int)(f.Speed * 100.0);
                trkSpeed_UpdateLabel();
                chkOverwrite.Checked = f.OverwriteExisting;

                panelControls.Tag = 0;

                panelControls.Visible = true;
            }
            else
            {
                panelControls.Visible = false;
                textCommand.Text = string.Empty;
            }
        }

        private void listFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public bool IsUpdating()
        {
            return (int)panelControls.Tag == 1;
        }

        public void UpdateCommand()
        {
            FileConversionInfo f = queue[listFiles.SelectedIndex];
            textCommand.Text = f.ToStringNameless();
        }

        private void numAudioGain_ValueChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.AudioGain = numAudioGain.Value;
            }

            UpdateCommand();
        }

        private void numAudioDelay_ValueChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.AudioDelaySeconds = numAudioDelay.Value;
            }

            UpdateCommand();
        }

        private void txtCrop_TextChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.CropFilter = txtCrop.Text;
            }

            UpdateCommand();
        }

        private void txtStart_TextChanged(object sender, EventArgs e)
        {
            if (TimeSpan.TryParse(txtStart.Text, out TimeSpan trimStart))
            {
                txtStart.ForeColor = Color.Black;
                if (IsUpdating()) return;

                foreach (int i in listFiles.SelectedIndices)
                {
                    FileConversionInfo f = queue[i];
                    f.TrimStart = trimStart;
                }
            }
            else txtStart.ForeColor = Color.Red;

            UpdateCommand();
        }

        private void txtEnd_TextChanged(object sender, EventArgs e)
        {
            if (TimeSpan.TryParse(txtEnd.Text, out TimeSpan trimEnd))
            {
                txtEnd.ForeColor = Color.Black;
                if (IsUpdating()) return;

                foreach (int i in listFiles.SelectedIndices)
                {
                    FileConversionInfo f = queue[i];
                    f.TrimEnd = trimEnd;
                }
            }
            else txtEnd.ForeColor = Color.Red;

            UpdateCommand();
        }

        private bool requiresHvc1(string encoder)
        {
            return new[] {
                "libx265",
                "hevc_nvenc"
            }
                .Contains(encoder.ToLower());
        }

        private void dropEncoder_TextChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            FileConversionInfo current = queue[listFiles.SelectedIndex];
            UpdateEncoderPresets(dropEncoder.Text);

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.Preset = ConvertEncoderPreset(f.Preset, f.Encoder, dropEncoder.Text);
                f.Encoder = dropEncoder.Text;
            }
            // note that this can change after the above loop
            cmbPreset.Text = current.Preset;

            // Show option to add the hvc1 tag for apple devices if using HEVC encoding
            bool isHevc = requiresHvc1(dropEncoder.Text);

            // display checkbox to add hvc1 tag if required for the encoder, or no encoder is selected
            // the checkbox may be required if you already have a x265 video and just want to add the tag
            chkHvc1.Visible = isHevc || dropEncoder.Text.ToLower() == "none" || string.IsNullOrWhiteSpace(dropEncoder.Text);
            // Uncheck hvc1 tag if HEVC encoding isn't selected
            if (!isHevc) chkHvc1.Checked = false;
            // recheck hvc1 tag if settings always set it, but ONLY for hevc and libx265, NOT for "NONE" encoder (intentional)
            else if (Settings.Default.AlwaysHvc1) chkHvc1.Checked = true;

            UpdateCRFLabel();


            UpdateCommand();
        }

        string ConvertEncoderPreset(string Preset, string OldEncoder, string NewEncoder)
        {
            bool OldIsCPU = FileConversionInfo.isCPUEncoder(OldEncoder);
            bool NewIsCPU = FileConversionInfo.isCPUEncoder(NewEncoder);
            if (OldIsCPU == NewIsCPU) return Preset;

            if (NewIsCPU && !OldIsCPU) // from GPU to CPU
            {
                return Settings.Default.DefaultPresetCPU;
            }
            else // from CPU to GPU
            {
                return Settings.Default.DefaultPresetGPU;
            }
        }

        void UpdateEncoderPresets(string NewEncoder)
        {
            bool OldIsCPU = (string)cmbPreset.Items[0] == CPUEncoderPresets[0];
            bool NewIsCPU = FileConversionInfo.isCPUEncoder(NewEncoder);
            if (OldIsCPU == NewIsCPU) return;

            cmbPreset.Items.Clear();
            if (NewIsCPU && !OldIsCPU) // from GPU to CPU
            {
                cmbPreset.Items.AddRange(CPUEncoderPresets.ToArray());
            }
            else // from CPU to GPU
            {
                cmbPreset.Items.AddRange(GPUEncoderPresets.ToArray());
            }
        }

        void CopySelectedEdits()
        {
            if (listFiles.SelectedIndices.Count != 1)
            {
                MessageBox.Show("You must select only one file settings to copy.", "Copy Settings");
                return;
            }
            copiedEdits = queue[listFiles.SelectedIndex].Clone();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            CopySelectedEdits();
        }

        private void pasteEdits()
        {
            // first get the things the user wants to paste
            using (var dialog = new formPasteEdits())
            {
                dialog.copiedEdits = copiedEdits;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var edits = dialog.editsToPaste;
                    FileConversionInfo c = copiedEdits;
                    /*
                    0 - trim
                    1 - encoder
                    2 - tags
                    3 - crop filter
                    4 - audio gain
                    5 - audio delay
                    6 - crf
                    7 - encoder preset
                    8 - fps
                    9 - video speed
                    10 - additional options
                    11 - output name
                    12 - overwrite (-y)
                    */
                    foreach (int i in listFiles.SelectedIndices)
                    {
                        FileConversionInfo f = queue[i];
                        if (edits[0])
                        {
                            f.TrimStart = c.TrimStart;
                            f.TrimEnd = c.TrimEnd;
                            f.TrimUseDuration = c.TrimUseDuration;
                        }
                        if (edits[1])
                        {
                            // check if it won't be pasting the preset
                            if (!edits[7])
                            {
                                // verify preset eligibility
                                bool oldIsCPU = FileConversionInfo.isCPUEncoder(f.Encoder);
                                bool newIsCPU = FileConversionInfo.isCPUEncoder(c.Encoder);
                                if (oldIsCPU != newIsCPU)
                                {
                                    // the new encoder is not the same type as old encoder
                                    // so we need to assume a matching preset for it since we aren't pasting presets
                                    // we'll go with the user's default preset for that type
                                    f.Preset = newIsCPU ? Settings.Default.DefaultPresetCPU : Settings.Default.DefaultPresetGPU;
                                }
                            }
                            // now we can update the encoder
                            f.Encoder = c.Encoder;
                        }
                        if (edits[2])  f.Tags = c.Tags;
                        if (edits[3])  f.CropFilter = c.CropFilter;
                        if (edits[4])
                        {
                            f.AudioGain = c.AudioGain;
                            f.AudioUseDb = c.AudioUseDb;
                        }
                        if (edits[5])  f.AudioDelaySeconds = c.AudioDelaySeconds;
                        if (edits[6])  f.CRF = c.CRF;
                        if (edits[7])
                        {
                            // check if it won't be pasting the encoder
                            if (!edits[1])
                            {
                                // verify preset eligibility
                                bool oldIsCPU = FileConversionInfo.isCPUEncoder(f.Encoder);
                                bool newIsCPU = FileConversionInfo.isCPUEncoder(c.Encoder);
                                if (oldIsCPU != newIsCPU)
                                {
                                    // the current encoder is not the same type as old encoder
                                    // since the user wants to use this new preset, we have to assume a
                                    // valid new encoder
                                    // if the user's default encoder is valid, use that, otherwise...
                                    // default to hevc_nvenc / libx265
                                    bool defaultIsCPU = FileConversionInfo.isCPUEncoder(Settings.Default.DefaultEncoder);
                                    if (defaultIsCPU == newIsCPU) f.Encoder = Settings.Default.DefaultEncoder;
                                    else if (newIsCPU) f.Encoder = "libx265";
                                    else f.Encoder = "hevc_nvenc";
                                }
                            }
                            // now we can update preset
                            f.Preset = c.Preset;
                        }
                        if (edits[8])  f.FPS = c.FPS;
                        if (edits[9])  f.Speed = c.Speed;
                        if (edits[10]) f.AdditionalOptions = c.AdditionalOptions;
                        if (edits[11]) f.OutputName = c.OutputName;
                        if (edits[12]) f.OverwriteExisting = c.OverwriteExisting;
                    }

                    UpdateUI();
                }
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (copiedEdits != null) pasteEdits();
        }

        private void btnCropPresets_Click(object sender, EventArgs e)
        {
            menuCropPresets.Show(btnCropPresets, new Point(0, btnCropPresets.Height));
        }

        bool isRunning = false;
        async void RunQueue(List<FileConversionInfo> files)
        {
            btnRun.Enabled = false;
            menuFileRun.Enabled = false;
            menuFileRunSelected.Enabled = false;
            if (!isRunning)
            {
                isRunning = true;
                btnRun.Text = "Stop";
                btnRun.Enabled = true;
                ProcessQueue(files);
            }
            else
            {
                while (process != null && !process.HasExited)
                {
                    process.Kill();
                    await Task.Delay(100);
                }
                btnRun.Enabled = true;
                menuFileRun.Enabled = true;
                menuFileRunSelected.Enabled = true;
                btnRun.Text = "Run Queue";
                isRunning = false;
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            RunQueue(queue);
        }

        Process process = null;

        private async void ProcessQueue(List<FileConversionInfo> files)
        {
            foreach (FileConversionInfo f in files)
            {
                string overwrite = f.outputFileAlreadyExists();
                if (overwrite == null) continue;

                process = new Process();
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = f.ToString() + overwrite;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                txtOutput.AppendText("Running: " + "ffmpeg " + f.ToString() + "\r\n");

                process.OutputDataReceived += (s, e2) =>
                {
                    if (!string.IsNullOrEmpty(e2.Data))
                    {
                        txtOutput.Invoke(new Action(() => txtOutput.AppendText(e2.Data + "\r\n")));
                        ParseFFmpegOutput(e2.Data, 0);
                    }
                };

                process.ErrorDataReceived += (s, e2) =>
                {
                    if (!string.IsNullOrEmpty(e2.Data))
                    {
                        txtOutput.Invoke(new Action(() => txtOutput.AppendText(e2.Data + "\r\n")));
                        ParseFFmpegOutput(e2.Data, 0);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await Task.Run(() => process.WaitForExit());

                ffmpegDuration = 0;
                progressBar1.Value = 0;
                txtOutput.AppendText("Process exited with code: " + process.ExitCode + "\r\n");

                if (f.TempFilename != null)
                {
                    File.Delete(f.TempFilename);
                    Console.WriteLine($"Deleted temp file: {f.TempFilename}");
                    f.TempFilename = null;
                }
            }

            Invoke(new Action(() =>
            {
                btnRun.Text = "Run Queue";
                menuFileRun.Enabled = true;
                menuFileRunSelected.Enabled = true;
            }));

            isRunning = false;
        }

        private double ffmpegDuration = 0;

        private void ParseFFmpegOutput(string output, int totalFrames)
        {
            // frame=  250 fps= 24 q=  28.0 size=   102400kB time=00:00:10.50 bitrate=  80.0kbits/s
            // get time and frame information
            if (ffmpegDuration == 0)
            {
                var durationRegex = new Regex(@"Duration: (\d+):(\d+):(\d+).(\d+)");
                var durationMatch = durationRegex.Match(output);
                if (durationMatch.Success)
                {
                    int hours = int.Parse(durationMatch.Groups[1].Value);
                    int minutes = int.Parse(durationMatch.Groups[2].Value);
                    int seconds = int.Parse(durationMatch.Groups[3].Value);
                    int milliseconds = int.Parse(durationMatch.Groups[4].Value);
                    ffmpegDuration = hours * 3600 + minutes * 60 + seconds + (milliseconds / 100.0);
                }
            }

            var currentRegex = new Regex(@"time=(\d+):(\d+):(\d+).(\d+)");
            var currentMatch = currentRegex.Match(output);
            if (currentMatch.Success)
            {

                int hours = int.Parse(currentMatch.Groups[1].Value);
                int minutes = int.Parse(currentMatch.Groups[2].Value);
                int seconds = int.Parse(currentMatch.Groups[3].Value);
                int milliseconds = int.Parse(currentMatch.Groups[4].Value);
                var curSeconds = hours * 3600 + minutes * 60 + seconds + (milliseconds / 100.0);

                if (ffmpegDuration > 0)
                    UpdateProgressBar(curSeconds, ffmpegDuration);
            }
        }

        private TimeSpan GetFfprobeDuration(string output)
        {
            var durationRegex = new Regex(@"Duration: (\d+):(\d+):(\d+).(\d+)");
            var durationMatch = durationRegex.Match(output);
            if (durationMatch.Success)
            {
                int hours = int.Parse(durationMatch.Groups[1].Value);
                int minutes = int.Parse(durationMatch.Groups[2].Value);
                int seconds = int.Parse(durationMatch.Groups[3].Value);
                int milliseconds = int.Parse(durationMatch.Groups[4].Value);
                return new TimeSpan(0, hours, minutes, seconds, milliseconds);
            }

            return TimeSpan.Zero;
        }

        private void UpdateProgressBar(double curTime, double duration)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<double, double>(UpdateProgressBar), curTime, duration);
            }
            else
            {
                progressBar1.Value = Math.Min(100, (int)(curTime / duration * 100));
            }
        }

        private void txtOutName_TextChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.OutputName = txtOutName.Text;
                lblOutputFilename.Text = f.GetOutput(true);
            }

            UpdateCommand();
        }


        private void chkOverwrite_CheckedChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.OverwriteExisting = chkOverwrite.Checked;
            }

            UpdateCommand();
        }

        private async void btnGetEnd_Click(object sender, EventArgs e)
        {
            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];

                var probe = new Process();
                probe.StartInfo.FileName = "ffprobe";
                probe.StartInfo.Arguments = $"\"{f.Filename}\"";
                probe.StartInfo.RedirectStandardOutput = true;
                probe.StartInfo.RedirectStandardError = true;
                probe.StartInfo.UseShellExecute = false;
                probe.StartInfo.CreateNoWindow = true;

                txtOutput.AppendText("Running: " + "ffprobe \"" + f.Filename + "\"\r\n");

                TimeSpan duration = TimeSpan.Zero;
                probe.OutputDataReceived += (s, e2) =>
                {
                    if (!string.IsNullOrEmpty(e2.Data))
                    {
                        txtOutput.Invoke(new Action(() => txtOutput.AppendText(e2.Data + "\r\n")));
                        if (duration == TimeSpan.Zero)
                            duration = GetFfprobeDuration(e2.Data);
                    }
                };

                probe.ErrorDataReceived += (s, e2) =>
                {
                    if (!string.IsNullOrEmpty(e2.Data))
                    {
                        txtOutput.Invoke(new Action(() => txtOutput.AppendText(e2.Data + "\r\n")));
                        if (duration == TimeSpan.Zero)
                            duration = GetFfprobeDuration(e2.Data);
                    }
                };

                probe.Start();
                probe.BeginOutputReadLine();
                probe.BeginErrorReadLine();

                await Task.Run(() => probe.WaitForExit());

                f.TrimEnd = duration;
                if (i == listFiles.SelectedIndex)
                {
                    txtEnd.Text = f.TrimEnd.ToString(@"hh\:mm\:ss\.fff");
                }
            }
        }

        private void btnCopyCommand_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedIndex != -1)
            {
                FileConversionInfo f = queue[listFiles.SelectedIndex];
                Clipboard.SetText("ffmpeg " + f.ToString());
            }
        }

        private void SetTag(string tag)
        {
            if (listFiles.SelectedIndex == -1) return;
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.Tags = tag.Trim();
            }

            UpdateCommand();
        }

        private void SetCrop(string crop)
        {
            if (listFiles.SelectedIndex == -1) return;
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.CropFilter = crop.Trim();
            }

            UpdateCommand();
        }

        private void CropPresets_Load()
        {
            menuCropPresets.Items.Clear();
            foreach (var preset in cropPresets)
            {
                ToolStripMenuItem presetItem = new ToolStripMenuItem
                {
                    Text = preset.Key,
                    Tag = preset.Value
                };
                presetItem.Click += (presetSender, presetE) =>
                {
                    string cropPreset = ((ToolStripMenuItem)presetSender).Tag.ToString();
                    // Cast sender object as menu item, then cast its tag to a string, and set it
                    SetCrop(cropPreset);
                    txtCrop.Text = cropPreset;
                };
                // add preset to the crop presets menu
                menuCropPresets.Items.Add(presetItem);
            }
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            // Initialise crop preset buttons
            CropPresets_Load();
            // Load saved defaults
            Settings_Load();
            // Load presets
            UpdatePresets();

            toolTipInfo2.Create();
        }

        private void Settings_Load()
        {
            // Initialise default encoder dropdown
            menuOptionsEncoderDrop.Items.Clear();
            menuOptionsEncoderDrop.Items.AddRange(dropEncoder.Items.Cast<string>().ToArray());
            // Set the drop item from settings if set
            if (!string.IsNullOrWhiteSpace(Settings.Default.DefaultEncoder))
                menuOptionsEncoderDrop.Text = Settings.Default.DefaultEncoder;

            menuOptionsHvc1.Checked = Settings.Default.AlwaysHvc1;

            // CRF / CQP
            menuOptionsCRF.Text = Settings.Default.DefaultCRF.ToString();

            // Default Encoder Presets
            menuOptionsPresetCPU.Text = Settings.Default.DefaultPresetCPU.ToString();
            menuOptionsPresetGPU.Text = Settings.Default.DefaultPresetGPU.ToString();

            // Additional options
            menuOptionsAdditionalOptions.Text = Settings.Default.AdditionalOptions;

            // Output Name
            menuOptionsDefaultOut.Text = Settings.Default.DefaultOutput;
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                    "mpegui\r\n\r\n" +
                    "A simple ffmpeg GUI that does very basic things that I wanted in very specific scenarios.\r\n\r\n" +
                    "Version " + Application.ProductVersion,

                    "About",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
        }

        private void chkHvc1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHvc1.Checked) SetTag("hvc1");
            else SetTag("");
        }

        private void menuOptionsHvc1_Click(object sender, EventArgs e)
        {
            menuOptionsHvc1.Checked = !menuOptionsHvc1.Checked;
            Settings.Default.AlwaysHvc1 = menuOptionsHvc1.Checked;
            Settings.Default.Save();
        }

        private void menuOptionsEncoderDrop_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.DefaultEncoder = menuOptionsEncoderDrop.Text;
            Settings.Default.Save();
        }

        private void trkCRF_Scroll(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.CRF = trkCRF.Value;
            }
            UpdateCRFLabel();

            UpdateCommand();
        }

        private void UpdateCRFLabel()
        {
            if (FileConversionInfo.isCPUEncoder(queue[listFiles.SelectedIndex].Encoder))
            {
                lblCRF.Text = $"CRF: {trkCRF.Value}";
            }
            else
            {
                lblCRF.Text = $"CQP: {trkCRF.Value}";
            }
        }

        private void cmbPreset_TextChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.Preset = cmbPreset.Text;
            }

            UpdateCommand();
        }

        private void menuOptionsCRF_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(menuOptionsCRF.Text, out int crf))
            {
                Settings.Default.DefaultCRF = crf;
                Settings.Default.Save();
            }
        }

        private void menuOptionsPresetCPU_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.DefaultPresetCPU = menuOptionsPresetCPU.Text.Trim();
            Settings.Default.Save();
        }

        private void menuOptionsPresetGPU_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.DefaultPresetGPU = menuOptionsPresetGPU.Text.Trim();
            Settings.Default.Save();
        }

        private void menuOptionsAdditionalOptions_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.AdditionalOptions = menuOptionsAdditionalOptions.Text.Trim();
            Settings.Default.Save();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Stops the user from being able to put newlines in the additional options multiline textbox
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void txtAdditionalOptions_TextChanged(object sender, EventArgs e)
        {
            // Removes any new lines from the text just in case
            if (txtAdditionalOptions.Text.Contains(Environment.NewLine))
            {
                txtAdditionalOptions.Text = txtAdditionalOptions.Text.Replace(Environment.NewLine, string.Empty);
            }

            if (IsUpdating()) return;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.AdditionalOptions = txtAdditionalOptions.Text.Trim();
            }

            UpdateCommand();
        }

        private void UpdateFPS(decimal fps)
        {
            if (fps < 1.0m)
            {
                cmbFPS.SelectedIndex = 0; // selects "Same as source"
            }
            else
            {
                cmbFPS.Text = fps.ToString();
            }
        }

        private void cmbFPS_TextChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            // verify the input text is actually a valid decimal number
            bool isDecimal = decimal.TryParse(cmbFPS.Text, out decimal fps);
            // check if "same as source" is selected
            if (cmbFPS.SelectedIndex == 0)
            {
                fps = 0.0m;
                isDecimal = true;
            }
            // update fps if input is a decimal / same as source
            if (isDecimal)
            {
                foreach (int i in listFiles.SelectedIndices)
                {
                    FileConversionInfo f = queue[i];
                    f.FPS = fps;
                }
            }
            // otherwise just do nothing
            // in theory we could force it to do "same as source" (or 0.0m) here, but it might be
            // more respectable to the user to leave it as it was originally

            UpdateCommand();
        }

        private void trkSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            // snaps the trackbar to SmallChange value
            var bar = (TrackBar)sender;
            if (bar.Value > 1 && bar.Value % bar.SmallChange != 0)
            {
                bar.Value = bar.SmallChange * ((bar.Value + bar.SmallChange / 2) / bar.SmallChange);
            }

            // makes sure that the value can't be 0, without setting the minimum to 1 (which shifts the visual ticks slightly, yuck)
            if (bar.Value <= 0) bar.Value = 1;

            // update file conversion info
            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.Speed = trkSpeed.Value / 100.0;
            }
            trkSpeed_UpdateLabel();

            UpdateCommand();
        }

        private void trkSpeed_UpdateLabel()
        {
            lblSpeed.Text = $"Speed (Video only) | {(double)trkSpeed.Value / 100.0:0.00}x";
        }

        private void trkSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            trkSpeed_KeyUpdate();
        }

        private void trkSpeed_KeyUp(object sender, KeyEventArgs e)
        {
            trkSpeed_KeyUpdate();
        }

        private void trkSpeed_KeyUpdate()
        {
            bool isAltDown = (Control.ModifierKeys & Keys.Alt) == Keys.Alt;
            bool isShiftDown = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

            if (isAltDown)
                // changes in 0.01 (fine grain)
                trkSpeed.SmallChange = 1;
            else if (isShiftDown)
                // changes in 0.10 (better precision, most would only need this)
                trkSpeed.SmallChange = 10;
            else
                // changes in 0.50 (normal, generally fine)
                trkSpeed.SmallChange = 50;
        }

        void updateTrimMode()
        {
            // make sure it isn't already updating the trim mode/updating the UI from selection
            if (IsUpdating()) return;

            // update file conversion info
            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.TrimUseDuration = optDuration.Checked;
            }

            UpdateCommand();
        }

        private void optDuration_CheckedChanged(object sender, EventArgs e)
        {
            // we only need to run this on one radio button's checked changed event because there will only ever be the 2 options
            // and the checked changed event fires for both when either is selected
            updateTrimMode();
        }

        private void cmbAudioMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // i always want changing this to affect the numerical up/down for Audio Gain
            // that way we never have to do it anywhere else
            bool nowUsingDb = cmbAudioMode.SelectedIndex == 1;
            if (nowUsingDb) numAudioGain.Minimum = -99.9m;
            else numAudioGain.Minimum = 0m;

            // now we can leave if it's currently setting values of things
            if (IsUpdating()) return;

            // update file conversion info
            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                bool prevUsingDb = f.AudioUseDb;
                f.AudioUseDb = nowUsingDb;
                // switch between 1.00 and 0.00 because it's different for multiplier mode / dB mode
                // also multiplier cannot be negative, but dB can
                if (!prevUsingDb && nowUsingDb && f.AudioGain == 1m) f.AudioGain = 0m;
                else if (prevUsingDb && !nowUsingDb && f.AudioGain <= 0m) f.AudioGain = 1m;
            }

            UpdateUI();
        }

        List<string> presets = new List<string>();
        string presetPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Presets");

        string GetPresetFilename(string presetName)
        {
            return Path.Combine(presetPath, presetName + ".json");
        }

        Preset GetPreset(string presetName)
        {
            string filename = GetPresetFilename(presetName);
            string text = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<Preset>(text);
        }

        void LoadPresetToSelected(string presetName)
        {
            // check that any files are selected
            if (listFiles.SelectedItems.Count == 0) return;

            // make sure the preset name exists
            if (CheckPresets(presetName))
            {
                MessageBox.Show("This preset no longer exists. It will be automatically removed.",
                    "Preset Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UpdatePresets();
                return;
            }

            Preset preset = GetPreset(presetName);

            if (preset == null)
            {
                MessageBox.Show("There was an error while trying to load this preset.\n" +
                    "No changes have been made.",
                    "Error Loading Preset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // update file conversion info
            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.SetPreset(preset);
            }

            UpdateUI();

            MessageBox.Show($"\"{presetName}\" has been applied to the selected files.",
                "Preset Applied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        void UpdatePresets()
        {
            // make sure the presets list is accurate
            CheckPresets();

            // clear current load items
            menuPresetLoad.DropDownItems.Clear();
            // clear current save items, excluding save as button
            while (menuPresetSave.DropDownItems.Count > 1)
                menuPresetSave.DropDownItems.RemoveAt(1);
            // clear current default items, excluding reset button
            while (menuPresetDefault.DropDownItems.Count > 1)
                menuPresetDefault.DropDownItems.RemoveAt(1);
            // clear current delete items
            menuPresetDelete.DropDownItems.Clear();
            // clear current right-click cms set preset items, excluding reset button
            while (cmsFilesSet.DropDownItems.Count > 1)
                cmsFilesSet.DropDownItems.RemoveAt(1);
            // add separator to save, default and list CMS set preset items if there are any presets
            if (presets.Count > 0)
            {
                menuPresetSave.DropDownItems.Add(new ToolStripSeparator());
                menuPresetDefault.DropDownItems.Add(new ToolStripSeparator());
                cmsFilesSet.DropDownItems.Add(new ToolStripSeparator());
            }

            // add any other items
            foreach (string preset in presets)
            {
                // add load item
                ToolStripMenuItem btnLoad = new ToolStripMenuItem
                {
                    Text = preset
                };
                btnLoad.Click += (s, e) => { LoadPresetToSelected(preset); };
                menuPresetLoad.DropDownItems.Add(btnLoad);

                // add load item for right-click cms menu
                btnLoad = new ToolStripMenuItem
                {
                    Text = preset
                };
                btnLoad.Click += (s, e) => { LoadPresetToSelected(preset); };
                cmsFilesSet.DropDownItems.Add(btnLoad);

                // add save item
                ToolStripMenuItem btnSave = new ToolStripMenuItem
                {
                    Text = preset
                };
                btnSave.Click += (s, e) =>
                {
                    if (MessageBox.Show(
                        "Are you sure you want to overwrite this preset?",
                        "Save Preset",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SavePreset(preset);
                    }
                };
                menuPresetSave.DropDownItems.Add(btnSave);

                // add default items
                ToolStripMenuItem btnDefault = new ToolStripMenuItem
                {
                    Text = preset,
                    Checked = preset == Settings.Default.DefaultPreset,
                };
                btnDefault.Click += (s, e) =>
                {
                    Settings.Default.DefaultPreset = preset;
                    Settings.Default.Save();
                    foreach (ToolStripMenuItem item in menuPresetDefault.DropDownItems.OfType<ToolStripMenuItem>())
                        item.Checked = false;
                    btnDefault.Checked = true;
                };
                menuPresetDefault.DropDownItems.Add(btnDefault);

                // add default items
                ToolStripMenuItem btnDelete = new ToolStripMenuItem
                {
                    Text = preset
                };
                btnDelete.Click += (s, e) =>
                {
                    if (MessageBox.Show($"Are you sure you want to delete your \"{preset}\" preset?",
                        "Delete Preset",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string filename = GetPresetFilename(preset);
                        if (File.Exists(filename))
                            File.Delete(filename);
                        UpdatePresets();
                    }
                };
                menuPresetDelete.DropDownItems.Add(btnDelete);
            }
        }

        bool CheckPresets(string query = null)
        {
            
            if (!Directory.Exists(presetPath)) Directory.CreateDirectory(presetPath);
            presets.Clear();
            foreach (string file in Directory.GetFiles(presetPath, "*.json"))
                presets.Add(Path.GetFileNameWithoutExtension(file));

            if (!string.IsNullOrWhiteSpace(query))
                if (presets.Contains(query)) return false;
            
            return true;
        }

        string GetCurrentAsJson()
        {
            // can only save the current as a preset if only one item is selected
            if (listFiles.SelectedItems.Count != 1) return null;

            FileConversionInfo f = queue[listFiles.SelectedIndex].Clone();
            f.Filename = string.Empty;
            Preset preset = new Preset(f);

            return JsonSerializer.Serialize(preset, new JsonSerializerOptions() { WriteIndented = true });
        }

        void SavePreset(string name = null, string json = null)
        {
            if (json == null)
            {
                json = GetCurrentAsJson();
                if (json == null)
                {
                    MessageBox.Show(
                        "Select only one file from the list with the settings you want to save in your preset.",
                        "Save Preset");
                    return;
                }
            }

            // no supplied name typically always means the user clicked SaveAs button
            // but might not in the future
            while (name == null)
            {
                string input = Interaction.InputBox("Enter the name of your preset:", "Save Preset Name");

                // return on empty input/cancel pressed
                if (string.IsNullOrWhiteSpace(input)) return;

                // check if user inputted invalid filename characters
                char[] invalidChars = Path.GetInvalidFileNameChars();
                var foundInvalidChars = input.Where(c => invalidChars.Contains(c)).ToArray();
                if (foundInvalidChars.Length > 0)
                {
                    MessageBox.Show(
                        "Name contains the following invalid characters:\n" +
                        string.Join(" ", foundInvalidChars) + "\n\n" +
                        "Please try again.",

                        "Preset Name Invalid",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                // check if user inputted a name that is taken
                else if (!CheckPresets(input))
                {
                    MessageBox.Show(
                        $"There is already a preset with the name \"{input}\".\n\n" +
                        "Please try again.",

                        "Preset Name Taken",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                // else the name is valid
                else name = input;
            }

            string filename = Path.Combine(presetPath, name + ".json");
            File.WriteAllText(filename, json);
            UpdatePresets();
        }

        private void menuPresetSaveAs_Click(object sender, EventArgs e)
        {
            SavePreset();
        }

        private void menuPresetDefaultReset_Click(object sender, EventArgs e)
        {
            Settings.Default.DefaultPreset = null;
            Settings.Default.Save();
            foreach (ToolStripMenuItem item in menuPresetDefault.DropDownItems.OfType<ToolStripMenuItem>())
                item.Checked = false;
        }

        private void menuPresetReload_Click(object sender, EventArgs e)
        {
            UpdatePresets();
        }

        private void cmsFiles_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listFiles.SelectedItems.Count == 0)
            {
                if (listFiles.Items.Count == 0)
                {
                    // don't open the cms if there will be nothing to do in it
                    e.Cancel = true;
                }
                else
                {
                    cmsFilesCopy  .Visible = false;
                    cmsFilesPaste .Visible = false;
                    cmsFilesSet   .Visible = false;
                    cmsFilesSep1  .Visible = false;
                    cmsFilesRemove.Visible = false;
                    cmsFilesSep2  .Visible = false;
                }
            }
            else
            {
                cmsFilesCopy  .Visible = true;
                cmsFilesPaste .Visible = true;
                cmsFilesSet   .Visible = true;
                cmsFilesSep1  .Visible = true;
                cmsFilesRemove.Visible = true;
                cmsFilesSep2  .Visible = true;

                // allow pasting only if there are edits copied
                cmsFilesPaste.Enabled = copiedEdits != null;
            }
        }

        private void cmsFilesCopy_Click(object sender, EventArgs e)
        {
            CopySelectedEdits();
        }

        private void cmsFilesPaste_Click(object sender, EventArgs e)
        {
            if (copiedEdits != null) pasteEdits();
        }

        private void cmsFilesRemove_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void cmsFilesClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the list of files?\n\nThis cannot be undone.",
                "Clear All Files",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                queue.Clear();
                listFiles.Items.Clear();
                UpdateUI();
            }
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                AddFiles(openFileDialog.FileNames);
            }
        }

        private void menuFileRunSelected_Click(object sender, EventArgs e)
        {
            List<FileConversionInfo> files = listFiles.SelectedIndices
                .Cast<int>()
                .Select(i => queue[i])
                .ToList();
            RunQueue(files);
        }

        private void menuFileRun_Click(object sender, EventArgs e)
        {
            RunQueue(queue);
        }

        private void btnBrowseFilename_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtOutName.Text = saveFileDialog.FileName;
            }
        }

        private void menuOptionsDefaultOut_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.DefaultOutput = menuOptionsDefaultOut.Text.Trim();
            Settings.Default.Save();
        }

        private void chkNormaliseAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (IsUpdating()) return;

            // update file conversion info
            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.AudioNormalise = chkNormaliseAudio.Checked;
            }

            UpdateCommand();
        }
    }
}
