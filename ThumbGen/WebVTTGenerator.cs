using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThumbGen
{
    public class WebVTTGenerator
    {
        private readonly PipeWriter _output;
        private readonly TimeSpan _duration;

        internal WebVTTGenerator(PipeWriter output, TimeSpan duration)
        {
            _output = output;
            _duration = duration;
        }

        /// <param name="textHeader">
        /// An optional text header to the right of WEBVTT. You could use this to add a description to the file. 
        /// You may use anything in the text header except newlines or the string "-->".
        /// </param>
        public static Task<WebVTTGenerator> CreateAsync(string outputFileName, TimeSpan duration, string? textHeader = null, CancellationToken ct = default)
        {
            var fileStream = File.Open(outputFileName, FileMode.Truncate, FileAccess.Write);
            return CreateAsync(fileStream, duration, textHeader, ct);
        }

        /// <param name="textHeader">
        /// An optional text header to the right of WEBVTT. You could use this to add a description to the file. 
        /// You may use anything in the text header except newlines or the string "-->".
        /// </param>
        public static Task<WebVTTGenerator> CreateAsync(Stream outputStream, TimeSpan duration, string? textHeader = null, CancellationToken ct = default)
        {
            return CreateAsync(PipeWriter.Create(outputStream), duration, textHeader, ct);
        }

        /// <param name="textHeader">
        /// An optional text header to the right of WEBVTT. You could use this to add a description to the file. 
        /// You may use anything in the text header except newlines or the string "-->".
        /// </param>
        public static async Task<WebVTTGenerator> CreateAsync(PipeWriter output, TimeSpan duration, string? textHeader = null, CancellationToken ct = default)
        {
            output.Write("WEBVTT"u8);

            if (textHeader is not null)
            {
                // There must be at least one space after WEBVTT.
                output.Write(" "u8);
                Encoding.UTF8.GetBytes(textHeader.AsSpan(), output);
            }

            // A blank line, which is equivalent to two consecutive newlines.
            output.Write("\n\n"u8);

            await output.FlushAsync(ct);

            return new WebVTTGenerator(output, duration);
        }

        public async Task AddCuesAsync(
            string imageUrl,
            IReadOnlyList<ThumbnailFrameMetadata> frameMetadata,
            CancellationToken ct = default)
        {
            for (var i = 0; i < frameMetadata.Count; i++)
            {
                var frame = frameMetadata[i];
                var nextFrame = frameMetadata.ElementAtOrDefault(i + 1);

                var start = frame.Timestamp;
                var end = nextFrame?.Timestamp ?? _duration;

                var webvttSection = $"""

                    {start:hh\:mm\:ss\.fff} --> {end:hh\:mm\:ss\.fff}
                    {imageUrl}#xywh={frame.X},{frame.Y},{frame.Width},{frame.Height}

                    """;
                Encoding.UTF8.GetBytes(webvttSection.AsSpan(), _output);

                var result = await _output.FlushAsync(ct);

                if (result.IsCanceled || result.IsCompleted)
                {
                    return;
                }
            }
        }

        public ValueTask FinishAsync()
        {
            return _output.CompleteAsync();
        }
    }
}
