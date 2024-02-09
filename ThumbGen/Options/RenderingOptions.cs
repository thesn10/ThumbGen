using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThumbGen.Builder;

namespace ThumbGen.Options
{
    public class RenderingOptions
    {
        internal Color? BgColor { get; set; }
        internal LinearGradient? BgGradient { get; set; }
        internal Color? AspectOverlapColor { get; set; }
        internal LinearGradient? AspectOverlapGradient { get; set; }
        internal Color? TimeCodeBgColor { get; set; }
        internal LinearGradient? TimeCodeBgGradient { get; set; }
        internal Color? TimeCodeColor { get; set; }
        internal LinearGradient? TimeCodeGradient { get; set; }

        internal bool PreserveAspect { get; set; } = true;
        internal bool AspectOverlap { get; set; } = true;
        internal Size BorderSize { get; set; } = new Size(0, 0);
        internal Size? Size { get; set; } = null;
        internal Size? FrameSize { get; set; } = null;
        internal TilingOptions TilingOptions { get; } = new TilingOptions();
        internal float? TimeCodeFontSize { get; private set; }
        internal string? WatermarkFilename { get; set; }
        internal Size? WatermarkSize { get; set; }
        internal WatermarkPosition? WatermarkPosition { get; set; }

        public RenderingOptions UseBackgroundColor(Color color)
        {
            BgColor = color;
            BgGradient = null;
            return this;
        }

        public RenderingOptions UseBackgroundGradient(LinearGradient gradient)
        {
            BgColor = null;
            BgGradient = gradient;
            return this;
        }

        public RenderingOptions UseAspectOverlapColor(Color color)
        {
            AspectOverlapColor = color;
            AspectOverlapGradient = null;
            return this;
        }

        public RenderingOptions UseAspectOverlapGradient(LinearGradient gradient)
        {
            AspectOverlapColor = null;
            AspectOverlapGradient = gradient;
            return this;
        }

        public RenderingOptions UseTimeCodeBackgroundColor(Color color)
        {
            TimeCodeBgColor = color;
            TimeCodeBgGradient = null;
            return this;
        }

        public RenderingOptions UseTimeCodeBackgroundGradient(LinearGradient gradient)
        {
            TimeCodeBgColor = null;
            TimeCodeBgGradient = gradient;
            return this;
        }

        public RenderingOptions UseTimeCodeColor(Color color)
        {
            TimeCodeColor = color;
            TimeCodeGradient = null;
            return this;
        }

        public RenderingOptions UseTimeCodeGradient(LinearGradient gradient)
        {
            TimeCodeColor = null;
            TimeCodeGradient = gradient;
            return this;
        }

        public RenderingOptions WithOutputSize(int width = -1, int height = -1)
        {
            Size = new Size(width, height);
            FrameSize = null;
            return this;
        }

        public RenderingOptions PreserveFrameAspect(bool preserveAspect = true)
        {
            PreserveAspect = preserveAspect;
            return this;
        }

        public RenderingOptions WithWatermark(string filename, int width, int height, WatermarkPosition position = Builder.WatermarkPosition.BottomRight)
        {
            WatermarkFilename = filename;
            WatermarkSize = new Size(width, height);
            WatermarkPosition = position;
            return this;
        }


        public RenderingOptions UseAspectOverlap(bool aspectOverlap = true)
        {
            AspectOverlap = aspectOverlap;
            return this;
        }

        /// <summary>
        /// The size of the captured frames from the video in the thumbnail
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public RenderingOptions WithFrameSize(int width = -1, int height = -1)
        {
            Size = null;
            FrameSize = new Size(width, height);
            return this;
        }

        public RenderingOptions WithBorder(Size borderSize)
        {
            BorderSize = borderSize;
            return this;
        }

        public RenderingOptions WithTiling(Action<TilingOptions> options)
        {
            options(TilingOptions);
            return this;
        }

        public RenderingOptions WithTimeCode(float size)
        {
            TimeCodeFontSize = size;
            return this;
        }

        internal ThumbnailSizing CalcSizes(int width, int height)
        {
            var totalSize = new Size();
            var frameSize = new SizeF();
            var borderSize = BorderSize;

            var columns = TilingOptions.Columns;
            var rows = TilingOptions.Rows;
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

        internal ThumbnailSizing CalcSizes2(int width, int height, int? columns = null, int? rows = null)
        {
            var totalSize = new Size();
            var frameSize = new SizeF();
            var borderSize = BorderSize;

            columns ??= TilingOptions.Columns;
            rows ??= TilingOptions.Rows;

            var totalBorderWidth = (columns.Value + 1) * BorderSize.Width;
            var totalBorderHeight = (rows.Value + 1) * BorderSize.Height;

            var aspect = width / (float)height;

            if (Size is not null)
            {
                totalSize.Width = Size.Value.Width == -1 ? (int)(Size.Value.Height * aspect) : Size.Value.Width;
                totalSize.Height = Size.Value.Height == -1 ? (int)(Size.Value.Width / aspect) : Size.Value.Height;

                frameSize.Width = (totalSize.Width - totalBorderWidth) / (float)columns.Value;
                frameSize.Height = (totalSize.Height - totalBorderHeight) / (float)rows.Value;
            }
            else if (FrameSize is not null)
            {
                frameSize.Width = FrameSize.Value.Width == -1 ? FrameSize.Value.Height * aspect : FrameSize.Value.Width;
                frameSize.Height = FrameSize.Value.Height == -1 ? FrameSize.Value.Width / aspect : FrameSize.Value.Height;

                totalSize.Width = (int)(columns.Value * frameSize.Width + totalBorderWidth);
                totalSize.Height = (int)(rows.Value * frameSize.Height + totalBorderHeight);
            }

            return new ThumbnailSizing(totalSize, frameSize, borderSize);
        }
    }
}
