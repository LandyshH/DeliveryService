namespace Core.Files;

public interface IFileWriter
{
    void WriteItemsListToFile<T>(string filePath, List<T> items);
}