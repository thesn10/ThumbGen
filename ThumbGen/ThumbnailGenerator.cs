using System;
using System.Collections.Generic;
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

    internal ThumbnailGenerator(
        VideoFrameCaptureManager frameCapture,
        ThumbnailRenderer renderer)
    {

        _frameCapture = frameCapture;
        _renderer = renderer;
    }

    public ValueTask<List<ThumbnailRenderResult>> ExecuteAsync(ThumbGenOptions thumbGenOptions, CancellationToken ct = default)
    {
        return ExecuteCoreAsync(thumbGenOptions, ct).ToListAsync(ct);
    }

    public async IAsyncEnumerable<ThumbnailRenderResult> ExecuteCoreAsync(
        ThumbGenOptions thumbGenOptions, 
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var startTime = thumbGenOptions.StartTime ?? thumbGenOptions.StartTimePercent * _frameCapture.Duration;
        var endTime = thumbGenOptions.EndTime ?? thumbGenOptions.EndTimePercent * _frameCapture.Duration;

        var totalFrames = thumbGenOptions.TotalFrames ?? _renderer.FramesPerThumbnail;
        var allFrames = CaptureFramesAsync(thumbGenOptions, startTime, endTime, totalFrames, ct);

        var webvttGenerator = await CreateWebVTTOrDefault(thumbGenOptions, ct).ConfigureAwait(false);

        var thumbnailFileIndex = 0;
        await foreach (var renderResult in _renderer.RenderMultipleAsync(allFrames, ct).ConfigureAwait(false))
        {
            yield return renderResult;
            if (ct.IsCancellationRequested) yield break;

            var imageFilePath = thumbGenOptions.GetFilePath(thumbnailFileIndex);
            await renderResult.Image.SaveToFileAsync(imageFilePath).ConfigureAwait(false);

            var imageUrl = thumbGenOptions.GetWebVTTImageUrl(imageFilePath, thumbnailFileIndex);
            if (webvttGenerator is not null)
                await webvttGenerator.AddCuesAsync(imageUrl, renderResult.FrameMetadata, ct).ConfigureAwait(false);

            thumbnailFileIndex++;
        }

        if (webvttGenerator is not null)
            await webvttGenerator.FinishAsync().ConfigureAwait(false);
    }

    private IAsyncEnumerable<Frame> CaptureFramesAsync(
        ThumbGenOptions thumbGenOptions, TimeSpan? startTime, TimeSpan? endTime, 
        int totalFrames, CancellationToken ct = default)
    {
        if (thumbGenOptions.Interval.HasValue)
        {
            return _frameCapture.PerformFrameCaptureAsync(thumbGenOptions.Interval.Value, startTime, endTime, thumbGenOptions.TotalFrames, ct);
        }
        else
        {
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