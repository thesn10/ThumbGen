using System;
using System.Collections.Generic;
using ThumbGen.Options;

namespace ThumbGen.FrameCapture;

public class VideoFrameCaptureManager
{
    private readonly VideoFrameExtractor _frameExtractor;

    public TimeSpan Duration => _frameExtractor.Duration;

    public VideoFrameCaptureManager(
        VideoFrameExtractor frameExtractor)
    {
        _frameExtractor = frameExtractor;
    }

    public IReadOnlyList<Frame> PerformFrameCapture(
        TimeSpan? endTime,
        TimeSpan? startTime,
        int totalFrames)
    {
        var frames = new Frame[totalFrames];

        if (startTime is null)
            startTime = TimeSpan.Zero;

        if (endTime is null)
            endTime = _frameExtractor.Duration;

        if (endTime > _frameExtractor.Duration)
        {
            throw new InvalidOperationException("EndTime is larger than duration");
        }

        if (startTime > _frameExtractor.Duration)
        {
            throw new InvalidOperationException("StartTime is larger than duration");
        }

        var averageTimePerFrame = (endTime.Value - startTime.Value) / totalFrames;

        for (var frameNr = 0; frameNr < totalFrames; frameNr++)
        {
            var frameTime = startTime.Value + averageTimePerFrame * frameNr;
            var videoFrame = _frameExtractor.GetAtTimestamp(frameTime, out var frameTs);

            if (videoFrame is null)
            {
                throw new Exception("Video frame was null");
            }

            frames[frameNr] = new Frame(videoFrame, frameTs);
        }

        return frames;
    }
}