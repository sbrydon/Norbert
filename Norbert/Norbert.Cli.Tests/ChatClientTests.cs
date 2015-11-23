using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Cli.Irc;
using Norbert.Modules.Common;

namespace Norbert.Cli.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class ChatClientTests
    {
        private Config _config;

        [TestInitialize]
        public void Initialize()
        {
            _config = new Config(ConfigHelper.ValidNameValueCollection);
        }

        [TestMethod]
        public void Connect_ConnectAsync_Called()
        {
            var mock = new Mock<IIrcClientAdapter>();

            var client = new ChatClient(_config, mock.Object);
            client.Connect();

            mock.Verify(m => m.ConnectAsync());
        }

        [TestMethod]
        public void Disconnect_Quit_Called()
        {
            var mock = new Mock<IIrcClientAdapter>();

            var client = new ChatClient(_config, mock.Object);
            client.Disconnect();

            mock.Verify(m => m.Quit(_config.QuitMsg));
        }

        [TestMethod]
        public void Disconnect_SocketError_Caught()
        {
            var mock = new Mock<IIrcClientAdapter>();
            mock.Setup(m => m.Quit(_config.QuitMsg)).Throws(new SocketException());

            var client = new ChatClient(_config, mock.Object);
            client.Disconnect();

            mock.Verify();
        }

        [TestMethod]
        public void SendMessage_SendMessage_Called()
        {
            var mock = new Mock<IIrcClientAdapter>();

            var client = new ChatClient(_config, mock.Object);
            client.SendMessage("hi", "#chan1");

            mock.Verify(m => m.SendMessage("hi", "#chan1"));
        }

        [TestMethod]
        public void Connection_Complete_Join_Config_Channels()
        {
            var mock = new Mock<IIrcClientAdapter>();

            var client = new ChatClient(_config, mock.Object);
            mock.Raise(m => m.ConnectionComplete += null, new EventArgs());

            foreach (var channel in _config.Channels)
                mock.Verify(m => m.JoinChannel(channel));
        }

        [TestMethod]
        public void Message_Received_Message_Received_Raised()
        {
            var mock = new Mock<IIrcClientAdapter>();
            var expEventArgs = new MessageReceivedEventArgs(false, "#chan1", "jim", "hi");

            var client = new ChatClient(_config, mock.Object);
            var raisedWithArgs = false;
            client.MessageReceived += (s, e) => raisedWithArgs = e == expEventArgs;
            
            mock.Raise(m => m.MessageReceived += null, expEventArgs);

            Assert.IsTrue(raisedWithArgs);
        }
    }
}