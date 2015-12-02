using System;

namespace Norbert.Irc
{
    public class PrivateMessageEventArgs : EventArgs
    {
        public bool IsChannelMessage { get; private set; }
        public string Source { get; private set; }
        public string Nick { get; private set; }
        public string Message { get; private set; }

        public PrivateMessageEventArgs(bool isChannelMessage, string source, string nick, string message)
        {
            IsChannelMessage = isChannelMessage;
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}