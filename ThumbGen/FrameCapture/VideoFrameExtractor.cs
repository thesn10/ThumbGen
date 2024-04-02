using EmguFFmpeg;
using FFmpeg.AutoGen;
using System;

namespace ThumbGen.FrameCapture
{
    public class VideoFrameExtractor
    {
        private readonly MediaReader _reader;

        public bool FastMode { get; set; }

        private readonly MediaStream _stream;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Duration { get; private set; }

        public VideoFrameExtractor(string filename)
        {
            _reader = new MediaReader(filename);
            Duration = TimeSpan.FromMilliseconds(_reader.AVFormatContext.duration / 1000);
            for (var i = 0; i < _reader.Count; i++)
            {
                var stream = _reader[i];
                
                if ((stream.Stream.disposition & ffmpeg.AV_DISPOSITION_ATTACHED_PIC) == ffmpeg.AV_DISPOSITION_ATTACHED_PIC)
                {
                    continue;
                }

                var codec = stream.Codec;
                if (codec.AVCodecContext.codec_type != AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    continue;
                }

                _stream = stream;
                Width = codec.AVCodecContext.width;
                Height = codec.AVCodecContext.height;
            }

            if (_stream is null)
                throw new ArgumentException("No video stream found");
        }

        public (VideoFrame, TimeSpan) GetAtTimestamp(TimeSpan ts)
        {
            _reader.Seek(ts);

            using var packet = new MediaPacket();

            while (true)
            {
                var ret = _reader.ReadPacket(packet);
                if (ret == ffmpeg.AVERROR_EOF)
                {
                    throw new Exception("Reached end of file not finding vaild frame");
                }
                else if (ret < 0)
                {
                    throw new FFmpegException(ret);
                }

                if (packet.StreamIndex != _stream.Index)
                {
                    continue;
                }

                var decoder = (MediaDecoder)_stream.Codec;
                ret = decoder.SendPacket(packet);
                if (ret < 0)
                {
                    throw new FFmpegException(ret);
                }

                var frame = new VideoFrame();

                while (decoder.ReceiveFrame(frame) >= 0)
                {
                    var frameTs = _reader[packet.StreamIndex].ToTimeSpan(frame.Pts);
                    if (FastMode || frameTs >= ts)
                    {
                        return (frame, frameTs);
                    }
                }
            }
        }
    }
}
