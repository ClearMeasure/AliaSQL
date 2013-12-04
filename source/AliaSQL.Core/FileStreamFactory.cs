using System.IO;


namespace AliaSQL.Core
{
    public class FileStreamFactory : IFileStreamFactory
    {
        public Stream ConstructReadFileStream(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return stream;
        }

        public Stream ConstructWriteFileStream(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            return stream;
        }
    }
}