using System;
using System.Drawing;
using ThumbGen.Builder;
using ThumbGen.Engine;
using ThumbGen.FrameCapture;

namespace ThumbGen;

public class ThumbnailGenerator
{
    private readonly ThumbGenOptions _thumbGenOptions;
    private readonly IThumbnailEngineFactory _thumbnailEngineFactory;
    private readonly VideoFrameExtractor _frameExtractor;

    private readonly VideoFrameCaptureManager _frameCapture;
    private readonly Size _totalSize;
    private readonly SizeF _frameSize;
    private readonly SizeF _borderSize;

    internal ThumbnailGenerator(
        VideoFrameExtractor frameExtractor, 
        ThumbGenOptions thumbGenOptions, 
        IThumbnailEngineFactory thumbnailEngine, 
        ThumbnailSizing sizing)
    {
        _frameExtractor = frameExtractor;
        _thumbGenOptions = thumbGenOptions;
        _thumbnailEngineFactory = thumbnailEngine;
        (_totalSize, _frameSize, _borderSize) = sizing;

        var endTime = _thumbGenOptions.EndTime ?? _thumbGenOptions.EndTimePercent * _frameExtractor.Duration ?? _frameExtractor.Duration;
        var startTime = _thumbGenOptions.StartTime ?? _thumbGenOptions.StartTimePercent * _frameExtractor.Duration ?? TimeSpan.Zero;

        _frameCapture = new VideoFrameCaptureManager(_frameExtractor, endTime, startTime, thumbGenOptions.TilingOptions1);
    }

    public IThumbnailResult GenerateThumbnail()
    {
        // background
        //graphics.FillRectangle(_bgBrush, 0, 0, _totalSize.Width, _totalSize.Height);
        var thumbnailEngine = _thumbnailEngineFactory.CreateNew();

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
                        thumbnailEngine.DrawAspectOverlap(x, y, width, height);
                        //graphics.FillRectangle(_aspectOverlapBrush, x, y, width, height);

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

            thumbnailEngine.DrawImage(frame.VideoFrame, x, y, width, height);
            //graphics.DrawImage(frame.VideoFrame.ToBitmap(), x, y, width, height);
            frame.VideoFrame.Dispose();

            if (_thumbGenOptions.TimeCodeFontSize is not null)
            {
                var tsString = frame.GetTimestampString();
                var fontSize = _thumbGenOptions.TimeCodeFontSize.Value;

                thumbnailEngine.DrawTimeCode(tsString, "Consolas", fontSize, originX, originY, _frameSize);
            }
        }

        return thumbnailEngine.Finish();
    }
}