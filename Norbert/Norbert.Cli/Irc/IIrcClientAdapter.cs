using System;
using Norbert.Modules.Common;

namespace Norbert.Cli.Irc
{
    public interface IIrcClientAdapter
    {
        event EventHandler<EventArgs> ConnectionComplete;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        void ConnectAsync();
        void Quit(string reason);
        void JoinChannel(string channel);
        void SendMessage(string message, params string[] destinations);
    }
}