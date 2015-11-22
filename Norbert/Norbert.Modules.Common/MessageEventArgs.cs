using System;

namespace Norbert.Modules.Common
{
    public class MessageEventArgs : EventArgs
    {
        public bool IsPrivateMessage { get; }

        public string Source { get; }

        public string Nick { get; }

        public string Message { get; }

        public MessageEventArgs(bool isPrivateMessage, string source, string nick, string message)
        {
            IsPrivateMessage = isPrivateMessage;
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}