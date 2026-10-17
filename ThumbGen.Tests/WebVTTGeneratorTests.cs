using System.Text;

namespace ThumbGen.Tests;

public class WebVTTGeneratorTests
{
    private static ThumbnailFrameMetadata[] Frames() =>
    [
        new(TimeSpan.FromSeconds(0), 0, 0, 192, 108),
        new(TimeSpan.FromSeconds(5), 192, 0, 192, 108),
        new(TimeSpan.FromSeconds(10), 384, 0, 192, 108),
    ];

    private static async Task<string> Generate(string? header, TimeSpan duration, params (string Url, ThumbnailFrameMetadata[] Frames)[] batches)
    {
        var stream = new MemoryStream();
        var generator = await WebVTTGenerator.CreateAsync(stream, duration, header);

        foreach (var (url, frames) in batches)
            await generator.AddCuesAsync(url, frames);

        await generator.FinishAsync();
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    [Fact]
    public async Task Output_StartsWithWebVttHeader()
    {
        var vtt = await Generate("storyboard", TimeSpan.FromSeconds(15));

        Assert.StartsWith("WEBVTT storyboard\n\n", vtt);
    }

    [Fact]
    public async Task Output_WithoutTextHeader_HasBareSignature()
    {
        var vtt = await Generate(null, TimeSpan.FromSeconds(15));

        Assert.Equal("WEBVTT\n\n", vtt);
    }

    [Fact]
    public async Task Cues_UseNextFrameTimestampAsEnd_AndDurationForLastFrame()
    {
        var vtt = await Generate(null, TimeSpan.FromSeconds(15), ("/media/a.jpg", Frames()));

        Assert.Contains("00:00:00.000 --> 00:00:05.000\n/media/a.jpg#xywh=0,0,192,108", vtt);
        Assert.Contains("00:00:05.000 --> 00:00:10.000\n/media/a.jpg#xywh=192,0,192,108", vtt);
        Assert.Contains("00:00:10.000 --> 00:00:15.000\n/media/a.jpg#xywh=384,0,192,108", vtt);
    }

    [Fact]
    public async Task Cues_AcrossMultipleBatches_ContinueSeamlessly()
    {
        var first = Frames()[..2];
        var second = new ThumbnailFrameMetadata[] { new(TimeSpan.FromSeconds(10), 0, 0, 192, 108) };

        var vtt = await Generate(null, TimeSpan.FromSeconds(15), ("/media/a.jpg", first), ("/media/b.jpg", second));

        Assert.Contains("00:00:05.000 --> 00:00:10.000\n/media/a.jpg#xywh=192,0,192,108", vtt);
        Assert.Contains("00:00:10.000 --> 00:00:15.000\n/media/b.jpg#xywh=0,0,192,108", vtt);
    }

    [Fact]
    public async Task CreateAsync_WithFilename_WritesFile()
    {
        using var dir = new TempDirectory();

        var generator = await WebVTTGenerator.CreateAsync(dir.File("out.vtt"), TimeSpan.FromSeconds(5));
        await generator.FinishAsync();

        Assert.Equal("WEBVTT\n\n", await File.ReadAllTextAsync(dir.File("out.vtt")));
    }
}
