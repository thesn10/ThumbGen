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

            var sizing = opts.CalcSizes(frameExtractor.Width, frameExtractor.Height);

            var engine = new MagickEngineFactory(opts);
            var renderer = new ThumbnailRenderer(opts, engine, sizing);

            return new ThumbnailGenerator(frameCapture, renderer, opts);
        }
    }
}
