using ThumbGen.Builder;
using ThumbGen.Engine;
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

    public IThumbnailResult GenerateThumbnail()
    {
        var endTime = _thumbGenOptions.EndTime ?? _thumbGenOptions.EndTimePercent * _frameCapture.Duration;
        var startTime = _thumbGenOptions.StartTime ?? _thumbGenOptions.StartTimePercent * _frameCapture.Duration;

        var totalFrames = _thumbGenOptions.TilingOptions1.Rows * _thumbGenOptions.TilingOptions1.Columns;
        var frames = _frameCapture.PerformFrameCapture(endTime, startTime, totalFrames);

        var thumbnail = _renderer.RenderThumbnailFromFrames(frames);

        return thumbnail;
    }
}