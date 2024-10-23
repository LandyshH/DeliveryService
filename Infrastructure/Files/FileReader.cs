using Core.Files;
using Serilog;

namespace Infrastructure.Files;

public class FileReader : IFileReader
{
    public string[] ReadFileText(string path)
    {
        if (!File.Exists(path))
        {
            Log.Error($"Файл не найден: {path}");
            return [];
        }

        try
        {
            var lines = File.ReadAllLines(path);
            return lines;
        }
        catch (Exception ex)
        {
            Log.Error($"Ошибка при чтении файла: {ex.Message}");
            return [];
        }
    }
}