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
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
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
            
            if (Env.IsServiceEnabled("HighNoon")) HighNoonService.Setup();
            if (Env.IsServiceEnabled("NoMute")) NoMuteService.Setup();
            if (Env.IsServiceEnabled("NamebendingIntegration")) NamebendingIntegrationService.Setup();
            if (Env.IsServiceEnabled("Soundboard")) SoundboardService.Setup();

            Log.Open("/ Activity check;");

            Log.Quiet("HighNoon: " + fmtActivity("HighNoon"));
            Log.Quiet("NoMute: " + fmtActivity("NoMute"));
            Log.Quiet("NamebendingIntegration: " + fmtActivity("NamebendingIntegration"));
            Log.Quiet("Soundboard: " + fmtActivity("Soundboard"));

            Log.Close("/ Activity check;");

            Log.Loud("SoundBending active!");
        });

        public override void OnLateUpdate()
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

        // This is called slightly after the usual "OnSceneWasLoaded", hopefully keeping SoundBending from causing issues.
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            // On first gym load: go to startup. Avoids RumbleModdingAPI not being quite ready yet.
            if (!startupComplete)
            {
                if (sceneName != "Gym") return;
                
                Startup(sceneName);
                startupComplete = true;
                
                return;
            }
            
            // Every other load (except Loader), refresh services and called scene loaders. General bookkeeping stuff, hence the hard logging wrap.
            Log.Wrap("[SoundBending.Mod] OnSceneWasLoaded", "Refreshing services...", "Services refreshed & SceneLoaded events called!", () => {
                RefreshServices(sceneName);
        
                if (Env.IsServiceEnabled("HighNoon")) HighNoonService.SceneLoaded(sceneName);
                if (Env.IsServiceEnabled("NoMute")) NoMuteService.SceneLoaded(sceneName);
                if (Env.IsServiceEnabled("NamebendingIntegration")) NamebendingIntegrationService.SceneLoaded(sceneName);
                if (Env.IsServiceEnabled("Soundboard")) SoundboardService.SceneLoaded(sceneName);
            });
        }

        public override void OnApplicationQuit()
        {
            Log.Open("[SoundBending.Mod] OnApplicationQuit;");
            
            if (Env.IsServiceEnabled("HighNoon")) HighNoonService.Cleanup();
            if (Env.IsServiceEnabled("NoMute")) NoMuteService.Cleanup();
            if (Env.IsServiceEnabled("NamebendingIntegration")) NamebendingIntegrationService.Cleanup();
            if (Env.IsServiceEnabled("Soundboard")) SoundboardService.Cleanup();
        
            Log.Loud("/ Deinitializing managers;");

            SoundboardActions.Deinit();
            Input.Deinit();
            Audio.Deinit();
            State.Deinit();
            Log.Deinit();
            Env.Deinit();
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