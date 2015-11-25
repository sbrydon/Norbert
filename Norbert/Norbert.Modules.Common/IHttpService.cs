using System.Threading.Tasks;

namespace Norbert.Modules.Common
{
    public interface IHttpService
    {
        Task<dynamic> GetAsync(string url);
    }
}