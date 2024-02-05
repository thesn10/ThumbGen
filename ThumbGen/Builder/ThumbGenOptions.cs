using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using ThumbGen.Options;
using ThumbGen.Util;

namespace ThumbGen.Builder
{
    public record ThumbnailSizing(Size TotalSize, SizeF FrameSize, SizeF BorderSize);


    public class ThumbGenOptions
    {
        public Func<int, string> GetFilePath { get; set; } = (index) => FilePathWithIndex(index, "thumbnail.jpg");
        public Func<string?, int, string> GetWebVTTImageUrl { get; set; } = (imagePath, index) => Path.GetFileName(imagePath) ?? string.Empty;
        public string WebVTTFilename { get; set; } = "storyboard.vtt";
        public bool GenerateWebVTT { get; set; } = false;
        public int? TotalFrames { get; set; }
        public TimeSpan? Interval { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool FastMode { get; set; }
        public double? EndTimePercent { get; set; }
        public double? StartTimePercent { get; set; }

        public ThumbGenOptions WithOutputFilename(string filepath)
        {
            GetFilePath = (index) => FilePathWithIndex(index, filepath);
            return this;
        }

        public ThumbGenOptions WithWebVTT(string? filepath = null, Func<string, int, string>? getImageUrl = null)
        {
            GenerateWebVTT = true;
            if (filepath is not null)
                WebVTTFilename = filepath;
            if (getImageUrl is not null)
                GetWebVTTImageUrl = getImageUrl;
            return this;
        }

        public ThumbGenOptions WithInterval(TimeSpan interval)
        {
            Interval = interval;
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

        public ThumbGenOptions UseFastMode()
        {
            FastMode = true;
            return this;
        }

        private static string FilePathWithIndex(int index, string filepath)
        {
            if (index == 0) return filepath;
            return FilenameUtils.ChangeFilename(filepath, oldName => oldName + index);
        }
    }
}
