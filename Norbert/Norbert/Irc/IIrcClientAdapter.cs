using System;
using ChatSharp.Events;

namespace Norbert.Irc
{
    public interface IIrcClientAdapter
    {
        string Nick { get; }
        event EventHandler<EventArgs> ConnectionComplete;
        event EventHandler<RawMessageEventArgs> RawMessageSent;
        event EventHandler<RawMessageEventArgs> RawMessageReceived;
        event EventHandler<PrivateMessageEventArgs> PrivateMessageReceived;
        void ConnectAsync();
        void Quit(string reason);
        void JoinChannel(string channel);
        void SendMessage(string message, params string[] destinations);
    }
}