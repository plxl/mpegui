using mpegui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mpegui
{
    public partial class Form1 : Form
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
        private int copiedIndex = -1;

        public Form1()
        {
            InitializeComponent();
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
                    foreach (var file in files)
                    {
                        if (System.IO.File.Exists(file))
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
                            queue.Add(f);
                            listFiles.Items.Add(System.IO.Path.GetFileName(file));
                            listFiles.ClearSelected();
                            listFiles.SelectedIndex = listFiles.Items.Count - 1;
                        }
                    }
                }
            }
        }

        private void listFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listFiles.SelectedIndex != -1)
            {
                queue.RemoveAt(listFiles.SelectedIndex);
                listFiles.Items.RemoveAt(listFiles.SelectedIndex);
            }
        }

        private void listFiles_SelectedIndexChanged(object sender, EventArgs e)
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
                if (cmbPreset.Items.Contains(f.Preset)) cmbPreset.SelectedItem = f.Preset;
                else cmbPreset.Text = f.Preset;
                trkCRF.Value = f.CRF;
                txtAdditionalOptions.Text = f.AdditionalOptions;
                UpdateCRFLabel();
                UpdateFPS(f.FPS);
                trkSpeed.Value = (int)(f.Speed * 100.0);
                trkSpeed_UpdateLabel();

                panelControls.Tag = 0;

                panelControls.Visible = true;
            }
            else
            {
                panelControls.Visible = false;
            }
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedIndices.Count != 1)
            {
                MessageBox.Show("You must select only one file settings to copy.", "Copy Settings");
                return;
            }
            copiedIndex = listFiles.SelectedIndex;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (copiedIndex == -1) return;
            FileConversionInfo copy = queue[copiedIndex];

            var result = MessageBox.Show("Do you also want to copy TrimStart and TrimEnd values?",
                "Copy Settings",
                MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel) return;
            bool copyTrims = result == DialogResult.Yes;

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                if (copyTrims)
                {
                    f.TrimStart = copy.TrimStart;
                    f.TrimEnd = copy.TrimEnd;
                    f.TrimUseDuration = copy.TrimUseDuration;
                }
                f.AudioGain = copy.AudioGain;
                f.AudioDelaySeconds = copy.AudioDelaySeconds;
                f.CropFilter = copy.CropFilter;
                f.Encoder = copy.Encoder;
                f.Tags = copy.Tags;
                f.CRF = copy.CRF;
                f.Preset = copy.Preset;
                f.FPS = copy.FPS;
                f.Speed = copy.Speed;
                f.AdditionalOptions = copy.AdditionalOptions;
                if (Settings.Default.CopyOverwrite) f.OverwriteExisting = copy.OverwriteExisting;
            }
        }

        private void btnCropPresets_Click(object sender, EventArgs e)
        {
            menuCropPresets.Show(btnCropPresets, new Point(0, btnCropPresets.Height));
        }

        bool isRunning = false;
        private async void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            if (!isRunning)
            {
                isRunning = true;
                ProcessQueue();
                btnRun.Text = "Stop";
                btnRun.Enabled = true;
            }
            else
            {
                while (!process.HasExited)
                {
                    process.Kill();
                    await Task.Delay(100);
                }
                btnRun.Enabled = true;
                btnRun.Text = "Run Queue";
                isRunning = false;
            }
        }

        Process process = null;

        private async void ProcessQueue()
        {
            foreach (FileConversionInfo f in queue)
            {
                process = new Process();
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = f.ToString();
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
            }

            btnRun.Invoke(new Action(() => btnRun.Text = "Run Queue"));
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

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialise crop preset buttons
            CropPresets_Load();
            // Load saved defaults
            Settings_Load();
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

            // Default Presets
            menuOptionsPresetCPU.Text = Settings.Default.DefaultPresetCPU.ToString();
            menuOptionsPresetGPU.Text = Settings.Default.DefaultPresetGPU.ToString();

            // Copy Overwrite setting (this means if you copy and paste the settings
            // from one item to others, it will include whether "Overwrite if exists" is checked
            menuOptionsCopyOverwrite.Checked = Settings.Default.CopyOverwrite;

            // Additional options
            menuOptionsAdditionalOptions.Text = Settings.Default.AdditionalOptions;
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

        private void menuOptionsCopyOverwrite_Click(object sender, EventArgs e)
        {
            menuOptionsCopyOverwrite.Checked = !menuOptionsCopyOverwrite.Checked;
            Settings.Default.CopyOverwrite = menuOptionsCopyOverwrite.Checked;
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

        private void btnCRFinfo_Click(object sender, EventArgs e)
        {
            // provide some easy to digest info on crf / cqp
            MessageBox.Show(
                "CRF/CQP (Constant Rate Factor/Constant Quantiser Parameter)\n\n" +
                "These are similar quality-based encoder modes:\n" +
                "- CRF is used by Software (CPU) encoders for adaptive quality and better efficiency\n" +
                "- CQP is used by Hardware (GPU) encoders with fixed compression for faster performance\n\n" +
                "Generally: higher value = lower quality but smaller filesize.\n\n" +
                "The results will vary across encoders. For example, AV1 works better at lower bitrates: " +
                "h264_nvenc, CQP 24 ≈ av1_nvenc, CQP 32-34 (roughly).\n",

                "Information about CRF / CQP"
            );
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
            lblSpeed.Text = $"Speed (Video/Audio) | {(double)trkSpeed.Value / 100.0:0.00}x";
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

        private void btnSpeedInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "This sets the speed using the setpts video filter, and the .\n\n" +
                "IMPORTANT:\n" +
                "This does not affect audio right now, but you can manually set it in additional options:\n" +
                "Use '-filter:a atempo=x' where x is your speed multiplier (cannot be less than 0.5 or more than 100).\n\n" +
                "FURTHER: You may want to set your desired FPS using the '-r (fps)' option in additional options, " +
                "as FFmpeg will otherwise drop frames it doesn't need.\n\n" +
                "CONTROLS:\n" +
                "Hold Shift while sliding for finer control\n" +
                "Hold Alt while sliding for even finer control"
                ,

                "Speed Information"
            );
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
    }

    public class FileConversionInfo
    {
        public string Filename { get; set; }
        public string OutputName { get; set; }
        public TimeSpan TrimStart { get; set; }
        public TimeSpan TrimEnd { get; set; }
        public bool TrimUseDuration { get; set; }
        public decimal AudioGain { get; set; }
        public decimal AudioDelaySeconds { get; set; }
        public string CropFilter { get; set; }
        public string Encoder { get; set; }
        public string Tags { get; set; }
        public int CRF { get; set; }
        public string Preset { get; set; }
        public decimal FPS { get; set; }
        public double Speed { get; set; }
        public string AdditionalOptions { get; set; }
        public bool OverwriteExisting { get; set; }

        public FileConversionInfo(string filename)
        {
            Filename = filename;
            OutputName = "{filename}_out.mp4";
            CropFilter = string.Empty;
            Encoder = "av1_nvenc";
            Tags = string.Empty;
            AdditionalOptions = string.Empty;
            Preset = string.Empty;
            CRF = 22;
            FPS = 0;
            Speed = 1.00;
        }

        public string GetDelay(bool nameless = false)
        {
            string delay = string.Empty;

            if (AudioDelaySeconds != 0)
            {
                delay = $"-itsoffset {AudioDelaySeconds} -i {(nameless ? "[in_file]" : $"\"{Filename}\"")} -map 0:v -map 1:a ";
            }

            return delay;
        }

        public string GetVideoFilters()
        {
            List<string> filters = new List<string>();
            // crop filter
            if (!string.IsNullOrWhiteSpace(CropFilter))
            {
                filters.Add($"crop={CropFilter}");
            }
            // Speed (setpts) filter (may require -r to be set for fps instead of below filter)
            if (Speed != 1.00)
            {
                // note that we need to divide 1 by the speed multiplier to get the PTS multiplier
                filters.Add($"setpts={1.00 / Speed:0.00}*PTS");
            }
            // FPS filter (more precise than just doing -r to drop/duplicate frames)
            if (FPS >= 1.0m)
            {
                filters.Add($"fps={FPS}");
            }
            // add any video filters found in Additional Options, excludes the quotation marks automatically
            string vf = GetVideoFilterString(AdditionalOptions, false, true);
            if (vf != null)
            {
                foreach (string filter in vf.Split(','))
                {
                    // trim to remove spaces
                    filters.Add(filter.Trim());
                }
            }

            return filters.Count == 0 ? string.Empty : $"-vf \"{string.Join(",", filters)}\" ";
        }

        public string GetGain()
        {
            return AudioGain == 0 ? string.Empty : $"-af \"volume={AudioGain}\" ";
        }

        public string GetTrim()
        {
            string trim = string.Empty;
            if (TrimStart != TimeSpan.Zero)
            {
                trim += $"-ss {TrimStart} ";
            }
            if (TrimEnd != TimeSpan.Zero)
            {
                if (TrimUseDuration) trim += $"-t {TrimEnd} ";
                else trim += $"-t {TrimEnd.Subtract(TrimStart)} ";
            }
            return trim;
        }

        public string GetEncoder()
        {
            // NOTE: I just automatically add the hvc1 tag to any HEVC encoding because it still works fine and just lets iOS play it
            if (Encoder.Length == 0 || Encoder.ToLower() == "none") return "";
            return $"-c:v {Encoder} ";
        }

        public string GetCRF()
        {
            string crf = string.Empty;

            if (isCPUEncoder(Encoder))
            {
                crf = $"-crf {CRF} ";
            }
            else if (Encoder.Length > 0)
            {
                crf = $"-rc constqp -qp {CRF} ";
            }

            return crf;
        }

        public static bool isCPUEncoder(string encoder)
        {
            return new[] {
                "libx264",
                "libx265"
            }
                .Contains(encoder.ToLower());
        }

        public string GetPreset()
        {
            return Preset == string.Empty ? string.Empty : $"-preset {Preset} ";
        }

        public string GetTags()
        {
            return Tags == string.Empty ? string.Empty : $"-tag:v {Tags} ";
        }

        private string GetVideoFilterString(string vf, bool includeVfOption, bool removeQuotes)
        {
            if (vf.Contains("-vf "))
            {
                // ensure that there is a space after the -vf option
                vf = vf.Substring(vf.IndexOf("-vf "));
                string[] vf_parts = vf.Split(new char[] { ' ' });
                // ensure there is at least one element after the '-vf'
                if (vf_parts.Length >= 2)
                {
                    // assumes video filter is one word
                    vf = vf_parts[1];
                    // check if the video filter starts with a quote, because then it could contain spaces
                    if (vf_parts[1].Length > 1 && vf_parts[1][0] == '"')
                    {
                        // find the last part that has the ending quote
                        int last_part = -1;
                        for (int i = 1; i < vf_parts.Length; i++)
                        {
                            // Length has to be greater than 1 so avoid coutning the first quote also as the last
                            if (vf_parts[i].Length > 1 && vf_parts[i].Last() == '"')
                            {
                                last_part = i;
                                break;
                            }
                        }
                        if (last_part >= 1)
                        {
                            // joins the array by spaces again, only up to the last quote
                            vf = string.Join(" ", vf_parts.Skip(1).Take(last_part));
                        }
                    }

                    // remove any existing quotes from the start/end if needed
                    if (removeQuotes)
                    {
                        if (vf.Length > 1 && vf[0] == '"')
                        {
                            vf = vf.Substring(1);
                        }
                        if (vf.Length > 1 && vf.Last() == '"')
                        {
                            vf = vf.Substring(0, vf.Length - 1);
                        }
                    }
                    // add the -vf text back if needed
                    return (includeVfOption ? "-vf " : "") + vf;
                }
            }
            
            return null;
        }

        public IEnumerable<string> GetAdditionalOptionsBefore(string addopts)
        {
            // only to be used in the normal get additional options function
            if (addopts.Contains("before:"))
            {
                List<string> parsedOptions = new List<string>();
                bool foundStart = false;
                int startIndex = -1;
                string[] options = addopts.Split(' ');
                for (int i = 0; i < options.Length; i++)
                {
                    // get this option
                    string option = options[i];
                    // get next option (or default start if no next)
                    string nextOption = i < options.Length - 1 ? options[i + 1] : "-";
                    if (!foundStart)
                    {
                        if (option.StartsWith("before:-"))
                        {
                            // check if the next option is also the start of an option
                            // because that would imply this option does not have a parameter, it is just a toggle
                            if (nextOption.Replace("before:", "").StartsWith("-"))
                            {
                                parsedOptions.Add(option);
                            }
                            else
                            {
                                foundStart = true;
                                startIndex = i;
                            }
                        }
                    }
                    else
                    {
                        // check if the next option is the start of a new option
                        // implying the end of this option's arguments
                        if (nextOption.Replace("before:", "").StartsWith("-"))
                        {
                            // joins the elements from the startindex to the current index
                            parsedOptions.Add(
                                string.Join(
                                    " ",
                                    options.Skip(startIndex).Take(i - startIndex + 1)
                                )
                            );
                            foundStart = false;
                        }
                    }
                }

                if (parsedOptions.Count > 0)
                {
                    return parsedOptions;
                }
            }

            // if no successful returns, return an empty list
            // eval on ienumerable.any()
            return Enumerable.Empty<string>();
        }

        public string GetAdditionalOptions(bool before = false)
        {
            string addopts = AdditionalOptions.Trim();
            string vf = GetVideoFilterString(addopts, true, false);
            if (!string.IsNullOrWhiteSpace(vf))
            {
                // removes video filter from string
                addopts = addopts.Replace(vf, "");
                addopts = addopts.Trim();
                // remove any potential residue double spaces as a result of this removal
                // this is probably not the best way to handle this, but it'll do
                while (addopts.Contains("  "))
                {
                    addopts = addopts.Replace("  ", " ");
                }
            }

            // search for 'before:' options that will be added before any other options (but after input -i)
            // example use case for this is setting frame rate (with -r method) at the start of options like so:
            // before:-r 15
            IEnumerable<string> beforeOptions = GetAdditionalOptionsBefore(addopts);
            if (before)
            {
                if (beforeOptions.Any())
                {
                    IEnumerable<string> cleanedOptions =
                        beforeOptions.Select(s => s.StartsWith("before:") ? s.Substring(7) : s);
                    return string.Join(" ", cleanedOptions) + " ";
                }

                return string.Empty;
            }
            else if (beforeOptions.Any())
            {
                // remove the before options from the after options
                foreach (string option in beforeOptions)
                {
                    int i = addopts.IndexOf(option);
                    int optLen = option.Length;
                    if (i > 0 && addopts[i - 1] == ' ')
                    {
                        addopts = addopts.Remove(i - 1, optLen + 1);
                    }
                    else if (i + optLen < addopts.Length && addopts[i+optLen] == ' ')
                    {
                        addopts = addopts.Remove(i, optLen + 1);
                    }
                    else
                    {
                        addopts = addopts.Remove(i, optLen);
                    }
                }
            }

            return addopts == string.Empty ? string.Empty : addopts + " ";
        }

        public string GetOutput(bool filenameOnly = false)
        {
            var file =
                OutputName; //.Replace(".mp4", "").Replace(".MP4", "").Replace(".Mp4", "").Replace(".mP4", "") + ".mp4";
            if (file.Contains("{filename") && file.Contains("}"))
            {
                var fnFirstHalf = file.Substring(file.IndexOf("{filename"));
                var fnstr = fnFirstHalf.Substring(0, file.IndexOf("}") + 1);
                var replaceWith = System.IO.Path.GetFileNameWithoutExtension(Filename);
                if (fnstr.Contains("-"))
                {
                    // adds support for trimming the end of the filename
                    // for example
                    // {filename-3}mov
                    // would output the original filename, subtract the extension (like 'mp4'), plus 'mov'.
                    var segments = fnstr.Split('-');
                    if (segments.Length == 2 && segments[1].Length >= 2)
                    {
                        if (int.TryParse(segments[1].Substring(0, segments[1].Length - 1), out int i))
                        {
                            replaceWith = replaceWith.Substring(0, replaceWith.Length - i);
                        }
                    }
                }

                file = file.Replace(fnstr, replaceWith);
            }

            if (filenameOnly) return $"\"{file}\"";
            return '"' +
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(Filename),
                    file
                ) +
                '"';
        }

        public string GetOverwrite()
        {
            return OverwriteExisting ? " -y" : string.Empty;
        }

        public override string ToString()
        {
            return
                $"-i \"{Filename}\" "
                + GetAdditionalOptions(before: true)
                + GetTrim()
                + GetDelay()
                + GetVideoFilters()
                + GetGain()
                + GetEncoder()
                + GetCRF()
                + GetPreset()
                + GetTags()
                + GetAdditionalOptions()
                + GetOutput()
                + GetOverwrite()
                ;
        }

        public string ToStringNameless()
        {
            return
                $"ffmpeg -i [in_file] "
                + GetAdditionalOptions(before: true)
                + GetTrim()
                + GetDelay(nameless: true)
                + GetVideoFilters()
                + GetGain()
                + GetEncoder()
                + GetCRF()
                + GetPreset()
                + GetTags()
                + GetAdditionalOptions()
                + "[out_file]"
                + GetOverwrite()
                ;
        }
    }
}
