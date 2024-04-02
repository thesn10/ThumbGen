using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen
{
    public static class Diagnostics
    {
        public static ActivitySource? ActivitySource { get; set; }

        public static class Constants
        {
            public const string FrameCaptureInterval = "thumbgen.frameCapture.interval";
            public const string FrameCaptureStartTime = "thumbgen.frameCapture.startTime";
            public const string FrameCaptureEndTime = "thumbgen.frameCapture.endTime";
            public const string FrameCaptureTotalFrames = "thumbgen.frameCapture.totalFrames";
            public const string FrameCaptureMaxFrames = "thumbgen.frameCapture.maxFrames";


            public const string FrameCaptureFrameTime = "thumbgen.frameCapture.frameTime";
            public const string FrameCaptureFrameNr = "thumbgen.frameCapture.frameNr";
            public const string FrameCaptureFrameTimestamp = "thumbgen.frameCapture.frameTimestamp";
            public const string FrameCaptureElapsedTime = "thumbgen.frameCapture.elapsedTime";

            public const string RendererFrameCount = "thumbgen.renderer.frameCount";
            public const string RendererCanvasWidth = "thumbgen.renderer.canvasWidth";
            public const string RendererCanvasHeight = "thumbgen.renderer.canvasHeight";
            public const string RendererElapsedTime = "thumbgen.renderer.elapsedTime";
        }
    }
}
