using System;
using System.Net.Sockets;
using ChatSharp.Events;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Events;

namespace Norbert.Irc
{
    public class ChatClient : IChatClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatClient));
        private readonly Config _config;
        private readonly IIrcClientAdapter _adapter;

        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageEventArgs> MessageSent = delegate { }; 

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

            _adapter.RawMessageReceived += delegate (object s, RawMessageEventArgs e)
            {
                if (e.Message.Contains("PONG"))
                    return;

                Log.Debug($"<- {e.Message}");
            };

            _adapter.RawMessageSent += delegate (object s, RawMessageEventArgs e)
            {
                if (e.Message.Contains("PING"))
                    return;

                Log.Debug($"-> {e.Message}");
            };

            _adapter.PrivateMessageReceived += delegate (object s, PrivateMessageEventArgs e)
            {
                MessageReceived(s, new MessageEventArgs(!e.IsChannelMessage, e.Message.StartsWith(_adapter.Nick),
                    e.Source, e.Nick, e.Message));
            };
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
            Log.Debug($"Sending: '{message}' -> {string.Join(",", destinations)}");
            _adapter.SendMessage(message, destinations);

            var nick = _adapter.Nick;
            foreach (var dest in destinations)
            {
                var isPrivate = dest.Contains("#");
                var eventArgs = new MessageEventArgs(isPrivate, false, dest, nick, message);
                MessageSent(_adapter, eventArgs);
            }
        }
    }
}