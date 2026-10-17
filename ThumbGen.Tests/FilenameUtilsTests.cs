using ThumbGen.Util;

namespace ThumbGen.Tests;

public class FilenameUtilsTests
{
    [Fact]
    public void ChangeFilename_ReplacesNameAndKeepsDirectoryAndExtension()
    {
        var result = FilenameUtils.ChangeFilename(Path.Combine("dir", "thumb.jpg"), name => name + "_1");

        Assert.Equal(Path.Combine("dir", "thumb_1.jpg"), result);
    }

    [Fact]
    public void ChangeFilename_WithoutDirectory_ReturnsBareFilename()
    {
        Assert.Equal("thumb_2.png", FilenameUtils.ChangeFilename("thumb.png", name => name + "_2"));
    }
}
