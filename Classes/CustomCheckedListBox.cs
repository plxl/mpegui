using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace mpegui
{
    internal class CustomCheckedListBox : CheckedListBox
    {
        public List<int> RedIndices { get; set; } = new List<int>();

        public CustomCheckedListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            // optimise painting to reduce flicker
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                  ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        public void SetRed(IEnumerable<int> redIndices)
        {
            RedIndices.AddRange(redIndices);
            Invalidate();
        }

        public void ClearRed()
        {
            RedIndices.Clear();
            Invalidate();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string text = Items[e.Index].ToString();
            bool isChecked = GetItemChecked(e.Index);

            // draw plain background (i don't want to show blue selection background)
            using (Brush b = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(b, e.Bounds);

            // draw checkbox
            CheckBoxState state = isChecked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
            Size checkSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
            Point checkLocation = new Point(e.Bounds.X + 1, e.Bounds.Y + (e.Bounds.Height - checkSize.Height) / 2);
            CheckBoxRenderer.DrawCheckBox(e.Graphics, checkLocation, state);

            // draw text
            Color foreColor = RedIndices.Contains(e.Index) ? Color.Red : ForeColor;
            using (Brush b = new SolidBrush(foreColor))
            {
                float textX = checkLocation.X + checkSize.Width + 2;
                float textY = e.Bounds.Y + (e.Bounds.Height - e.Font.Height) / 2f;
                e.Graphics.DrawString(text, e.Font, b, textX, textY);
            }

            //e.DrawFocusRectangle();
        }
    }
}
