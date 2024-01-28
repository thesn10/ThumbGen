using EmguFFmpeg;
using System.Drawing;
using ThumbGen.Builder;

namespace ThumbGen.Engine
{
    public interface IThumbnailEngine
    {
        void DrawAspectOverlap(float x, float y, float width, float height);
        void DrawImage(VideoFrame videoFrame, float x, float y, float width, float height);
        void DrawTimeCode(string tsString, string fontFamily, float fontSize, float originX, float originY, SizeF frameSize);
        void DrawWatermark(string watermarkFilename, float x, float y, float width, float height);
        IThumbnailResult Finish();
    }
}
