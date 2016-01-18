using System.Threading.Tasks;
using Norbert.Modules.Common;

namespace Norbert.Modules.Maps
{
    public class MapsClient : IMapsClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _apiKey;

        public MapsClient(IHttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GetStaticUrlAsync(string place)
        {
            var mapUrl = new StaticMapUrl(place, _apiKey);
            return await _httpClient.GetShortUrlAsync(_apiKey, mapUrl.Formatted);
        }
    }
}