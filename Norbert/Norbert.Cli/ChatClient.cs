using System;
using ChatSharp;
using ChatSharp.Events;
using Norbert.Modules.Common;

namespace Norbert.Cli
{
    public class ChatClient : IChatClient
    {
        private readonly IrcClient _client;

        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };

        public ChatClient(IrcClient client)
        {
            _client = client;
            _client.PrivateMessageRecieved += OnPrivateMessageReceived;
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