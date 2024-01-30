using ImageMagick;
using System.Drawing;

namespace ThumbGen.Magick
{
    public class MagickThumbnailResult : IThumbnailImage
    {
        private readonly IMagickImage<byte> _image;

        public MagickThumbnailResult(IMagickImage<byte> image)
        {
            _image = image;
        }

        public void SaveToFile(string filePath)
        {
            _image.Write(filePath);
        }

        public Task SaveToFileAsync(string filePath)
        {
            return _image.WriteAsync(filePath);
        }
    }
}
