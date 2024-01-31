using System;
using System.IO;

namespace ThumbGen.App;

public class ThumbnailOptions
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan Interval { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Filename { get; set; }
    public bool FastMode { get; set; }
    public FileInfo WebVTT { get; set; }
    public string ImagePath { get; set; }
    public int FrameWidth { get; set; }
    public int FrameHeight { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public int BorderWidth { get; set; }
    public string BackgroundGradientStart { get; set; }
    public string BackgroundGradientEnd { get; set; }
    // Add other rendering options here
}
