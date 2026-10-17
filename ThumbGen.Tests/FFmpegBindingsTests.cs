namespace ThumbGen.Tests;

public class FFmpegBindingsTests
{
    [Fact]
    public void EnsureInitialized_AfterInitialization_DoesNotThrow()
    {
        FFmpegBindings.EnsureInitialized();
    }
}
