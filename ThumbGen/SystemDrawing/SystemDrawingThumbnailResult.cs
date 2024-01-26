using System.Drawing;
using System.Runtime.Versioning;

namespace ThumbGen.SystemDrawing
{
    [SupportedOSPlatform("windows")]
    internal class SystemDrawingThumbnailResult : IThumbnailResult
    {
        private readonly Bitmap _bitmap;

        public SystemDrawingThumbnailResult(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public void SaveToFile(string filePath)
        {
            _bitmap.Save(filePath);
        }
    }
}
