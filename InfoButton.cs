using mpegui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/* i'm using a custom implementation of a balloon tooltip because it fixes a
 * tiny bug with the WinForms balloon tooltip where using the .Show() method doesn't
 * always align the tooltip correctly to your specified point...
 * how has this not been fixed after decades, Microsoft? */

namespace mpegui
{
    [DesignerCategory("Code")]
    [DefaultEvent("Click")]
    internal class InfoButton : Panel
    {
        private string infoText = string.Empty;
        private string infoTitle = string.Empty;
        private BalloonToolTip toolTip;

        public InfoButton()
        {
            BackgroundImage = Resources.info;
            BackgroundImageLayout = ImageLayout.Zoom;
            Size = new Size(20, 20);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                if (toolTip == null) toolTip = new BalloonToolTip(this);
                toolTip.Create();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        private bool mouseOutside = true;
        protected override void OnMouseEnter(EventArgs e)
        {
            if (mouseOutside)
            {
                mouseOutside = false;
                if (toolTip != null)
                {
                    toolTip.icon = ToolTipIcon.Info;
                    toolTip.strText = infoText;
                    toolTip.strTitle = infoTitle;
                    toolTip.Show((new Point(Width / 2, Height / 2)));
                }
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            // check if the cursor has truly left control's bounds
            // this is because the cursor can "leave" just be being on the tip of the
            // tooltip, but we wouldn't want that to hide it
            var cursorPos = PointToClient(Cursor.Position);
            mouseOutside = !ClientRectangle.Contains(cursorPos);
            if (mouseOutside) toolTip.Hide();

            base.OnMouseLeave(e);
        }

        [Category("Design")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string InfoText
        {
            get => infoText;
            set
            {
                infoText = value;
            }
        }

        [Category("Design")]
        public string InfoTitle
        {
            get => infoTitle;
            set
            {
                infoTitle = value;
            }
        }
    }
}
