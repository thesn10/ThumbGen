using EmguFFmpeg;
using FFmpeg.AutoGen;
using System;

namespace ThumbGen.FrameCapture
{
    public class VideoFrameExtractor
    {
        private readonly MediaReader _reader;

        public bool FastMode { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Duration { get; private set; }

        public VideoFrameExtractor(string filename)
        {
            _reader = new MediaReader(filename);
            Duration = TimeSpan.FromMilliseconds(_reader.AVFormatContext.duration / 1000);
            for (var i = 0; i < _reader.Count; i++)
            {
                var codec = _reader[i].Codec;
                if (codec.AVCodecContext.codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    Width = codec.AVCodecContext.width;
                    Height = codec.AVCodecContext.height;
                }
            }
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
                    throw new Exception("Reached end of file finding vaild frame");
                }

                if (ret < 0 && ret != ffmpeg.AVERROR_EOF)
                {
                    throw new FFmpegException(ret);
                }

                var decoder = (MediaDecoder)_reader[packet.StreamIndex].Codec;
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
