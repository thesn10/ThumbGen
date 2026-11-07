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

    public int Width => Bitmap.Width;

    public int Height => Bitmap.Height;

    public void SaveToStream(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg) =>
        Bitmap.Save(stream, ToImageFormat(format));

    public Task SaveToStreamAsync(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg) =>
        Task.Run(() => SaveToStream(stream, format));

    public void SaveToFile(string filePath, ThumbnailImageFormat? format = null)
    {
        var resolvedFormat = format ?? FormatFromExtension(filePath);
        if (resolvedFormat is { } knownFormat)
            Bitmap.Save(filePath, ToImageFormat(knownFormat));
        else
            Bitmap.Save(filePath);
    }

    public Task SaveToFileAsync(string filePath, ThumbnailImageFormat? format = null) =>
        Task.Run(() => SaveToFile(filePath, format));

    private static ImageFormat ToImageFormat(ThumbnailImageFormat format) =>
        format == ThumbnailImageFormat.Png ? ImageFormat.Png : ImageFormat.Jpeg;

    private static ThumbnailImageFormat? FormatFromExtension(string filePath) =>
        Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => ThumbnailImageFormat.Jpeg,
            ".png" => ThumbnailImageFormat.Png,
            _ => null
        };
}
