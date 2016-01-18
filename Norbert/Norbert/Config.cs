using System.Collections.Specialized;
using Norbert.Exceptions;

namespace Norbert
{
    public class Config
    {
        public string Server { get; }
        public string Nick { get; }
        public string User { get; }
        public string[] Channels { get; }
        public string QuitMsg { get; }
        public string GoogleApiKey { get; }

        public Config(NameValueCollection appSettings)
        {
            Server = appSettings.Get("server");
            if (string.IsNullOrWhiteSpace(Server))
                throw new ConfigException("server invalid");

            Nick = appSettings.Get("nick");
            if (string.IsNullOrWhiteSpace(Nick))
                throw new ConfigException("nick invalid");

            User = appSettings.Get("user");
            if (string.IsNullOrWhiteSpace(User))
                throw new ConfigException("user invalid");

            var channels = appSettings.Get("channels");
            if (string.IsNullOrWhiteSpace(channels))
                throw new ConfigException("channels invalid");
            Channels = channels.Split();

            QuitMsg = appSettings.Get("quitMsg");
            if (string.IsNullOrWhiteSpace(QuitMsg))
                throw new ConfigException("quitMsg invalid");

            GoogleApiKey = appSettings.Get("googleApiKey");
            if (string.IsNullOrWhiteSpace(GoogleApiKey))
                throw new ConfigException("googleApiKey invalid");
        }
    }
}