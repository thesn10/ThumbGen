using EmguFFmpeg;
using System;
using System.Drawing;

namespace ThumbGen.FrameCapture;

public record Frame(VideoFrame VideoFrame, TimeSpan Timestamp, int Row, int Column)
{
    public string GetTimestampString() => Timestamp.ToString(@"hh\:mm\:ss");
}
