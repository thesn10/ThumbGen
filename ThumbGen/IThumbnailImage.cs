using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen
{
    public record ThumbnailFrameMetadata(TimeSpan Timestamp, int X, int Y, int Width, int Height);
    public record ThumbnailRenderResult(IThumbnailImage Image, IReadOnlyList<ThumbnailFrameMetadata> FrameMetadata);

    public interface IThumbnailImage
    {
        void SaveToFile(string filePath);
        Task SaveToFileAsync(string filePath);
    }
}
