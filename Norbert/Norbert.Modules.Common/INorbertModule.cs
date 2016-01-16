namespace Norbert.Modules.Common
{
    public interface INorbertModule
    {
        void Loaded(IConfigLoader configLoader, IChatClient chatClient, 
            IHttpClient httpClient, IFileSystem fileSystem, IRandomiser randomiser);
        void Unloaded();
    }
}
