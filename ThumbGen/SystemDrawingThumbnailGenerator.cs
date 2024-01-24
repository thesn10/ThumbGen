using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;

namespace ThumbGen;

[SupportedOSPlatform("windows")]
public class SystemDrawingThumbnailGenerator
{
    private readonly ThumbGenOptions _thumbGenOptions;
    private readonly VideoFrameExtractor _frameExtractor;

    private readonly Brush _bgBrush = new SolidBrush(Color.Black);
    private readonly Brush _aspectOverlapBrush = new SolidBrush(Color.Black);
    private readonly Brush _timeCodeBgBrush = new SolidBrush(Color.Black);
    private readonly Brush _timeCodeBrush = new SolidBrush(Color.White);
    private readonly FrameCapture _frameCapture;
    private readonly Size _totalSize;
    private readonly SizeF _frameSize;
    private readonly SizeF _borderSize;

    public SystemDrawingThumbnailGenerator(VideoFrameExtractor frameExtractor, ThumbGenOptions thumbGenOptions)
    {
        _frameExtractor = frameExtractor;
        _thumbGenOptions = thumbGenOptions;

        var endTime = _thumbGenOptions.EndTime ?? _thumbGenOptions.EndTimePercent * _frameExtractor.Duration ?? _frameExtractor.Duration;
        var startTime = _thumbGenOptions.StartTime ?? _thumbGenOptions.StartTimePercent * _frameExtractor.Duration ?? TimeSpan.Zero;

        _frameCapture = new FrameCapture(endTime, startTime, thumbGenOptions.TilingOptions1);

        (_totalSize, _frameSize, _borderSize) = _thumbGenOptions.CalcSizes(_frameExtractor.Width, _frameExtractor.Height);

        _bgBrush = MapToBrush(_thumbGenOptions.BgGradient, _thumbGenOptions.BgColor);
        _aspectOverlapBrush = MapToBrush(_thumbGenOptions.AspectOverlapGradient, _thumbGenOptions.AspectOverlapColor);
        _timeCodeBgBrush = MapToBrush(_thumbGenOptions.TimeCodeBgGradient, _thumbGenOptions.TimeCodeBgColor);
        _timeCodeBrush = MapToBrush(_thumbGenOptions.TimeCodeGradient, _thumbGenOptions.TimeCodeColor);
    }

    public Bitmap GenerateBitmap()
    {
        var b = new Bitmap(_totalSize.Width, _totalSize.Height);
        var graphics = Graphics.FromImage(b);

        DrawToGraphics(graphics);

        return b;
    }

    public void DrawToGraphics(Graphics graphics)
    {
        // background
        graphics.FillRectangle(_bgBrush, 0, 0, _totalSize.Width, _totalSize.Height);

        var frames = _frameCapture.CaptureFrames();
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
                        graphics.FillRectangle(_aspectOverlapBrush, x, y, width, height);

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

            graphics.DrawImage(frame.Bitmap, x, y, width, height);

            // timecode
            if (_thumbGenOptions.TimeCodeFont != null)
            {
                var tsString = frame.GetTimestampString();

                var textFont = _thumbGenOptions.TimeCodeFont;
                var textRealSize = graphics.MeasureString(tsString, textFont);

                var textx = originX + _frameSize.Width - textRealSize.Width;
                var texty = originY + _frameSize.Height - textRealSize.Height;
                graphics.FillRectangle(_timeCodeBgBrush, textx, texty, textRealSize.Width, textRealSize.Height);
                graphics.DrawString(tsString, textFont, _timeCodeBrush, textx, texty);
            }
        }

        // export
        graphics.Flush();
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
            new Rectangle(0, 0, _frameExtractor.Width, _frameExtractor.Height),
            gradient.Color1, gradient.Color2, gradient.Angle);
    }
}