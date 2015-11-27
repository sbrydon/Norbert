using System;
using Norbert.Modules.Common.Events;

namespace Norbert.Irc
{
    public interface IIrcClientAdapter
    {
        event EventHandler<EventArgs> ConnectionComplete;
        event EventHandler<MessageEventArgs> MessageReceived;
        event EventHandler<MessageEventArgs> MessageSent;
        void ConnectAsync();
        void Quit(string reason);
        void JoinChannel(string channel);
        void SendMessage(string message, params string[] destinations);
    }
}