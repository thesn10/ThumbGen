﻿using FFmpeg.AutoGen;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace ThumbGen.App;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var ffmpegPath = Environment.GetEnvironmentVariable("TG_FFMPEG_ROOT_PATH");
        if (!string.IsNullOrWhiteSpace(ffmpegPath))
            ffmpeg.RootPath = ffmpegPath;

        var rootCommand = new RootCommand
        {
            CommandLineOptions.StartTimeOption,
            CommandLineOptions.IntervalOption,
            CommandLineOptions.EndTimeOption,
            CommandLineOptions.FilenameOption,
            CommandLineOptions.FastModeOption,
            CommandLineOptions.WebVTTOption,
            CommandLineOptions.ImagePathOption,
            CommandLineOptions.FrameWidthOption,
            CommandLineOptions.FrameHeightOption,
            CommandLineOptions.ColumnsOption,
            CommandLineOptions.RowsOption,
            CommandLineOptions.BorderWidthOption,
            CommandLineOptions.BackgroundGradientStartOption,
            CommandLineOptions.BackgroundGradientEndOption
        };

        var thumbnailOptionsBinder = new ThumbnailOptionsBinder();

        rootCommand.SetHandler(RunCommand.Run, thumbnailOptionsBinder);

        return await rootCommand.InvokeAsync(args);
    }
}
