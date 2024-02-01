using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using ThumbGen.Engine;
using ThumbGen.Options;

namespace ThumbGen.SystemDrawing
{
    [SupportedOSPlatform("windows")]
    internal class SystemDrawingRenderEngine : IThumbnailRenderEngine
    {
        private readonly Brush _bgBrush;
        private readonly Brush _aspectOverlapBrush;
        private readonly Brush _timeCodeBgBrush;
        private readonly Brush _timeCodeBrush;
        private readonly Size _size;

        public SystemDrawingRenderEngine(RenderingOptions opts, Size totalSize, bool tryEnableUnixSupport = false)
        {
            if (tryEnableUnixSupport)
                AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true); // only works for net 6 or lower

            _size = totalSize;
            _bgBrush = MapToBrush(opts.BgGradient, opts.BgColor);
            _aspectOverlapBrush = MapToBrush(opts.AspectOverlapGradient, opts.AspectOverlapColor);
            _timeCodeBgBrush = MapToBrush(opts.TimeCodeBgGradient, opts.TimeCodeBgColor);
            _timeCodeBrush = MapToBrush(opts.TimeCodeGradient, opts.TimeCodeColor, Color.White);
        }

        public IThumbnailCanvas CreateCanvas()
        {
            var bitmap = new Bitmap(_size.Width, _size.Height);
            return new SystemDrawingCanvas(bitmap, _bgBrush, _aspectOverlapBrush, _timeCodeBgBrush, _timeCodeBrush);
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
