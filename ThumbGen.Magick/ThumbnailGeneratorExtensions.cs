using ThumbGen.Magick;
using ThumbGen.Options;
using ThumbGen.Renderer;

namespace ThumbGen
{
    public static class ThumbnailGeneratorExtensions
    {
        public static ThumbnailGeneratorBuilder UseMagickRenderer(this ThumbnailGeneratorBuilder builder, RenderingOptions opts)
        {
            builder.RendererFactory = (width, height) =>
            {
                var sizing = opts.CalcSizes2(width, height);

                var engine = new MagickThumbnailRenderEngine(opts, sizing.TotalSize);
                var renderer = new ThumbnailRenderer(opts, engine, sizing);
                return renderer;
            };
            return builder;
        }

        public static ThumbnailGeneratorBuilder UseMagickRenderer(this ThumbnailGeneratorBuilder builder, Action<RenderingOptions> configure)
        {
            var opts = new RenderingOptions();
            configure(opts);

            return UseMagickRenderer(builder, opts);
        }
    }
}
