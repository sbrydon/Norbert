using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Helpers;

namespace Norbert.Modules.Tumblr
{
    public class TumblrClient : ITumblrClient
    {
        private const string BaseUrl = "http://api.tumblr.com/v2";
        private readonly IHttpClient _httpClient;
        private readonly string _apiKey;

        public TumblrClient(IHttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<List<dynamic>> GetPhotoPostsAsync(string tag, DateTime before, int limit)
        {
            var timestamp = before.ToTimestamp();
            var q = $"api_key={_apiKey}&tag={tag}&before={timestamp}&limit={limit}";

            string url = $"{BaseUrl}/tagged?{q}";
            var posts = await _httpClient.GetAsync(url);

            return ((IEnumerable<dynamic>) posts.response)
                .Where(p => p.type == "photo" &&
                            DynamicHelper.HasProperty(() => p.image_permalink) &&
                            (p.image_permalink != null && p.image_permalink.ToString() != string.Empty))
                .ToList();
        }
    }
}