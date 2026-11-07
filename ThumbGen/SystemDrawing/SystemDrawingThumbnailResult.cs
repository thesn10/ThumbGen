using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace ThumbGen.SystemDrawing;

/// <summary>
/// Thumbnail produced by the System.Drawing renderer. Cast <see cref="ThumbnailRenderResult.Image"/> to this type
/// to access the underlying <see cref="System.Drawing.Bitmap"/> for custom export.
/// </summary>
#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
public class SystemDrawingThumbnailResult(Bitmap bitmap) : IThumbnailImage
{
    /// <summary>
    /// The rendered bitmap. It is owned by this result; clone it if it must outlive the result.
    /// </summary>
    public Bitmap Bitmap { get; } = bitmap;

    public void SaveToStream(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg) =>
        Bitmap.Save(stream, format == ThumbnailImageFormat.Png ? ImageFormat.Png : ImageFormat.Jpeg);

    public Task SaveToStreamAsync(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg) =>
        Task.Run(() => SaveToStream(stream, format));

    public void SaveToFile(string filePath) => Bitmap.Save(filePath);

    public Task SaveToFileAsync(string filePath) => Task.Run(() => Bitmap.Save(filePath));
}
