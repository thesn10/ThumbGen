using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace ThumbGen.SystemDrawing
{
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    internal class SystemDrawingThumbnailResult : IThumbnailImage
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

        public Task SaveToFileAsync(string filePath)
        {
            return Task.Run(() => _bitmap.Save(filePath));
        }
    }
}
