//
//          The owning service for the "HighNoon" feature
//

using Managers;
using Il2CppRUMBLE.Players;
using Il2CppSystem.Collections.Generic;
using RumbleModdingAPI;

namespace Services
{
    static class HighNoonService
    {
        private static bool soundPlayed;
        private static List<Player> players;
        
        public static void Setup()
        {
            Calls.onLocalPlayerHealthChanged += HealthChanged;
            Calls.onRemotePlayerHealthChanged += HealthChanged;
            
            Log.Loud("[SoundBending.Services.HighNoon] Setup: HealthChange listeners active");
        }
        
        public static void Run()
        {
            Log.Quiet("[SoundBending.Services.HighNoon] Run: noop lol");
            // noop
        }
        
        public static void SceneLoaded(string sceneName)
        {
            if (sceneName == "Loader") return;
            
            soundPlayed = false;
            players = Calls.Players.GetAllPlayers();
            
            Log.Loud("[SoundBending.Services.HighNoon] SceneLoaded: State reset");
        }
        
        public static void Cleanup()
        {
            Calls.onLocalPlayerHealthChanged -= HealthChanged;
            Calls.onRemotePlayerHealthChanged -= HealthChanged;
            
            Log.Loud("[SoundBending.Services.HighNoon] Cleanup: HighNoon disabled");
        }
        
        private static void HealthChanged()
        {
            if (players.Count == 1)
            {
                Log.Quiet("[SoundBending.Services.HighNoon] Only one player currently active; HighNoon will not trigger. ");
                return;
            }

            foreach (var player in players)
            {
                if (player.Data.HealthPoints != 1)
                {
                    Log.Quiet("[SoundBending.Services.HighNoon] Player " + 
                              player.Data.GeneralData.PublicUsername + 
                              " has " + 
                              player.Data.HealthPoints + 
                              " hp remaining, not 1.");
                    
                    return;
                }
            }

            if (soundPlayed)
            {
                Log.Quiet("[SoundBending.Services.HighNoon] Sound played already?");
                return;
            }
            soundPlayed = true;
                    
            Audio.PlaySfx("high_noon");
            Audio.PlayRemote(Env.SfxRoot + "high_noon.mp3");
            
            Log.Loud("[SoundBending.Services.HighNoon] > HealthChanged: It's High Noon...");
        }
    }
}