using System;
using ChatSharp;
using ChatSharp.Events;
using log4net;
using Norbert.Modules.Common.Events;

namespace Norbert.Cli.Irc
{
    public class IrcClientAdapter : IIrcClientAdapter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IrcClientAdapter));
        private readonly IrcClient _client;

        public event EventHandler<EventArgs> ConnectionComplete = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageEventArgs> MessageSent = delegate { };

        public IrcClientAdapter(string serverAddress, string nick, string user)
        {
            _client = new IrcClient(serverAddress, new IrcUser(nick, user));
            _client.ConnectionComplete += (s, e) => ConnectionComplete(s, e);

            _client.PrivateMessageRecieved += delegate(object s, PrivateMessageEventArgs e)
            {
                var msg = e.PrivateMessage;
                var msgEventArgs = new MessageEventArgs(!msg.IsChannelMessage, 
                    msg.Message.StartsWith(nick), msg.Source, msg.User.Nick, msg.Message);

                MessageReceived(s, msgEventArgs);
            };

            _client.RawMessageRecieved += (s, e) => Log.Debug($"<- {e.Message}");
            _client.RawMessageSent += (s, e) => Log.Debug($"-> {e.Message}");
        }

        public void ConnectAsync()
        {
            _client.ConnectAsync();
        }

        public void Quit(string reason)
        {
            _client.Quit(reason);
        }

        public void JoinChannel(string channel)
        {
            _client.JoinChannel(channel);
        }

        public void SendMessage(string message, params string[] destinations)
        {
            _client.SendMessage(message, destinations);

            var nick = _client.User.Nick;
            foreach (var dest in destinations)
            {
                var isPrivate = dest.Contains("#");
                var eventArgs = new MessageEventArgs(isPrivate, false, dest, nick, message);
                MessageSent(_client, eventArgs);
            }
        }
    }
}