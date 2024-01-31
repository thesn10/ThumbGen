using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen.Tests
{
    internal static class VideoConstants
    {
        public static string Video_360p_MP4() => Path.GetFullPath("./bbb-h264-360p.mp4");
        public static string Video_360p_WEBM() => Path.GetFullPath("./bbb-vp9-360p.webm");
    }
}
