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
            .WithFilename(Path.GetFullPath("./thumbnail_systemdrawing.bmp"))
            .UseFastMode()
            .WithWebVTT("storyboard.vtt", (imagePath, index) => "/media/" + Path.GetFileName(imagePath));

        var renderingOpts = new RenderingOptions()
            .WithFrameSize(192, 108)
            .WithTiling(options =>
            {
                options.Columns = 4;
                options.Rows = 4;
            })
            .WithBorder(new Size(8, 8))
            .UseBackgroundGradient(
                new LinearGradient(Color.Cyan, Color.Blue, 45))
            .WithWatermark("./logo.svg", 605, 178, WatermarkPosition.Center)
            .WithTimeCode(14f)
            .UseTimeCodeBackgroundColor(Color.Black)
            .PreserveFrameAspect(false)
            .UseAspectOverlap(true)
            .UseAspectOverlapColor(Color.Black)
            .UseTimeCodeColor(Color.White);

        var thumbnailGenerator = ThumbnailGeneratorBuilder
            .Create()
            .WithFFMpegVideoCapture(VideoConstants.Video_360p_MP4())
            .UseSystemDrawingRenderer(renderingOpts)
            // or
            //.UseMagickRenderer(renderingOpts)
            .Build(opts);

        await thumbnailGenerator.ExecuteAsync();

        await ThumbnailGeneratorBuilder
            .Create()
            .WithFFMpegVideoCapture(VideoConstants.Video_360p_MP4())
            .UseMagickRenderer(renderingOpts)
            .Build(opts.WithFilename(Path.GetFullPath("./thumbnail_magick.jpg")))
            .ExecuteAsync();
    }
}