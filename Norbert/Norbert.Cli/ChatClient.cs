using System;
using ChatSharp;
using ChatSharp.Events;
using log4net;
using Norbert.Modules.Common;

namespace Norbert.Cli
{
    public class ChatClient : IChatClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatClient));
        private readonly Config _config;
        private readonly IrcClient _client;

        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };

        public ChatClient(Config config)
        {
            _config = config;
            _client = new IrcClient(_config.Server, new IrcUser(_config.Nick, _config.User));

            _client.ConnectionComplete += (s, e) =>
            {
                Log.Info($"Connected to {_config.Server}, joining channels..");

                foreach (var channel in _config.Channels)
                {
                    _client.JoinChannel(channel);
                    Log.Info($"Joined {_config.Server}/{channel}");
                }
            };

            _client.PrivateMessageRecieved += OnPrivateMessageReceived;
        }

        public void Connect()
        {
            _client.ConnectAsync();
            Log.Info($"Connecting to {_config.Server}..");
        }

        public void Disconnect()
        {
            _client.Quit(_config.QuitMsg);
            Log.Info($"Disconnected from {_config.Server}, reason: Quit");
        }

        public void SendMessage(string message, params string[] destinations)
        {
            _client.SendMessage(message, destinations);
        }

        private void OnPrivateMessageReceived(object sender, PrivateMessageEventArgs eventArgs)
        {
            var msg = eventArgs.PrivateMessage;
            var msgEventArgs = new MessageEventArgs(!msg.IsChannelMessage, msg.Source, msg.User.Nick, msg.Message);
            MessageReceived(this, msgEventArgs);
        }
    }
}