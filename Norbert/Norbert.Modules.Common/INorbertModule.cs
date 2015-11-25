namespace Norbert.Modules.Common
{
    public interface INorbertModule
    {
        void Loaded(IConfigLoader configLoader, IFileSystem fileSystem, 
            IChatClient chatClient, IHttpClient httpClient);
        void Unloaded();
    }
}
