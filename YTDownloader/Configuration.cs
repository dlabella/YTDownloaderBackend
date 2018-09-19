using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace YTDownloader
{
    [Serializable]
    public class Configuration
    {
        public string TempPath { get; set; }
        public string YoutubeDlBin { get; set; }
        public string Ip { get;set;}
        public int Port { get;set;}
        public string UrlPrefix { get;set;}
        public string Debug { get;set;}
        public string FilesDownloadPath { get;set;}
        public string AudioDownloadPath { get; set; }

        private Configuration()
        {
            TempPath = "C:\\tmp";
        }
        public static Configuration Load()
        {
            var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Environment.OSVersion.Platform == PlatformID.Win32NT ? "config.windows.json" : "config.linux.json");
            var data = File.ReadAllText(configFile);
            return JsonConvert.DeserializeObject<Configuration>(data);
        }
    }
}
