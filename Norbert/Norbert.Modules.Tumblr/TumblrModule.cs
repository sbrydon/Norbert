using log4net;
using Norbert.Modules.Common;

namespace Norbert.Modules.Tumblr
{
    public class TumblrModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (TumblrModule));
        // ReSharper disable once NotAccessedField.Local
        private TumblrPhotos _tumblrPhotos;

        public void Loaded(IConfigLoader configLoader, IChatClient chatClient, 
            IHttpClient httpClient, IFileSystem fileSystem, IRandomiser randomiser)
        {
            var config = LoadConfig(configLoader);
            var tumblrClient = new TumblrClient(httpClient, config.ApiKey);
            _tumblrPhotos = new TumblrPhotos(chatClient, tumblrClient, randomiser);
        }

        public void Unloaded()
        {
        }

        private static Config LoadConfig(IConfigLoader configLoader)
        {
            var config = configLoader.Load<Config>("Tumblr/Config.json") ?? new Config();

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