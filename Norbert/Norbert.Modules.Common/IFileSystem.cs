namespace Norbert.Modules.Common
{
    public interface IFileSystem
    {
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        void AppendText(string path, string text);
    }
}