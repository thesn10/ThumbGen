using ImageMagick;
using System.Drawing;
using ThumbGen.Builder;
using ThumbGen.Engine;
using ThumbGen.Options;

namespace ThumbGen.Magick
{
    internal class MagickThumbnailRenderEngine : IThumbnailRenderEngine
    {
        private readonly MagickEngineColorOptions _colorOpts;
        private readonly MagickColor? _bgColor;
        private readonly MagickImage? _bgImage;
        private readonly Size _size;

        public MagickThumbnailRenderEngine(RenderingOptions opts, Size totalSize)
        {
            _size = totalSize;
            _colorOpts = new MagickEngineColorOptions()
            {
                AspectOverlapColor = GetColor(opts.AspectOverlapColor),
                AspectOverlapImage = GetGradient(opts.AspectOverlapGradient),
                TimeCodeBgColor = GetColor(opts.TimeCodeBgColor),
                TimeCodeBgImage = GetGradient(opts.TimeCodeBgGradient),
                TimeCodeColor = GetColor(opts.TimeCodeColor, Color.White),
                TimeCodeImage = GetGradient(opts.TimeCodeGradient),
            };

            _bgColor = GetColor(opts.BgColor);
            _bgImage = GetGradient(opts.BgGradient);
        }

        public IThumbnailCanvas CreateCanvas()
        {
            var image = _bgImage?.Clone() ?? new MagickImage(_bgColor ?? MagickColors.Black, _size.Width, _size.Height);

            return new MagickThumbnailCanvas(image, _colorOpts);
        }

        private MagickColor? GetColor(Color? color, Color? defaultColor = null)
        {
            if (color is null)
            {
                return defaultColor.HasValue ? 
                    MagickColorUtil.FromColor(defaultColor.Value) : 
                    MagickColors.Black;
            }

            return MagickColorUtil.FromColor(color.Value);
        }

        private MagickImage? GetGradient(LinearGradient? gradient)
        {
            if (gradient is null) return null;

            var color1 = MagickColorUtil.FromColor(gradient.Color1);
            var color2 = MagickColorUtil.FromColor(gradient.Color2);

            var settings = new MagickReadSettings()
            {
                Width = _size.Width,
                Height = _size.Height,
            };
            settings.SetDefine(MagickFormat.Gradient, "angle", (int)Math.Round(gradient.Angle));

            return new MagickImage($"gradient:{color1}-{color2}", settings);
        }
    }
}
