using System;
using System.Runtime.Versioning;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Options;
using ThumbGen.Renderer;
using ThumbGen.SystemDrawing;

namespace ThumbGen
{
    public class ThumbnailGeneratorBuilder
    {
        public VideoFrameCaptureManager? FrameCaptureManager { get; set; }
        public Func<int, int, ThumbnailRenderer>? RendererFactory { get; set; }

        public static ThumbnailGeneratorBuilder Create() => new ThumbnailGeneratorBuilder();

        public ThumbnailGeneratorBuilder WithFFmpegVideoCapture(string input)
        {
            var frameExtractor = new VideoFrameExtractor(input);
            var frameCapture = new VideoFrameCaptureManager(frameExtractor);

            FrameCaptureManager = frameCapture;
            return this;
        }

        [SupportedOSPlatform("windows")]
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer(RenderingOptions opts)
        {
            RendererFactory = (width, height) =>
            {
                var sizing = opts.CalcSizes2(width, height);

                var engine = new SystemDrawingRenderEngine(opts, sizing.TotalSize);
                var renderer = new ThumbnailRenderer(opts, engine, sizing);
                return renderer;
            };
            return this;
        }

        [SupportedOSPlatform("windows")]
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer(Action<RenderingOptions> configure)
        {
            var opts = new RenderingOptions();
            configure(opts);

            return UseSystemDrawingRenderer(opts);
        }

        public ThumbnailGenerator Build()
        {
            ArgumentNullException.ThrowIfNull(FrameCaptureManager);
            ArgumentNullException.ThrowIfNull(RendererFactory);

            var renderer = RendererFactory(FrameCaptureManager.Width, FrameCaptureManager.Height);

            return new ThumbnailGenerator(FrameCaptureManager, renderer);
        }
    }
}
