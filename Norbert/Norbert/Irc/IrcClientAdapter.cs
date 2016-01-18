using System;
using ChatSharp;
using ChatSharp.Events;

namespace Norbert.Irc
{
    public class IrcClientAdapter : IIrcClientAdapter
    {
        private readonly IrcClient _client;

        public string Nick => _client.User.Nick;

        public event EventHandler<EventArgs> ConnectionComplete = delegate { };
        public event EventHandler<RawMessageEventArgs> RawMessageSent = delegate { };
        public event EventHandler<RawMessageEventArgs> RawMessageReceived = delegate { };
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived = delegate { };

        public IrcClientAdapter(string serverAddress, string nick, string user)
        {
            _client = new IrcClient(serverAddress, new IrcUser(nick, user));
            _client.ConnectionComplete += (s, e) => ConnectionComplete(s, e);

            _client.RawMessageSent += (s, e) => RawMessageSent(s, e);
            _client.RawMessageRecieved += (s, e) => RawMessageReceived(s, e);

            _client.PrivateMessageRecieved += delegate(object s, ChatSharp.Events.PrivateMessageEventArgs e)
            {
                var msg = e.PrivateMessage;
                PrivateMessageReceived(s, new PrivateMessageEventArgs(msg.IsChannelMessage, msg.Source, 
                    msg.User.Nick, msg.Message));
            };
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
        }
    }
}