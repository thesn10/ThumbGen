using EmguFFmpeg;
using System.Drawing;
using System.Runtime.Versioning;
using ThumbGen.Engine;
using ThumbGen.FrameCapture;

namespace ThumbGen.SystemDrawing
{
    [SupportedOSPlatform("windows")]
    internal class SystemDrawingEngine : IThumbnailEngine
    {
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;

        private readonly Brush _aspectOverlapBrush;
        private readonly Brush _timeCodeBgBrush;
        private readonly Brush _timeCodeBrush;

        public SystemDrawingEngine(Bitmap bitmap, Brush bgBrush, Brush aspectOverlapBrush, Brush timeCodeBgBrush, Brush timeCodeBrush)
        {
            _bitmap = bitmap;
            _graphics = Graphics.FromImage(bitmap);
            _graphics.FillRectangle(bgBrush, 0, 0, _bitmap.Width, _bitmap.Height);

            _aspectOverlapBrush = aspectOverlapBrush;
            _timeCodeBgBrush = timeCodeBgBrush;
            _timeCodeBrush = timeCodeBrush;
        }

        public void DrawAspectOverlap(float x, float y, float width, float height)
        {
            _graphics.FillRectangle(_aspectOverlapBrush, x, y, width, height);
        }

        public void DrawImage(VideoFrame videoFrame, float x, float y, float width, float height)
        {
            _graphics.DrawImage(videoFrame.ToBitmap(), x, y, width, height);
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

        public IThumbnailResult Finish()
        {
            _graphics.Flush();
            return new SystemDrawingThumbnailResult(_bitmap);
        }
    }
}
