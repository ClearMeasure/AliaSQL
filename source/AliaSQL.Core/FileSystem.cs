using System;
using System.IO;
using System.Text;

namespace AliaSQL.Core
{
    public class FileSystem : IFileSystem
    {
        private readonly IFileStreamFactory _streamFactory;

        public FileSystem(IFileStreamFactory streamFactory)
        {
            _streamFactory = streamFactory;
        }

        public FileSystem():this(new FileStreamFactory())
        {
        
        }

        public void SaveFile(string filename, byte[] fileContent)
        {
            if (fileContent != null)
            {
                var fileStream = _streamFactory.ConstructWriteFileStream(filename);

                using (var writer = new BinaryWriter(fileStream))
                {
                    writer.Write(fileContent);
                }
            }
        }

        public bool FileExists(string relativePath)
        {
            var retval = File.Exists(relativePath);
            return retval;
        }

        public Stream ReadIntoFileStream(string path)
        {
            try
            {
                var stream = _streamFactory.ConstructReadFileStream(path);
                return stream;
            }
            catch (IOException ex)
            {
                if (ex.Message.IndexOf("it is being used by another process") >= 0)
                {
                    throw new ApplicationException("The file you chose cannot be read because it is open in another application.  Please close the file in the other application and try again.");
                }

                throw;
            }
        }

        public string[] GetAllFilesWithExtensionWithinFolder(string folder, string fileExtension)
        {
            var fileNames = new string[0];

            if (Directory.Exists(folder))
            {
                var searchPattern = string.Format("*.{0}", fileExtension);
                fileNames = Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories);
            }

            return fileNames;
        }

        public string ReadTextFile(string filename)
        {
            var stream = _streamFactory.ConstructReadFileStream(filename);

            Encoding encoding = GetEncoding(filename);
            using (var reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM)
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// Function originally from http://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return Encoding.ASCII;
        }

        public StreamReader ReadFileIntoStreamReader(string filename)
        {
            var stream = _streamFactory.ConstructReadFileStream(filename);
            var reader = new StreamReader(stream);
            return reader;
        }
    }
}