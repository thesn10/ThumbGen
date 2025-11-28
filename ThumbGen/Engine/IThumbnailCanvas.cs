using System.Drawing;
using FFmpegSharp;
using ThumbGen.Builder;

namespace ThumbGen.Engine
{
    public interface IThumbnailCanvas
    {
        void DrawAspectOverlap(float x, float y, float width, float height);
        void DrawImage(MediaFrame videoFrame, float x, float y, float width, float height);
        void DrawTimeCode(string tsString, string fontFamily, float fontSize, float originX, float originY, SizeF frameSize);
        void DrawWatermark(string watermarkFilename, float x, float y, float width, float height);
        IThumbnailImage Finish();
    }
}
