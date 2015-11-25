using System.Threading.Tasks;

namespace Norbert.Modules.Common
{
    public interface IHttpClient
    {
        Task<dynamic> GetAsync(string url);
    }
}