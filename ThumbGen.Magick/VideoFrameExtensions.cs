using FFmpeg.AutoGen.Abstractions;
using ImageMagick;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System;
using FFmpegSharp;

namespace ThumbGen.Magick
{
    internal static class VideoFrameExtensions
    {
        private static MagickImage BgraToMagickImage(MediaFrame frame)
        {
            var image = new MagickImage();
            byte[] rawBytes = frame.GetBytes();

            var pixelMapping = (frame.Format == (int)AVPixelFormat.AV_PIX_FMT_BGRA) ? PixelMapping.BGRA : PixelMapping.BGR;
            image.ReadPixels(rawBytes, new PixelReadSettings((uint)frame.Width, (uint)frame.Height, StorageType.Char, pixelMapping));
            return image;
        }

        public static MagickImage ToMagickImage(this MediaFrame frame)
        {
            if (frame.Format == 28 || frame.Format == 3)
            {
                return BgraToMagickImage(frame);
            }
            
            using var pixelConverter = PixelConverter.Create(frame.Width, frame.Height, AVPixelFormat.AV_PIX_FMT_BGRA);
            return BgraToMagickImage(pixelConverter.ConvertFrame(frame));
        }
    }
}
