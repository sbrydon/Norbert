﻿using log4net;
using Norbert.Modules.Common;

namespace Norbert.Modules.Maps
{
    public class MapsModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MapsModule));
        // ReSharper disable once NotAccessedField.Local
        private MapListener _mapListener;

        public void Loaded(IConfigLoader configLoader, IChatClient chatClient, 
            IHttpClient httpClient, IFileSystem fileSystem, IRandomiser randomiser)
        {
            var config = LoadConfig(configLoader);
            var mapsClient = new MapsClient(httpClient, config.ApiKey);
            _mapListener = new MapListener(chatClient, mapsClient);
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