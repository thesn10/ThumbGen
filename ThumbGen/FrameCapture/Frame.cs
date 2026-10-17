using System;
using FFmpegSharp;

namespace ThumbGen.FrameCapture;

public record Frame(MediaFrame VideoFrame, TimeSpan Timestamp) : IDisposable
{
    public void Dispose() => VideoFrame.Dispose();

    public string GetTimestampString() => Timestamp.ToString(@"hh\:mm\:ss");
}
