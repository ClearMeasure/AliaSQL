using System.IO;

namespace AliaSQL.Core
{
    public interface IResourceFileLocator
    {
        string ReadTextFile(string assembly, string resourceName);
        byte[] ReadBinaryFile(string assembly, string resourceName);
        bool FileExists(string assembly, string resourceName);
        Stream ReadFileAsStream(string assembly, string resourceName);
    }
}