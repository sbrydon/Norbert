﻿using System;

namespace Norbert.Modules.Common.Events
{
    public class MessageEventArgs : EventArgs
    {
        public bool IsCommand { get; }
        public string Source { get; }
        public string Nick { get; }
        public string Message { get; }

        public MessageEventArgs(bool isCommand, string source, string nick, string message)
        {
            IsCommand = isCommand;
            Source = source;
            Nick = nick;
            Message = message;
        }
    }
}