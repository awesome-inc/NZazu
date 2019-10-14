using System.Diagnostics.CodeAnalysis;
using System.IO;
using NEdifis.Attributes;

namespace NZazu.Contracts.Suggest
{
    [ExcludeFromConventions("this is a file system wrapper")]
    [ExcludeFromCodeCoverage]
    [Because("this is a file system wrapper")]
    internal class FileSystem : IFileSystem
    {
        public bool FileExists(string file)
        {
            return new FileInfo(file).Exists;
        }

        public string[] ReadAllLines(string file)
        {
            return File.ReadAllLines(file);
        }
    }
}