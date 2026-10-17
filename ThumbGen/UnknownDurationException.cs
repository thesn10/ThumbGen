namespace ThumbGen
{
    public class UnknownDurationException : ThumbGenException
    {
        public string Filename { get; }

        public UnknownDurationException(string filename)
            : base($"The duration of '{filename}' is unknown")
        {
            Filename = filename;
        }
    }
}
