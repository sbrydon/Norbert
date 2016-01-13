using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Maps.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class StaticMapsTests
    {
        private const string ValidCmd = "map of bristol";
        private const string StaticUrl = "https://example.org";

        private Mock<IChatClient> _mockChatClient;
        private Mock<IMapsClient> _mockMapsClient;

        [TestInitialize]
        public void Initialise()
        {
            _mockChatClient = new Mock<IChatClient>();
            _mockMapsClient = new Mock<IMapsClient>();
        }

        [TestMethod]
        public void Command_Received_Match_Replies()
        {
            _mockMapsClient
                .Setup(m => m.GetStaticUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(StaticUrl);

            var staticMaps = new StaticMaps(_mockChatClient.Object, _mockMapsClient.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs("#chan1", "JIM", "map edinburgh");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage($"JIM: {StaticUrl}", "#chan1"), 
                Times.Exactly(2));
        }

        [TestMethod]
        public void Command_Received_Non_Match_Or_Empty_Ignored()
        {
            var staticMaps = new StaticMaps(_mockChatClient.Object, _mockMapsClient.Object);

            var cmd = new CommandEventArgs(null, null, "doughnut");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs(null, null, "map of");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Command_Received_Match_Trims_Tag()
        {
            _mockMapsClient
                .Setup(m => m.GetStaticUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(StaticUrl);

            var staticMaps = new StaticMaps(_mockChatClient.Object, _mockMapsClient.Object);

            var cmd = new CommandEventArgs(null, null, ValidCmd + " ");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockMapsClient.Verify(m => m.GetStaticUrlAsync("bristol"));
        }

        [TestMethod]
        public void Command_Received_Match_Http_Exception_Caught()
        {
            _mockMapsClient
                .Setup(m => m.GetStaticUrlAsync(It.IsAny<string>()))
                .Throws(new HttpClientException(null, null));

            var staticMaps = new StaticMaps(_mockChatClient.Object, _mockMapsClient.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage("JIM: Whoops, something went wrong",
                "#chan1"));
        }
    }
}