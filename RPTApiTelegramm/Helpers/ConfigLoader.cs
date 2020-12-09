using Newtonsoft.Json;
using System;
using System.IO;
namespace RPTApi.Helpers
{
    public static class ConfigLoader
    {
        public static Config GetConfig(string cfgFilePath = "../bot.cfg")
        {
            if (!File.Exists(cfgFilePath))
                throw new ArgumentException($"Файл конфигурации не был обнаружен по задданному пути {cfgFilePath}");
            using (StreamReader sr = new StreamReader(cfgFilePath))
            {
                var json = sr.ReadToEnd();
                var cfg = JsonConvert.DeserializeObject<Config>(json);
                return cfg;
            }    
        }
    }
}
