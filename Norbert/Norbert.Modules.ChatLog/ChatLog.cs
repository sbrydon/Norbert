using System;
using System.IO;
using log4net;
using Norbert.Modules.Common;

namespace Norbert.Modules.ChatLog
{
    public class ChatLog : INorbertModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatLog));
        private IChatClient _client;
        private string _path;

        public void Loaded(IChatClient client)
        {
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
            var path = Properties.Settings.Default.Path;
            if (string.IsNullOrWhiteSpace(path))
            {
                _path = $"{AppDomain.CurrentDomain.BaseDirectory}ChatLogs";
                Log.Info("No path specified, using default");
            }
            else
            {
                _path = path;
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
