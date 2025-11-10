using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mpegui.Classes
{
    public enum FadeMode
    {
        Video,
        Audio,
        Both,
    }
    public class FadeInfo
    {
        public bool Enabled { get; set; }
        public FadeMode Mode { get; set; }
        public int Milliseconds { get; set; }

        public FadeInfo(FadeMode mode, int milliseconds, bool enabled)
        {
            Mode = mode;
            Milliseconds = milliseconds;
            Enabled = enabled;
        }

        public FadeInfo Clone()
        {
            return new FadeInfo(Mode, Milliseconds, Enabled);
        }
    }
}
