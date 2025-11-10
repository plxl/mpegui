using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mpegui.Classes
{
    internal enum FadeMode
    {
        None,
        Video,
        Audio,
        Both,
    }
    internal class FadeInfo
    {
        public FadeMode Mode { get; set; }
        public int Milliseconds { get; set; }

        public FadeInfo(FadeMode mode, int milliseconds)
        {
            Mode = mode;
            Milliseconds = milliseconds;
        }
    }
}
