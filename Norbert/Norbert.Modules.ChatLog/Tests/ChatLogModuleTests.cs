using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.ChatLog.Tests
{
    [TestClass]
    public class ChatLogModuleTests
    {
        private ChatLogModule _module;
        private Mock<IConfigLoader> _mockLoader;
        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IChatClient> _mockClient;

        [TestInitialize]
        public void Initialise()
        {
            _module = new ChatLogModule();

            _mockLoader = new Mock<IConfigLoader>();
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

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

            LoadModule();

            _mockLoader.Verify(m => m.Load<Config>(path), Times.Once);
        }

        [TestMethod]
        public void Loaded_Load_Exception_Caught()
        {
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Throws(new LoadConfigException(null, null));

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

        [TestMethod]
        public void Message_Received_Appends_To_Log_File()
        {
            LoadModule();

            var msg = new MessageEventArgs(false, false, "#chan1", "JIM", "HELLO");
            _mockClient.Raise(m => m.MessageReceived += null, msg);

            const string file = "ChatLogs/#chan1.log";
            const string regex = @"^\[.+\]\s<JIM>\sHELLO$";
            _mockFileSystem.Verify(m => m.AppendText(file, It.IsRegex(regex)), Times.Once);
        }

        [TestMethod]
        public void Message_Received_Exception_Caught()
        {
            _mockFileSystem
                .Setup(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            LoadModule();

            var msg = new MessageEventArgs(false, false, "", "", "");
            _mockClient.Raise(m => m.MessageReceived += null, msg);

            _mockFileSystem.Verify(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public void Message_Sent_Appends_To_Log_File()
        {
            LoadModule();

            var msg = new MessageEventArgs(false, false, "#chan1", "NORBERT", "WELCOME");
            _mockClient.Raise(m => m.MessageSent += null, msg);

            const string file = "ChatLogs/#chan1.log";
            const string regex = @"^\[.+\]\s<NORBERT>\sWELCOME$";
            _mockFileSystem.Verify(m => m.AppendText(file, It.IsRegex(regex)), Times.Once);
        }

        [TestMethod]
        public void Message_Sent_Exception_Caught()
        {
            _mockFileSystem
                .Setup(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            LoadModule();

            var msg = new MessageEventArgs(false, false, "", "", "");
            _mockClient.Raise(m => m.MessageSent += null, msg);

            _mockFileSystem.Verify(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()), 
                Times.Once);
        }

        private void LoadModule()
        {
            _module.Loaded(_mockLoader.Object, _mockFileSystem.Object, _mockClient.Object, null);
        }
    }
}