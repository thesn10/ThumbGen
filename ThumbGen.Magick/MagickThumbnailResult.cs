using ImageMagick;
using System.Drawing;

namespace ThumbGen.Magick
{
    public class MagickThumbnailResult : IThumbnailImage
    {
        public IMagickImage<byte> Image { get; init; }

        public MagickThumbnailResult(IMagickImage<byte> image)
        {
            Image = image;
        }

        public void SaveToFile(string filePath)
        {
            Image.Write(filePath);
        }

        public Task SaveToFileAsync(string filePath)
        {
            return Image.WriteAsync(filePath);
        }
    }
}
