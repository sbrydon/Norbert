using System.Threading.Tasks;

namespace Norbert.Modules.Maps
{
    public interface IMapsClient
    {
        Task<string> GetStaticUrlAsync(string place);
    }
}