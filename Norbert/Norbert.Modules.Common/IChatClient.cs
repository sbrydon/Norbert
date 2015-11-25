using System;
using Norbert.Modules.Common.Events;

namespace Norbert.Modules.Common
{
    public interface IChatClient
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        void SendMessage(string message, params string[] destinations);
    }
}