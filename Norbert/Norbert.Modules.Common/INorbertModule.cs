namespace Norbert.Modules.Common
{
    public interface INorbertModule
    {
        void Loaded(IConfigLoader configLoader, IFileSystem fileSystem, IChatClient client);
        void Unloaded();
    }
}
