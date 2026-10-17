using System;

namespace ThumbGen
{
    public class FFmpegBindingsException : ThumbGenException
    {
        public FFmpegBindingsException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
