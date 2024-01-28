using EmguFFmpeg;
using System;
using System.Drawing;

namespace ThumbGen.FrameCapture;

public record Frame(VideoFrame VideoFrame, TimeSpan Timestamp)
{
    public string GetTimestampString() => Timestamp.ToString(@"hh\:mm\:ss");
}
