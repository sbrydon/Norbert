using System;

namespace Norbert.Modules.Common
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public bool IsPrivateMessage { get; }
        public string Source { get; }
        public string Nick { get; }
        public string Message { get; }

        public MessageReceivedEventArgs(bool isPrivateMessage, string source, string nick, string message)
        {
            IsPrivateMessage = isPrivateMessage;
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}