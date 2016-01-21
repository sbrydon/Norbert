using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Music.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class SingListenerTests
    {
        private const string ValidCmd = "sing about wraps";

        private Mock<IChatClient> _mockChatClient;
        private Mock<IMusixClient> _mockMusixClient;
        private Mock<IRandomiser> _mockRandomiser;

        private List<dynamic> _tracks;
        private Lyrics _lyrics1;
        private Lyrics _lyrics2;

        [TestInitialize]
        public void Initialise()
        {
            _mockChatClient = new Mock<IChatClient>();
            _mockMusixClient = new Mock<IMusixClient>();
            _mockRandomiser = new Mock<IRandomiser>();

            _tracks = new List<dynamic>
            {
                new
                {
                    track_id = 1,
                    artist_name = "Roy Doe",
                    track_name = "Shenandoeah",
                    track_share_url = "http://example.org/1"
                },
                new
                {
                    track_id = 2,
                    artist_name = "Electric Doe",
                    track_name = "Doethrone",
                    track_share_url = "http://example.org/2"
                }
            };

            _lyrics1 = new Lyrics("Roy Doe", "Shenandoeah", "http://example.org/1", "Doop\nDoop");
            _lyrics2 = new Lyrics("Electric Doe", "Doethrone", "http://example.org/2", "No\nNo");
        }

        [TestMethod]
        public void Command_Received_Match_Replies()
        {
            _mockMusixClient
                .Setup(m => m.GetTracksAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_tracks);

            _mockMusixClient
                .Setup(m => m.GetLyricsAsync(It.IsAny<object>()))
                .ReturnsAsync(_lyrics1);

            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", null, ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs("#chan1", null, "sing wraps");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(_lyrics1.Snippet, "#chan1"),
                Times.Exactly(2));

            _mockChatClient.Verify(m => m.SendMessage(_lyrics1.Attribution, "#chan1"),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Command_Received_Non_Match_Or_Empty_Ignored()
        {
            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, "baguette");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs(null, null, "sing about");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Command_Received_Match_Gets_25_Tracks_With_Query()
        {
            _mockMusixClient
                .Setup(m => m.GetTracksAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_tracks);

            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string expQuery = "wraps";
            const int expLimit = 25;

            _mockMusixClient.Verify(m => m.GetTracksAsync(expQuery, expLimit));
        }

        [TestMethod]
        public void Command_Received_Match_Replies_With_Lyrics_From_Random_Track()
        {
            const int randomIndex = 1;
            object expTrack = _tracks[randomIndex];

            _mockRandomiser.Setup(m => m.NextInt(It.IsAny<int>())).Returns(randomIndex);

            _mockMusixClient.Setup(m => m.GetTracksAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_tracks);

            _mockMusixClient.Setup(m => m.GetLyricsAsync(expTrack))
                .ReturnsAsync(_lyrics2);

            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", null, ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockMusixClient.Verify(m => m.GetLyricsAsync(expTrack));
        }

        [TestMethod]
        public void Command_Received_Match_Trims_Query()
        {
            _mockMusixClient
                .Setup(m => m.GetTracksAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_tracks);

            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, ValidCmd + " ");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockMusixClient.Verify(m => m.GetTracksAsync("wraps", It.IsAny<int>()));
        }

        [TestMethod]
        public void Command_Received_Match_Http_Exception_Caught()
        {
            _mockMusixClient
                .Setup(m => m.GetTracksAsync(It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new HttpClientException(null, null));

            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage("JIM: Whoops, something went wrong",
                "#chan1"));
        }

        [TestMethod]
        public void Command_Received_Match_No_Posts_Whoops()
        {
            _mockMusixClient
                .Setup(m => m.GetTracksAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new List<dynamic>());

            var listener = new SingListener(_mockChatClient.Object, _mockMusixClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage("JIM: Whoops, no lyrics found",
                "#chan1"));
        }
    }
}