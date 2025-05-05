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
            { "1:1 Square", "crop='min(iw\\,ih)':'min(iw\\,ih)':(iw-min(iw\\,ih))/2:(ih-min(iw\\,ih))/2" }, // dynamic for any resolution for a centered square crop
            { "16:9 to 9:16 Vertical", "crop=ih*9/16:ih:(iw-ih*9/16)/2:0" }, // only works with a 16:9 video (i use this for making beat saber reels)
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
                e.Effect= DragDropEffects.None;
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
                            queue.Add(f);
                            listFiles.Items.Add(System.IO.Path.GetFileName(file));
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
                dropEncoder.Text = f.Encoder;
                chkHvc1.Visible = requiresHvc1(f.Encoder);
                chkHvc1.Checked = f.Tags == "hvc1";

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

            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                f.Encoder = dropEncoder.Text;
            }

            // Show option to add the hvc1 tag for apple devices if using HEVC encoding
            bool isHevc = requiresHvc1(dropEncoder.Text);

            // display checkbox to add hvc1 tag if required for the encoder, or no encoder is selected
            // the checkbox may be required if you already have a x265 video and just want to add the tag
            chkHvc1.Visible = isHevc || dropEncoder.Text.ToLower() == "none" || string.IsNullOrWhiteSpace(dropEncoder.Text);
            // Uncheck hvc1 tag if HEVC encoding isn't selected
            if (!isHevc) chkHvc1.Checked = false;
            // recheck hvc1 tag if settings always set it, but ONLY for hevc and libx265, NOT for "NONE" encoder (intentional)
            else if (Settings.Default.AlwaysHvc1) chkHvc1.Checked = true;

            UpdateCommand();
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

            bool copyTrims = false;
            var result = MessageBox.Show("Do you also want to copy TrimStart and TrimEnd values?",
                "Copy Settings",
                MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel) return;
            if (result == DialogResult.Yes) copyTrims = true;



            foreach (int i in listFiles.SelectedIndices)
            {
                FileConversionInfo f = queue[i];
                if (copyTrims)
                {
                    f.TrimStart = copy.TrimStart;
                    f.TrimEnd = copy.TrimEnd;
                }
                f.AudioGain = copy.AudioGain;
                f.AudioDelaySeconds = copy.AudioDelaySeconds;
                f.CropFilter = copy.CropFilter;
                f.Encoder = copy.Encoder;
            }
        }

        private void btnCropPresets_Click(object sender, EventArgs e)
        {
            menuCropPresets.Show(btnCropPresets, new Point(0, btnCropPresets.Height));
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (btnRun.Text.StartsWith("Run"))
            {
                ProcessQueue();
                btnRun.Text = "Stop";
            }
            else
            {
                btnRun.Enabled = false;
                while (!process.HasExited)
                {
                    process.Kill();
                    await Task.Delay(100);
                }
                btnRun.Enabled = true;
                btnRun.Text = "Run Queue";
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
                process.StartInfo.RedirectStandardOutput = true; // Redirects the output
                process.StartInfo.RedirectStandardError = true;  // Redirects error output
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;        // Do not create a window

                txtOutput.AppendText("Running: " + "ffmpeg " + f.ToString() + "\r\n");

                // Subscribe to the output events
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
        }

        private double ffmpegDuration = 0;

        private void ParseFFmpegOutput(string output, int totalFrames)
        {
            // Example of FFmpeg output line:
            // frame=  250 fps= 24 q=  28.0 size=   102400kB time=00:00:10.50 bitrate=  80.0kbits/s

            // Regex to capture time and frame information
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
            // Update progress bar based on frame count or your own logic
            if (InvokeRequired)
            {
                Invoke(new Action<double, double>(UpdateProgressBar), curTime, duration);
            }
            else
            {
                progressBar1.Value = (int)((curTime / duration) * 100);
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
                probe.StartInfo.RedirectStandardOutput = true; // Redirects the output
                probe.StartInfo.RedirectStandardError = true;  // Redirects error output
                probe.StartInfo.UseShellExecute = false;
                probe.StartInfo.CreateNoWindow = true;        // Do not create a window

                txtOutput.AppendText("Running: " + "ffprobe \"" + f.Filename + "\"\r\n");

                TimeSpan duration = TimeSpan.Zero;
                // Subscribe to the output events
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
                    // Cast sender object as menu item, then cast its tag to a string, and set it
                    SetCrop(((ToolStripMenuItem)presetSender).Tag.ToString());
                };
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
    }


    public class FileConversionInfo
    {
        public string Filename { get; set; }
        public string OutputName { get; set; }
        public TimeSpan TrimStart { get; set; }
        public TimeSpan TrimEnd { get; set; }
        public decimal AudioGain { get; set; }
        public decimal AudioDelaySeconds { get; set; }
        public string CropFilter { get; set; }
        public string Encoder { get; set; }
        public string Tags { get; set; }
        public bool OverwriteExisting { get; set; }

        public FileConversionInfo(string filename)
        {
            Filename = filename;
            OutputName = "{filename}_out.mp4";
            CropFilter = string.Empty;
            Encoder = "av1_nvenc";
            Tags = string.Empty;
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

        public string GetCrop()
        {
            return CropFilter == string.Empty ? string.Empty : "-vf \"" + CropFilter + "\" ";
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
                trim += $"-t {TrimEnd.Subtract(TrimStart)} ";
            }
            return trim;
        }

        public string GetEncoder()
        {
            // NOTE: I just automatically add the hvc1 tag to any HEVC encoding because it still works fine and just lets iOS play it
            if (Encoder.Length == 0 || Encoder.ToLower() == "none") return "";
            return $"-c:v {Encoder} ";
        }

        public string GetTags()
        {
            return Tags == string.Empty ? string.Empty : $"-tag:v {Tags} ";
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
                + GetDelay()
                + GetCrop()
                + GetGain()
                + GetTrim()
                + GetEncoder()
                + GetTags()
                + GetOutput()
                + GetOverwrite()
                ;
        }

        public string ToStringNameless()
        {
            return
                $"ffmpeg -i [in_file] "
                + GetDelay(nameless: true)
                + GetCrop()
                + GetGain()
                + GetTrim()
                + GetEncoder()
                + GetTags()
                + "[out_file]"
                + GetOverwrite()
                ;
        }
    }
}
