using System.Configuration;
using Norbert.Cli.Exceptions;

namespace Norbert.Cli
{
    public class Config
    {
        public string Server { get; private set; }
        public string Nick { get; private set; }
        public string User { get; private set; }
        public string[] Channels { get; private set; }
        public string QuitMsg { get; private set; }

        public static Config Load()
        {
            var appSettings = ConfigurationManager.AppSettings;
            var config = new Config { Server = appSettings.Get("server") };

            if (string.IsNullOrWhiteSpace(config.Server))
                throw new LoadConfigException("server invalid");

            config.Nick = appSettings.Get("nick");
            if (string.IsNullOrWhiteSpace(config.Nick))
                throw new LoadConfigException("nick invalid");

            config.User = appSettings.Get("user");
            if (string.IsNullOrWhiteSpace(config.User))
                throw new LoadConfigException("user invalid");

            var channels = appSettings.Get("channels");
            if (string.IsNullOrWhiteSpace(channels))
                throw new LoadConfigException("channels invalid");
            config.Channels = channels.Split();

            config.QuitMsg = appSettings.Get("quitMsg");
            if (string.IsNullOrWhiteSpace(config.QuitMsg))
                throw new LoadConfigException("quitMsg invalid");

            return config;
        }
    }
}