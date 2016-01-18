using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Norbert.Exceptions;

namespace Norbert.Tests
{
    [TestClass]
    public class ConfigTests
    {
        private NameValueCollection _appSettings;

        [TestInitialize]
        public void Initialize()
        {
            _appSettings = ConfigHelper.ValidNameValueCollection;
        }

        [TestMethod]
        public void Ctor_Valid_Settings_Sets()
        {
            var config = new Config(_appSettings);

            Assert.AreEqual(config.Server, _appSettings.Get("server"));
            Assert.AreEqual(config.Nick, _appSettings.Get("nick"));
            Assert.AreEqual(config.User, _appSettings.Get("user"));
            CollectionAssert.AreEqual(config.Channels, _appSettings.Get("channels").Split());
            Assert.AreEqual(config.QuitMsg, _appSettings.Get("quitMsg"));
            Assert.AreEqual(config.GoogleApiKey, _appSettings.Get("googleApiKey"));
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

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void Ctor_Invalid_GoogleApiKey_Throws()
        {
            InstantiateWithNullOrWhitespace("googleApiKey");
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
