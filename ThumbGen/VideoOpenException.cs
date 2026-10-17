using System;

namespace ThumbGen
{
    public class VideoOpenException : ThumbGenException
    {
        public string Filename { get; }

        public VideoOpenException(string filename, Exception innerException)
            : base($"Could not open '{filename}': {innerException.Message}", innerException)
        {
            Filename = filename;
        }
    }
}
