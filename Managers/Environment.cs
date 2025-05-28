using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;

//
//      Manages configuration data
//

namespace Managers
{
    public static class Env
    {
        private static bool useLogger;
        
        // Handles local logging before AND after Log is prepared, since the logging manager relies on Env data, so Env is prepared first. chicken-or-the-egg and all that.
        private static void Msg(string msg)
        {
            if (useLogger)
            {
                Log.Loud(msg);
                return;
            }
            
            MelonLogger.Msg(msg);
            File.AppendAllText(LogfilePath, "| " + msg + "\n");
        }
        
        public static void Prepare()
        {
            File.Create(LogfilePath).Close();
            
            File.WriteAllText(LogfilePath, string.Empty);
            
            if (!Directory.Exists(MelonEnvironment.UserDataDirectory + @"\SoundBending\"))
            {
                Directory.CreateDirectory(MelonEnvironment.UserDataDirectory + @"\SoundBending\");
                
                Msg("[SoundBending.Managers.Env] Prepare: Root directory doesn't exist! Fixing your mistakes...");
            }

            if (!File.Exists(ConfigPath))
            {
                File.Create(ConfigPath).Close();
                
                // forgive me for my crimes
                File.WriteAllText(ConfigPath, 
@"{
    ""Services"": [""Soundboard"", ""HighNoon"", ""NoMute"", ""NamebendingIntegration""],

    ""Devices"": {
        ""Input"": ""PLEASE FILL THIS FROM LOG INFO "",
        ""Output"": ""PLEASE FILL THIS FROM LOG INFO "",
        ""PipeOutput"": ""PLEASE FILL THIS FROM LOG INFO ""
    },

    ""NamebendingIntegration"": {
        ""Prefer"": ""customBentName"",
        ""Strict"": true
    },

    ""LolLevel"": 2
}");

                Msg("[SoundBending.Managers.Env] Prepare: You forgot the configuration file, goober.");
            }
            
            Directory.CreateDirectory(SoundRoot);
            Directory.CreateDirectory(SfxRoot);
            
            ReloadConfiguration();
            
            if (Config.LogLevel > 0)
            {
                Msg("[SoundBending.Managers.Env] Prepare: Environment initialized.");
            }
            
            useLogger = true;
        }

        public static readonly string SoundRoot = MelonEnvironment.UserDataDirectory + @"\SoundBending\Sounds\";
        public static readonly string SfxRoot = MelonEnvironment.UserDataDirectory + @"\SoundBending\Sfx\";
        
        public static readonly string NBSoundRoot = MelonEnvironment.UserDataDirectory + @"\NameBending\soundbendingSounds\";

        private static readonly string ConfigPath = MelonEnvironment.UserDataDirectory + @"\SoundBending\Config.json";

        public static readonly string NBNamePath = MelonEnvironment.UserDataDirectory + @"\NameBending\customBentName.json";
        public static readonly string NBTitlePath = MelonEnvironment.UserDataDirectory + @"\NameBending\customBentTitle.json";
        
        public static readonly string LogfilePath = MelonEnvironment.UserDataDirectory + @"\SoundBending\Latest.log";

        public static bool IsServiceEnabled(string serviceName) => Config.Services.Contains(serviceName);
        
        public static string InputDevice => Config.Devices.Input;
        public static string OutputDevice => Config.Devices.Output;
        public static string OutputPipe => Config.Devices.PipeOutput;
        
        public static int LogLevel => Config.LogLevel;
        

        public static Configuration Config { get; private set; }
        
        public struct Configuration
        {
            public struct DeviceMap
            {
                public string Input;
                public string Output;
                public string PipeOutput;
            }

            public struct NamebendingIntegrationSettings
            {
                public string Prefer;
                public bool Strict;
            }
            
            public List<string> Services;
            
            public NamebendingIntegrationSettings NamebendingIntegration;

            public DeviceMap Devices;

            public int LogLevel;
        }

        public static void ReloadConfiguration()
        {
            string json = File.ReadAllText(ConfigPath);
            
            Config = JsonConvert.DeserializeObject<Configuration>(json);
            
            if (Config.LogLevel > 0) Msg("[SoundBending.Managers.Env] > ReloadConfiguration: Configuration loaded with log level " + Config.LogLevel);
        }
    }
}