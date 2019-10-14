namespace NZazu.Contracts.Suggest
{
    public interface IFileSystem
    {
        bool FileExists(string file);
        string[] ReadAllLines(string file);
    }
}