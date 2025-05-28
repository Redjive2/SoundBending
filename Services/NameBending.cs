//
//      The owning service for the integration with NameBending. Disables autoplay for the Soundboard service.
//

using Managers;
using NBManagers;

namespace Services
{
    public static class NamebendingIntegrationService
    {
        public static void Setup()
        {
            NBConfiguration.Prepare();
            
            Log.Loud("[SoundBending.Services.NameBending] Setup: This service is still under construction! It currently doesn't do anything.");
        }
        
        public static void Run()
        {
            // noop
        }
        
        public static void SceneLoaded(string sceneName)
        {
            /*if (sceneName != "Gym")
            {
                PlayNBSound("");
            }*/
        }

        public static void Cleanup()
        {
            
        }
        
        
        public static void PlayNBSound(string soundName)
        {
            string path = Env.NBSoundRoot + soundName + ".mp3";
            Audio.PlayLocal(path);
            Audio.PlayRemote(path);
        }
    }
}