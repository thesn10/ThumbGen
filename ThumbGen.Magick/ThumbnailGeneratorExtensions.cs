using ThumbGen.Magick;
using ThumbGen.Options;

namespace ThumbGen
{
    public static class ThumbnailGeneratorExtensions
    {
        public static ThumbnailGeneratorBuilder UseMagickRenderer(this ThumbnailGeneratorBuilder builder)
        {
            var sizing = builder.Options.RenderingOptions.CalcSizes2();

            var engine = new MagickEngineFactory(builder.Options.RenderingOptions, sizing.TotalSize);
            var renderer = new ThumbnailRenderer(builder.Options.RenderingOptions, engine, sizing);

            builder.Renderer = renderer;
            return builder;
        }
    }
}
