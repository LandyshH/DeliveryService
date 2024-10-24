using Core.Files;

namespace Infrastructure.Files;

public class FileWriter : IFileWriter
{
    public void WriteItemsListToFile<T>(string path, List<T> items)
    {
        using var writer = new StreamWriter(path);
        foreach (var item in items)
        {
            writer.WriteLine(item?.ToString());
        }
    }
}