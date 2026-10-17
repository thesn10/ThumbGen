namespace ThumbGen
{
    public class NoVideoStreamException : ThumbGenException
    {
        public string Filename { get; }

        public NoVideoStreamException(string filename)
            : base($"'{filename}' does not contain a video stream")
        {
            Filename = filename;
        }
    }
}
