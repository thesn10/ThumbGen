using ImageMagick;
using System.Drawing;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Magick;

namespace ThumbGen;

/*public class MagickThumbnailGeneratorOld
{
    private readonly ThumbGenOptions _thumbGenOptions;
    private readonly VideoFrameExtractor _frameExtractor;

    private readonly MagickColor? _bgColor;
    private readonly MagickImage? _bgImage;
    private readonly MagickColor? _aspectOverlapColor;
    private readonly MagickImage? _aspectOverlapImage;
    private readonly MagickColor? _timeCodeBgColor;
    private readonly MagickImage? _timeCodeBgImage;
    private readonly MagickColor? _timeCodeColor;
    private readonly MagickImage? _timeCodeImage;
    private readonly VideoFrameCaptureManager _frameCapture;
    private readonly Size _totalSize;
    private readonly SizeF _frameSize;
    private readonly SizeF _borderSize;

    public MagickThumbnailGeneratorOld(VideoFrameExtractor frameExtractor, ThumbGenOptions thumbGenOptions)
    {
        _frameExtractor = frameExtractor;
        _thumbGenOptions = thumbGenOptions;

        var endTime = _thumbGenOptions.EndTime ?? _thumbGenOptions.EndTimePercent * _frameExtractor.Duration ?? _frameExtractor.Duration;
        var startTime = _thumbGenOptions.StartTime ?? _thumbGenOptions.StartTimePercent * _frameExtractor.Duration ?? TimeSpan.Zero;

        _frameCapture = new VideoFrameCaptureManager(_frameExtractor, endTime, startTime, thumbGenOptions.TilingOptions1);

        (_totalSize, _frameSize, _borderSize) = _thumbGenOptions.CalcSizes(_frameExtractor.Width, _frameExtractor.Height);

        _bgColor = GetColor(_thumbGenOptions.BgColor);
        _bgImage = GetGradient(_thumbGenOptions.BgGradient);
        _aspectOverlapColor = GetColor(_thumbGenOptions.AspectOverlapColor);
        _aspectOverlapImage = GetGradient(_thumbGenOptions.AspectOverlapGradient);
        _timeCodeBgColor = GetColor(_thumbGenOptions.TimeCodeBgColor);
        _timeCodeBgImage = GetGradient(_thumbGenOptions.TimeCodeBgGradient);
        _timeCodeColor = GetColor(_thumbGenOptions.TimeCodeColor);
        _timeCodeImage = GetGradient(_thumbGenOptions.TimeCodeGradient);
    }

    public IMagickImage<ushort> GenerateBitmap()
    {
        var image = DrawToGraphics();

        return image;
    }

    public IMagickImage<ushort> DrawToGraphics()
    {
        //var image = _bgImage?.Clone() ?? new MagickImage(_bgColor ?? MagickColors.Black, _totalSize.Width, _totalSize.Height);

        //IDrawables<ushort> drawable = new Drawables();

        var frames = _frameCapture.PerformFrameCapture();
        var frameAspect = _frameExtractor.Width / (double)_frameExtractor.Height;

        foreach (var frame in frames)
        {
            var originX = _frameSize.Width * frame.Column + _borderSize.Width * (frame.Column + 1);
            var originY = _frameSize.Height * frame.Row + _borderSize.Height * (frame.Row + 1);
            var x = originX;
            var y = originY;
            var width = _frameSize.Width;
            var height = _frameSize.Height;

            if (_thumbGenOptions.PreserveAspect)
            {
                var aspect = _frameSize.Width / (double)_frameSize.Height;
                if (Math.Abs(aspect - frameAspect) > 0.01)
                {
                    if (_thumbGenOptions.AspectOverlap) 
                    {
                        //DrawBgImageOrColor(image, _aspectOverlapImage, _aspectOverlapColor)
                        //    .Rectangle(x, y, width, height)
                        //    .Draw(image);
                    }

                    if (aspect > frameAspect)
                    {
                        width = (int)(_frameSize.Height * frameAspect);
                        x += (_frameSize.Width - width) / 2;
                    }
                    else if (aspect < frameAspect)
                    {
                        height = (int)(_frameSize.Width / frameAspect);
                        y += (_frameSize.Height - height) / 2;
                    }
                }
            }

            var bmp = frame.VideoFrame.ToMagickImage();
            frame.VideoFrame.Dispose();

            var geo = new MagickGeometry((int)width, (int)height) 
            {
                IgnoreAspectRatio = true,
                //LimitPixels = true,
                FillArea = true,
            };
            bmp.Resize(geo);

            //drawable.Composite(x, y, bmp);
            //image.Composite(bmp, (int)x, (int)y);

            // timecode
            if (_thumbGenOptions.TimeCodeFontSize != null)
            {
                var tsString = frame.GetTimestampString();

                //var textFont = _thumbGenOptions.TimeCodeFont;
                var textRealSize = new Size(); //graphics.MeasureString(tsString, textFont);

                var textx = originX + _frameSize.Width - textRealSize.Width;
                var texty = originY + _frameSize.Height - textRealSize.Height;

                //DrawBgImageOrColor(image, _timeCodeBgImage, _timeCodeBgColor)
                //    .Rectangle(x, y, textRealSize.Width, textRealSize.Height)
                //    .Draw(image);

                //DrawBgImageOrColor(image, _timeCodeImage, _timeCodeColor)
                //    .FontPointSize((double)_thumbGenOptions.TimeCodeFontSize)
                //    .Font("Consolas")
                //    .TextAlignment(TextAlignment.Left)
                //    .Text(textx, texty, tsString)
                //    .Draw(image);
            }
        }

        //return image;
    }
}*/