using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Norbert.Cli.Exceptions;

namespace Norbert.Cli.Tests
{
    [TestClass]
    public class ConfigTests
    {
        private NameValueCollection _appSettings;

        [TestInitialize]
        public void Initialize()
        {
            _appSettings = new NameValueCollection
            {
                { "server", "irc.example.org" },
                { "nick", "norbert" },
                { "user", "norbert" },
                { "channels", "#chan1 #chan2" },
                { "quitMsg", "Bye!" }
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void Ctor_Invalid_Server_Throws()
        {
            InstantiateWithNullOrWhitespace("server");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void Ctor_Invalid_Nick_Throws()
        {
            InstantiateWithNullOrWhitespace("nick");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void Ctor_Invalid_User_Throws()
        {
            InstantiateWithNullOrWhitespace("user");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void Ctor_Invalid_Channels_Throws()
        {
            InstantiateWithNullOrWhitespace("channels");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void Ctor_Invalid_QuitMsg_Throws()
        {
            InstantiateWithNullOrWhitespace("quitMsg");
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        private void InstantiateWithNullOrWhitespace(string key)
        {
            _appSettings.Set(key, null);
            new Config(_appSettings);

            _appSettings.Set(key, string.Empty);
            new Config(_appSettings);

            _appSettings.Set(key, " ");
            new Config(_appSettings);
        }
    }
}
