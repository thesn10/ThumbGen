using ThumbGen.Magick;
using ThumbGen.Renderer;
using Microsoft.Extensions.Logging;

namespace ThumbGen
{
    public static class ThumbnailGeneratorExtensions
    {
        public static ThumbnailGeneratorBuilder UseMagickRenderer(this ThumbnailGeneratorBuilder builder)
        {
            builder.RendererFactory = (opts, videoSize) =>
            {
                var sizing = opts.CalcSizes2(videoSize.Width, videoSize.Height);
                var logger = builder.LoggerFactory?.CreateLogger(typeof(ThumbnailRenderer));

                var engine = new MagickThumbnailRenderEngine(opts, sizing.TotalSize);
                var renderer = new ThumbnailRenderer(opts, engine, sizing, logger);
                return renderer;
            };

            return builder;
        }
    }
}
