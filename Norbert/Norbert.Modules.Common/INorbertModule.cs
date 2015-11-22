namespace Norbert.Modules.Common
{
    public interface INorbertModule
    {
        void Loaded(IChatClient client);
        void Unloaded();
    }
}
