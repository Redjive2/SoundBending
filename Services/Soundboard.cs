using Managers;
using MelonLoader;

namespace Services
{
    public static class SoundboardService
    {
        private const int MaxDelay = 45;
        private static int delay;
        
        public static void Setup()
        {
            // noop
            Log.Loud("[SoundBending.Services.Soundboard] Setup: noop lol");
        }

        public static void Run() => Log.WrapLazy(
        "[SoundBending.Services.Soundboard] Run", null, null, () => {
            if (delay > 0)
            {
                delay--;
                return;
            }

            if (Input.Lock.Active())
            {
                SoundboardActions.ToggleLock();
                Log.Loud("/ ToggleLock: Control lock set to " + State.Locked);

                delay = MaxDelay;

                return;
            }

            if (State.Locked)
            {
                return;
            }


            if (Input.AllActive(Input.Triggers, Input.VolumeUp))
            {
                SoundboardActions.VolumeUp();

                Log.Loud("/ VolumeUp: Volume set to " + State.Volume);

                delay = MaxDelay;

                return;
            }


            if (Input.AllActive(Input.Triggers, Input.VolumeDown))
            {
                SoundboardActions.VolumeDown();

                Log.Loud("/ VolumeDown: Volume set to " + State.Volume);

                delay = MaxDelay;

                return;
            }


            if (Input.AllActive(Input.Triggers, Input.Autoplay))
            {
                SoundboardActions.ToggleAutoplay();

                Log.Loud("/ ToggleAutoplay: Autoplay set to " + State.PlayOnLoad +
                         "  (does not work if NameBending integration active)");

                delay = MaxDelay;

                return;
            }

            if (Input.AllActive(Input.Triggers, Input.Mute))
            {
                SoundboardActions.ToggleMute();

                Log.Loud("/ ToggleMute: Mute set to " + State.Muted);

                delay = MaxDelay;

                return;
            }


            if (Input.AllActive(Input.Triggers, Input.NextFile))
            {
                SoundboardActions.NextAudioFile();

                Log.Loud("/ NextAudioFile: Track set to " + State.CurrentSound);

                delay = MaxDelay;

                return;
            }


            if (Input.AllActive(Input.Triggers, Input.Playback))
            {
                SoundboardActions.TogglePlayback();

                Log.Loud("/ TogglePlayback: Playback toggled to " + !State.Paused);

                delay = MaxDelay;
            }
        });
        
        public static void SceneLoaded(string sceneName)
        {
            if (State.PlayOnLoad && sceneName != "Gym" && !Env.IsServiceEnabled("NamebendingIntegration"))
            {
                string path = Env.SoundRoot + State.CurrentSound + ".mp3";
                Audio.PlayLocal(path);
                Audio.PlayRemote(path);
                
                Log.Loud("[SoundBending.Services.Soundboard] SceneLoaded: Playing " + State.CurrentSound);
            }
        }
        
        public static void Cleanup()
        {
            // noop
            Log.Loud("[SoundBending.Services.Soundboard] Cleanup: noop lol");
        }
    }
}