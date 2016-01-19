using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Norbert.Modules.Common;

namespace Norbert.Modules.Music
{
    public class MusixClient : IMusixClient
    {
        private const string BaseUrl = "http://api.musixmatch.com/ws/1.1";

        private readonly IHttpClient _httpClient;
        private readonly string _apiKey;

        public MusixClient(IHttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<List<dynamic>> GetTracksAsync(string query, int limit)
        {
            var q = $"q_lyrics={query}&page_size={limit}&f_has_lyrics=1" +
                    $"&f_lyrics_language=en&format=json&apikey={_apiKey}";

            string url = $"{BaseUrl}/track.search?{q}";
            var tracks = await _httpClient.GetAsync(url);

            return ((IEnumerable<dynamic>) tracks.message.body.track_list)
                .Select(t => t.track)
                .ToList();
        }

        public async Task<dynamic> GetLyricsAsync(dynamic trackId)
        {
            var q = $"track_id={trackId}&format=json&apikey={_apiKey}";
            string url = $"{BaseUrl}/track.lyrics.get?{q}";

            var lyrics = await _httpClient.GetAsync(url);
            return lyrics.message.body.lyrics;
        }

        public async Task<dynamic> GetShortUrlAsync(string longUrl)
        {
            return await _httpClient.GetShortUrlAsync(longUrl);
        } 
    }
}