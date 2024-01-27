using EmguFFmpeg;
using ImageMagick;
using System.Drawing;
using ThumbGen.Engine;

namespace ThumbGen.Magick
{
    internal class MagickEngine : IThumbnailEngine
    {
        private readonly IMagickImage<byte> _image;
        private readonly MagickEngineColorOptions _colorOpts;

        public MagickEngine(IMagickImage<byte> image, MagickEngineColorOptions colorOpts)
        {
            _image = image;
            _colorOpts = colorOpts;
        }

        public void DrawAspectOverlap(float x, float y, float width, float height)
        {
            DrawBgImageOrColor(_image, _colorOpts.AspectOverlapImage, _colorOpts.AspectOverlapColor)
                .Rectangle(x, y, width, height)
                .Draw(_image);
        }

        public void DrawImage(VideoFrame videoFrame, float x, float y, float width, float height)
        {
            var bmp = videoFrame.ToMagickImage();

            var geo = new MagickGeometry((int)width, (int)height)
            {
                IgnoreAspectRatio = true,
                //LimitPixels = true,
                FillArea = true,
            };
            bmp.Resize(geo);

            //drawable.Composite(x, y, bmp);
            _image.Composite(bmp, (int)x, (int)y);
        }

        public void DrawTimeCode(string tsString, string fontFamily, float fontSize, float originX, float originY, SizeF frameSize)
        {
            var settings = new MagickReadSettings
            {
                Font = "Consolas",
                TextGravity = Gravity.Center,
                BackgroundColor = MagickColors.Transparent,
                FillColor = _colorOpts.TimeCodeColor,
                FillPattern = _colorOpts.TimeCodeImage,
                FontPointsize = (double)fontSize,
                Density = new Density(120), // TODO
            };

            using var caption = new MagickImage($"caption:{tsString}", settings);

            var textX = originX + frameSize.Width - caption.Width;
            var textY = originY + frameSize.Height - caption.Height;

            DrawBgImageOrColor(_image, _colorOpts.TimeCodeBgImage, _colorOpts.TimeCodeBgColor)
                .Rectangle(textX, textY, textX + caption.Width, textY + caption.Height)
                .Draw(_image);

            _image.Composite(caption, (int)textX, (int)textY, CompositeOperator.Over);
        }

        public IThumbnailResult Finish()
        {
            return new MagickThumbnailResult(_image);
        }

        private IDrawables<byte> DrawBgImageOrColor(IMagickImage<byte> image, MagickImage? bgImage, MagickColor? color, MagickColor? defaultColor = null)
        {
            if (bgImage is not null)
            {
                image.Settings.FillPattern = bgImage;
                return new Drawables();
            }

            return new Drawables().FillColor(color ?? defaultColor ?? MagickColors.Black);
        }
    }
}
