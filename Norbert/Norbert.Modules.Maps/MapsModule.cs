using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Maps
{
    public class MapsModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MapsModule));

        private static readonly Regex Regex =
            new Regex(@"where(?:'*s)?\s*(?:is\s*)?(?<name>.*)", RegexOptions.IgnoreCase);

        private IConfigLoader _configLoader;
        private IChatClient _chatClient;
        private IHttpClient _httpClient;
        private string _apiKey;

        public void Loaded(IConfigLoader configLoader, IFileSystem fileSystem,
            IChatClient chatClient, IHttpClient httpClient)
        {
            _httpClient = httpClient;
            _chatClient = chatClient;
            _configLoader = configLoader;

            _chatClient.MessageReceived += OnMessageReceived;
            SetupApiKey();
        }

        public void Unloaded()
        {
        }

        private void SetupApiKey()
        {
            var config = new Config();
            try
            {
                config = _configLoader.Load<Config>("Maps/Config.json");
            }
            catch (LoadConfigException e)
            {
                Log.Error(e.Message);
            }

            if (string.IsNullOrWhiteSpace(config.ApiKey))
            {
                _apiKey = null;
                Log.Warn("No 'apikey' defined in Config.json");
            }
            else
            {
                _apiKey = config.ApiKey;
                Log.Info("Found 'apikey' in Config.json");
            }
        }

        private async void OnMessageReceived(object sender, MessageEventArgs msg)
        {
            if (msg.IsPrivate || !msg.IsCommand)
            {
                Log.Debug($"Message ignored: IsPrivate={msg.IsPrivate}, IsCommand={msg.IsCommand}");
                return;
            }

            var match = Regex.Match(msg.Message);
            if (!match.Success)
            {
                Log.Debug($"Message ignored: '{msg.Message}' doesn't match '{Regex}'");
                return;
            }

            var name = match.Groups["name"].Value.TrimEnd();
            if (name == string.Empty)
            {
                Log.Debug($"Message ignored: matches '{Regex}' but <name> is empty");
                return;
            }

            Log.Debug($"Replying: '{msg.Message}' matches '{Regex}', <name> = '{name}'");

            try
            {
                var loc = await GetRandomLocation();
                var mapUrl = new MapUrl(loc.Lat, loc.Lon, name, _apiKey);
                var shortUrl = await GetShortUrl(mapUrl.Formatted);

                _chatClient.SendMessage($"{msg.Nick}: {name} is in {loc.Address} - {shortUrl}", msg.Source);
            }
            catch (HttpClientException)
            {
                _chatClient.SendMessage($"{msg.Nick}: Whoops, something went wrong", msg.Source);
            }
        }

        private async Task<Location> GetRandomLocation()
        {
            var area = RandomCoords.NextArea();
            Log.Debug($"Random area chosen: '{area.Name}'");

            while (true)
            {
                var coords = RandomCoords.NextCoords(area);
                Log.Debug($"Random coords chosen: '{coords[0]},{coords[1]}'");

                var q = $"latlng={coords[0]},{coords[1]}&key={_apiKey}";
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?{q}";

                var response = await _httpClient.GetAsync(url);
                var result = ((IEnumerable<dynamic>) response.results).FirstOrDefault();

                if (result == null)
                {
                    Log.Debug("No results found! Retrying..");
                    continue;
                }

                Address address = Address.FromComponents(result.address_components);
                if (address == null)
                {
                    Log.Debug($"Unable to create address from '{result.address_components}'. Reyrying..");
                    continue;
                }

                return new Location(coords[0], coords[1], address.Formatted);
            }
        }

        private async Task<dynamic> GetShortUrl(string longUrl)
        {
            var url = $"https://www.googleapis.com/urlshortener/v1/url?key={_apiKey}";
            var body = new {longUrl};
            var response = await _httpClient.PostAsync(url, body);

            Log.Debug($"URL shortened to '{response.id}'");
            return response.id;
        }
    }
}