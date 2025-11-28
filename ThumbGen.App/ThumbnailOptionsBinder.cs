using System.CommandLine;
using System.CommandLine.Binding;

namespace ThumbGen.App;

public class ThumbnailOptionsBinder
{
    public static ThumbnailOptions GetBoundValue(ParseResult parseResult) =>
        new()
        {
            StartTime = parseResult.GetValue(CommandLineOptions.StartTimeOption),
            Interval = parseResult.GetValue(CommandLineOptions.IntervalOption),
            EndTime = parseResult.GetValue(CommandLineOptions.EndTimeOption),
            InputFile = parseResult.GetValue(CommandLineOptions.InputFileArgument),
            OutputFile = parseResult.GetValue(CommandLineOptions.OutputFileArgument),
            FastMode = parseResult.GetValue(CommandLineOptions.FastModeOption),
            WebVTT = parseResult.GetValue(CommandLineOptions.WebVTTOption),
            ImagePath = parseResult.GetValue(CommandLineOptions.ImagePathOption),
            FrameWidth = parseResult.GetValue(CommandLineOptions.FrameWidthOption),
            FrameHeight = parseResult.GetValue(CommandLineOptions.FrameHeightOption),
            Columns = parseResult.GetValue(CommandLineOptions.ColumnsOption),
            Rows = parseResult.GetValue(CommandLineOptions.RowsOption),
            BorderWidth = parseResult.GetValue(CommandLineOptions.BorderWidthOption),
            BackgroundGradientStart = parseResult.GetValue(CommandLineOptions.BackgroundGradientStartOption),
            BackgroundGradientEnd = parseResult.GetValue(CommandLineOptions.BackgroundGradientEndOption),
            // Add other rendering options here
        };
}
