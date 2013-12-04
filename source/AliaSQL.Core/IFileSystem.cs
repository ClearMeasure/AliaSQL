using System.IO;

namespace AliaSQL.Core
{
    public interface IFileSystem
    {
        void SaveFile(string filename, byte[] fileContent);
        bool FileExists(string relativePath);
        Stream ReadIntoFileStream(string path);
        string[] GetAllFilesWithExtensionWithinFolder(string folder, string fileExtension);
        string ReadTextFile(string filename);
        StreamReader ReadFileIntoStreamReader(string filename);
    }
}