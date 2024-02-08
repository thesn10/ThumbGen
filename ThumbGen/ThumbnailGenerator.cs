using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Renderer;

namespace ThumbGen;

public class ThumbnailGenerator
{

    private readonly VideoFrameCaptureManager _frameCapture;
    private readonly ThumbnailRenderer _renderer;
    private readonly ILogger? _logger;

    internal ThumbnailGenerator(
        VideoFrameCaptureManager frameCapture,
        ThumbnailRenderer renderer,
        ILogger? logger = null)
    {

        _frameCapture = frameCapture;
        _renderer = renderer;
        _logger = logger;
    }

    public ValueTask<List<ThumbnailRenderResult>> ExecuteAsync(ThumbGenOptions thumbGenOptions, CancellationToken ct = default)
    {
        return ExecuteCoreAsync(thumbGenOptions, ct).ToListAsync(ct);
    }

    public async IAsyncEnumerable<ThumbnailRenderResult> ExecuteCoreAsync(
        ThumbGenOptions thumbGenOptions, 
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var allFrames = CaptureFramesAsync(thumbGenOptions, ct);

        var webvttGenerator = await CreateWebVTTOrDefault(thumbGenOptions, ct).ConfigureAwait(false);

        _logger?.LogInformation("Starting thumbnail generation");

        var sw = Stopwatch.StartNew();

        var thumbnailFileIndex = 0;
        await foreach (var renderResult in _renderer.RenderMultipleAsync(allFrames, ct).ConfigureAwait(false))
        {
            sw.Stop();
            _logger?.LogInformation("Generated thumbnail {thumbnailIndex} in {elapsedTime}", thumbnailFileIndex, sw.Elapsed);

            yield return renderResult;
            if (ct.IsCancellationRequested) 
                yield break;

            var imageFilePath = thumbGenOptions.GetFilePath?.Invoke(thumbnailFileIndex) ?? null;
            if (imageFilePath is not null)
            {
                await renderResult.Image.SaveToFileAsync(imageFilePath).ConfigureAwait(false);
            }

            var imageUrl = thumbGenOptions.GetWebVTTImageUrl(imageFilePath, thumbnailFileIndex);
            if (webvttGenerator is not null)
                await webvttGenerator.AddCuesAsync(imageUrl, renderResult.FrameMetadata, ct).ConfigureAwait(false);

            thumbnailFileIndex++;
            sw.Restart();
        }

        if (webvttGenerator is not null)
            await webvttGenerator.FinishAsync().ConfigureAwait(false);

        _logger?.LogInformation("Finished rendering {thumbnailCount} thumbnails", thumbnailFileIndex);
    }

    private IAsyncEnumerable<Frame> CaptureFramesAsync(
        ThumbGenOptions thumbGenOptions, CancellationToken ct = default)
    {
        var startTime = thumbGenOptions.StartTime ?? thumbGenOptions.StartTimePercent * _frameCapture.Duration;
        var endTime = thumbGenOptions.EndTime ?? thumbGenOptions.EndTimePercent * _frameCapture.Duration;

        if (thumbGenOptions.Interval.HasValue)
        {
            var interval = thumbGenOptions.Interval.Value;
            _logger?.LogInformation("Capturing frames from {startTime} to {endTime} in interval {interval}", startTime, endTime, interval);

            return _frameCapture.PerformFrameCaptureAsync(interval, startTime, endTime, thumbGenOptions.TotalFrames, ct);
        }
        else
        {
            var totalFrames = thumbGenOptions.TotalFrames ?? _renderer.FramesPerThumbnail;
            _logger?.LogInformation("Capturing {totalFrames} frames from {startTime} to {endTime}", totalFrames, startTime, endTime);

            return _frameCapture.PerformFrameCaptureAsync(totalFrames, startTime, endTime, ct);
        }
    }

    private Task<WebVTTGenerator?> CreateWebVTTOrDefault(ThumbGenOptions thumbGenOptions, CancellationToken ct = default)
    {
        if (thumbGenOptions.GenerateWebVTT)
        {
            return WebVTTGenerator.CreateAsync(thumbGenOptions.WebVTTFilename, _frameCapture.Duration, ct: ct)!;
        }
        return Task.FromResult<WebVTTGenerator?>(null);
    }
}