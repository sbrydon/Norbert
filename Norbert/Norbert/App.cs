﻿using System;
using System.Configuration;
using log4net;
using Norbert.Exceptions;
using Norbert.Irc;

namespace Norbert
{
    class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));

        private static Config _config;
        private static ModuleManager _moduleManager;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Norbert, press any key to exit");
            Console.WriteLine();
            Log.Info("Norbert started");

            try
            {
                _config = new Config(ConfigurationManager.AppSettings);
            }
            catch (ConfigException e)
            {
                Log.Fatal(e.Message);
                Log.Info("Norbert ended");
                return;
            }

            var adapter = new IrcClientAdapter(_config.Server, _config.Nick, _config.User);
            var client = new ChatClient(_config, adapter);
            _moduleManager = new ModuleManager(new ConfigLoader("Modules"), new FileSystem(),
                client, new HttpService(_config.GoogleApiKey), new Randomiser());

            try
            {
                _moduleManager.LoadModules();
            }
            catch (LoadModuleException e)
            {
                Log.Fatal(e.Message);
                Log.Info("Norbert ended");
                return;
            }
            
            client.Connect();
            Console.ReadKey();

            client.Disconnect();
            _moduleManager.UnloadModules();

            Log.Info("Norbert ended");
        }
    }
}
