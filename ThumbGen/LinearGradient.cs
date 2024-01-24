using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen
{
    public class LinearGradient
    {
        public LinearGradient(Color color1, Color color2, float angle)
        {
            Color1 = color1;
            Color2 = color2;
            Angle = angle;
        }

        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public float Angle { get; set; }
    }
}
