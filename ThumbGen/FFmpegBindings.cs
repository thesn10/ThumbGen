using System;
using FFmpeg.AutoGen;
using DynamicallyLoadedBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace ThumbGen
{
    public static class FFmpegBindings
    {
        /// <summary>
        /// Loads the FFmpeg libraries from <paramref name="librariesPath"/>, or from the default
        /// library search locations when it is null or empty.
        /// </summary>
        /// <exception cref="FFmpegBindingsException">The libraries could not be loaded or have an incompatible version.</exception>
        public static void Initialize(string? librariesPath = null)
        {
            if (!string.IsNullOrWhiteSpace(librariesPath))
            {
                ffmpeg.RootPath = librariesPath;
                DynamicallyLoadedBindings.LibrariesPath = librariesPath;
            }

            DynamicallyLoadedBindings.Initialize();

            EnsureInitialized();
        }

        /// <summary>
        /// Verifies that usable FFmpeg bindings of the expected version are loaded.
        /// </summary>
        /// <exception cref="FFmpegBindingsException">The bindings are not usable.</exception>
        public static void EnsureInitialized()
        {
            uint version;
            try
            {
                version = ffmpeg.avformat_version();
            }
            catch (Exception ex) when (ex is NullReferenceException
                                           or NotSupportedException
                                           or DllNotFoundException
                                           or EntryPointNotFoundException)
            {
                throw new FFmpegBindingsException(
                    "FFmpeg bindings are not initialized or the FFmpeg libraries could not be found. " +
                    $"Call {nameof(FFmpegBindings)}.{nameof(Initialize)}(librariesPath) at startup " +
                    $"and make sure the FFmpeg {ExpectedFFmpegVersion} shared libraries are available.", ex);
            }

            var major = (int)(version >> 16);
            if (major != ffmpeg.LIBAVFORMAT_VERSION_MAJOR)
            {
                throw new FFmpegBindingsException(
                    $"Incompatible FFmpeg libraries: found libavformat major version {major}, " +
                    $"but ThumbGen requires {ffmpeg.LIBAVFORMAT_VERSION_MAJOR} (FFmpeg {ExpectedFFmpegVersion}).");
            }
        }

        private const string ExpectedFFmpegVersion = "8.1.x";
    }
}
