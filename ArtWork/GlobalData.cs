﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace ArtWork
{
    internal class GlobalData
    {
        public static void Init()
        {
            if (File.Exists(AppConfig.SavePath))
            {
                try
                {
                    var json = File.ReadAllText(AppConfig.SavePath);
                    Config = JsonConvert.DeserializeObject<AppConfig>(json);
                }
                catch
                {
                    Config = new AppConfig();
                }
            }
            else
            {
                Config = new AppConfig();
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Config);
            File.WriteAllText(AppConfig.SavePath, json);
        }

        public static AppConfig Config { get; set; }

        internal class AppConfig
        {
            public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}AppConfig.json";

            public string DataPath { get; set; } = Environment.CurrentDirectory + @"\data";
            public string Lang { get; set; } = "en-US";
        }
    }
}