//
//          The owning service for the "NoMute" feature
//

using Managers;
using RumbleModdingAPI;

namespace Services
{
    static class NoMuteService
    {
        private static string mutePlayerActionName;
        
        public static void Setup()
        {
            var inputManager = Calls.Managers.GetInputManager();

            mutePlayerActionName = inputManager.mutePlayerActionName;
            inputManager.mutePlayerActionName = "whatever";
            
            Log.Loud("[SoundBending.Services.NoMute] Setup: Mute button disabled");
        }
        
        public static void Run()
        {
            Log.Quiet("[SoundBending.Services.NoMute] Run: noop lol");
            // noop
        }

        public static void SceneLoaded(string _)
        {
            Log.Loud("[SoundBending.Services.NoMute] SceneLoaded: noop lol");
            // noop
        }

        public static void Cleanup()
        {
            var inputManager = Calls.Managers.GetInputManager();

            inputManager.mutePlayerActionName = mutePlayerActionName;
            
            Log.Loud("[SoundBending.Services.NoMute] Cleanup: Mute button enabled");
        }
    }
}