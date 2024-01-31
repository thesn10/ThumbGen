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

        var totalFrames = _thumbGenOptions.TotalFrames ?? _renderer.FramesPerThumbnail;
        var allFrames = CaptureFramesAsync(startTime, endTime, totalFrames);

        var webvttGenerator = await CreateWebVTTOrDefault(ct);

        var thumbnailFileIndex = 0;
        await foreach (var frames in allFrames.Buffer(_renderer.FramesPerThumbnail))
        {
            if (frames.Count == 0) break;
            if (ct.IsCancellationRequested) break;

            var renderResult = await Task.Run(() => _renderer.Render(frames.AsReadOnly()));

            var imageFilePath = _thumbGenOptions.GetFilePath(thumbnailFileIndex);
            await renderResult.Image.SaveToFileAsync(imageFilePath);

            var imageUrl = _thumbGenOptions.GetWebVTTImageUrl(imageFilePath, thumbnailFileIndex);
            if (webvttGenerator is not null)
                await webvttGenerator.AddCuesAsync(imageUrl, renderResult.FrameMetadata, ct);

            thumbnailFileIndex++;
        }

        if (webvttGenerator is not null)
            await webvttGenerator.FinishAsync();
    }

    private IAsyncEnumerable<Frame> CaptureFramesAsync(TimeSpan? startTime, TimeSpan? endTime, int totalFrames)
    {
        if (_thumbGenOptions.Interval.HasValue)
        {
            return _frameCapture.PerformFrameCaptureAsync(_thumbGenOptions.Interval.Value, startTime, endTime, _thumbGenOptions.TotalFrames);
        }
        else
        {
            return _frameCapture.PerformFrameCaptureAsync(totalFrames, startTime, endTime);
        }
    }

    private Task<WebVTTGenerator?> CreateWebVTTOrDefault(CancellationToken ct = default)
    {
        if (_thumbGenOptions.GenerateWebVTT)
        {
            return WebVTTGenerator.CreateAsync(_thumbGenOptions.WebVTTFilename, _frameCapture.Duration, ct: ct)!;
        }
        return Task.FromResult<WebVTTGenerator?>(null);
    }
}