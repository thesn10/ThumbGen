using System;
using System.Collections.Generic;
using System.Drawing;
using ThumbGen.Builder;
using ThumbGen.Engine;
using ThumbGen.FrameCapture;

namespace ThumbGen
{
    public class ThumbnailRenderer
    {
        private readonly ThumbGenOptions _thumbGenOptions;
        private readonly IThumbnailEngineFactory _thumbnailEngineFactory;

        private readonly Size _totalSize;
        private readonly SizeF _frameSize;
        private readonly SizeF _borderSize;

        internal ThumbnailRenderer(
            ThumbGenOptions thumbGenOptions,
            IThumbnailEngineFactory thumbnailEngine,
            ThumbnailSizing sizing)
        {
            _thumbGenOptions = thumbGenOptions;
            _thumbnailEngineFactory = thumbnailEngine;
            (_totalSize, _frameSize, _borderSize) = sizing;
        }

        public IThumbnailResult RenderThumbnailFromFrames(IReadOnlyList<Frame> frames)
        {
            var thumbnailEngine = _thumbnailEngineFactory.CreateNew();

            for (var row = 0; row < _thumbGenOptions.TilingOptions1.Rows; row++)
            {
                for (var column = 0; column < _thumbGenOptions.TilingOptions1.Columns; column++)
                {
                    var frameNr = column + row * _thumbGenOptions.TilingOptions1.Columns;
                    var frame = frames[frameNr];

                    var originX = _frameSize.Width * column + _borderSize.Width * (column + 1);
                    var originY = _frameSize.Height * row + _borderSize.Height * (row + 1);
                    var x = originX;
                    var y = originY;
                    var width = _frameSize.Width;
                    var height = _frameSize.Height;

                    if (_thumbGenOptions.PreserveAspect)
                    {
                        var frameAspect = frame.VideoFrame.Width / (double)frame.VideoFrame.Height;
                        var aspect = _frameSize.Width / (double)_frameSize.Height;
                        if (Math.Abs(aspect - frameAspect) > 0.01)
                        {
                            if (_thumbGenOptions.AspectOverlap)
                                thumbnailEngine.DrawAspectOverlap(x, y, width, height);

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
                    frame.VideoFrame.Dispose();

                    if (_thumbGenOptions.TimeCodeFontSize is not null)
                    {
                        var tsString = frame.GetTimestampString();
                        var fontSize = _thumbGenOptions.TimeCodeFontSize.Value;

                        thumbnailEngine.DrawTimeCode(tsString, "Consolas", fontSize, originX, originY, _frameSize);
                    }
                }
            }

            if (_thumbGenOptions.WatermarkFilename is not null &&
                _thumbGenOptions.WatermarkSize is not null &&
                _thumbGenOptions.WatermarkPosition is not null)
            {
                var watermarkWidth = _thumbGenOptions.WatermarkSize.Value.Width;
                var watermarkHeight = _thumbGenOptions.WatermarkSize.Value.Height;
                var (watermarkX, watermarkY) = _thumbGenOptions.WatermarkPosition.Value switch
                {
                    WatermarkPosition.Center => ((_totalSize.Width - watermarkWidth) / 2, (_totalSize.Height - watermarkHeight) / 2),
                    WatermarkPosition.TopLeft => (0, 0),
                    WatermarkPosition.TopRight => (_totalSize.Width - watermarkWidth, 0),
                    WatermarkPosition.BottomLeft => (0, _totalSize.Height - watermarkHeight),
                    WatermarkPosition.BottomRight => (_totalSize.Width - watermarkWidth, _totalSize.Height - watermarkHeight),
                    _ => throw new NotImplementedException(),
                };

                thumbnailEngine.DrawWatermark(_thumbGenOptions.WatermarkFilename, watermarkX, watermarkY, watermarkWidth, watermarkHeight);
            }

            return thumbnailEngine.Finish();
        }
    }
}
