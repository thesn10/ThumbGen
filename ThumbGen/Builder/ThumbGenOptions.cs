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
        internal SizeF BorderSize { get; set; } = new SizeF(0, 0);
        internal Size Size { get; set; } = new Size(-1, -1);
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

        public ThumbGenOptions(string filename)
        {
            Filename = filename;
        }

        public ThumbGenOptions WithSize(Size size)
        {
            Size = size;
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
            Size = new Size(width, height);
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

            if (Size.Height >= 0 && Size.Width >= 0)
            {
                totalSize.Width = Size.Width;
                var totalBorderWidth = (columns + 1) * BorderSize.Width;
                var totalFrameWidth = Size.Width - totalBorderWidth;
                frameSize.Width = (totalFrameWidth / columns);

                totalSize.Height = Size.Height;
                var totalBorderHeight = (rows + 1) * BorderSize.Height;
                var totalFrameHeight = Size.Height - totalBorderHeight;
                frameSize.Height = (totalFrameHeight / rows);
            }
            else if (Size.Height >= 0)
            {
                totalSize.Height = Size.Height;
                var totalBorderHeight = (rows + 1) * BorderSize.Height;
                var totalFrameHeight = Size.Height - totalBorderHeight;
                frameSize.Height = (totalFrameHeight / rows);

                frameSize.Width = frameSize.Height * frameAspect;
                var totalBorderWidth = (columns + 1) * BorderSize.Width;
                var totalFrameWidth = columns * frameSize.Width;
                totalSize.Width = (int)Math.Round(totalFrameWidth + totalBorderWidth);
            }
            else if (Size.Width >= 0)
            {
                totalSize.Width = Size.Width;
                var totalBorderWidth = (columns + 1) * BorderSize.Width;
                var totalFrameWidth = Size.Width - totalBorderWidth;
                frameSize.Width = (totalFrameWidth / columns);

                frameSize.Height = frameSize.Width / frameAspect;
                var totalBorderHeight = (rows + 1) * BorderSize.Height;
                var totalFrameHeight = rows * frameSize.Height;
                totalSize.Height = (int)Math.Round(totalFrameHeight + totalBorderHeight);
            }
            else
            {
                frameSize.Width = width;
                var totalBorderWidth = (columns + 1) * BorderSize.Width;
                var totalFrameWidth = columns * width;
                totalSize.Width = (int)Math.Round(totalFrameWidth + totalBorderWidth);

                frameSize.Height = height;
                var totalBorderHeight = (rows + 1) * BorderSize.Height;
                var totalFrameHeight = rows * height;
                totalSize.Height = (int)Math.Round(totalFrameHeight + totalBorderHeight);
            }


            /*if (_size.Width >= 0)
                {
                    totalSize.Width = _size.Width;
                    var totalBorderWidth = (columns + 1) * _borderSize.Width;
                    var totalFrameWidth = _size.Width - totalBorderWidth;
                    frameSize.Width = (totalFrameWidth / 3);
                }
                else
                {
                    frameSize.Width = _frameExtractor.Width;
                    var totalBorderWidth = (columns + 1) * _borderSize.Width;
                    var totalFrameWidth = columns * _frameExtractor.Width;
                    totalSize.Width = totalFrameWidth + totalBorderWidth;
                }

                int rows = _tilingOptions.Rows;

                if (_size.Height >= 0)
                {
                    totalSize.Height = _size.Height;
                    var totalBorderHeight = (columns + 1) * _borderSize.Height;
                    var totalFrameHeight = _size.Height - totalBorderHeight;
                    frameSize.Height = (totalFrameHeight / 3);
                }
                else
                {
                    frameSize.Height = _frameExtractor.Height;
                    var totalBorderHeight = (rows + 1) * _borderSize.Height;
                    var totalFrameHeight = rows * _frameExtractor.Height;
                    totalSize.Height = totalFrameHeight + totalBorderHeight;
                }*/
            return new ThumbnailSizing(totalSize, frameSize, borderSize);
        }
    }
}
