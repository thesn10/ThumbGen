using System.Runtime.Versioning;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;

namespace ThumbGen.SystemDrawing
{
    [SupportedOSPlatform("windows")]
    public static class SystemDrawingThumbnailGenerator
    {
        public static ThumbnailGenerator Create(string input, ThumbGenOptions opts)
        {
            var frameExtractor = new VideoFrameExtractor(input);
            var frameCapture = new VideoFrameCaptureManager(frameExtractor);

            var sizing = opts.RenderingOptions.CalcSizes2(frameExtractor.Width, frameExtractor.Height);

            var engine = new SystemDrawingEngineFactory(opts.RenderingOptions, sizing.TotalSize);
            var renderer = new ThumbnailRenderer(opts.RenderingOptions, engine, sizing);

            return new ThumbnailGenerator(frameCapture, renderer, opts);
        }
    }
}
