using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;

namespace Norbert.Modules.ChatLog.Tests
{
    [TestClass]
    public class ChatLogModuleTests
    {
        private ChatLogModule _module;
        private Mock<IConfigLoader> _mockLoader;
        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IChatClient> _mockChatClient;

        [TestInitialize]
        public void Initialise()
        {
            _module = new ChatLogModule();

            _mockLoader = new Mock<IConfigLoader>();
            _mockFileSystem = new Mock<IFileSystem>();
            _mockChatClient = new Mock<IChatClient>();
        }

        [TestMethod]
        public void Loaded_Loads_Config()
        {
            const string path = "ChatLog/Config.json";

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

        [TestMethod]
        public void Loaded_No_Log_Path_Creates_Default_Log_Dir()
        {
            LoadModule();

            _mockFileSystem.Verify(m => m.CreateDirectory("ChatLogs"), Times.Once);
        }

        [TestMethod]
        public void Loaded_Custom_Log_Path_Creates_Custom_Dir()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config {Path = "Custom"});

            LoadModule();

            _mockFileSystem.Verify(m => m.CreateDirectory("Custom"), Times.Once);
        }

        [TestMethod]
        public void Loaded_Log_Path_Exists_Doesnt_Create_Dir()
        {
            _mockFileSystem
                .Setup(m => m.DirectoryExists(It.IsAny<string>()))
                .Returns(true);

            LoadModule();

            _mockFileSystem.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Loaded_File_System_Exception_Caught()
        {
            _mockFileSystem
                .Setup(m => m.DirectoryExists(It.IsAny<string>()))
                .Throws(new Exception());

            LoadModule();
        }

        private void LoadModule()
        {
            _module.Loaded(_mockLoader.Object, _mockChatClient.Object, null, _mockFileSystem.Object, null);
        }
    }
}