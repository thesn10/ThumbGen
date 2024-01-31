using System;
using System.Runtime.Versioning;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Options;
using ThumbGen.SystemDrawing;

namespace ThumbGen
{
    public class ThumbnailGeneratorBuilder
    {
        public VideoFrameCaptureManager? FrameCaptureManager { get; set; }
        public ThumbnailRenderer? Renderer { get; set; }
        public ThumbGenOptions Options { get; init; }

        public ThumbnailGeneratorBuilder(ThumbGenOptions opts)
        {
            Options = opts;
        }

        public static ThumbnailGeneratorBuilder Create(ThumbGenOptions opts) => new ThumbnailGeneratorBuilder(opts);

        public ThumbnailGeneratorBuilder WithFFMpegVideoCapture(string input)
        {
            var frameExtractor = new VideoFrameExtractor(input);
            var frameCapture = new VideoFrameCaptureManager(frameExtractor);

            FrameCaptureManager = frameCapture;
            return this;
        }

        [SupportedOSPlatform("windows")]
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer()
        {
            var sizing = Options.RenderingOptions.CalcSizes2();

            var engine = new SystemDrawingEngineFactory(Options.RenderingOptions, sizing.TotalSize);
            var renderer = new ThumbnailRenderer(Options.RenderingOptions, engine, sizing);

            Renderer = renderer;
            return this;
        }

        public ThumbnailGenerator Build()
        {
            ArgumentNullException.ThrowIfNull(FrameCaptureManager);
            ArgumentNullException.ThrowIfNull(Renderer);

            return new ThumbnailGenerator(FrameCaptureManager, Renderer, Options);
        }
    }
}
