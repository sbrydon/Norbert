using log4net;
using Norbert.Modules.Common;

namespace Norbert.Modules.Maps
{
    public class MapsModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MapsModule));
        // ReSharper disable once NotAccessedField.Local
        private StaticMaps _staticMaps;

        public void Loaded(IConfigLoader configLoader, IFileSystem fileSystem,
            IChatClient chatClient, IHttpClient httpClient)
        {
            var config = LoadConfig(configLoader);
            var mapsClient = new MapsClient(httpClient, config.ApiKey);
            _staticMaps = new StaticMaps(chatClient, mapsClient);
        }

        public void Unloaded()
        {
        }

        private static Config LoadConfig(IConfigLoader configLoader)
        {
            var config = configLoader.Load<Config>("Maps/Config.json") ?? new Config();

            if (string.IsNullOrWhiteSpace(config.ApiKey))
            {
                Log.Warn("No 'apikey' defined in Config.json");
            }
            else
            {
                Log.Info("Found 'apikey' in Config.json");
            }

            return config;
        }
    }
}