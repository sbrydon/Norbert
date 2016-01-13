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
            return await GetShortUrlAsync(mapUrl.Formatted);
        }

        private async Task<dynamic> GetShortUrlAsync(string longUrl)
        {
            var url = $"https://www.googleapis.com/urlshortener/v1/url?key={_apiKey}";
            var body = new { longUrl };
            var response = await _httpClient.PostAsync(url, body);

            return response.id;
        }
    }
}