using System;
using System.Collections.Generic;
using ThumbGen.Options;

namespace ThumbGen.FrameCapture;

public class VideoFrameCaptureManager
{
    private readonly VideoFrameExtractor _frameExtractor;
    private readonly TimeSpan _endTime;
    private readonly TimeSpan _startTime;
    private readonly TilingOptions _tilingOptions;

    public int TotalFrames { get; private set; }
    public TimeSpan AverageTimePerFrame { get; private set; }

    public VideoFrameCaptureManager(
        VideoFrameExtractor frameExtractor, 
        TimeSpan endTime, 
        TimeSpan startTime, 
        TilingOptions tilingOptions)
    {
        _endTime = endTime;
        _startTime = startTime;
        _tilingOptions = tilingOptions;
        _frameExtractor = frameExtractor;

        if (_endTime > _frameExtractor.Duration)
        {
            throw new InvalidOperationException("EndTime is larger than duration");
        }

        if (_startTime > _frameExtractor.Duration)
        {
            throw new InvalidOperationException("StartTime is larger than duration");
        }

        TotalFrames = _tilingOptions.Rows * _tilingOptions.Columns;
        AverageTimePerFrame = (_endTime - _startTime) / TotalFrames;
    }

    public TimeSpan CalculateFrameTime(int frameNr) => _startTime + AverageTimePerFrame * frameNr;

    public IEnumerable<Frame> PerformFrameCapture()
    {
        var frames = new Frame[TotalFrames];

        for (var row = 0; row < _tilingOptions.Rows; row++)
        {
            for (var column = 0; column < _tilingOptions.Columns; column++)
            {
                var frameNr = column + row * _tilingOptions.Columns;
                var frameTime = _startTime + AverageTimePerFrame * frameNr;

                var videoFrame = _frameExtractor.GetAtTimestamp(frameTime, out var frameTs);

                if (videoFrame is null)
                {
                    throw new Exception("Video frame was null");
                }

                frames[frameNr] = new Frame(videoFrame, frameTs, row, column);
            }
        }

        return frames;
    }
}