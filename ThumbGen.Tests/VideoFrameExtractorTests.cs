using System.Text;
using ThumbGen.FrameCapture;

namespace ThumbGen.Tests;

public class VideoFrameExtractorTests
{
    [Fact]
    public void Constructor_ReadsVideoProperties()
    {
        using var extractor = new VideoFrameExtractor(VideoConstants.Video_360p_MP4());

        Assert.Equal(360, extractor.Height);
        Assert.True(extractor.Width > 0);
        Assert.True(extractor.Duration > TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GetAtTimestamp_ReturnsFrameAtOrAfterRequestedTime()
    {
        using var extractor = new VideoFrameExtractor(VideoConstants.Video_360p_MP4());
        var requested = TimeSpan.FromSeconds(60);

        var (frame, timestamp) = extractor.GetAtTimestamp(requested);

        Assert.True(timestamp >= requested);
        Assert.Equal(extractor.Width, frame.Width);
        Assert.Equal(extractor.Height, frame.Height);
    }

    [Fact]
    public void GetAtTimestamp_InFastMode_ReturnsFirstDecodedFrame()
    {
        using var extractor = new VideoFrameExtractor(VideoConstants.Video_360p_MP4()) { FastMode = true };

        var (_, timestamp) = extractor.GetAtTimestamp(TimeSpan.FromSeconds(60));

        Assert.True(timestamp <= TimeSpan.FromSeconds(60) + TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void GetAtTimestamp_BeyondEndOfVideo_ThrowsVideoDecodeException()
    {
        using var extractor = new VideoFrameExtractor(VideoConstants.Video_360p_MP4());

        Assert.Throws<VideoDecodeException>(() => extractor.GetAtTimestamp(extractor.Duration + TimeSpan.FromHours(1)));
    }

    [Fact]
    public void Constructor_WithMissingFile_ThrowsVideoOpenException()
    {
        var ex = Assert.Throws<VideoOpenException>(() => new VideoFrameExtractor("does-not-exist.mp4"));

        Assert.Equal("does-not-exist.mp4", ex.Filename);
        Assert.NotNull(ex.InnerException);
    }

    [Fact]
    public void Constructor_WithInvalidFile_ThrowsVideoOpenException()
    {
        using var dir = new TempDirectory();
        File.WriteAllText(dir.File("garbage.mp4"), "not a video");

        Assert.Throws<VideoOpenException>(() => new VideoFrameExtractor(dir.File("garbage.mp4")));
    }

    [Fact]
    public void Constructor_WithAudioOnlyFile_ThrowsNoVideoStreamException()
    {
        using var dir = new TempDirectory();
        File.WriteAllBytes(dir.File("silence.wav"), SilentWav(sampleRate: 8000, seconds: 1));

        var ex = Assert.Throws<NoVideoStreamException>(() => new VideoFrameExtractor(dir.File("silence.wav")));

        Assert.Equal(dir.File("silence.wav"), ex.Filename);
    }

    [Fact]
    public void Constructor_WithUnknownDuration_ThrowsUnknownDurationException()
    {
        using var dir = new TempDirectory();
        File.WriteAllBytes(dir.File("unknown.wav"), WavHeaderWithoutData(sampleRate: 8000));

        var ex = Assert.Throws<UnknownDurationException>(() => new VideoFrameExtractor(dir.File("unknown.wav")));

        Assert.Equal(dir.File("unknown.wav"), ex.Filename);
    }

    [Fact]
    public void Dispose_ReleasesVideoFile()
    {
        using var dir = new TempDirectory();
        File.Copy(VideoConstants.Video_360p_MP4(), dir.File("video.mp4"));
        var extractor = new VideoFrameExtractor(dir.File("video.mp4"));

        extractor.Dispose();

        File.Delete(dir.File("video.mp4"));
    }

    [Fact]
    public void FrameTimestampString_IsFormattedAsClockTime()
    {
        using var extractor = new VideoFrameExtractor(VideoConstants.Video_360p_MP4());
        var (videoFrame, _) = extractor.GetAtTimestamp(TimeSpan.FromSeconds(1));

        var frame = new Frame(videoFrame, new TimeSpan(1, 2, 3));

        Assert.Equal("01:02:03", frame.GetTimestampString());
    }

    private static byte[] WavHeaderWithoutData(int sampleRate)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.ASCII);
        writer.Write("RIFF"u8);
        writer.Write(36);
        writer.Write("WAVEfmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(sampleRate);
        writer.Write(sampleRate);
        writer.Write((short)1);
        writer.Write((short)8);
        writer.Write("data"u8);
        writer.Write(0);
        return stream.ToArray();
    }

    private static byte[] SilentWav(int sampleRate, int seconds)
    {
        var dataLength = sampleRate * seconds;
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.ASCII);
        writer.Write("RIFF"u8);
        writer.Write(36 + dataLength);
        writer.Write("WAVEfmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(sampleRate);
        writer.Write(sampleRate);
        writer.Write((short)1);
        writer.Write((short)8);
        writer.Write("data"u8);
        writer.Write(dataLength);
        writer.Write(Enumerable.Repeat((byte)128, dataLength).ToArray());
        return stream.ToArray();
    }
}
