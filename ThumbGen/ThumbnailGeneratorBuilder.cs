using System;
using System.Drawing;
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
        private RenderingOptions _renderingOptions = new RenderingOptions();

        public Func<string, VideoFrameCaptureManager>? FrameCaptureManagerFactory { get; set; }
        public Func<RenderingOptions, Size, ThumbnailRenderer>? RendererFactory { get; set; }

        public static ThumbnailGeneratorBuilder Create() => new ThumbnailGeneratorBuilder();

        public ThumbnailGeneratorBuilder WithFFmpegVideoCapture()
        {
            FrameCaptureManagerFactory = (input) =>
            {
                var frameExtractor = new VideoFrameExtractor(input);
                var frameCapture = new VideoFrameCaptureManager(frameExtractor); 
                return frameCapture;
            };

            return this;
        }

        [SupportedOSPlatform("windows")]
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer()
        {
            RendererFactory = (opts, videoSize) =>
            {
                var sizing = opts.CalcSizes2(videoSize.Width, videoSize.Height);

                var engine = new SystemDrawingRenderEngine(opts, sizing.TotalSize);
                var renderer = new ThumbnailRenderer(opts, engine, sizing);
                return renderer;
            };

            return this;
        }

        public ThumbnailGeneratorBuilder ConfigureRendering(Action<RenderingOptions> configure)
        {
            configure(_renderingOptions);

            return this;
        }

        public ThumbnailGeneratorBuilder ConfigureRendering(RenderingOptions options)
        {
            _renderingOptions = options;

            return this;
        }

        public ThumbnailGenerator Build(string inputFilePath)
        {
            ArgumentNullException.ThrowIfNull(FrameCaptureManagerFactory);
            ArgumentNullException.ThrowIfNull(RendererFactory);

            var frameCaptureManager = FrameCaptureManagerFactory(inputFilePath);
            var videoSize = new Size(frameCaptureManager.Width, frameCaptureManager.Height);
            var renderer = RendererFactory(_renderingOptions, videoSize);

            return new ThumbnailGenerator(frameCaptureManager, renderer);
        }
    }
}
