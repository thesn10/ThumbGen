using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen.Magick
{
    public static class MagickColorUtil
    {
        public static MagickColor FromColor(Color color)
        {
            return new MagickColor(color.R, color.G, color.B, color.A);
        }
    }
}
