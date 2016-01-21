using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Music
{
    public class SingListener
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SingListener));

        private static readonly Regex Regex =
            new Regex(@"sing\s*(?:about\s*)?(?<query>.*)", RegexOptions.IgnoreCase);

        private readonly IChatClient _chatClient;
        private readonly IMusixClient _musixClient;
        private readonly IRandomiser _randomiser;

        public SingListener(IChatClient chatClient, IMusixClient musixClient, IRandomiser randomiser)
        {
            _chatClient = chatClient;
            _musixClient = musixClient;
            _randomiser = randomiser;

            _chatClient.CommandReceived += OnCommandReceived;
        }

        private async void OnCommandReceived(object sender, CommandEventArgs cmd)
        {
            var match = Regex.Match(cmd.Message);
            if (!match.Success)
            {
                Log.Debug($"Message ignored: '{cmd.Message}' doesn't match '{Regex}'");
                return;
            }

            var query = match.Groups["query"].Value.TrimEnd();
            if (query == string.Empty)
            {
                Log.Debug($"Message ignored: matches '{Regex}' but <query> is empty");
                return;
            }

            Log.Debug($"Replying: '{cmd.Message}' matches '{Regex}', <query> = '{query}'");

            try
            {
                var lyrics = await GetRandomLyrics(query);
                if (lyrics == null)
                {
                    _chatClient.SendMessage($"{cmd.Nick}: Whoops, no lyrics found", cmd.Source);
                    return;
                }

                _chatClient.SendMessage(lyrics.Snippet, cmd.Source);
                _chatClient.SendMessage(lyrics.Attribution, cmd.Source);
            }
            catch (HttpClientException)
            {
                _chatClient.SendMessage($"{cmd.Nick}: Whoops, something went wrong", cmd.Source);
            }
        }

        private async Task<Lyrics> GetRandomLyrics(string query)
        {
            var tracks = await _musixClient.GetTracksAsync(query, 25);
            if (tracks.Count == 0)
                return null;

            while (tracks.Count > 0)
            {
                var randomTrack = tracks[_randomiser.NextInt(tracks.Count - 1)];
                var lyrics = await _musixClient.GetLyricsAsync(randomTrack);

                if (lyrics != null)
                    return lyrics;

                tracks.Remove(randomTrack);
            }

            return null;
        }
    }
}