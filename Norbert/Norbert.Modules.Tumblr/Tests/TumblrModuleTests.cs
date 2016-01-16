using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;

namespace Norbert.Modules.Tumblr.Tests
{
    [TestClass]
    public class TumblrModuleTests
    {
        private TumblrModule _module;
        private Mock<IConfigLoader> _mockLoader;
        private Mock<IChatClient> _mockChatClient;

        [TestInitialize]
        public void Initialise()
        {
            _module = new TumblrModule();

            _mockLoader = new Mock<IConfigLoader>();
            _mockChatClient = new Mock<IChatClient>();
        }

        [TestMethod]
        public void Loaded_Loads_Config()
        {
            const string path = "Tumblr/Config.json";

            _mockLoader
                .Setup(m => m.Load<Config>(path))
                .Returns(new Config());

            LoadModule();

            _mockLoader.Verify(m => m.Load<Config>(path), Times.Once);
        }

        [TestMethod]
        public void Loaded_Config_Null_Uses_Default()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(default(Config));

            LoadModule();
        }

        private void LoadModule()
        {
            _module.Loaded(_mockLoader.Object, _mockChatClient.Object, null, null, null);
        }
    }
}