using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ThumbGen.FrameCapture;

public class VideoFrameCaptureManager
{
    private readonly VideoFrameExtractor _frameExtractor;

    public TimeSpan Duration => _frameExtractor.Duration;
    public int Width => _frameExtractor.Width;
    public int Height => _frameExtractor.Height;

    public VideoFrameCaptureManager(
        VideoFrameExtractor frameExtractor)
    {
        _frameExtractor = frameExtractor;
    }

    public IReadOnlyList<Frame> PerformFrameCapture(
        int totalFrames,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null)
    {
        EnsureTimeSpansNotNull(ref startTime, ref endTime);

        var averageTimePerFrame = (endTime.Value - startTime.Value) / totalFrames;

        return PerformFrameCapture(averageTimePerFrame, startTime, endTime, totalFrames);
    }

    public IReadOnlyList<Frame> PerformFrameCapture(
        TimeSpan interval,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        int? maxFrames = null)
    {
        EnsureTimeSpansNotNull(ref startTime, ref endTime);

        var totalFrames = (int)Math.Floor((endTime.Value - startTime.Value) / interval);
        maxFrames = maxFrames.HasValue ? Math.Min(maxFrames.Value, totalFrames) : totalFrames;

        var frames = new Frame[maxFrames.Value];

        for (var frameNr = 0; frameNr < maxFrames; frameNr++)
        {
            var frameTime = startTime.Value + interval * frameNr;

            if (frameTime > endTime)
                break;

            var (videoFrame, frameTs) = _frameExtractor.GetAtTimestamp(frameTime);

            if (videoFrame is null)
            {
                throw new Exception("Video frame was null");
            }

            frames[frameNr] = new Frame(videoFrame, frameTs);
        }

        return frames;
    }

    public IAsyncEnumerable<Frame> PerformFrameCaptureAsync(
        int totalFrames,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        CancellationToken ct = default)
    {
        EnsureTimeSpansNotNull(ref startTime, ref endTime);

        var averageTimePerFrame = (endTime.Value - startTime.Value) / totalFrames;

        return PerformFrameCaptureAsync(averageTimePerFrame, startTime, endTime, totalFrames, ct);
    }

    public async IAsyncEnumerable<Frame> PerformFrameCaptureAsync(
        TimeSpan interval,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null,
        int? maxFrames = null,
        [EnumeratorCancellation]CancellationToken ct = default)
    {
        EnsureTimeSpansNotNull(ref startTime, ref endTime);

        var totalFrames = (int)Math.Floor((endTime.Value - startTime.Value) / interval);
        maxFrames = maxFrames.HasValue ? Math.Min(maxFrames.Value, totalFrames) : totalFrames;

        var frames = new Frame[maxFrames.Value];

        for (var frameNr = 0; frameNr < maxFrames; frameNr++)
        {
            if (ct.IsCancellationRequested) 
                yield break;

            var frameTime = startTime.Value + interval * frameNr;

            if (frameTime > endTime)
                break;

            var (videoFrame, frameTs) = await Task.Run(() => _frameExtractor.GetAtTimestamp(frameTime)).ConfigureAwait(false);

            if (videoFrame is null)
            {
                throw new Exception("Video frame was null");
            }

            yield return new Frame(videoFrame, frameTs);
        }
    }

    public void EnsureTimeSpansNotNull(
        [NotNull] ref TimeSpan? startTime,
        [NotNull] ref TimeSpan? endTime)
    {
        startTime ??= TimeSpan.Zero;
        endTime ??= _frameExtractor.Duration;

        if (endTime > _frameExtractor.Duration)
        {
            throw new ArgumentException("EndTime is larger than duration", nameof(endTime));
        }

        if (startTime > _frameExtractor.Duration)
        {
            throw new ArgumentException("StartTime is larger than duration", nameof(startTime));
        }
    }
}