using System;

namespace ThumbGen
{
    public class ThumbGenException : Exception
    {
        public ThumbGenException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
