using System;
using FFmpeg.AutoGen;
using DynamicallyLoadedBindings = FFmpeg.AutoGen.Bindings.DynamicallyLoaded.DynamicallyLoadedBindings;

namespace ThumbGen
{
    /// <summary>
    /// Loads and validates the native FFmpeg libraries ThumbGen depends on.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Initialize"/> once at application startup, before using ThumbGen. Consumers that use
    /// a different FFmpeg.AutoGen binding (e.g. statically linked) can skip it; ThumbGen only requires the
    /// bindings to be usable and of a compatible version, which <see cref="EnsureInitialized"/> verifies.
    /// </remarks>
    public static class FFmpegBindings
    {
        /// <summary>
        /// Loads the FFmpeg libraries with <c>FFmpeg.AutoGen.Bindings.DynamicallyLoaded</c> and verifies them.
        /// </summary>
        /// <param name="librariesPath">
        /// Directory containing the FFmpeg shared libraries. When null or empty, the default library
        /// search locations of the operating system are used.
        /// </param>
        /// <exception cref="InvalidOperationException">The libraries could not be loaded or have an incompatible version.</exception>
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
        /// Verifies that the FFmpeg bindings are initialized and match the version ThumbGen was built for.
        /// </summary>
        /// <exception cref="InvalidOperationException">The bindings are not usable.</exception>
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
                // Uninitialized bindings leave the function delegates null; bindings that could not find the
                // libraries replace them with stubs that throw NotSupportedException.
                throw new InvalidOperationException(
                    "FFmpeg bindings are not initialized or the FFmpeg libraries could not be found. " +
                    $"Call {nameof(FFmpegBindings)}.{nameof(Initialize)}(librariesPath) at startup " +
                    "(or set up your own FFmpeg.AutoGen bindings) and make sure FFmpeg " +
                    $"{ExpectedFFmpegVersion} shared libraries are available.", ex);
            }

            var major = (int)(version >> 16);
            if (major != ffmpeg.LIBAVFORMAT_VERSION_MAJOR)
            {
                throw new InvalidOperationException(
                    $"Incompatible FFmpeg libraries: found libavformat major version {major}, " +
                    $"but ThumbGen requires {ffmpeg.LIBAVFORMAT_VERSION_MAJOR} (FFmpeg {ExpectedFFmpegVersion}).");
            }
        }

        private const string ExpectedFFmpegVersion = "8.1.x";
    }
}
