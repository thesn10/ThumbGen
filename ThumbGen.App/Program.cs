using FFmpeg.AutoGen;
using System;
using System.Drawing;
using System.IO;
using ThumbGen.Builder;

namespace ThumbGen.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ffmpeg.RootPath = "D:\\Users\\sinan\\Downloads\\ffmpeg-master-latest-win64-gpl-shared\\bin";
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

            var input = Path.GetFullPath("./video.webm");//Path.GetFullPath("./video.webm");
            var output = Path.GetFullPath("./thumbnail.bmp");
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
                .WithTimeCode(14f)
                //.UseTimecodeBackgroundColor(Color.Black)
                .UseFastMode();

            var thumbnailGenerator = new SystemDrawingThumbnailGenerator(new VideoFrameExtractor(input), opts);
            var bitmap = thumbnailGenerator.GenerateBitmap();

            bitmap.Save(output);
        }
    }
}
