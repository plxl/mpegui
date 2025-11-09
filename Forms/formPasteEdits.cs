using mpegui.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.Json;

namespace mpegui
{
    public partial class formPasteEdits : Form
    {
        public Dictionary<string, bool> editsToPaste { get; set; }
        public FileConversionInfo copiedEdits { get; set; }
        public formPasteEdits()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (copiedEdits == null) return;
            var options = getOptions(copiedEdits);
            editsToPaste = options.Keys
                .Select((key, index) => new { key, isChecked = listEdits.GetItemChecked(index) })
                .ToDictionary(x => x.key, x => x.isChecked);

            Settings.Default.PasteEdits = JsonSerializer.Serialize(editsToPaste);
            Settings.Default.Save();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void formPasteEdits_Load(object sender, EventArgs e)
        {
            if (copiedEdits == null) return;
            var options = getOptions(copiedEdits);

            var json = Settings.Default.PasteEdits;
            var prevStates = new Dictionary<string, bool>();
            try
            {
                prevStates = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);
            }
            catch { Console.WriteLine("WARNING: Failed to deserialise settings for edits to paste."); }
            
            if (prevStates.Count != options.Count)
                prevStates = options
                    .Keys
                    .Select((key, index) => new { key, value = index != 0 })
                    .ToDictionary(x => x.key, x => x.value);

            Settings.Default.PasteEdits = JsonSerializer.Serialize(prevStates);

            listEdits.Items.Clear();
            foreach (var kvp in options)
                listEdits.Items.Add($"{kvp.Key}: {kvp.Value}", prevStates[kvp.Key]);
        }

        Dictionary<string, string> getOptions(FileConversionInfo c)
        {
            return new Dictionary<string, string>
            {
                ["Trim"]                = getTrimString(),
                ["Tags"]                = blnkStr(c.Tags),

                ["Encoder"]             = blnkStr(c.Encoder),
                ["Encoder Preset"]      = blnkStr(c.Preset),
                ["CRF/CQP"]             = c.CRF.ToString(),
                ["Crop Filter"]         = blnkStr(c.CropFilter),
                ["Frames Per Second"]   = c.FPS < 1.0m ? "Same as source" : c.FPS.ToString(),
                ["Video Speed"]         = $"{c.Speed:0.00}x",

                ["Audio Gain"]          = $"{c.AudioGain}{(c.AudioUseDb ? "dB" : "x")}",
                ["Audio Delay"]         = $"{c.AudioDelaySeconds.ToString()} seconds",
                ["Audio Normalisation"] = ynb(c.AudioNormalise),

                ["Additional Options"]  = blnkStr(c.AdditionalOptions),
                ["Output Name"]         = c.OutputName,
                ["Overwrite If Exists"] = ynb(c.OverwriteExisting)
            };
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
            bool pastingPreset = listEdits.CheckedIndices.Contains(8);
            // because the ItemCheck event fires before it actually updates the CheckedIndicies, we need to do another check:
            if (e.Index == 1) pastingEncoder = e.NewValue == CheckState.Checked;
            if (e.Index == 8) pastingPreset = e.NewValue == CheckState.Checked;

            // check if one is enabled but not the either
            bool incompatibility = pastingEncoder ^ pastingPreset;
            lblIncompatibility.Visible = incompatibility;
            infoIncompatibility.Visible = incompatibility;
            if (incompatibility) listEdits.SetRed(new int[] { 1, 8 });
            else listEdits.ClearRed();
        }

        private void listEdits_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdateIncompatibilityLabel(e);
        }

        private void lnkAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < listEdits.Items.Count; i++)
            {
                listEdits.SetItemChecked(i, true);
            }
        }

        private void lnkNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (int i = 0; i < listEdits.Items.Count; i++)
            {
                listEdits.SetItemChecked(i, false);
            }
        }
    }
}
