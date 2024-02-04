# ThumbGen

ThumbGen is a customizable cross-platform, blazing-fast video thumbnail generator for .NET 8. This versatile tool allows you to choose between two rendering engines: System.Drawing (for Windows) or ImageMagick. With ThumbGen, you can effortlessly generate thumbnails for your videos with various options and configurations.

## Example Usage

```csharp
var thumbnailGenerator = ThumbnailGeneratorBuilder
    .Create()
    .WithFFmpegVideoCapture()
    .UseSystemDrawingRenderer()
    .ConfigureRendering(renderingOpts => renderingOpts
        .WithFrameSize(-1, 108)
        .WithTiling(options =>
        {
            options.Columns = 4;
            options.Rows = 4;
        })
        .WithBorder(new Size(8, 8))
        .UseBackgroundGradient(
            new LinearGradient(Color.Cyan, Color.Blue, 45))
        .WithTimeCode(14f)
        .UseTimeCodeBackgroundColor(Color.Black)
        .PreserveFrameAspect(false)
        .UseAspectOverlap(true)
        .UseAspectOverlapColor(Color.Black)
        .UseTimeCodeColor(Color.White))
    .Build("input-video.mp4");

var opts = new ThumbGenOptions()
    .WithStartTime(TimeSpan.FromSeconds(60))
    .WithInterval(TimeSpan.FromSeconds(5))
    .WithOutputFilename(Path.GetFullPath("./output.jpg"))
    .WithWebVTT("storyboard.vtt");

await thumbnailGenerator.ExecuteAsync(opts);
```
To Use magick renderer, just replace `UseSystemDrawingRenderer` with `UseMagickRenderer`

Feel free to customize the options and rendering settings based on your specific requirements.

## FFmpeg

ThumbGen relies on FFmpeg libraries for its functionality. Ensure you set `ffmpeg.RootPath` with the full path to the FFmpeg libraries for the proper execution of the tool.

```csharp
using FFmpeg.AutoGen;

ffmpeg.RootPath = "/your/path/to/ffmpeg/libraries/"
```

## Installation

To get started with ThumbGen, simply install the necessary NuGet packages:

```bash
dotnet add package ThumbGen
dotnet add package ThumbGen.Magick # if using ImageMagick renderer
```

Ensure that you also set the FFmpeg library path as mentioned in the dependencies.

## License

This project is licensed under the [MIT License](LICENSE.md). Feel free to use and contribute to make ThumbGen even better!