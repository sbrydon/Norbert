using System;

namespace Norbert.Modules.Common.Events
{
    public class MessageEventArgs : EventArgs
    {
        public bool IsPrivate { get; }
        public bool IsCommand { get; }
        public string Source { get; }
        public string Nick { get; }
        public string Message { get; }

        public MessageEventArgs(bool isPrivate, bool isCommand, string source, string nick, string message)
        {
            IsPrivate = isPrivate;
            IsCommand = isCommand;
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}