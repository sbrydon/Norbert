using System;
using ChatSharp;
using ChatSharp.Events;
using Norbert.Modules.Common.Events;

namespace Norbert.Cli.Irc
{
    public class IrcClientAdapter : IIrcClientAdapter
    {
        private readonly IrcClient _client;

        public event EventHandler<EventArgs> ConnectionComplete = delegate { };
        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };

        public IrcClientAdapter(string serverAddress, string nick, string user)
        {
            _client = new IrcClient(serverAddress, new IrcUser(nick, user));
            _client.ConnectionComplete += (s, e) => ConnectionComplete(s, e);
            _client.PrivateMessageRecieved += delegate(object s, PrivateMessageEventArgs e)
            {
                var msg = e.PrivateMessage;
                var msgEventArgs = new MessageReceivedEventArgs(!msg.IsChannelMessage, 
                    msg.Message.StartsWith(nick), msg.Source, msg.User.Nick, msg.Message);

                MessageReceived(s, msgEventArgs);
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