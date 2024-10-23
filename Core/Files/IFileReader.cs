namespace Core.Files;

public interface IFileReader
{
    string[] ReadFileText(string path);
}