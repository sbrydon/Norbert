using System;
using log4net;
using Norbert.Modules.Common;

namespace Norbert.Modules.ChatLog
{
    public class ChatLogModule : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatLogModule));
        // ReSharper disable once NotAccessedField.Local
        private ChatListener _chatListener;

        public void Loaded(IConfigLoader configLoader, IChatClient chatClient, 
            IHttpClient httpClient, IFileSystem fileSystem, IRandomiser randomiser)
        {
            var config = LoadConfig(configLoader, fileSystem);
            _chatListener = new ChatListener(fileSystem, chatClient, config);
        }

        public void Unloaded()
        {
        }

        private static Config LoadConfig(IConfigLoader configLoader, IFileSystem fileSystem)
        {
            var config = configLoader.Load<Config>("ChatLog/Config.json") ?? new Config();

            if (string.IsNullOrWhiteSpace(config.Path))
            {
                config.Path = "ChatLogs";
                Log.Info("No path specified, using 'ChatLogs'");
            }
            else
            {
                config.Path = config.Path;
            }

            try
            {
                if (fileSystem.DirectoryExists(config.Path))
                {
                    Log.Info("Path exists, skipping creation");
                }
                else
                {
                    Log.Info("Path does not exist, creating it..");
                    fileSystem.CreateDirectory(config.Path);
                }

                Log.Info($"Path is '{config.Path}'");
            }
            catch (Exception e)
            {
                Log.Error($"Error creating '{config.Path}': {e.Message}");
                Log.Error("Path not set");
            }

            return config;
        }
    }
}
