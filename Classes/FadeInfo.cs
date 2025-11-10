using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mpegui.Classes
{
    public enum FadeMode
    {
        None,
        Video,
        Audio,
        Both,
    }
    public class FadeInfo
    {
        public FadeMode Mode { get; set; }
        public int Milliseconds { get; set; }

        public FadeInfo(FadeMode mode, int milliseconds)
        {
            Mode = mode;
            Milliseconds = milliseconds;
        }

        public FadeInfo Clone()
        {
            return new FadeInfo(Mode, Milliseconds);
        }
    }
}
