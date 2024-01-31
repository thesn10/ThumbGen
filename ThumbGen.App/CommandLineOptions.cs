using System;
using System.CommandLine;
using System.IO;

namespace ThumbGen.App;

public static class CommandLineOptions
{
    public static Option<TimeSpan> StartTimeOption { get; } = 
        new Option<TimeSpan>("--start-time", () => TimeSpan.FromSeconds(60), "Start time");
    public static Option<TimeSpan> IntervalOption { get; } = 
        new Option<TimeSpan>("--interval", () => TimeSpan.FromSeconds(5), "Interval");
    public static Option<TimeSpan> EndTimeOption { get; } = 
        new Option<TimeSpan>("--end-time", () => TimeSpan.FromMinutes(4), "End time");
    public static Option<string> FilenameOption { get; } = 
        new Option<string>("--filename", () => Path.GetFullPath("./thumbnail_systemdrawing.bmp"), "Output filename");
    public static Option<bool> FastModeOption { get; } = 
        new Option<bool>("--fast-mode", "Use fast mode");
    public static Option<FileInfo> WebVTTOption { get; } = 
        new Option<FileInfo>("--webvtt", () => new FileInfo("./storyboard.vtt"), "WebVTT file");
    public static Option<string> ImagePathOption { get; } = 
        new Option<string>("--image-path", () => "/media/" + Path.GetFileName(Path.GetFullPath("./thumbnail_systemdrawing.bmp")), "Image path for WebVTT");
    public static Option<int> FrameWidthOption { get; } = 
        new Option<int>("--frame-width", () => 192, "Frame width");
    public static Option<int> FrameHeightOption { get; } = 
        new Option<int>("--frame-height", () => 108, "Frame height");
    public static Option<int> ColumnsOption { get; } = 
        new Option<int>("--columns", () => 4, "Columns for tiling");
    public static Option<int> RowsOption { get; } = 
        new Option<int>("--rows", () => 4, "Rows for tiling");
    public static Option<int> BorderWidthOption { get; } = 
        new Option<int>("--border-width", () => 8, "Border width");
    public static Option<string> BackgroundGradientStartOption { get; } = 
        new Option<string>("--background-gradient-start", "Background gradient start color");
    public static Option<string> BackgroundGradientEndOption { get; } = 
        new Option<string>("--background-gradient-end", "Background gradient end color");
}
