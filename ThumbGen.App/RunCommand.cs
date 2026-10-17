using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using ThumbGen.Builder;
using ThumbGen.Options;

namespace ThumbGen.App
{
    internal static class RunCommand
    {
        internal static async Task<int> Run(ParseResult parseResult, CancellationToken ct)
        {
            var loggerFactory = LoggerFactory.Create(
                builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information));
            
            var options = ThumbnailOptionsBinder.GetBoundValue(parseResult);
            
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
                .WithFFmpegVideoCapture()
                .WithLogging(loggerFactory);

            if (OperatingSystem.IsWindows())
                builder.UseSystemDrawingRenderer();
            else
                builder.UseMagickRenderer();

            builder.ConfigureRendering(renderingOpts);

            try
            {
                var thumbnailGenerator = builder.Build(options.InputFile.FullName);
                await thumbnailGenerator.ExecuteAsync(opts, ct);
            }
            catch (ThumbGenException ex)
            {
                await Console.Error.WriteLineAsync($"Error: {ex.Message}");
                return 1;
            }

            return 0;
        }
    }
}