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
            var textRealSize = new Size(); //graphics.MeasureString(tsString, textFont);

            var textx = originX + frameSize.Width - textRealSize.Width;
            var texty = originY + frameSize.Height - textRealSize.Height;

            DrawBgImageOrColor(_image, _colorOpts.TimeCodeBgImage, _colorOpts.TimeCodeBgColor)
                .Rectangle(originX, originY, textRealSize.Width, textRealSize.Height)
                .Draw(_image);

            DrawBgImageOrColor(_image, _colorOpts.TimeCodeImage, _colorOpts.TimeCodeColor)
                .FontPointSize((double)fontSize)
                .Font("Consolas")
                .TextAlignment(TextAlignment.Left)
                .Text(textx, texty, tsString)
                .Draw(_image);
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
