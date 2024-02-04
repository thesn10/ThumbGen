using System;
using System.Drawing;
using System.Threading.Tasks;
using ThumbGen.Builder;
using ThumbGen.Options;

namespace ThumbGen.App
{
    internal static class RunCommand
    {
        internal static async Task<int> Run(ThumbnailOptions options)
        {
            var opts = new ThumbGenOptions()
                .WithStartTime(options.StartTime)
                .WithInterval(options.Interval)
                .WithEndTime(options.EndTime)
                .WithOutputFilename(options.OutputFile.FullName)
                .UseFastMode()
                .WithWebVTT(options.WebVTT.FullName, (imgPath, index) => options.ImagePath);

            var renderingOpts = new RenderingOptions()
                .WithFrameSize(options.FrameWidth, options.FrameHeight)
                .WithTiling(tileOptions =>
                {
                    tileOptions.Columns = options.Columns;
                    tileOptions.Rows = options.Rows;
                })
                .WithBorder(new Size(options.BorderWidth, options.BorderWidth));

            if (options.BackgroundGradientStart is not null && options.BackgroundGradientEnd is not null)
                renderingOpts.UseBackgroundGradient(
                    new LinearGradient(Color.FromName(options.BackgroundGradientStart), Color.FromName(options.BackgroundGradientEnd), 45));

            var builder = ThumbnailGeneratorBuilder
                .Create()
                .WithFFmpegVideoCapture();

            if (OperatingSystem.IsWindows())
                builder.UseSystemDrawingRenderer();
            else
                builder.UseMagickRenderer();

            var thumbnailGenerator = builder
                .ConfigureRendering(renderingOpts)
                .Build("video.webm");

            await thumbnailGenerator.ExecuteAsync(opts);

            return 0;
        }
    }
}