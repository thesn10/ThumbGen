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

    public VideoFrameCaptureManager(TimeSpan endTime, TimeSpan startTime, TilingOptions tilingOptions)
    {
        _endTime = endTime;
        _startTime = startTime;
        _tilingOptions = tilingOptions;

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
        var frames = new List<Frame>(TotalFrames);

        for (var row = 0; row < _tilingOptions.Rows; row++)
        {
            for (var column = 0; column < _tilingOptions.Columns; column++)
            {
                var frameNr = column + row * _tilingOptions.Columns;
                var frameTime = _startTime + AverageTimePerFrame * frameNr;

                var bmp = _frameExtractor.GetAtTimestamp(frameTime, out var frameTs);

                if (bmp is null)
                {
                    // TODO
                    throw new InvalidOperationException("Bitmap was null");
                }

                frames[frameNr] = new Frame(bmp, frameTs, row, column);
            }
        }

        return frames;
    }
}