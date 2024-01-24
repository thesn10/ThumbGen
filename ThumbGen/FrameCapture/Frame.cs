using System;
using System.Drawing;

namespace ThumbGen.FrameCapture;

public record Frame(Bitmap Bitmap, TimeSpan Timestamp, int Row, int Column)
{
    public string GetTimestampString() => Timestamp.ToString(@"hh\:mm\:ss");
}
