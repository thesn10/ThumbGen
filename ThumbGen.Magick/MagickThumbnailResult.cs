using ImageMagick;
using System.IO;
using System.Threading.Tasks;

namespace ThumbGen.Magick;

/// <summary>
/// Thumbnail produced by the Magick renderer. Cast <see cref="ThumbnailRenderResult.Image"/> to this type
/// to access the underlying <see cref="IMagickImage{T}"/> for custom export.
/// </summary>
public class MagickThumbnailResult(IMagickImage<byte> image) : IThumbnailImage
{
    /// <summary>
    /// The rendered image. It is owned by this result; clone it if it must outlive the result.
    /// </summary>
    public IMagickImage<byte> Image { get; } = image;

    public void SaveToStream(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg) =>
        Image.Write(stream, ToMagickFormat(format));

    public Task SaveToStreamAsync(Stream stream, ThumbnailImageFormat format = ThumbnailImageFormat.Jpeg) =>
        Image.WriteAsync(stream, ToMagickFormat(format));

    public void SaveToFile(string filePath, ThumbnailImageFormat? format = null)
    {
        if (format is { } explicitFormat)
            Image.Write(filePath, ToMagickFormat(explicitFormat));
        else
            Image.Write(filePath);
    }

    public Task SaveToFileAsync(string filePath, ThumbnailImageFormat? format = null) =>
        format is { } explicitFormat
            ? Image.WriteAsync(filePath, ToMagickFormat(explicitFormat))
            : Image.WriteAsync(filePath);

    private static MagickFormat ToMagickFormat(ThumbnailImageFormat format) => format switch
    {
        ThumbnailImageFormat.Png => MagickFormat.Png,
        _ => MagickFormat.Jpeg
    };
}
