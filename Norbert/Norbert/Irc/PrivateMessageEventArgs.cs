using System;

namespace Norbert.Irc
{
    public class PrivateMessageEventArgs : EventArgs
    {
        public bool IsChannelMessage { get; }
        public string Source { get; }
        public string Nick { get; }
        public string Message { get; }

        public PrivateMessageEventArgs(bool isChannelMessage, string source, string nick, string message)
        {
            IsChannelMessage = isChannelMessage;
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}