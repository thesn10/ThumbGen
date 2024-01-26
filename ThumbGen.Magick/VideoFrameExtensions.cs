using EmguFFmpeg;
using FFmpeg.AutoGen;
using ImageMagick;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System;

namespace ThumbGen.Magick
{
    internal static class VideoFrameExtensions
    {
        private unsafe static MagickImage BgraToMagickImage(VideoFrame frame)
        {
            int width = frame.Width;
            int height = frame.Height;
            var image = new MagickImage();
            var linesize = frame.Linesize[0];
            int byteWidth = (frame.AVFrame.format == (int)AVPixelFormat.AV_PIX_FMT_BGRA) ? 4 * width : 3 * width;

            byte[] rawBytes = new byte[byteWidth * height];
            /*fixed (byte* p = rawBytes)
            {
                FFmpegHelper.CopyPlane(frame.Data[0], frame.Linesize[0], (nint)p, byteWidth, byteWidth, height);
            }*/
            for (int i = 0; i < height; i++)
                Marshal.Copy(frame.Data[0] + i * linesize, rawBytes, i * byteWidth, byteWidth);

            var pixelMapping = (frame.AVFrame.format == (int)AVPixelFormat.AV_PIX_FMT_BGRA) ? PixelMapping.BGRA : PixelMapping.BGR;
            image.ReadPixels(rawBytes, new PixelReadSettings(width, height, StorageType.Char, pixelMapping));
            return image;
        }

        public static MagickImage ToMagickImage(this VideoFrame frame)
        {
            if (frame.AVFrame.format == 28 || frame.AVFrame.format == 3)
            {
                return BgraToMagickImage(frame);
            }

            using VideoFrame dstFrame = new VideoFrame(frame.AVFrame.width, frame.AVFrame.height, AVPixelFormat.AV_PIX_FMT_BGRA);
            using PixelConverter pixelConverter = new PixelConverter(dstFrame);
            return BgraToMagickImage(pixelConverter.ConvertFrame(frame));
        }
    }
}
