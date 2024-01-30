using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;

namespace ThumbGen;

public class ThumbnailGenerator
{
    private readonly ThumbGenOptions _thumbGenOptions;

    private readonly VideoFrameCaptureManager _frameCapture;
    private readonly ThumbnailRenderer _renderer;

    internal ThumbnailGenerator(
        VideoFrameCaptureManager frameCapture,
        ThumbnailRenderer renderer,
        ThumbGenOptions thumbGenOptions)
    {
        _thumbGenOptions = thumbGenOptions;

        _frameCapture = frameCapture;
        _renderer = renderer;
    }

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var startTime = _thumbGenOptions.StartTime ?? _thumbGenOptions.StartTimePercent * _frameCapture.Duration;
        var endTime = _thumbGenOptions.EndTime ?? _thumbGenOptions.EndTimePercent * _frameCapture.Duration;

        var framesPerThumbnail = _thumbGenOptions.RenderingOptions.TilingOptions.Rows * _thumbGenOptions.RenderingOptions.TilingOptions.Columns;
        var totalFrames = _thumbGenOptions.TotalFrames ?? framesPerThumbnail;

        var allFrames = CaptureFramesAsync(startTime, endTime, totalFrames);

        var webvttGenerator = await WebVTTGenerator.CreateAsync(_thumbGenOptions.WebVTTFilename, _frameCapture.Duration);

        for (var thumbnailFileIndex = 0; ; thumbnailFileIndex++)
        {
            var startIndex = thumbnailFileIndex * framesPerThumbnail;
            var frames = await allFrames.Skip(startIndex).Take(framesPerThumbnail).ToListAsync(ct);
            if (frames.Count == 0)
            {
                break;
            }

            var renderResult = await Task.Run(() => _renderer.Render(frames));

            var imageFilePath = _thumbGenOptions.GetFilePath(thumbnailFileIndex);

            await renderResult.Image.SaveToFileAsync(imageFilePath);

            var imageUrl = _thumbGenOptions.GetWebVTTImageUrl(imageFilePath, thumbnailFileIndex);
            await webvttGenerator.AddCuesAsync(imageUrl, renderResult.FrameMetadata, ct);
        }

        await webvttGenerator.FinishAsync();
    }

    private IAsyncEnumerable<Frame> CaptureFramesAsync(TimeSpan? startTime, TimeSpan? endTime, int totalFrames)
    {
        if (_thumbGenOptions.Interval.HasValue)
        {
            return _frameCapture.PerformFrameCaptureAsync(_thumbGenOptions.Interval.Value, startTime, endTime, totalFrames);
        }
        else
        {
            return _frameCapture.PerformFrameCaptureAsync(totalFrames, startTime, endTime);
        }
    }
}