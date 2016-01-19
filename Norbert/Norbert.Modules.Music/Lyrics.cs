using System.Collections.Generic;
using System.Linq;

namespace Norbert.Modules.Music
{
    public class Lyrics
    {
        public string Artist { get; }
        public string Track { get; }
        public string Url { get; }
        public string Snippet { get; }

        public Lyrics(string artist, string track, string url, dynamic lyrics)
        {
            Artist = artist;
            Track = track;
            Url = url;

            IEnumerable<string> lines = lyrics.ToString().Split("\n".ToCharArray());
            lines = lines
                .Where(l => !string.IsNullOrEmpty(l) && !l.Contains("NOT for Commercial use"))
                .Take(4);

            Snippet = string.Join(" 🎵🎵 ", lines);
        }
    }
}