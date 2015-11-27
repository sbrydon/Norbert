using System.IO;
using Norbert.Modules.Common;

namespace Norbert
{
    public class FileSystem : IFileSystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void AppendText(string path, string text)
        {
            using (var writer = File.AppendText(path))
            {
                writer.WriteLine(text);
            }
        }
    }
}