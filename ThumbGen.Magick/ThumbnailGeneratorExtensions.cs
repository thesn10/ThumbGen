using ThumbGen.Magick;
using ThumbGen.Options;

namespace ThumbGen
{
    public static class ThumbnailGeneratorExtensions
    {
        public static ThumbnailGeneratorBuilder UseMagickRenderer(this ThumbnailGeneratorBuilder builder, RenderingOptions opts)
        {
            var sizing = opts.CalcSizes2();

            var engine = new MagickThumbnailRenderEngine(opts, sizing.TotalSize);
            var renderer = new ThumbnailRenderer(opts, engine, sizing);

            builder.Renderer = renderer;
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
