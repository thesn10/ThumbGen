using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen
{
    public record ThumbnailFrameMetadata(TimeSpan Timestamp, int X, int Y, int Width, int Height);
    public record ThumbnailRenderResult(IThumbnailImage Image, IReadOnlyList<ThumbnailFrameMetadata> FrameMetadata);

    public enum ThumbnailImageFormat
    {
        Jpeg,
        Png
    }

    public interface IThumbnailImage
    {
        void SaveToStream(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg);
        Task SaveToStreamAsync(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg);
        void SaveToFile(string filePath);
        Task SaveToFileAsync(string filePath);
    }
}
