using System;
using FFmpegSharp;

namespace ThumbGen.FrameCapture;

public record Frame(MediaFrame VideoFrame, TimeSpan Timestamp)
{
    public string GetTimestampString() => Timestamp.ToString(@"hh\:mm\:ss");
}
