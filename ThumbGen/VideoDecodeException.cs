using System;

namespace ThumbGen
{
    public class VideoDecodeException : ThumbGenException
    {
        public VideoDecodeException(string message)
            : base(message)
        {
        }

        public VideoDecodeException(Exception innerException)
            : base($"Could not decode video: {innerException.Message}", innerException)
        {
        }
    }
}
