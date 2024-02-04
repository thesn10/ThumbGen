using FFmpeg.AutoGen;
using System.Drawing;

namespace ThumbGen.Tests;

public class ThumbnailGeneratorTests
{
    [Fact]
    public async Task ThumbnailGenerator_DoesNotThrowAnyExeption()
    {
        var ffmpegPath = Environment.GetEnvironmentVariable("TG_FFMPEG_ROOT_PATH");
        if (!string.IsNullOrWhiteSpace(ffmpegPath))
            ffmpeg.RootPath = ffmpegPath;

        var opts = new ThumbGenOptions()
            .WithStartTime(TimeSpan.FromSeconds(60))
            .WithInterval(TimeSpan.FromSeconds(5))
            .WithEndTime(TimeSpan.FromMinutes(4))
            .WithOutputFilename(Path.GetFullPath("./thumbnail_systemdrawing.bmp"))
            .UseFastMode()
            .WithWebVTT("storyboard.vtt", (imagePath, index) => "/media/" + Path.GetFileName(imagePath));

        var renderingOpts = new RenderingOptions()
            .WithFrameSize(-1, 108)
            .WithTiling(options =>
            {
                options.Columns = 4;
                options.Rows = 4;
            })
            .WithBorder(new Size(8, 8))
            .UseBackgroundGradient(
                new LinearGradient(Color.Cyan, Color.Blue, 45))
            .WithTimeCode(14f)
            .UseTimeCodeBackgroundColor(Color.Black)
            .PreserveFrameAspect(false)
            .UseAspectOverlap(true)
            .UseAspectOverlapColor(Color.Black)
            .UseTimeCodeColor(Color.White);

        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException();

        var thumbnailGenerator = ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseSystemDrawingRenderer()
            .ConfigureRendering(renderingOpts)
            .Build(VideoConstants.Video_360p_MP4());

        await thumbnailGenerator.ExecuteAsync(opts);

        await ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseMagickRenderer()
            .ConfigureRendering(renderingOpts)
            .Build(VideoConstants.Video_360p_MP4())
            .ExecuteAsync(opts.WithOutputFilename(Path.GetFullPath("./thumbnail_magick.jpg")));
    }

    [Fact]
    public async Task Parralel_Execution()
    {
        var ffmpegPath = Environment.GetEnvironmentVariable("TG_FFMPEG_ROOT_PATH");
        if (!string.IsNullOrWhiteSpace(ffmpegPath))
            ffmpeg.RootPath = ffmpegPath;

        var opts = new ThumbGenOptions()
            .WithStartTime(TimeSpan.FromSeconds(60))
            //.WithInterval(TimeSpan.FromSeconds(5))
            .WithEndTime(TimeSpan.FromMinutes(4))
            .UseFastMode()
            .WithWebVTT("storyboard.vtt", (imagePath, index) => "/media/" + Path.GetFileName(imagePath));

        var renderingOpts = new RenderingOptions()
            .WithFrameSize(-1, 108)
            .WithTiling(options =>
            {
                options.Columns = 4;
                options.Rows = 4;
            })
            .WithBorder(new Size(8, 8))
            .UseBackgroundGradient(
                new LinearGradient(Color.Cyan, Color.Blue, 45))
            .WithTimeCode(14f)
            .UseTimeCodeBackgroundColor(Color.Black)
            .PreserveFrameAspect(false)
            .UseAspectOverlap(true)
            .UseAspectOverlapColor(Color.Black)
            .UseTimeCodeColor(Color.White);

        var generator = ThumbnailGeneratorBuilder
            .Create()
            .WithFFmpegVideoCapture()
            .UseMagickRenderer()
            .ConfigureRendering(renderingOpts)
            .Build(VideoConstants.Video_360p_MP4());

        await generator.ExecuteAsync(opts.WithOutputFilename(Path.GetFullPath("./thumbnail_parralel1.jpg")));
        await generator.ExecuteAsync(opts.WithOutputFilename(Path.GetFullPath("./thumbnail_parralel2.jpg")));
    }
}