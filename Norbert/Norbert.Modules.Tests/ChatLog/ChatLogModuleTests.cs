using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.ChatLog;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Tests.ChatLog
{
    [TestClass]
    public class ChatLogModuleTests
    {
        private Mock<IConfigLoader> _mockLoader;
        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IChatClient> _mockClient;

        [TestInitialize]
        public void Initialise()
        {
            _mockLoader = new Mock<IConfigLoader>();
            _mockFileSystem = new Mock<IFileSystem>();
            _mockClient = new Mock<IChatClient>();
        }

        [TestMethod]
        public void Loaded_Loads_Config()
        {
            const string path = "ChatLog/Config.json";

            _mockLoader
                .Setup(m => m.Load<Config>(path))
                .Returns(new Config());

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);

            _mockLoader.Verify(m => m.Load<Config>(path));
        }

        [TestMethod]
        public void Loaded_Load_Exception_Caught()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Throws(new LoadConfigException(null, null));

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);
        }

        [TestMethod]
        public void Loaded_No_Log_Path_Uses_ChatLogs()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);

            _mockFileSystem.Verify(m => m.CreateDirectory("ChatLogs"));
        }

        [TestMethod]
        public void Loaded_Path_Doesnt_Exist_Creates_Log_Path()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            _mockFileSystem
                .Setup(m => m.DirectoryExists(It.IsAny<string>()))
                .Returns(false);

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);

            _mockFileSystem.Verify(m => m.CreateDirectory(It.IsAny<string>()));
        }

        [TestMethod]
        public void Loaded_Path_Exists_Doesnt_Create_Log_Path()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            _mockFileSystem
                .Setup(m => m.DirectoryExists(It.IsAny<string>()))
                .Returns(true);

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);

            _mockFileSystem.Verify(m => m.CreateDirectory(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Loaded_File_System_Exception_Caught()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            _mockFileSystem
                .Setup(m => m.DirectoryExists(It.IsAny<string>()))
                .Throws(new Exception());

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);
        }

        [TestMethod]
        public void Message_Received_Appends_To_Log()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);

            var msg = new MessageReceivedEventArgs(false, "#chan1", "jim", "hi");
            _mockClient.Raise(m => m.MessageReceived += null, msg);

            _mockFileSystem.Verify(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void Message_Received_Exception_Caught()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            _mockFileSystem
                .Setup(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var module = new ChatLogModule();
            module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object);

            var msg = new MessageReceivedEventArgs(false, "", "", "");
            _mockClient.Raise(m => m.MessageReceived += null, msg);
        }
    }
}