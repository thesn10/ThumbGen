using System;
using System.Collections.Generic;
using System.Text;

namespace ThumbGen.Options
{
    public class TilingOptions
    {
        public int Columns { get; set; } = 1;
        public int Rows { get; set; } = 1;
        public bool AspectOverlap { get; set; } = true;
        public bool AutoFrameDistance { get; set; } = true;
        public TimeSpan FrameDistance { get; set; }
    }
}
