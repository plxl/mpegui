using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace mpegui
{
    public class FileConversionInfo
    {
        public string Filename { get; set; }
        public string OutputName { get; set; }
        public TimeSpan TrimStart { get; set; }
        public TimeSpan TrimEnd { get; set; }
        public bool TrimUseDuration { get; set; }
        public decimal AudioGain { get; set; }
        public bool AudioUseDb { get; set; }
        public decimal AudioDelaySeconds { get; set; }
        public bool AudioNormalise { get; set; }
        public string CropFilter { get; set; }
        public string Encoder { get; set; }
        public string Tags { get; set; }
        public int CRF { get; set; }
        public string Preset { get; set; }
        public decimal FPS { get; set; }
        public double Speed { get; set; }
        public string AdditionalOptions { get; set; }
        public bool OverwriteExisting { get; set; }
        public string TempFilename { get; set; }

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

            // by default it's multiplier mode, so 1.00x is no change
            AudioGain = 1m;
        }

        public FileConversionInfo Clone()
        {
            FileConversionInfo clone = new FileConversionInfo(Filename);
            clone.OutputName = OutputName;
            clone.TrimStart = TrimStart;
            clone.TrimEnd = TrimEnd;
            clone.TrimUseDuration = TrimUseDuration;
            clone.AudioGain = AudioGain;
            clone.AudioUseDb = AudioUseDb;
            clone.AudioDelaySeconds = AudioDelaySeconds;
            clone.AudioNormalise = AudioNormalise;
            clone.CropFilter = CropFilter;
            clone.Encoder = Encoder;
            clone.Tags = Tags;
            clone.CRF = CRF;
            clone.Preset = Preset;
            clone.FPS = FPS;
            clone.Speed = Speed;
            clone.AdditionalOptions = AdditionalOptions;
            clone.OverwriteExisting = OverwriteExisting;
            return clone;
        }

        public void SetPreset(Preset preset)
        {
            // get all properties included in the toCopy list
            IEnumerable<PropertyInfo> props = typeof(FileConversionInfo)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && preset.ToCopy.Contains(p.Name));

            foreach (PropertyInfo prop in props)
            {
                var value = prop.GetValue(preset.ConversionInfo);
                prop.SetValue(this, value);
            }
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

        public string GetFilters()
        {
            List<string> vfilters = new List<string>();
            List<string> afilters = new List<string>();
            // crop filter
            if (!string.IsNullOrWhiteSpace(CropFilter))
            {
                vfilters.Add($"crop={CropFilter}");
            }
            // Speed (setpts) filter (may require -r to be set for fps instead of below filter)
            if (Speed != 1.00)
            {
                // note that we need to divide 1 by the speed multiplier to get the PTS multiplier
                vfilters.Add($"setpts={1.00 / Speed:0.00}*PTS");
            }
            // FPS filter (more precise than just doing -r to drop/duplicate frames)
            if (FPS >= 1.0m)
            {
                vfilters.Add($"fps={FPS}");
            }
            // add any video filters found in Additional Options, excludes the quotation marks automatically
            string vf = GetFilterString("-vf", AdditionalOptions, false, true);
            if (vf != null)
            {
                foreach (string filter in vf.Split(','))
                {
                    // trim to remove spaces
                    vfilters.Add(filter.Trim());
                }
            }

            // audio gain
            if (AudioUseDb && AudioGain != 0m)
            {
                afilters.Add($"volume={AudioGain}dB");
            }
            else if (AudioGain != 1m)
            {
                afilters.Add($"volume={AudioGain}");
            }

            // audio normalisation filter (true peak = -1dB, integrated loudness = -16 LUFS, loudness range = 11)
            if (AudioNormalise)
            {
                afilters.Add("loudnorm=I=-16:TP=-1.0:LRA=11");
            }

            // add any audio filters found in Additional Options, excludes the quotation marks automatically
            string af = GetFilterString("-af", AdditionalOptions, false, true);
            if (af != null)
            {
                foreach (string filter in af.Split(','))
                {
                    // trim to remove spaces
                    afilters.Add(filter.Trim());
                }
            }

            // combine video and audio filters into single string
            string sout = vfilters.Count == 0 ? string.Empty : $"-vf \"{string.Join(",", vfilters)}\" ";
            sout += afilters.Count == 0 ? string.Empty : $"-af \"{string.Join(",", afilters)}\" ";
            return sout;
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

        private string GetFilterString(string ftype, string s, bool includeVfOption, bool removeQuotes)
        {
            if (s.Contains(ftype))
            {
                // ensure that there is a space after the -vf option
                s = s.Substring(s.IndexOf(ftype));
                string[] vf_parts = s.Split(new char[] { ' ' });
                // ensure there is at least one element after the '-vf'
                if (vf_parts.Length >= 2)
                {
                    // assumes video filter is one word
                    s = vf_parts[1];
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
                            s = string.Join(" ", vf_parts.Skip(1).Take(last_part));
                        }
                    }

                    // remove any existing quotes from the start/end if needed
                    if (removeQuotes)
                    {
                        if (s.Length > 1 && s[0] == '"')
                        {
                            s = s.Substring(1);
                        }
                        if (s.Length > 1 && s.Last() == '"')
                        {
                            s = s.Substring(0, s.Length - 1);
                        }
                    }
                    // add the -vf/-af text back if needed
                    return (includeVfOption ? ftype + " " : "") + s;
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
            string vf = GetFilterString("-vf", addopts, true, false);
            string af = GetFilterString("-af", addopts, true, false);
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
            if (!string.IsNullOrWhiteSpace(af))
            {
                // removes audio filter from string
                addopts = addopts.Replace(af, "");
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
                    else if (i + optLen < addopts.Length && addopts[i + optLen] == ' ')
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
                            if (i > replaceWith.Length) i = replaceWith.Length;
                            replaceWith = replaceWith.Substring(0, replaceWith.Length - i);
                        }
                    }
                }

                file = file.Replace(fnstr, replaceWith);
            }

            // determine if the filename is a specific path and not just relative
            bool isFullPath = Path.IsPathRooted(file) && !string.IsNullOrWhiteSpace(Path.GetPathRoot(file));
            // only return the user-inputted filename
            if (filenameOnly || isFullPath) return $"\"{file}\"";

            // else return the file relative the input file's directory
            return $"\"{Path.Combine(Path.GetDirectoryName(Filename), file)}\"";
        }

        public string GetOverwrite()
        {
            return OverwriteExisting ? " -y" : string.Empty;
        }

        public string outputFileAlreadyExists()
        {
            // checks if the output file will already exist
            // if it does, it will prompt the user asking if they want to overwrite it
            string outFilename = GetOutput().Trim('"');
            if (!OverwriteExisting && File.Exists(outFilename))
            {
                // check if the output filename is exactly the same as the input filename
                // because ffmpeg does not have built-in support for overwriting files in-place
                // we need to move the input file to a temporary location to replace it
                string fullFilename = Path.GetFullPath(Filename);
                string fullOutname = Path.GetFullPath(outFilename);
                if (string.Equals(fullFilename, fullOutname, StringComparison.OrdinalIgnoreCase))
                {
                    string tempPath = Path.GetTempFileName();
                    if (File.Exists(tempPath)) File.Delete(tempPath);
                    if (MessageBox.Show("The output file destination matches the input file for this item:\n\n" +
                        $"In File: {fullFilename}\n" +
                        $"Out File: {fullOutname}\n\n" +
                        $"Are you sure you want to replace the input file? This will be permanent.\n" +
                        $"If you select 'No', this file will be skipped and the remainder of the queue will be processed.",
                        
                        "Overwrite Input File",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Console.WriteLine($"Moving input file to temporary file: {tempPath}");
                        File.Move(fullFilename, tempPath);
                        TempFilename = tempPath;
                        return " -y";
                    }
                    return null;
                }
                else if (MessageBox.Show("The output filename for the current item in the queue already exists.\n\n" +
                    "Do you want to replace it?\n\n" +
                    $"In File: {Filename}\n" +
                    $"Out File: {outFilename}\n\n" +
                    "If you select 'No', this file will be skipped and the remainder of the queue will be processed.",
                    
                    "Overwrite File",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    return " -y";
                }
                else
                {
                    // user does not want to overwrite the file, so
                    // return null to say it already exists and can't proceed
                    return null;
                }
            } 
            
            // empty string means process the file without overwriting
            return string.Empty;
        }

        public override string ToString()
        {
            return
                $"-i \"{TempFilename ?? Filename}\" "
                + GetAdditionalOptions(before: true)
                + GetTrim()
                + GetDelay()
                + GetFilters()
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
                + GetFilters()
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
