using Core.Files;

namespace Infrastructure.Files;

public class FileWriter : IFileWriter
{
    public void WriteItemsListToFile<T>(string filePath, List<T> items)
    {
        using var writer = new StreamWriter(filePath);
        foreach (var item in items)
        {
            writer.WriteLine(item?.ToString());
        }
    }
}