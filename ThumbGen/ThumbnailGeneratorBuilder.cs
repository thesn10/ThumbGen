using Microsoft.Extensions.Logging;
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
        private RenderingOptions _renderingOptions = new();

        public Func<string, VideoFrameCaptureManager>? FrameCaptureManagerFactory { get; set; }
        public Func<RenderingOptions, Size, ThumbnailRenderer>? RendererFactory { get; set; }
        public ILoggerFactory? LoggerFactory { get; set; }

        public static ThumbnailGeneratorBuilder Create() => new();

        public ThumbnailGeneratorBuilder WithLogging(ILoggerFactory? loggerFactory)
        {
            LoggerFactory = loggerFactory;

            return this;
        }

        public ThumbnailGeneratorBuilder WithFFmpegVideoCapture()
        {
            FrameCaptureManagerFactory = (input) =>
            {
                var logger = LoggerFactory?.CreateLogger(typeof(VideoFrameCaptureManager));
                var frameExtractor = new VideoFrameExtractor(input);
                var frameCapture = new VideoFrameCaptureManager(frameExtractor, logger); 
                return frameCapture;
            };

            return this;
        }
#if NET5_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public ThumbnailGeneratorBuilder UseSystemDrawingRenderer()
        {
            RendererFactory = (opts, videoSize) =>
            {
                var sizing = opts.CalcSizes2(videoSize.Width, videoSize.Height);
                var logger = LoggerFactory?.CreateLogger(typeof(ThumbnailRenderer));

                var engine = new SystemDrawingRenderEngine(opts, sizing.TotalSize);
                var renderer = new ThumbnailRenderer(opts, engine, sizing, logger);
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
#if NET7_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(FrameCaptureManagerFactory);
            ArgumentNullException.ThrowIfNull(RendererFactory);
#else
#pragma warning disable IDE0079
#pragma warning disable CA2208
            if (FrameCaptureManagerFactory is null) 
                throw new ArgumentNullException(nameof(FrameCaptureManagerFactory));
            if (RendererFactory is null)
                throw new ArgumentNullException(nameof(RendererFactory));
#pragma warning restore CA2208
#pragma warning restore IDE0079
#endif

            var frameCaptureManager = FrameCaptureManagerFactory(inputFilePath);
            var videoSize = new Size(frameCaptureManager.Width, frameCaptureManager.Height);
            var renderer = RendererFactory(_renderingOptions, videoSize);
            var logger = LoggerFactory?.CreateLogger(typeof(ThumbnailGenerator));

            return new ThumbnailGenerator(frameCaptureManager, renderer, logger);
        }
    }
}
