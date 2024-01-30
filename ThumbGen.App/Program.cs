using FFmpeg.AutoGen;
using ImageMagick;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Magick;
using ThumbGen.SystemDrawing;

namespace ThumbGen.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ffmpeg.RootPath = "D:\\Users\\sinan\\Downloads\\ffmpeg-master-latest-win64-gpl-shared\\bin";
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

            var input = Path.GetFullPath("./video.webm");//Path.GetFullPath("./video.webm");
            var opts = new ThumbGenOptions()
                .WithStartTime(TimeSpan.FromSeconds(60))
                //.WithEndTime(TimeSpan.FromMinutes(4))
                //.WithSize(new Size(1920, 1080))
                .WithRendering(opts => opts
                    .WithFrameSize(192 * 3, 108 * 3)
                    .WithTiling(options =>
                    {
                        options.Columns = 4;
                        options.Rows = 4;
                        options.AspectOverlap = true;
                    })
                    .PreserveFrameAspect(false)
                    .WithBorder(new Size(8, 8))
                    .UseBackgroundGradient(
                        new LinearGradient(Color.Cyan, Color.Blue, 45))
                    //.WithWatermark("./logo.svg", 605, 178, WatermarkPosition.Center)
                    .WithTimeCode(14f))
                    //.UseTimecodeBackgroundColor(Color.Black)
                .UseFastMode()
                .WithWebVTT("storyboard.vtt", (imagePath, index) => "/media/" + Path.GetFileName(imagePath));

            Stopwatch stopwatch = Stopwatch.StartNew();

            await SystemDrawingThumbnailGenerator
                .Create(input, opts.WithFilename(Path.GetFullPath("./thumbnail_systemdrawing.bmp")))
                .ExecuteAsync();

            stopwatch.Stop();
            Console.WriteLine("System.Drawing: " + stopwatch.Elapsed.ToString());

            stopwatch.Restart();

            await MagickThumbnailGenerator
                .Create(input, opts.WithFilename(Path.GetFullPath("./thumbnail_magick.jpg")))
                .ExecuteAsync();

            stopwatch.Stop();
            Console.WriteLine("Magick: " + stopwatch.Elapsed.ToString());
        }
    }
}
