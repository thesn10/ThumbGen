using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ThumbGen.Builder;
using ThumbGen.Engine;
using ThumbGen.FrameCapture;
using ThumbGen.Options;

namespace ThumbGen
{
    public class ThumbnailRenderer
    {
        private readonly RenderingOptions _renderingOptions;
        private readonly IThumbnailRenderEngine _thumbnailRenderEngine;

        private readonly Size _totalSize;
        private readonly SizeF _frameSize;
        private readonly SizeF _borderSize;

        internal ThumbnailRenderer(
            RenderingOptions thumbGenOptions,
            IThumbnailRenderEngine thumbnailEngine,
            ThumbnailSizing sizing)
        {
            _renderingOptions = thumbGenOptions;
            _thumbnailRenderEngine = thumbnailEngine;
            (_totalSize, _frameSize, _borderSize) = sizing;
        }

        public int FramesPerThumbnail => _renderingOptions.TilingOptions.Rows * _renderingOptions.TilingOptions.Columns;

        public ThumbnailRenderResult Render(IReadOnlyList<Frame> frames, CancellationToken ct = default)
        {
            var canvas = _thumbnailRenderEngine.CreateCanvas();
            var frameMetadata = new List<ThumbnailFrameMetadata>();

            for (var row = 0; row < _renderingOptions.TilingOptions.Rows; row++)
            {
                for (var column = 0; column < _renderingOptions.TilingOptions.Columns; column++)
                {
                    var frameNr = column + row * _renderingOptions.TilingOptions.Columns;
                    var frame = frames.ElementAtOrDefault(frameNr);

                    if (frame is null)
                    {
                        break;
                    }

                    var originX = _frameSize.Width * column + _borderSize.Width * (column + 1);
                    var originY = _frameSize.Height * row + _borderSize.Height * (row + 1);
                    var x = originX;
                    var y = originY;
                    var width = _frameSize.Width;
                    var height = _frameSize.Height;

                    if (_renderingOptions.PreserveAspect)
                    {
                        var frameAspect = frame.VideoFrame.Width / (double)frame.VideoFrame.Height;
                        var aspect = _frameSize.Width / (double)_frameSize.Height;
                        if (Math.Abs(aspect - frameAspect) > 0.01)
                        {
                            if (_renderingOptions.AspectOverlap)
                                canvas.DrawAspectOverlap(x, y, width, height);

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

                    canvas.DrawImage(frame.VideoFrame, x, y, width, height);
                    frame.VideoFrame.Dispose();

                    frameMetadata.Add(new ThumbnailFrameMetadata(frame.Timestamp, (int)x, (int)y, (int)width, (int)height));

                    if (_renderingOptions.TimeCodeFontSize is not null)
                    {
                        var tsString = frame.GetTimestampString();
                        var fontSize = _renderingOptions.TimeCodeFontSize.Value;

                        canvas.DrawTimeCode(tsString, "Consolas", fontSize, originX, originY, _frameSize);
                    }

                    if (ct.IsCancellationRequested) 
                        throw new OperationCanceledException();
                }
            }

            if (_renderingOptions.WatermarkFilename is not null &&
                _renderingOptions.WatermarkSize is not null &&
                _renderingOptions.WatermarkPosition is not null)
            {
                var watermarkWidth = _renderingOptions.WatermarkSize.Value.Width;
                var watermarkHeight = _renderingOptions.WatermarkSize.Value.Height;
                var (watermarkX, watermarkY) = _renderingOptions.WatermarkPosition.Value switch
                {
                    WatermarkPosition.Center => ((_totalSize.Width - watermarkWidth) / 2, (_totalSize.Height - watermarkHeight) / 2),
                    WatermarkPosition.TopLeft => (0, 0),
                    WatermarkPosition.TopRight => (_totalSize.Width - watermarkWidth, 0),
                    WatermarkPosition.BottomLeft => (0, _totalSize.Height - watermarkHeight),
                    WatermarkPosition.BottomRight => (_totalSize.Width - watermarkWidth, _totalSize.Height - watermarkHeight),
                    _ => throw new NotImplementedException(),
                };

                canvas.DrawWatermark(_renderingOptions.WatermarkFilename, watermarkX, watermarkY, watermarkWidth, watermarkHeight);
            }

            var image = canvas.Finish();

            return new ThumbnailRenderResult(image, frameMetadata);
        }

        public Task<ThumbnailRenderResult> RenderAsync(IReadOnlyList<Frame> frames, CancellationToken ct = default)
        {
            return Task.Run(() => Render(frames, ct), ct);
        }

        public async IAsyncEnumerable<ThumbnailRenderResult> RenderMultipleAsync(
            IAsyncEnumerable<Frame> frames, 
            [EnumeratorCancellation]CancellationToken ct = default)
        {
            await foreach (var currentFrames in frames.Buffer(FramesPerThumbnail).WithCancellation(ct))
            {
                if (currentFrames.Count == 0) break;
                if (ct.IsCancellationRequested) break;

                ThumbnailRenderResult renderResult;
                try
                {
                    renderResult = await RenderAsync(currentFrames.AsReadOnly(), ct);
                }
                catch (OperationCanceledException)
                {
                    yield break;
                }

                yield return renderResult;
            }
        }
    }
}
