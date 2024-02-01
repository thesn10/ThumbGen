using ThumbGen.Magick;
using ThumbGen.Options;
using ThumbGen.Renderer;

namespace ThumbGen
{
    public static class ThumbnailGeneratorExtensions
    {
        public static ThumbnailGeneratorBuilder UseMagickRenderer(this ThumbnailGeneratorBuilder builder)
        {
            builder.RendererFactory = (opts, videoSize) =>
            {
                var sizing = opts.CalcSizes2(videoSize.Width, videoSize.Height);

                var engine = new MagickThumbnailRenderEngine(opts, sizing.TotalSize);
                var renderer = new ThumbnailRenderer(opts, engine, sizing);
                return renderer;
            };

            return builder;
        }
    }
}
