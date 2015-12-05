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
    public class TumblrPhotosTests
    {
        private const string ValidCmd1 = "tumblr of burger";
        private const string ValidCmd2 = "tumblr burger";
        private const string InvalidCmd1 = "baguette";
        private const string InvalidCmd2 = "tumblr of";

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
                new {type = "photo", image_permalink = "photo_url_2"}
            };
        }

        [TestMethod]
        public void Command_Received_Non_Match_Or_Empty_Ignored()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);

            var cmd = new CommandEventArgs(null, null, InvalidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            cmd = new CommandEventArgs(null, null, InvalidCmd2);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Command_Received_Match_Replies()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);
            cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd2);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(@"JIM:\s.+"), "#chan1"),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Command_Received_Match_Gets_Photos_Before_Random_Date()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var expMin = new DateTime(2010, 1, 1);
            const string expTag = "burger";
            const int expLimit = 7;

            var expBefores = new[]
            {
                DateTime.Today,
                DateTime.Today.AddDays(-3),
                DateTime.Today.AddDays(-5)
            };
            _mockRandomiser.SetupSequence(m => m.NextDateTime(expMin))
                .Returns(expBefores[0])
                .Returns(expBefores[1])
                .Returns(expBefores[2]);

            var cmd = new CommandEventArgs(null, null, ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            foreach (var expBefore in expBefores)
            {
                _mockTumblrClient.Verify(
                    m => m.GetPhotoPostsAsync(expTag, expBefore, expLimit), Times.Once);
            }
        }

        [TestMethod]
        public void Command_Received_Match_Chooses_Random_Post()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var expPostIndices = new[] {1, 0};
            _mockRandomiser.SetupSequence(m => m.NextInt(It.IsAny<int>()))
                .Returns(expPostIndices[0])
                .Returns(expPostIndices[1]);

            foreach(var i in expPostIndices)
            {
                var cmd = new CommandEventArgs(null, null, ValidCmd1);
                _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

                string regex = _photoPosts[i].image_permalink;
                _mockChatClient.Verify(
                    m => m.SendMessage(It.IsRegex(regex), It.IsAny<string[]>()), Times.Once);
            }
        }

        [TestMethod]
        public void Command_Received_Match_Trims_Tag()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(_photoPosts);

            var cmd = new CommandEventArgs(null, null, ValidCmd1 + " ");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockTumblrClient.Verify(
                m => m.GetPhotoPostsAsync("burger", It.IsAny<DateTime>(), It.IsAny<int>()));
        }

        [TestMethod]
        public void Command_Received_Match_Http_Exception_Caught()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Throws(new HttpClientException(null, null));

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string regex = @"JIM:\sWhoops, something went wrong";
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), "#chan1"), Times.Once);
        }

        [TestMethod]
        public void Command_Received_Match_No_Posts_Whoops()
        {
            var photos = new TumblrPhotos(_mockChatClient.Object, _mockTumblrClient.Object,
                _mockRandomiser.Object);
            _mockTumblrClient
                .Setup(m => m.GetPhotoPostsAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .ReturnsAsync(new List<dynamic>());

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string regex = @"JIM:\sWhoops, no tumblrs found";
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), "#chan1"), Times.Once);
        }
    }
}