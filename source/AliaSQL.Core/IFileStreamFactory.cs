using System.IO;


namespace AliaSQL.Core
{
    public interface IFileStreamFactory
    {
        Stream ConstructReadFileStream(string path);
        Stream ConstructWriteFileStream(string path);
    }
}