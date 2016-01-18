using System.Collections.Specialized;

namespace Norbert.Tests
{
    public static class ConfigHelper
    {
        public static NameValueCollection ValidNameValueCollection => new NameValueCollection
        {
            {"server", "irc.example.org"},
            {"nick", "norbert"},
            {"user", "norbert"},
            {"channels", "#chan1 #chan2"},
            {"quitMsg", "Bye!"},
            {"googleApiKey", "your_google_api_key"}
        };
    }
}