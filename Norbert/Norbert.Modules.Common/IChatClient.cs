using System;

namespace Norbert.Modules.Common
{
    public interface IChatClient
    {
        event EventHandler<MessageEventArgs> MessageReceived;
        void SendMessage(string message, params string[] destinations);
    }
}