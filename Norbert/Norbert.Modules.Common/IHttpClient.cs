using System.Threading.Tasks;

namespace Norbert.Modules.Common
{
    public interface IHttpClient
    {
        Task<dynamic> GetAsync(string url);
        Task<dynamic> PostAsync(string url, dynamic body);
    }
}