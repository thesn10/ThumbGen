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

            var sizing = opts.CalcSizes(frameExtractor.Width, frameExtractor.Height);

            var engine = new SystemDrawingEngineFactory(opts);

            return new ThumbnailGenerator(frameExtractor, opts, engine, sizing);
        }
    }
}
