using System;
using System.Drawing;
using ThumbGen.Options;

namespace ThumbGen.Builder
{
    public record ThumbnailSizing(Size TotalSize, SizeF FrameSize, SizeF BorderSize);


    public class ThumbGenOptions
    {
        internal string Filename { get; }
        internal bool ConstantBorder { get; set; }
        internal Size BorderSize { get; set; } = new Size(0, 0);
        internal Size? Size { get; set; } = null;
        internal Size? FrameSize { get; set; } = null;
        internal Color? BgColor { get; set; }
        internal LinearGradient? BgGradient { get; set; }
        internal TilingOptions TilingOptions1 { get; } = new TilingOptions();
        internal TimeSpan? StartTime { get; set; }
        internal TimeSpan? EndTime { get; set; }
        internal bool PreserveAspect { get; set; } = true;
        internal bool AspectOverlap { get; set; } = true;
        internal bool FastMode { get; private set; }
        internal double? EndTimePercent { get; private set; }
        internal double? StartTimePercent { get; private set; }
        internal float? TimeCodeFontSize { get; private set; }
        internal LinearGradient? AspectOverlapGradient { get; set; }
        internal Color? AspectOverlapColor { get; set; } = Color.Black;
        internal LinearGradient? TimeCodeBgGradient { get; set; }
        internal Color? TimeCodeBgColor { get; set; } = Color.Black;
        internal Color? TimeCodeColor { get; set; } = Color.White;
        internal LinearGradient? TimeCodeGradient { get; set; }
        internal string? WatermarkFilename { get; set; }
        internal Size? WatermarkSize { get; set; }
        internal WatermarkPosition? WatermarkPosition { get; set; }

        public ThumbGenOptions(string filename)
        {
            Filename = filename;
        }

        public ThumbGenOptions WithSize(Size size)
        {
            Size = size;
            FrameSize = null;
            return this;
        }

        public ThumbGenOptions UseBackgroundColor(Color color)
        {
            BgColor = color;
            BgGradient = null;
            return this;
        }

        public ThumbGenOptions UseBackgroundGradient(LinearGradient gradient)
        {
            BgColor = null;
            BgGradient = gradient;
            return this;
        }

        public ThumbGenOptions WithOutputSize(int width = -1, int height = -1)
        {
            Size = new Size(width, height);
            return this;
        }

        public ThumbGenOptions WithOutputAspectRatio(double aspectRatio = 16d/9d)
        {
            return this;
        }

        public ThumbGenOptions PreserveFrameAspect(bool preserveAspect = true)
        {
            PreserveAspect = preserveAspect;
            return this;
        }

        public ThumbGenOptions WithWatermark(string filename, int width, int height, WatermarkPosition position = Builder.WatermarkPosition.BottomRight)
        {
            WatermarkFilename = filename;
            WatermarkSize = new Size(width, height);
            WatermarkPosition = position;
            return this;
        }


        public ThumbGenOptions UseAspectOverlap(bool aspectOverlap = true)
        {
            AspectOverlap = true;
            return this;
        }

        public ThumbGenOptions UseAspectOverlapColor(Color color)
        {
            AspectOverlapColor = color;
            AspectOverlapGradient = null;
            return this;
        }

        public ThumbGenOptions UseAspectOverlapGradient(LinearGradient gradient)
        {
            AspectOverlapColor = null;
            AspectOverlapGradient = gradient;
            return this;
        }

        public ThumbGenOptions UseTimeCodeBackgroundColor(Color color)
        {
            TimeCodeBgColor = color;
            TimeCodeBgGradient = null;
            return this;
        }

        public ThumbGenOptions UseTimeCodeBackgroundGradient(LinearGradient gradient)
        {
            TimeCodeBgColor = null;
            TimeCodeBgGradient = gradient;
            return this;
        }

        public ThumbGenOptions UseTimeCodeColor(Color color)
        {
            TimeCodeColor = color;
            TimeCodeGradient = null;
            return this;
        }

        public ThumbGenOptions UseTimeCodeGradient(LinearGradient gradient)
        {
            TimeCodeColor = null;
            TimeCodeGradient = gradient;
            return this;
        }

        /// <summary>
        /// The size of the captured frames from the video in the thumbnail. Does nothing if constant border is enabled
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public ThumbGenOptions WithFrameSize(int width = -1, int height = -1)
        {
            Size = null;
            FrameSize = new Size(width, height);
            return this;
        }

        public ThumbGenOptions WithBorder(Size borderSize, bool constant)
        {
            BorderSize = borderSize;
            ConstantBorder = constant;
            return this;
        }

        public ThumbGenOptions WithTiling(Action<TilingOptions> options) 
        {
            options(TilingOptions1);
            return this;
        }

        public ThumbGenOptions WithStartTime(TimeSpan startTime)
        {
            StartTime = startTime;
            return this;
        }

        /// <summary>
        /// Screenshot start time in percent (0 to 1) of the total duration
        /// </summary>
        /// <param name="durationPercent">Percent value between 0 and 1</param>
        /// <returns></returns>
        public ThumbGenOptions WithStartTime(double durationPercent)
        {
            StartTimePercent = durationPercent;
            return this;
        }

        public ThumbGenOptions WithEndTime(TimeSpan endTime)
        {
            EndTime = endTime;
            return this;
        }

        /// <summary>
        /// Screenshot end time in percent (0 to 1) of the total duration
        /// </summary>
        /// <param name="durationPercent">Percent value between 0 and 1</param>
        /// <returns></returns>
        public ThumbGenOptions WithEndTime(double durationPercent)
        {
            EndTimePercent = durationPercent;
            return this;
        }

        public ThumbGenOptions WithTimeCode(float size)
        {
            TimeCodeFontSize = size;
            return this;
        }

        public ThumbGenOptions UseFastMode()
        {
            FastMode = true;
            return this;
        }

        internal ThumbnailSizing CalcSizes(int width, int height)
        {
            var totalSize = new Size();
            var frameSize = new SizeF();
            var borderSize = BorderSize;

            var columns = TilingOptions1.Columns;
            var rows = TilingOptions1.Rows;
            var frameAspect = width / (float)height;

            var totalBorderWidth = (columns + 1) * BorderSize.Width;
            var totalBorderHeight = (rows + 1) * BorderSize.Height;

            if (Size is null)
                throw new InvalidOperationException();

            if (Size.Value.Height >= 0 && Size.Value.Width >= 0)
            {
                totalSize.Width = Size.Value.Width;
                frameSize.Width = (Size.Value.Width - totalBorderWidth) / columns;

                totalSize.Height = Size.Value.Height;
                frameSize.Height = (Size.Value.Height - totalBorderHeight) / rows;
            }
            else if (Size.Value.Height >= 0)
            {
                totalSize.Height = Size.Value.Height;
                frameSize.Height = (Size.Value.Height - totalBorderHeight) / rows;

                frameSize.Width = frameSize.Height * frameAspect;
                totalSize.Width = (int)Math.Round(columns * frameSize.Width + totalBorderWidth);
            }
            else if (Size.Value.Width >= 0)
            {
                totalSize.Width = Size.Value.Width;
                frameSize.Width = (Size.Value.Width - totalBorderWidth) / columns;

                frameSize.Height = frameSize.Width / frameAspect;
                totalSize.Height = (int)Math.Round(rows * frameSize.Height + totalBorderHeight);
            }
            else
            {
                frameSize.Width = width;
                totalSize.Width = columns * width + totalBorderWidth;

                frameSize.Height = height;
                totalSize.Height = rows * height + totalBorderHeight;
            }

            return new ThumbnailSizing(totalSize, frameSize, borderSize);
        }

        internal ThumbnailSizing CalcSizes2(int width, int height)
        {
            var totalSize = new Size();
            var frameSize = new SizeF();
            var borderSize = BorderSize;

            var columns = TilingOptions1.Columns;
            var rows = TilingOptions1.Rows;

            var totalBorderWidth = (columns + 1) * BorderSize.Width;
            var totalBorderHeight = (rows + 1) * BorderSize.Height;


            if (Size is not null)
            {
                totalSize.Width = Size.Value.Width;
                frameSize.Width = (totalSize.Width - totalBorderWidth) / (float)columns;

                totalSize.Height = Size.Value.Height;
                frameSize.Height = (totalSize.Height - totalBorderHeight) / (float)rows;
            }
            else if (FrameSize is not null)
            {
                frameSize.Width = FrameSize.Value.Width;
                totalSize.Width = columns * FrameSize.Value.Width + totalBorderWidth;

                frameSize.Height = FrameSize.Value.Height;
                totalSize.Height = rows * FrameSize.Value.Height + totalBorderHeight;
            }

            return new ThumbnailSizing(totalSize, frameSize, borderSize);
        }
    }
}
