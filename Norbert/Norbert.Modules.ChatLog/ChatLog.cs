using Norbert.Modules.Common;

namespace Norbert.Modules.ChatLog
{
    class ChatLog : INorbertModule
    {
        private IChatClient _client;

        public void Initialise(IChatClient client)
        {
            _client = client;
        }
    }
}
