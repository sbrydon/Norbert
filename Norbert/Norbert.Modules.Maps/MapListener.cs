using System.Text.RegularExpressions;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Maps
{
    public class MapListener
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MapListener));

        private static readonly Regex Regex =
            new Regex(@"map\s*(?:of\s*)?(?<place>.*)", RegexOptions.IgnoreCase);

        private readonly IChatClient _chatClient;
        private readonly IMapsClient _mapsClient;

        public MapListener(IChatClient chatClient, IMapsClient mapsClient)
        {
            _chatClient = chatClient;
            _mapsClient = mapsClient;

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

            var place = match.Groups["place"].Value.TrimEnd();
            if (place == string.Empty)
            {
                Log.Debug($"Message ignored: matches '{Regex}' but <place> is empty");
                return;
            }

            Log.Debug($"Replying: '{cmd.Message}' matches '{Regex}', <place> = '{place}'");

            try
            {
                var url = await _mapsClient.GetStaticUrlAsync(place);
                _chatClient.SendMessage($"{cmd.Nick}: {url}", cmd.Source);
            }
            catch (HttpClientException)
            {
                _chatClient.SendMessage($"{cmd.Nick}: Whoops, something went wrong", cmd.Source);
            }
        }
    }
}