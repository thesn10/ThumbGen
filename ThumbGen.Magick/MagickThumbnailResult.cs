using ImageMagick;

namespace ThumbGen.Magick
{
    public class MagickThumbnailResult : IThumbnailResult
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
    }
}
