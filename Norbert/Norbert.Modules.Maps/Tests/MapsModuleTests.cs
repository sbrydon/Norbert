using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.Maps.Tests
{
    [TestClass]
    public class MapsModuleTests
    {
        private const string ValidCmd1 = "where is bob";
        private const string ValidCmd2 = "where's bob";
        private const string InvalidCmd1 = "doughnut";
        private const string InvalidCmd2 = "where is";

        private MapsModule _module;
        private Mock<IConfigLoader> _mockLoader;
        private Mock<IChatClient> _mockChatClient;
        private Mock<IHttpClient> _mockHttpClient;
        private dynamic _results;

        [TestInitialize]
        public void Initialise()
        {
            _module = new MapsModule();

            _mockLoader = new Mock<IConfigLoader>();
            _mockLoader
                .Setup(m => m.Load<Config>(It.IsAny<string>()))
                .Returns(new Config());

            _mockChatClient = new Mock<IChatClient>();
            _mockHttpClient = new Mock<IHttpClient>();

            _results = new[]
            {
                new
                {
                    address_components = new dynamic[]
                    {
                        new {types = new[] {"political"}, long_name = "1: City"},
                        new {types = new[] {"political"}, long_name = "1: State"},
                        new {types = new[] {"political"}, long_name = "1: Country"},
                        new {types = new[] {"postal_code"}, long_name = "1: Postcode"}
                    }
                },
                new
                {
                    address_components = new dynamic[]
                    {
                        new {types = new[] {"political"}, long_name = "2: Town"},
                        new {types = new[] {"political"}, long_name = "2: City"},
                        new {types = new[] {"political"}, long_name = "2: State"},
                        new {types = new[] {"political"}, long_name = "2: Country"},
                        new {types = new[] {"postal_code"}, long_name = "2: Postcode"}
                    }
                },
                new
                {
                    address_components = new dynamic[]
                    {
                        new {types = new[] {"political"}, long_name = "3: State"},
                        new {types = new[] {"political"}, long_name = "3: Country"},
                        new {types = new[] {"postal_code"}, long_name = "3: Postcode"}
                    }
                },
                new
                {
                    address_components = new dynamic[]
                    {
                        new {types = new[] {"political"}, long_name = "4: Country"},
                        new {types = new[] {"postal_code"}, long_name = "4: Postcode"}
                    }
                }
            };
        }

        [TestMethod]
        public void Loaded_Loads_Config()
        {
            const string path = "Maps/Config.json";

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
        public void Command_Received_Non_Match_Or_Empty_Ignored()
        {
            LoadModule();

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
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {results = _results});
            _mockHttpClient
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new {id = "http://short.url"});

            LoadModule();

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);
            cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd2);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(@"JIM:\s.+"), "#chan1"),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Command_Received_Match_Reverse_Geocodes()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {results = _results});
            _mockHttpClient
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new {id = "http://short.url"});

            LoadModule();

            var cmd = new CommandEventArgs(null, null, ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string regex = @"https:\/\/maps\.googleapis\.com\/maps\/api\/geocode\/json.+";
            _mockHttpClient.Verify(m => m.GetAsync(It.IsRegex(regex)));
        }

        [TestMethod]
        public void Command_Received_Match_Shortens_Url()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {results = _results});
            _mockHttpClient
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new {id = "http://short.url"});

            LoadModule();

            var cmd = new CommandEventArgs(null, null, ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string regex = @"https:\/\/www\.googleapis\.com\/urlshortener\/v1\/url.+";
            _mockHttpClient.Verify(m => m.PostAsync(It.IsRegex(regex), It.IsAny<object>()));
        }

        [TestMethod]
        public void Command_Received_Match_Trims()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {results = _results});
            _mockHttpClient
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new {id = "http://short.url"});

            LoadModule();

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1 + " ");
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(@"JIM:\sbob is.+"), "#chan1"));
        }

        [TestMethod]
        public void Command_Received_Match_Reverse_Geocode_Http_Exception_Caught()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .Throws(new HttpClientException(null, null));

            LoadModule();

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string regex = @"JIM:\sWhoops, something went wrong";
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), "#chan1"), Times.Once);
        }

        [TestMethod]
        public void Command_Received_Match_Shorten_Url_Http_Exception_Caught()
        {
            _mockHttpClient
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new {results = _results});
            _mockHttpClient
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Throws(new HttpClientException(null, null));

            LoadModule();

            var cmd = new CommandEventArgs("#chan1", "JIM", ValidCmd1);
            _mockChatClient.Raise(m => m.CommandReceived += null, cmd);

            const string regex = @"JIM:\sWhoops, something went wrong";
            _mockChatClient.Verify(m => m.SendMessage(It.IsRegex(regex), "#chan1"), Times.Once);
        }

        private void LoadModule()
        {
            _module.Loaded(_mockLoader.Object, null, _mockChatClient.Object, _mockHttpClient.Object);
        }
    }
}