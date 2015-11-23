using System;
using System.Net.Sockets;
using log4net;
using Norbert.Modules.Common;

namespace Norbert.Cli.Irc
{
    public class ChatClient : IChatClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatClient));
        private readonly Config _config;
        private readonly IIrcClientAdapter _adapter;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };

        public ChatClient(Config config, IIrcClientAdapter adapter)
        {
            _config = config;
            _adapter = adapter;

            _adapter.ConnectionComplete += (s, e) =>
            {
                Log.Info($"Connected to {_config.Server}, joining channels..");

                foreach (var channel in _config.Channels)
                {
                    _adapter.JoinChannel(channel);
                    Log.Info($"Joined {_config.Server}/{channel}");
                }
            };

            _adapter.MessageReceived += (s, e) => MessageReceived(s, e);
        }

        public void Connect()
        {
            _adapter.ConnectAsync();
            Log.Info($"Connecting to {_config.Server}..");
        }

        public void Disconnect()
        {
            try
            {
                _adapter.Quit(_config.QuitMsg);
            }
            catch (SocketException)
            {
                Log.Error($"Socket error when sending 'Quit' to {_config.Server}");
            }

            Log.Info($"Disconnected from {_config.Server}, reason: Quit");
        }

        public void SendMessage(string message, params string[] destinations)
        {
            _adapter.SendMessage(message, destinations);
        }
    }
}