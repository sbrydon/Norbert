namespace Norbert.Modules.Common
{
    public interface INorbertModule
    {
        void Loaded(ConfigLoader configLoader, IChatClient client);
        void Unloaded();
    }
}
