using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Magick;

namespace ThumbGen.SystemDrawing
{
    public static class MagickThumbnailGenerator
    {
        public static ThumbnailGenerator Create(string input, ThumbGenOptions opts)
        {
            var frameExtractor = new VideoFrameExtractor(input);
            var frameCapture = new VideoFrameCaptureManager(frameExtractor);

            var sizing = opts.RenderingOptions.CalcSizes2(frameExtractor.Width, frameExtractor.Height);

            var engine = new MagickEngineFactory(opts.RenderingOptions, sizing.TotalSize);
            var renderer = new ThumbnailRenderer(opts.RenderingOptions, engine, sizing);

            return new ThumbnailGenerator(frameCapture, renderer, opts);
        }
    }
}
