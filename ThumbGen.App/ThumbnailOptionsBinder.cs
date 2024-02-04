using System.CommandLine.Binding;

namespace ThumbGen.App;

public class ThumbnailOptionsBinder : BinderBase<ThumbnailOptions>
{
    protected override ThumbnailOptions GetBoundValue(BindingContext bindingContext) =>
        new()
        {
            StartTime = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.StartTimeOption),
            Interval = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.IntervalOption),
            EndTime = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.EndTimeOption),
            InputFile = bindingContext.ParseResult.GetValueForArgument(CommandLineOptions.InputFileArgument),
            OutputFile = bindingContext.ParseResult.GetValueForArgument(CommandLineOptions.OutputFileArgument),
            FastMode = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.FastModeOption),
            WebVTT = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.WebVTTOption),
            ImagePath = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.ImagePathOption),
            FrameWidth = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.FrameWidthOption),
            FrameHeight = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.FrameHeightOption),
            Columns = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.ColumnsOption),
            Rows = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.RowsOption),
            BorderWidth = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.BorderWidthOption),
            BackgroundGradientStart = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.BackgroundGradientStartOption),
            BackgroundGradientEnd = bindingContext.ParseResult.GetValueForOption(CommandLineOptions.BackgroundGradientEndOption),
            // Add other rendering options here
        };
}
