using ImageMagick;

namespace ThumbGen.Magick
{
    internal class MagickEngineColorOptions
    {
        public required MagickColor? AspectOverlapColor { get; init; }
        public required MagickImage? AspectOverlapImage { get; init; }
        public required MagickColor? TimeCodeBgColor { get; init; }
        public required MagickImage? TimeCodeBgImage { get; init; }
        public required MagickColor? TimeCodeColor { get; init; }
        public required MagickImage? TimeCodeImage { get; init; }
    }
}
