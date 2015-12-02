using System;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Norbert.Modules.Common;

namespace Norbert
{
    public class ConfigLoader : IConfigLoader
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConfigLoader));
        private readonly string _basePath;

        public ConfigLoader(string basePath)
        {
            _basePath = basePath;
        }

        public T Load<T>(string path)
        {
            path = $"{_basePath}/{path}";

            try
            {
                var config = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                return config;
            }
            catch (Exception e)
            {
                Log.Error($"Error loading '{path}': {e.Message}");
                return default(T);
            }
        }
    }
}