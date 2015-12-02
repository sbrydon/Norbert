using System;

namespace Norbert.Modules.Common.Events
{
    public class CommandEventArgs : EventArgs
    {
        public string Source { get; }
        public string Nick { get; }
        public string Message { get; }

        public CommandEventArgs(string source, string nick, string message)
        {
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}