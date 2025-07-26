using mpegui.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace mpegui
{
    public partial class formPasteEdits : Form
    {
        public List<bool> editsToPaste { get; private set; }
        public FileConversionInfo copiedEdits { get; set; }
        public formPasteEdits()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            editsToPaste = new List<bool>();
            foreach (var item in listEdits.Items)
            {
                editsToPaste.Add(listEdits.GetItemChecked(listEdits.Items.IndexOf(item)));
            }
            Settings.Default.PasteEdits = editsToPaste;
            Settings.Default.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void formPasteEdits_Load(object sender, EventArgs e)
        {
            if (copiedEdits == null) return;
            FileConversionInfo c = copiedEdits;

            if (Settings.Default.PasteEdits is null)
                Settings.Default.PasteEdits = Enumerable.Repeat(false, 13).ToList();
            var prevStates = Settings.Default.PasteEdits;

            listEdits.Items.Clear();
            var edits = new Dictionary<string, bool>
            {
                { $"Trim: {getTrimString()}",                                                   prevStates[0]  },
                { $"Encoder: {blnkStr(c.Encoder)}",                                             prevStates[1]  },
                { $"Tags: {blnkStr(c.Tags)}",                                                   prevStates[2]  },
                { $"Crop Filter: {blnkStr(c.CropFilter)}",                                      prevStates[3]  },
                { $"Audio Gain: {c.AudioGain}{(c.AudioUseDb ? "dB" : "x")}",                    prevStates[4]  },
                { $"Audio Delay Seconds: {c.AudioDelaySeconds}",                                prevStates[5]  },
                { $"CRF/CQP: {c.CRF}",                                                          prevStates[6]  },
                { $"Encoder Preset: {blnkStr(c.Preset)}",                                       prevStates[7]  },
                { $"Frames per second: {(c.FPS < 1.0m ? "Same as source" : c.FPS.ToString())}", prevStates[8]  },
                { $"Video Speed: {c.Speed:0.00}x",                                              prevStates[9]  },
                { $"Additional Options: {blnkStr(c.AdditionalOptions)}",                        prevStates[10] },
                { $"Output Name: {c.OutputName}",                                               prevStates[11] },
                { $"Overwrite if exists: {ynb(c.OverwriteExisting)}",                           prevStates[12] },
            };

            foreach (var kvp in edits)
            {
                listEdits.Items.Add(kvp.Key, kvp.Value);
            }
        }
        private string blnkStr(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "NONE" : s;
        }
        private string ynb(bool b)
        {
            return b ? "YES" : "NO";
        }
        private string getTrimString()
        {
            FileConversionInfo c = copiedEdits;
            if (c.TrimStart == TimeSpan.Zero && c.TrimEnd == TimeSpan.Zero) return "NONE";
            string end = $" - {c.TrimEnd}";
            if (c.TrimUseDuration) end = $", Duration: {c.TrimEnd}";
            return $"{c.TrimStart}{end}";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void UpdateIncompatibilityLabel(ItemCheckEventArgs e)
        {
            bool pastingEncoder = listEdits.CheckedIndices.Contains(1);
            bool pastingPreset = listEdits.CheckedIndices.Contains(7);
            // because the ItemCheck event fires before it actually updates the CheckedIndicies, we need to do another check:
            if (e.Index == 1) pastingEncoder = e.NewValue == CheckState.Checked;
            if (e.Index == 7) pastingPreset = e.NewValue == CheckState.Checked;

            // check if one is enabled but not the either
            bool incompatibility = pastingEncoder ^ pastingPreset;
            lblIncompatibility.Visible = incompatibility;
            infoIncompatibility.Visible = incompatibility;
            if (incompatibility) listEdits.SetRed(new int[] { 1, 7 });
            else listEdits.ClearRed();
        }

        private void listEdits_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdateIncompatibilityLabel(e);
        }
    }
}
