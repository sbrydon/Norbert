using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Norbert.Cli.Exceptions;
using Norbert.Modules.Common;

namespace Norbert.Cli
{
    public class ModuleManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ModuleManager));
        private readonly IConfigLoader _configLoader;
        private readonly IFileSystem _fileSystem;
        private readonly IChatClient _client;
        private readonly IHttpService _httpService;
        private readonly List<INorbertModule> _modules = new List<INorbertModule>();

        public ModuleManager(IConfigLoader configLoader, IFileSystem fileSystem, 
            IChatClient client, IHttpService httpService)
        {
            _configLoader = configLoader;
            _fileSystem = fileSystem;
            _client = client;
            _httpService = httpService;
        }

        public void LoadModules()
        {
            Log.Info("Loading modules..");

            var modulesPath = $"{AppDomain.CurrentDomain.BaseDirectory}Modules";
            var files = Directory
                .EnumerateFiles(modulesPath, "*.dll", SearchOption.AllDirectories)
                .ToArray();

            if (!files.Any())
                Log.Warn("No modules found!");

            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFile(file);
                    var typeName = assembly.GetTypes().Single(t => typeof (INorbertModule).IsAssignableFrom(t)).FullName;
                    var type = assembly.GetType(typeName);

                    var module = (INorbertModule) Activator.CreateInstance(type);
                    _modules.Add(module);

                    module.Loaded(_configLoader, _fileSystem, _client, _httpService);
                    Log.Info($"{module.GetType().Name} loaded");
                }
                catch (Exception e)
                {
                    throw new LoadModuleException(file, e.Message);
                }
            }
        }

        public void UnloadModules()
        {
            Log.Info("Unloading modules..");

            foreach (var module in _modules)
            {
                module.Unloaded();
                Log.Info($"{module.GetType().Name} unloaded");
            }
        }
    }
}