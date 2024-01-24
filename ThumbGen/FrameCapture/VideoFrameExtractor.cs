using EmguFFmpeg;
using FFmpeg.AutoGen;
using System;
using System.Drawing;

namespace ThumbGen.FrameCapture
{
    public class VideoFrameExtractor
    {
        private readonly MediaReader _reader;

        public bool FastMode { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TimeSpan Duration { get; set; }

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

        public Bitmap GetAtTimestamp(TimeSpan ts, out TimeSpan frameTs)
        {
            _reader.Seek(ts);
            frameTs = ts;

            using var packet = new MediaPacket();

            while (true)
            {
                var ret = _reader.ReadPacket(packet);
                if (ret < 0 && ret != ffmpeg.AVERROR_EOF)
                {
                    throw new FFmpegException(ret);
                }

                var decoder = _reader[packet.StreamIndex].Codec as MediaDecoder;
                if (decoder.SendPacket(packet) < 0)
                {
                    return null;
                }

                using var frame = new VideoFrame();

                while (decoder.ReceiveFrame(frame) >= 0)
                {
                    frameTs = _reader[packet.StreamIndex].ToTimeSpan(frame.Pts);
                    if (FastMode || frameTs >= ts)
                    {
                        return frame.ToBitmap();
                    }
                }
            }
        }
    }
}
