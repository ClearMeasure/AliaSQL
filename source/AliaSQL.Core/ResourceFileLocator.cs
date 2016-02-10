using System;
using System.IO;
using System.Reflection;
using System.Text;


namespace AliaSQL.Core
{
    public class ResourceFileLocator : IResourceFileLocator
    {
        public string ReadTextFile(string assembly, string resourceName)
        {
            using (Stream stream = getStream(assembly, resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string contents = reader.ReadToEnd();
                    return contents;
                }
            }
        }

        public byte[] ReadBinaryFile(string assembly, string resourceName)
        {
            using (Stream stream = getStream(assembly, resourceName))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] contents = reader.ReadBytes((int)stream.Length);
                    return contents;
                }
            }
        }

        public bool FileExists(string assembly, string resourceName)
        {
            Stream stream = constructStream(assembly, resourceName);
            bool fileExists = stream != null;
            return fileExists;
        }

        public Stream ReadFileAsStream(string assembly, string resourceName)
        {
            return getStream(assembly, resourceName);
        }

        private Stream getStream(string assembly, string resourceName)
        {
            Stream stream = constructStream(assembly, resourceName);

            if (stream == null)
            {
                string template = "Resource file not found: {0}. Make sure the Build Action for the file is 'Embedded Resource'.";
                throw new ApplicationException(string.Format(template, resourceName));
            }

            return stream;
        }

        private Stream constructStream(string assembly, string resourceName)
        {
            Stream stream = Assembly.Load(assembly).GetManifestResourceStream(resourceName);
            return stream;
        }
    }
}