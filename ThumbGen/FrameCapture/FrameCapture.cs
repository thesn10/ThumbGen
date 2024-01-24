using System;
using System.Collections.Generic;
using ThumbGen.Options;

namespace ThumbGen.FrameCapture;

public class FrameCapture
{
    private readonly VideoFrameExtractor _frameExtractor;
    private readonly TimeSpan _endTime;
    private readonly TimeSpan _startTime;
    private readonly TilingOptions _tilingOptions;

    public int TotalImages { get; private set; }
    public TimeSpan TimePerFrame { get; private set; }

    public FrameCapture(TimeSpan endTime, TimeSpan startTime, TilingOptions tilingOptions)
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


        TotalImages = _tilingOptions.Rows * _tilingOptions.Columns;
        TimePerFrame = (_endTime - _startTime) / TotalImages; //TimeSpan.FromTicks((_endTime - _startTime).Ticks / totalImages);
    }

    public TimeSpan GetFrameTime(int frameNr) => _startTime + TimePerFrame * frameNr;

    public IEnumerable<Frame> CaptureFrames()
    {
        var frames = new List<Frame>(TotalImages);

        for (var row = 0; row < _tilingOptions.Rows; row++)
        {
            for (var column = 0; column < _tilingOptions.Columns; column++)
            {
                var frameNr = column + row * _tilingOptions.Columns;
                var frameTime = _startTime + TimePerFrame * frameNr;

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