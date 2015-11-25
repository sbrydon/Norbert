using System;
using System.Net.Http;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Cli
{
    public class HttpService : IHttpClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HttpService));
        private readonly HttpClient _client = new HttpClient();

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
                throw new HttpServiceException(uri, e.Message);
            }
        }
    }
}