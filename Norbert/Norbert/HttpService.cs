using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Exceptions;

namespace Norbert
{
    public class HttpService : IHttpClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HttpService));
        private const string MediaTypeJson = "application/json";
        
        private readonly HttpClient _client = new HttpClient();
        private readonly string _googleApiKey;

        public HttpService(string googleApiKey)
        {
            _googleApiKey = googleApiKey;
        }

        public async Task<dynamic> GetAsync(string uri)
        {
            Log.Debug($"GET: {uri}");

            try
            {
                var response = await _client.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<dynamic>(content);
            }
            catch (Exception e)
            {
                Log.Error($"GET error: {e.Message}");
                throw new HttpClientException(uri, e.Message);
            }
        }

        public async Task<dynamic> GetShortUrlAsync(string longUrl)
        {
            var url = $"https://www.googleapis.com/urlshortener/v1/url?key={_googleApiKey}";

            var body = new { longUrl };
            var response = await PostAsync(url, body);

            return response.id;
        }

        private async Task<dynamic> PostAsync(string uri, dynamic body)
        {
            Log.Debug($"POST: {uri}, body='{body}'");

            try
            {
                var content = JsonConvert.SerializeObject(body);
                var response = await _client
                    .PostAsync(uri, new StringContent(content, Encoding.UTF8, MediaTypeJson));

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<dynamic>(responseContent);
            }
            catch (Exception e)
            {
                Log.Error($"POST error: {e.Message}");
                throw new HttpClientException(uri, e.Message);
            }
        }
    }
}