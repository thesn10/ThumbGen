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

ThumbGen relies on the native FFmpeg shared libraries (**FFmpeg 8.1.x**, i.e. `libavformat.so.62` / `avformat-62.dll` or the platform equivalent). They are not bundled; you are responsible for installing or shipping them.

FFmpeg.AutoGen's function bindings stay uninitialized until you load them. Call `FFmpegBindings.Initialize` once at application startup, before using ThumbGen:

```csharp
using ThumbGen;

// Directory containing the FFmpeg libraries. Pass null to use the OS default library search locations.
FFmpegBindings.Initialize("/your/path/to/ffmpeg/libraries/");
```

If the bindings are not initialized, the libraries cannot be found, or their version does not match, ThumbGen throws a `FFmpegBindingsException` describing the problem.

If you use your own bindings (e.g. `FFmpeg.AutoGen.Bindings.StaticallyLinked`) instead, skip `Initialize`; ThumbGen only checks that the bindings are usable and of a compatible version.

If you call `DynamicallyLoadedBindings.Initialize()` yourself, note that `FFmpeg.AutoGen.DynamicallyLoadedBindings` (legacy) and `FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings` clash when both namespaces are imported; use a `using` alias.

## Installation

To get started with ThumbGen, simply install the necessary NuGet packages:

```bash
dotnet add package ThumbGen
dotnet add package ThumbGen.Magick # if using ImageMagick renderer
```

Make sure to initialize the FFmpeg bindings as described in the [FFmpeg](#ffmpeg) section.

## License

This project is licensed under the [MIT License](LICENSE.md). Feel free to use and contribute to make ThumbGen even better!