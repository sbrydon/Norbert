using System;
using System.IO;
using Newtonsoft.Json;
using Norbert.Modules.Common;
using Norbert.Modules.Common.Exceptions;

namespace Norbert.Cli
{
    public class ConfigLoader : IConfigLoader
    {
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
                throw new LoadConfigException(path, e.Message);
            }
        }
    }
}