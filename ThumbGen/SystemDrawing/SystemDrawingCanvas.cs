using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using FFmpeg.AutoGen.Abstractions;
using FFmpegSharp;
using ThumbGen.Engine;

namespace ThumbGen.SystemDrawing
{
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    internal class SystemDrawingCanvas : IThumbnailCanvas
    {
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;

        private readonly Brush _aspectOverlapBrush;
        private readonly Brush _timeCodeBgBrush;
        private readonly Brush _timeCodeBrush;

        public SystemDrawingCanvas(Bitmap bitmap, Brush bgBrush, Brush aspectOverlapBrush, Brush timeCodeBgBrush, Brush timeCodeBrush)
        {
            _bitmap = bitmap;
            _graphics = Graphics.FromImage(bitmap);
            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            _graphics.FillRectangle(bgBrush, 0, 0, _bitmap.Width, _bitmap.Height);

            _aspectOverlapBrush = aspectOverlapBrush;
            _timeCodeBgBrush = timeCodeBgBrush;
            _timeCodeBrush = timeCodeBrush;
        }

        public void DrawAspectOverlap(float x, float y, float width, float height)
        {
            _graphics.FillRectangle(_aspectOverlapBrush, x, y, width, height);
        }

        public void DrawImage(MediaFrame videoFrame, float x, float y, float width, float height)
        {
            _graphics.DrawImage(FrameToBitmap(videoFrame), x, y, width, height);
        }

        public void DrawTimeCode(string tsString, string fontFamily, float fontSize, float originX, float originY, SizeF frameSize)
        {
            var textFont = new Font(fontFamily, fontSize);
            var textRealSize = _graphics.MeasureString(tsString, textFont);

            var textx = originX + frameSize.Width - textRealSize.Width;
            var texty = originY + frameSize.Height - textRealSize.Height;
            _graphics.FillRectangle(_timeCodeBgBrush, textx, texty, textRealSize.Width, textRealSize.Height);
            _graphics.DrawString(tsString, textFont, _timeCodeBrush, textx, texty);
        }

        public void DrawWatermark(string watermarkFilename, float x, float y, float width, float height)
        {
            var image = Image.FromFile(watermarkFilename);
            _graphics.DrawImage(image, x, y, width, height);
        }

        public IThumbnailImage Finish()
        {
            _graphics.Flush();
            return new SystemDrawingThumbnailResult(_bitmap);
        }
        
        private static Bitmap FrameToBitmap(MediaFrame frame)
        {
            if ((AVPixelFormat)frame.Format == AVPixelFormat.AV_PIX_FMT_BGRA || (AVPixelFormat)frame.Format == AVPixelFormat.AV_PIX_FMT_BGR24)
                return BgraToMat(frame);
            
            using (PixelConverter converter = PixelConverter.Create(frame.Width, frame.Height, AVPixelFormat.AV_PIX_FMT_BGRA))
            {
                return BgraToMat(converter.ConvertFrame(frame));
            }
        }
        
        private static Bitmap BgraToMat(MediaFrame frame)
        {
            int width = frame.Width;
            int height = frame.Height;
            Bitmap bitmap = new Bitmap(width, height, (AVPixelFormat)frame.Format == AVPixelFormat.AV_PIX_FMT_BGRA ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            bitmapData.Stride = frame.Linesize[0];
            
            var data = frame.GetBytes();
            
            Marshal.Copy(data,0, bitmapData.Scan0, data.Length);
            
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
