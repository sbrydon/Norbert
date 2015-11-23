using System;
using System.IO;
using log4net;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Modules.ChatLog
{
    public class ChatLog : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatLog));
        private ConfigLoader _configLoader;
        private IChatClient _client;
        private string _path;

        public void Loaded(ConfigLoader configLoader, IChatClient client)
        {
            _configLoader = configLoader;
            _client = client;

            _client.MessageReceived += OnMessageReceived;
            SetupPath();
        }

        public void Unloaded()
        {
        }

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            AppendToLog(eventArgs);
        }

        private void SetupPath()
        {
            var config = new Config();
            try
            {
                config = _configLoader.Load<Config>("ChatLog/Config.json");
            }
            catch (LoadConfigException e)
            {
                Log.Error(e.Message);
            }

            if (string.IsNullOrWhiteSpace(config.Path))
            {
                _path = "ChatLogs";
                Log.Info("No path specified, using default");
            }
            else
            {
                _path = config.Path;
            }

            try
            {
                if (Directory.Exists(_path))
                {
                    Log.Info("Path exists, skipping creation");
                }
                else
                {
                    Log.Info("Path does not exist, creating it..");
                    Directory.CreateDirectory(_path);
                }

                Log.Info($"Path is {_path}");
            }
            catch (Exception e)
            {
                Log.Error($"Error creating {_path}: {e.Message}");
                Log.Error("Path not set");
            }
        }

        private void AppendToLog(MessageEventArgs msg)
        {
            var file = $"{msg.Source}.log";

            try
            {
                using (var writer = File.AppendText($"{_path}/{file}"))
                {
                    var entry = $"[{DateTime.Now}] <{msg.Nick}> {msg.Message}";
                    Log.Debug($"{file}: {entry}");
                    writer.WriteLine(entry);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error writing to {msg.Source}.log: {e.Message}");
            }
        }
    }
}
