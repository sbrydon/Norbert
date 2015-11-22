using System;

namespace Norbert.Modules.Common
{
    public interface IChatClient
    {
        void SendMessage(string message, params string[] destinations);
    }
}