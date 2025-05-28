using System.Collections.Generic;
using MelonLoader;
using Managers;
using Services;

namespace SoundBending
{
    public static class BuildInfo
    {
        public const string Name = "SoundBending"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "All the sound stuff"; // Description for the Mod.  (Set as null if none)
        public const string Author = "rdm8417"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.9.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Mod : MelonMod
    {
        // holds whether Startup has run so that it only gets called once.
        private bool startupComplete;
        
        // simply utility for OnApplicationStart / Activity check
        private static string fmtActivity(string serviceName)
        {
            return Env.IsServiceEnabled(serviceName) ? "ACTIVE" : "inactive";
        }

        private void Startup(string sceneName) => Log.Wrap(
        "[SoundBending.Mod] > Startup", null, null, () => {
            Env.Prepare();
            Log.Prepare();
            State.Prepare();
            Audio.Prepare();
            Input.Prepare();
            SoundboardActions.Prepare();

            MelonCoroutines.Start(Audio.Feed(Audio.MicIn, Audio.InputCableOut));
            
            RefreshServices(sceneName);

            Log.Open("/ Activity check;");

            Log.Quiet("HighNoon: " + fmtActivity("HighNoon"));
            Log.Quiet("NoMute: " + fmtActivity("NoMute"));
            Log.Quiet("NamebendingIntegration: " + fmtActivity("NamebendingIntegration"));
            Log.Quiet("Soundboard: " + fmtActivity("Soundboard"));

            Log.Close("/ Activity check;");

            Log.Loud("SoundBending active!");
        });

        public override void OnUpdate()
        {
            if (!startupComplete)
            {
                return;
            }
            
            Log.WrapLazy("[SoundBending.Mod] OnUpdate", null, null, () => {
                if (Env.IsServiceEnabled("HighNoon")) HighNoonService.Run();
                if (Env.IsServiceEnabled("NoMute")) NoMuteService.Run();
                if (Env.IsServiceEnabled("NamebendingIntegration")) NamebendingIntegrationService.Run();
                if (Env.IsServiceEnabled("Soundboard")) SoundboardService.Run();
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!startupComplete)
            {
                if (sceneName != "Gym") return;
                
                Startup(sceneName);
                startupComplete = true;
                
                return;
            }
            
            Log.Wrap("[SoundBending.Mod] OnSceneWasLoaded", "Refreshing services...", "Services refreshed & SceneLoaded events called!", () => {
                RefreshServices(sceneName);
        
                if (Env.IsServiceEnabled("HighNoon")) HighNoonService.SceneLoaded(sceneName);
                if (Env.IsServiceEnabled("NoMute")) NoMuteService.SceneLoaded(sceneName);
                if (Env.IsServiceEnabled("NamebendingIntegration")) NamebendingIntegrationService.SceneLoaded(sceneName);
                if (Env.IsServiceEnabled("Soundboard")) SoundboardService.SceneLoaded(sceneName);
            });
        }

        
        private void RefreshServices(string sceneName) => Log.Wrap(
        "[SoundBending.Mod] > RefreshServices", null, null, () => {
            // on first run, assume no services are currently on.
            List<string> prev = startupComplete ? Env.Config.Services : new List<string>();

            Env.ReloadConfiguration();
            State.Prepare();

            List<string> curr = Env.Config.Services;

            // enables any new services
            if (curr.Contains("HighNoon") && !prev.Contains("HighNoon"))
            {
                HighNoonService.Setup();
            }

            if (curr.Contains("NoMute") && !prev.Contains("NoMute"))
            {
                NoMuteService.Setup();
            }

            if (curr.Contains("Soundboard") && !prev.Contains("Soundboard"))
            {
                SoundboardService.Setup();
            }

            if (curr.Contains("NamebendingIntegration") && !prev.Contains("NamebendingIntegration"))
            {
                NamebendingIntegrationService.Setup();
            }


            // disables any old services
            if (!curr.Contains("HighNoon") && prev.Contains("HighNoon"))
            {
                HighNoonService.Cleanup();
            }

            if (!curr.Contains("NoMute") && prev.Contains("NoMute"))
            {
                NoMuteService.Cleanup();
            }

            if (!curr.Contains("Soundboard") && prev.Contains("Soundboard"))
            {
                SoundboardService.Cleanup();
            }

            if (!curr.Contains("NamebendingIntegration") && prev.Contains("NamebendingIntegration"))
            {
                NamebendingIntegrationService.Cleanup();
            }

            // runs any service scene load handlers
            if (Env.IsServiceEnabled("HighNoon"))
            {
                HighNoonService.SceneLoaded(sceneName);
            }

            if (Env.IsServiceEnabled("NoMute"))
            {
                NoMuteService.SceneLoaded(sceneName);
            }

            if (Env.IsServiceEnabled("NamebendingIntegration"))
            {
                NamebendingIntegrationService.SceneLoaded(sceneName);
                return;
            }

            if (Env.IsServiceEnabled("Soundboard"))
            {
                SoundboardService.SceneLoaded(sceneName);
            }
        });
    }
}