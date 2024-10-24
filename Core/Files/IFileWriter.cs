namespace Core.Files;

public interface IFileWriter
{
    void WriteItemsListToFile<T>(string path, List<T> items);
}