using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;

namespace Norbert.Modules.ChatLog.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class ChatListenerTests
    {
        private Mock<IFileSystem> _mockFileSystem;
        private Mock<IChatClient> _mockClient;
        private Config _config;

        [TestInitialize]
        public void Initialise()
        {
            _mockFileSystem = new Mock<IFileSystem>();
            _mockClient = new Mock<IChatClient>();
            _config = new Config {Path = "ChatLogs"};
        }

        [TestMethod]
        public void Message_Received_Appends_To_Log_File()
        {
            var listener = new ChatListener(_mockFileSystem.Object, _mockClient.Object, _config);
            var msg = new MessageEventArgs(false, "#chan1", "JIM", "HELLO");
            _mockClient.Raise(m => m.MessageReceived += null, msg);

            const string file = "ChatLogs/#chan1.log";
            const string regex = @"^\[.+\]\s<JIM>\sHELLO$";
            _mockFileSystem.Verify(m => m.AppendText(file, It.IsRegex(regex)));
        }

        [TestMethod]
        public void Message_Received_Exception_Caught()
        {
            _mockFileSystem
                .Setup(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var listener = new ChatListener(_mockFileSystem.Object, _mockClient.Object, _config);
            var msg = new MessageEventArgs(false, "", "", "");
            _mockClient.Raise(m => m.MessageReceived += null, msg);

            _mockFileSystem.Verify(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()));
        }

        [TestMethod]
        public void Message_Sent_Appends_To_Log_File()
        {
            var listener = new ChatListener(_mockFileSystem.Object, _mockClient.Object, _config);
            var msg = new MessageEventArgs(false, "#chan1", "NORBERT", "WELCOME");
            _mockClient.Raise(m => m.MessageSent += null, msg);

            const string file = "ChatLogs/#chan1.log";
            const string regex = @"^\[.+\]\s<NORBERT>\sWELCOME$";
            _mockFileSystem.Verify(m => m.AppendText(file, It.IsRegex(regex)));
        }

        [TestMethod]
        public void Message_Sent_Exception_Caught()
        {
            _mockFileSystem
                .Setup(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception());

            var listener = new ChatListener(_mockFileSystem.Object, _mockClient.Object, _config);
            var msg = new MessageEventArgs(false, "", "", "");
            _mockClient.Raise(m => m.MessageSent += null, msg);

            _mockFileSystem.Verify(m => m.AppendText(It.IsAny<string>(), It.IsAny<string>()));
        }
    }
}