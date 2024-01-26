using FFmpeg.AutoGen;
using ImageMagick;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using ThumbGen.Builder;
using ThumbGen.FrameCapture;
using ThumbGen.Magick;
using ThumbGen.SystemDrawing;

namespace ThumbGen.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ffmpeg.RootPath = "D:\\Users\\sinan\\Downloads\\ffmpeg-master-latest-win64-gpl-shared\\bin";
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

            var input = Path.GetFullPath("./video.webm");//Path.GetFullPath("./video.webm");
            var opts = new ThumbGenOptions(input)
                //.WithStartTime(TimeSpan.FromSeconds(60))
                //.WithEndTime(TimeSpan.FromMinutes(4))
                .WithSize(new Size(1920, 1080))
                .WithTiling(options =>
                {
                    options.Columns = 4;
                    options.Rows = 4;
                    options.AspectOverlap = true;
                })
                .PreserveFrameAspect(false)
                .WithBorder(new Size(8, 8), true)
                .UseBackgroundGradient(
                    new LinearGradient(Color.Cyan, Color.Blue, 45))
                //.WithTimeCode(14f)
                //.UseTimecodeBackgroundColor(Color.Black)
                .UseFastMode();

            Stopwatch stopwatch = Stopwatch.StartNew();

            var output = Path.GetFullPath("./thumbnail.bmp");
            var thumbnail = SystemDrawingThumbnailGenerator
                .Create(input, opts)
                .GenerateThumbnail();

            stopwatch.Stop();
            Console.WriteLine("System.Drawing: " + stopwatch.Elapsed.ToString());

            thumbnail.SaveToFile(output);

            stopwatch.Restart();

            var output2 = Path.GetFullPath("./thumbnail2.jpg");
            var thumbnail2 = MagickThumbnailGenerator
                .Create(input, opts)
                .GenerateThumbnail();

            stopwatch.Stop();
            Console.WriteLine("Magick: " + stopwatch.Elapsed.ToString());

            thumbnail.SaveToFile(output2);


            var color1 = MagickColorUtil.FromColor(Color.Black);
            var color2 = MagickColorUtil.FromColor(Color.White);

            var settings = new MagickReadSettings()
            {
                Width = 1920,
                Height = 1080,
            };
            settings.SetDefine(MagickFormat.Gradient, "angle", 45);

            var gradientTest = $"gradient:{color1}-{color2}";
            Console.WriteLine(gradientTest);
            //var gradientTest2 = $"gradient:#FFFFFFFFFFFFFFFF-#0000000000000000";
            //Console.WriteLine(gradientTest2);
            var image2 = new MagickImage(gradientTest, settings);
            image2.Write(Path.GetFullPath("./gradient2.jpg"));
        }
    }
}
