using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Tumblr.Tests
{
    [TestClass]
    public class TumblrModuleTests
    {
        private const string ValidCmd1 = "tumblr of burger";
        private const string ValidCmd2 = "tumblr burger";
        private const string InvalidCmd1 = "baguette";
        private const string InvalidCmd2 = "tumblr of";

        private TumblrModule _module;
        private Mock<IConfigLoader> _mockLoader;
        private Mock<IChatClient> _mockChatClient;
        private Mock<IHttpClient> _mockHttpClient;
        private dynamic _response;

        [TestInitialize]
        public void Initialise()
        {
            _module = new TumblrModule();

            _mockLoader = new Mock<IConfigLoader>();
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            _mockChatClient = new Mock<IChatClient>();
            _mockHttpClient = new Mock<IHttpClient>();

            _response = new[]
            {
                new {type = "photo", short_url = "photo_url_1"},
                new {type = "photo", short_url = "photo_url_2"},
                new {type = "quote", short_url = "quote_url"},
                new {type = "video", short_url = "video_url"}
            };
        }

        [TestMethod]
        public void Loaded_Loads_Config()
        {
            const string path = "Tumblr/Config.json";

            _mockLoader
                .Setup(m => m.Load<Config>(path))
                .Returns(new Config());

            LoadModule();

            _mockLoader.Verify(m => m.Load<Config>(path));
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
        public void Message_Received_Private_Ignored()
        {
            LoadModule();

            var msg = new MessageReceivedEventArgs(true, true, null, null, ValidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()), 
                Times.Never);
        }

        [TestMethod]
        public void Message_Received_Non_Command_Ignored()
        {
            LoadModule();

            var msg = new MessageReceivedEventArgs(false, false, null, null, ValidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()), 
                Times.Never);
        }

        [TestMethod]
        public void Message_Received_Non_Match_Ignored()
        {
            LoadModule();

            var msg = new MessageReceivedEventArgs(false, true, null, null, InvalidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            msg = new MessageReceivedEventArgs(false, true, null, null, InvalidCmd2);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            _mockChatClient.Verify(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string[]>()),
                Times.Never);
        }

        [TestMethod]
        public void Message_Received_Match_Replies()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {response = _response});

            LoadModule();

            var msg = new MessageReceivedEventArgs(false, true, "#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);
            msg = new MessageReceivedEventArgs(false, true, "#chan1", "JIM", ValidCmd2);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(@"JIM:\s.+"), "#chan1"), 
                Times.Exactly(2));
        }

        [TestMethod]
        public void Message_Received_Match_Queries_Tumblr_Tagged()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new { response = _response });

            LoadModule();

            var msg = new MessageReceivedEventArgs(false, true, null, null, ValidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            const string regex = @"http:\/\/api\.tumblr\.com\/v2\/tagged\/.*tag=burger.*";
            _mockHttpClient.Verify(m => m.GetAsync(It.IsRegex(regex)));
        }

        [TestMethod]
        public void Message_Received_Match_Http_Exception_Caught()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .Throws(new HttpClientException(null, null));

            LoadModule();

            var msg = new MessageReceivedEventArgs(false, true, "#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            const string regex = @"JIM:\sWhoops, something went wrong";
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), "#chan1"), Times.Once);
        }

        [TestMethod]
        public void Message_Received_Match_No_Posts_Whoops()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {response = new dynamic[0]});

            LoadModule();

            var msg = new MessageReceivedEventArgs(false, true, "#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.MessageReceived += null, msg);

            const string regex = @"JIM:\sWhoops, no tumblrs found";
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), "#chan1"), Times.Once);
        }

        [TestMethod]
        public void Message_Received_Match_Replies_With_Photo()
        {
            _mockHttpClient
                    .Setup(m => m.GetAsync(It.IsAny<string>()))
                    .ReturnsAsync(new { response = _response });

            LoadModule();

            for (var i = 1; i <= 10; i++)
            {
                var msg = new MessageReceivedEventArgs(false, true, null, null, ValidCmd1);
                _mockChatClient.Raise(m => m.MessageReceived += null, msg);

                _mockChatClient.Verify(m => m.SendMessage(It.IsRegex("photo_url"), It.IsAny<string[]>()),
                    Times.Exactly(i));
            }
        }

        [TestMethod]
        public void Message_Received_Match_Replies_Randomly()
        {
            _mockHttpClient
                    .Setup(m => m.GetAsync(It.IsAny<string>()))
                    .ReturnsAsync(new { response = _response });

            LoadModule();

            for (var i = 1; i <= 10; i++)
            {
                var msg = new MessageReceivedEventArgs(false, true, null, null, ValidCmd1);
                _mockChatClient.Raise(m => m.MessageReceived += null, msg);
            }

            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex("photo_url_1"), It.IsAny<string[]>()),
                    Times.AtLeastOnce);
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex("photo_url_2"), It.IsAny<string[]>()),
                    Times.AtLeastOnce);
        }

        private void LoadModule()
        {
            _module.Loaded(_mockLoader.Object, null, _mockChatClient.Object, _mockHttpClient.Object);
        }
    }
}