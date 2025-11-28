using System;
using System.CommandLine;
using System.IO;

namespace ThumbGen.App;

public static class CommandLineOptions
{
    public static Option<TimeSpan> StartTimeOption { get; } = 
        new("--start-time")
        {
            Description = "Start time",
            DefaultValueFactory = (_) => TimeSpan.FromSeconds(60),
        };
    public static Option<TimeSpan> IntervalOption { get; } = 
        new("--interval")
        {
            Description = "Interval",
            DefaultValueFactory = (_) => TimeSpan.FromSeconds(5),
        };
    public static Option<TimeSpan> EndTimeOption { get; } = 
        new("--end-time")
        {
            Description = "End time",
            DefaultValueFactory = (_) => TimeSpan.FromMinutes(4),
        };

    public static Argument<FileInfo> InputFileArgument { get; } =
        new("input-file")
        {
            Description = "Input filename",
            DefaultValueFactory = (_) => new FileInfo("./thumbnail_systemdrawing.bmp"),
        };
    public static Argument<FileInfo> OutputFileArgument { get; } = 
        new("output-file")
        {
            Description = "Output filename",
            DefaultValueFactory = (_) => new FileInfo("./thumbnail_systemdrawing.bmp"),
        };

    public static Option<bool> FastModeOption { get; } =
        new Option<bool>("--fast-mode")
        {
            Description = "Use fast mode",
            DefaultValueFactory = (_) => false,
        };

    public static Option<FileInfo> WebVTTOption { get; } =
        new Option<FileInfo>("--webvtt")
        {
            Description = "WebVTT file",
            DefaultValueFactory = (_) => new FileInfo("./storyboard.vtt"),
        };
    public static Option<string> ImagePathOption { get; } = 
        new Option<string>("--image-path")
        {
            Description = "Image path for WebVTT",
            DefaultValueFactory = (_) => "/media/" + Path.GetFileName(Path.GetFullPath("./thumbnail_systemdrawing.bmp")),
        };
    public static Option<int> FrameWidthOption { get; } = 
        new Option<int>("--frame-width")
        {
            Description = "Frame width",
            DefaultValueFactory = (_) => 192,
        };
    public static Option<int> FrameHeightOption { get; } = 
        new Option<int>("--frame-height")
        {
            Description = "Frame height",
            DefaultValueFactory = (_) => 108,
        };
    public static Option<int> ColumnsOption { get; } = 
        new Option<int>("--columns")
        {
            Description = "Columns for tiling",
            DefaultValueFactory = (_) => 4,
        };
    public static Option<int> RowsOption { get; } = 
        new Option<int>("--rows")
        {
            Description = "Rows for tiling",
            DefaultValueFactory = (_) => 4,
        };
    public static Option<int> BorderWidthOption { get; } = 
        new("--border-width")
        {
            Description = "Border width",
            DefaultValueFactory = (_) => 8,
        };
    public static Option<string> BackgroundGradientStartOption { get; } = 
        new("--background-gradient-start")
        {
            Description = "Background gradient start color",
        };
    public static Option<string> BackgroundGradientEndOption { get; } = 
        new("--background-gradient-end")
        {
            Description = "Background gradient end color",
        };
}
