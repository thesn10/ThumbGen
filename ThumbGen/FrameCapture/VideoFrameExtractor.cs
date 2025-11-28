using FFmpeg.AutoGen.Abstractions;
using System;
using FFmpegSharp;

namespace ThumbGen.FrameCapture
{
    public class VideoFrameExtractor
    {
        private readonly MediaDemuxer _reader;

        public bool FastMode { get; set; }

        private readonly MediaStream _stream;
        private readonly MediaDecoder _decoder;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public TimeSpan Duration { get; private set; }

        public VideoFrameExtractor(string filename)
        {
            _reader = MediaDemuxer.Open(filename);
            Duration = TimeSpan.FromMilliseconds((double)_reader.Duration / 1000);
            for (var i = 0; i < _reader.Count; i++)
            {
                var stream = _reader[i];
                
                if ((stream.Disposition & ffmpeg.AV_DISPOSITION_ATTACHED_PIC) == ffmpeg.AV_DISPOSITION_ATTACHED_PIC)
                {
                    continue;
                }

                var decoder = MediaDecoder.CreateDecoder(stream.CodecparRef);
                if (decoder.CodecType != AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    continue;
                }

                _stream = stream;
                _decoder = decoder;
                Width = decoder.Width;
                Height = decoder.Height;
            }

            if (_stream is null)
                throw new ArgumentException("No video stream found");
            if (_decoder is null)
                throw new ArgumentException("Could not create decoder");
        }

        public (MediaFrame, TimeSpan) GetAtTimestamp(TimeSpan ts)
        {
            _reader.Seek(ts);

            foreach (var packet in _reader.ReadPackets())
            {
                if (packet.StreamIndex != _stream.Index)
                {
                    continue;
                }

                int ret = _decoder.SendPacket(packet);
                if (ret < 0)
                {
                    throw new FFmpegException(ret);
                }

                var frame = new MediaFrame();

                while (_decoder.ReceiveFrame(frame) >= 0)
                {
                    var frameTs = _reader[packet.StreamIndex].ToTimeSpan(frame.Pts);
                    if (FastMode || frameTs >= ts)
                    {
                        return (frame, frameTs);
                    }
                }
            }
            
            throw new Exception("Reached end of file not finding vaild frame");
        }
    }
}
