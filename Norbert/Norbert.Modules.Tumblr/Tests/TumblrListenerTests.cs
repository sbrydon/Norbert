using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Tumblr.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public class TumblrListenerTests
    {
        private const string ValidCmd = "tumblr of burger";

        private Mock<IChatClient> _mockChatClient;
        private Mock<ITumblrClient> _mockTumblrClient;
        private Mock<IRandomiser> _mockRandomiser;
        private List<dynamic> _photoPosts;

        [TestInitialize]
        public void Initialise()
        {
            _mockChatClient = new Mock<IChatClient>();
            _mockTumblrClient = new Mock<ITumblrClient>();
            _mockRandomiser = new Mock<IRandomiser>();

            _photoPosts = new List<dynamic>
            {
                new {type = "photo", image_permalink = "photo_url_1"},
                new {type = "photo", image_permalink = "photo_url_2"},
                new {type = "photo", image_permalink = "photo_url_3"}
            };
        }

        [TestMethod]
        public void Command_Received_Match_Replies()
        {
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs("#chan1", "JIM", "tumblr burger");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(@"JIM:\s.+"), "#chan1"),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Command_Received_Non_Match_Or_Empty_Ignored()
        {
            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, "baguette");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs(null, null, "tumblr of");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Command_Received_Match_Gets_21_Photos_With_Tag()
        {
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string expTag = "burger";
            const int expLimit = 7;

            _mockTumblrClient.Verify(
                m => m.GetPhotoPostsAsync(expTag, It.IsAny<DateTime>(), expLimit), Times.Exactly(3));
        }

        [TestMethod]
        public void Command_Received_Replies_With_Random_Photo_Before_Random_Date_After_2010()
        {
            var expMin = new DateTime(2010, 1, 1);
            var expBefore = DateTime.Today.AddDays(-3);
            const int randomIndex = 1;

            _mockRandomiser.Setup(m => m.NextDateTime(expMin)).Returns(expBefore);
            _mockRandomiser.Setup(m => m.NextInt(It.IsAny<int>())).Returns(randomIndex);

            _mockTumblrClient.Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), expBefore, It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            string regex = _photoPosts[randomIndex].image_permalink;
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), It.IsAny<string[]>()));
        }

        [TestMethod]
        public void Command_Received_Match_Trims_Tag()
        {
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, ValidCmd + " ");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockTumblrClient.Verify(
                m => m.GetPhotoPostsAsync("burger", It.IsAny<DateTime>(), It.IsAny<int>()));
        }

        [TestMethod]
        public void Command_Received_Match_Http_Exception_Caught()
        {
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Throws(new HttpClientException(null, null));

            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage("JIM: Whoops, something went wrong",
                "#chan1"));
        }

        [TestMethod]
        public void Command_Received_Match_No_Posts_Whoops()
        {
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new List<dynamic>());

            var listener = new TumblrListener(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage("JIM: Whoops, no tumblrs found", 
                "#chan1"));
        }
    }
}