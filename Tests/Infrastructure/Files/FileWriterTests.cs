using Infrastructure.Files;

namespace Tests.Infrastructure.Files;

public class FileWriterTests
{
    private readonly FileWriter _fileWriter;

    public FileWriterTests()
    {
        _fileWriter = new FileWriter();
    }

    private const string FilePath = "output.txt";
    
    [Fact]
    public void WriteItemsListToFile_WhenItemsAreValid_ShouldWriteToFile()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2", "Item3" };

        // Act
        _fileWriter.WriteItemsListToFile(FilePath, items);

        // Assert
        var writtenContent = File.ReadAllLines(FilePath);
        Assert.Equal(items.Count, writtenContent.Length);
        Assert.Equal("Item1", writtenContent[0]);
        Assert.Equal("Item2", writtenContent[1]);
        Assert.Equal("Item3", writtenContent[2]);

        File.Delete(FilePath);
    }
    
    [Fact]
    public void WriteItemsListToFile_WhenItemsAreEmpty_ShouldCreateEmptyFile()
    {
        // Arrange
        var items = new List<string>();

        // Act
        _fileWriter.WriteItemsListToFile(FilePath, items);

        // Assert
        Assert.True(File.Exists(FilePath));
        var writtenContent = File.ReadAllLines(FilePath);
        Assert.Empty(writtenContent);

        File.Delete(FilePath);
    }
}