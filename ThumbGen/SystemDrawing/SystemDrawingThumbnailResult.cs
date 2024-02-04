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
        public Bitmap Bitmap { get; init; }

        public SystemDrawingThumbnailResult(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public void SaveToFile(string filePath)
        {
            Bitmap.Save(filePath);
        }

        public Task SaveToFileAsync(string filePath)
        {
            return Task.Run(() => Bitmap.Save(filePath));
        }
    }
}
