using System;

namespace ThumbGen
{
    public class VideoDecodeException : ThumbGenException
    {
        public VideoDecodeException(Exception innerException)
            : base($"Could not decode video: {innerException.Message}", innerException)
        {
        }
    }
}
