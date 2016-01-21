using System.Collections.Generic;
using System.Threading.Tasks;

namespace Norbert.Modules.Music
{
    public interface IMusixClient
    {
        Task<List<dynamic>> GetTracksAsync(string query, int limit);
        Task<Lyrics> GetLyricsAsync(dynamic track);
    }
}