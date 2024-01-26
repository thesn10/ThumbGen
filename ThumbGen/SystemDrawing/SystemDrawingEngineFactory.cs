using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using ThumbGen.Builder;
using ThumbGen.Engine;

namespace ThumbGen.SystemDrawing
{
    [SupportedOSPlatform("windows")]
    internal class SystemDrawingEngineFactory : IThumbnailEngineFactory
    {
        private readonly Brush _bgBrush;
        private readonly Brush _aspectOverlapBrush;
        private readonly Brush _timeCodeBgBrush;
        private readonly Brush _timeCodeBrush;
        private readonly Size _size;

        public SystemDrawingEngineFactory(ThumbGenOptions opts)
        {
            _size = opts.Size;
            _bgBrush = MapToBrush(opts.BgGradient, opts.BgColor);
            _aspectOverlapBrush = MapToBrush(opts.AspectOverlapGradient, opts.AspectOverlapColor);
            _timeCodeBgBrush = MapToBrush(opts.TimeCodeBgGradient, opts.TimeCodeBgColor);
            _timeCodeBrush = MapToBrush(opts.TimeCodeGradient, opts.TimeCodeColor);
        }


        public IThumbnailEngine CreateNew()
        {
            var bitmap = new Bitmap(_size.Width, _size.Height);
            return new SystemDrawingEngine(bitmap, _bgBrush, _aspectOverlapBrush, _timeCodeBgBrush, _timeCodeBrush);
        }

        private Brush MapToBrush(LinearGradient? gradient, Color? color, Color? defaultColor = null)
        {
            if (gradient is not null)
            {
                return GetGradient(gradient);
            }
            else if (color is not null)
            {
                return new SolidBrush(color.Value);
            }

            return new SolidBrush(defaultColor ?? Color.Black);
        }

        private LinearGradientBrush GetGradient(LinearGradient gradient)
        {
            return new LinearGradientBrush(
                new Rectangle(0, 0, _size.Width, _size.Height),
                    gradient.Color1, gradient.Color2, gradient.Angle);
        }
    }
}
