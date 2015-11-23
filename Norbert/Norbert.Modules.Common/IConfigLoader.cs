namespace Norbert.Modules.Common
{
    public interface IConfigLoader
    {
        T Load<T>(string path);
    }
}