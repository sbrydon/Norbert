using log4net;
using Norbert.Modules.Common;

namespace Norbert.Modules.Music
{
    public class MusicModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MusicModule));
        // ReSharper disable once NotAccessedField.Local
        private SingListener _listener;

        public void Loaded(IConfigLoader configLoader, IChatClient chatClient,
            IHttpClient httpClient, IFileSystem fileSystem, IRandomiser randomiser)
        {
            var config = LoadConfig(configLoader);
            var musixClient = new MusixClient(httpClient, config.ApiKey);
            _listener = new SingListener(chatClient, musixClient, randomiser);
        }

        public void Unloaded()
        {
        }

        private static Config LoadConfig(IConfigLoader configLoader)
        {
            var config = configLoader.Load<Config>("Music/Config.json") ?? new Config();

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