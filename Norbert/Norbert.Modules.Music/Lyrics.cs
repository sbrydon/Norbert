using System.Linq;

namespace Norbert.Modules.Music
{
    public class Lyrics
    {
        public string Snippet { get; }
        public string Attribution { get; }

        public Lyrics(string lyrics, string artist, string track, string url)
        {
            var lines = lyrics.Split("\n".ToCharArray())
                .Where(l => !string.IsNullOrEmpty(l) && !l.Contains("NOT for Commercial use"))
                .Take(4);

            Snippet = string.Join(" 🎵🎵 ", lines);
            Attribution = $"{track} by {artist} - {url}";
        }
    }
}