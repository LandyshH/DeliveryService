using Infrastructure.Files;

namespace Tests.Infrastructure.Files;

public class FileReaderTests
{
    private readonly FileReader _fileReader;

    public FileReaderTests()
    {
        _fileReader = new FileReader();
    }
    
    private const string FilePath = "data.txt";
    
    [Fact]
    public void ReadFileText_WhenFileExists_ShouldReturnLines()
    {
        // Arrange
        var expectedLines = new[] { "line1", "line2", "line3" };
        File.WriteAllLines(FilePath, expectedLines);

        // Act
        var result = _fileReader.ReadFileText(FilePath);

        // Assert
        Assert.Equal(expectedLines, result);
        
        File.Delete(FilePath);
    }
    
    [Fact]
    public void ReadFileText_WhenFileDoesNotExist_ShouldReturnEmptyArray()
    {
        // Arrange
        // Act
        var result = _fileReader.ReadFileText(FilePath);

        // Assert
        Assert.Empty(result);
    }
}