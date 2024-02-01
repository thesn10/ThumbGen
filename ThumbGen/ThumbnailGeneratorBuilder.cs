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

        public static ThumbnailGeneratorBuilder Create() => new ThumbnailGeneratorBuilder();

        public ThumbnailGeneratorBuilder WithFFMpegVideoCapture(string input)
        {
            var frameExtractor = new VideoFrameExtractor(input);
            var frameCapture = new VideoFrameCaptureManager(frameExtractor);

            FrameCaptureManager = frameCapture;
            return this;
        }

        [SupportedOSPlatform("windows")]
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer(RenderingOptions opts)
        {
            var sizing = opts.CalcSizes2();

            var engine = new SystemDrawingRenderEngine(opts, sizing.TotalSize);
            var renderer = new ThumbnailRenderer(opts, engine, sizing);

            Renderer = renderer;
            return this;
        }

        [SupportedOSPlatform("windows")]
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer(Action<RenderingOptions> configure)
        {
            var opts = new RenderingOptions();
            configure(opts);

            return UseSystemDrawingRenderer(opts);
        }

        public ThumbnailGenerator Build(ThumbGenOptions opts)
        {
            ArgumentNullException.ThrowIfNull(FrameCaptureManager);
            ArgumentNullException.ThrowIfNull(Renderer);

            return new ThumbnailGenerator(FrameCaptureManager, Renderer, opts);
        }

        public ThumbnailGenerator Build(Action<ThumbGenOptions> configure)
        {
            var opts = new ThumbGenOptions();
            configure(opts);

            return Build(opts);
        }
    }
}
