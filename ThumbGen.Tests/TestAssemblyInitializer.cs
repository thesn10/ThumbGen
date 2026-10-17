using System.Runtime.CompilerServices;

namespace ThumbGen.Tests;

internal static class TestAssemblyInitializer
{
    [ModuleInitializer]
    public static void InitializeFFmpeg() =>
        FFmpegBindings.Initialize(Environment.GetEnvironmentVariable("TG_FFMPEG_ROOT_PATH"));
}
