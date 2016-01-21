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

        public async Task<Lyrics> GetLyricsAsync(dynamic track)
        {
            var q = $"track_id={track.track_id}&format=json&apikey={_apiKey}";
            string url = $"{BaseUrl}/track.lyrics.get?{q}";

            var lyrics = await _httpClient.GetAsync(url);
            lyrics = lyrics.message.body.lyrics;

            if (lyrics.restricted == 1)
                return null;

            string artist = track.artist_name;
            string trackName = track.track_name;

            string share = track.track_share_url;
            url = await _httpClient.GetShortUrlAsync(share);

            return new Lyrics(lyrics.lyrics_body.ToString(), artist, trackName, url);
        }
    }
}