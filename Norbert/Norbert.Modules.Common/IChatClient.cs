using System;
using Norbert.Modules.Common.Events;

namespace Norbert.Modules.Common
{
    public interface IChatClient
    {
        event EventHandler<MessageEventArgs> MessageReceived;
        event EventHandler<MessageEventArgs> MessageSent;
        void SendMessage(string message, params string[] destinations);
    }
}