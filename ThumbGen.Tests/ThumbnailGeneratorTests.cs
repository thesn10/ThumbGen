using System.Drawing;

namespace ThumbGen.Tests;

public class ThumbnailGeneratorTests
{
    private static RenderingOptions SmallTiling() => new RenderingOptions()
        .WithFrameSize(-1, 108)
        .WithTiling(options =>
        {
            options.Columns = 4;
            options.Rows = 4;
        });

    private static ThumbGenOptions Options(TempDirectory dir, string outputName) => new ThumbGenOptions()
        .WithStartTime(TimeSpan.FromSeconds(60))
        .WithInterval(TimeSpan.FromSeconds(4))
        .WithEndTime(TimeSpan.FromMinutes(4))
        .WithOutputFilename(dir.File(outputName))
        .UseFastMode()
        .WithWebVTT(dir.File("storyboard.vtt"), (imagePath, index) => "/media/" + Path.GetFileName(imagePath));

    [WindowsFact]
    public async Task SystemDrawingRenderer_WritesThumbnailAndWebVTT()
    {
        using var dir = new TempDirectory();

        await ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseSystemDrawingRenderer()
            .ConfigureRendering(SmallTiling())
            .Build(VideoConstants.Video_360p_MP4())
            .ExecuteAsync(Options(dir, "thumbnail.bmp"));

        Assert.True(new FileInfo(dir.File("thumbnail.bmp")).Length > 0);
        Assert.StartsWith("WEBVTT", await File.ReadAllTextAsync(dir.File("storyboard.vtt")));
    }

    [Fact]
    public async Task MagickRenderer_WritesThumbnailAndWebVTT()
    {
        using var dir = new TempDirectory();

        await ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseMagickRenderer()
            .ConfigureRendering(SmallTiling())
            .Build(VideoConstants.Video_360p_MP4())
            .ExecuteAsync(Options(dir, "thumbnail.jpg"));

        Assert.True(new FileInfo(dir.File("thumbnail.jpg")).Length > 0);

        var vtt = await File.ReadAllTextAsync(dir.File("storyboard.vtt"));
        Assert.StartsWith("WEBVTT", vtt);
        Assert.Contains("/media/thumbnail.jpg#xywh=", vtt);
    }

    [Fact]
    public async Task MagickRenderer_WithTimeCodeAndStyling_WritesThumbnail()
    {
        using var dir = new TempDirectory();
        var renderingOpts = SmallTiling()
            .WithBorder(new Size(8, 8))
            .UseBackgroundGradient(new LinearGradient(Color.Cyan, Color.Blue, 45))
            .WithTimeCode(14f)
            .UseTimeCodeBackgroundColor(Color.Black)
            .UseTimeCodeColor(Color.White);

        await ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseMagickRenderer()
            .ConfigureRendering(renderingOpts)
            .Build(VideoConstants.Video_360p_MP4())
            .ExecuteAsync(Options(dir, "thumbnail.jpg"));

        Assert.True(new FileInfo(dir.File("thumbnail.jpg")).Length > 0);
    }

    [Fact]
    public async Task Generator_CanBeExecutedRepeatedly()
    {
        using var dir = new TempDirectory();
        var generator = ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseMagickRenderer()
            .ConfigureRendering(SmallTiling())
            .Build(VideoConstants.Video_360p_MP4());

        await generator.ExecuteAsync(Options(dir, "first.jpg"));
        await generator.ExecuteAsync(Options(dir, "second.jpg"));

        Assert.True(File.Exists(dir.File("first.jpg")));
        Assert.True(File.Exists(dir.File("second.jpg")));
    }

    [Fact]
    public void Build_WithMissingFile_ThrowsVideoOpenException()
    {
        var builder = ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseMagickRenderer();

        var ex = Assert.Throws<VideoOpenException>(() => builder.Build("does-not-exist.mp4"));
        Assert.Equal("does-not-exist.mp4", ex.Filename);
    }
}
