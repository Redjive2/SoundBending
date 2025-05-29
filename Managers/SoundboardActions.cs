using System.Collections;
using MelonLoader;
using NAudio.Wave;
using UnityEngine;

namespace Managers
{
    public static class SoundboardActions
    {
        public static void Prepare()
        {
            // noop lol
            Log.Loud("[SoundBending.Managers.SoundboardActions] Prepare: Soundboard actions initialized (noop)");
        }
        
        public static void Deinit()
        {
            // noop lol
            Log.Loud("[SoundBending.Managers.SoundboardActions] Deinit: noop lol");
        }
        
        public static IEnumerator Resume()
        {
            if (State.SoundChanged)
            {
                Audio.PlaySound(State.CurrentSound);
                State.SoundChanged = false;
                yield break;
            }

            Audio.LocalWaveOut.Play();
            Audio.RemoteWaveOut.Play();

            while (Audio.LocalWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        
        public static void ToggleLock()
        {
            if (!State.Locked)
            {
                State.Locked = true;
                Audio.PlaySfx("lock_controls");
            }
            else
            {
                State.Locked = false;
                Audio.PlaySfx("open_controls");
            }
        }

        public static void VolumeUp()
        {
            if (State.Volume >= 0.98f)
            {
                Audio.PlaySfx("volstuck");

                return;
            }

            State.Volume += 0.10f;

            Audio.RemoteVolumeProvider.Volume = State.Volume;
            Audio.LocalVolumeProvider.Volume = State.Volume;

            Log.Loud($"[SoundBending.Managers.SoundboardActions] VolumeUp: Volume = {State.Volume}");

            Audio.PlaySfx("volchange");
        }


        public static void VolumeDown()
        {
            if (State.Volume <= 0.02f)
            {
                Audio.PlaySfx("volstuck");

                return;
            }

            State.Volume -= 0.10f;

            Audio.RemoteVolumeProvider.Volume = State.Volume;
            Audio.LocalVolumeProvider.Volume = State.Volume;

            Audio.PlaySfx("volchange");

            Log.Loud($"[SoundBending.Managers.SoundboardActions] VolumeDown: Volume = {State.Volume}");
        }


        public static void ToggleAutoplay()
        {
            if (!State.PlayOnLoad)
            {
                Audio.PlaySfx("activate_autoplay");
            }
            else
            { 
                Audio.PlaySfx("disable_autoplay");
            }

            State.PlayOnLoad = !State.PlayOnLoad;

            Log.Loud($"[SoundBending.Managers.SoundboardActions] ToggleAutoplay: PlayOnLoad = {State.PlayOnLoad}");
        }


        public static void ToggleMute()
        {
            if (State.Muted)
            {
                Audio.PlaySfx("unmute");
            }
            else
            {
                Audio.PlaySfx("mute");
            }

            State.Muted = !State.Muted;

            Log.Loud($"[SoundBending.Managers.SoundboardActions] ToggleMute: Muted = {State.Muted}");
        }


        public static void NextAudioFile()
        {
            State.NextSound();
            
            if (Audio.LocalWaveOut.PlaybackState == PlaybackState.Playing)
            {
                Audio.LocalWaveOut.Stop();
                Audio.RemoteWaveOut.Stop();
                
                Audio.PlaySound(State.CurrentSound);

                return;
            }
            
            Audio.PlaySfx("next_audio");
            
            State.SoundChanged = true;
            
            Log.Loud("[SoundBending.Managers.SoundboardActions] NextAudioFile: Next track selected");
        }


        public static void TogglePlayback()
        {
            // Playback has finished, but not paused - just restart
            if (!State.Paused && Audio.LocalWaveOut.PlaybackState != PlaybackState.Playing)
            {
                Log.Loud("[SoundBending.Managers.SoundboardActions] TogglePlayback: Playback has already finished; restarting");
                
                MelonCoroutines.Start(Restart());
                
                return;
            }
            
            if (State.Paused)
            {
                if (Audio.LocalWaveOut.PlaybackState != PlaybackState.Playing)
                {
                    MelonCoroutines.Start(Resume());
                }
            }
            else
            {
                Audio.LocalWaveOut.Pause();
                Audio.RemoteWaveOut.Pause();
            }
            
            State.Paused = !State.Paused;

            Log.Loud($"[SoundBending.Managers.SoundboardActions] TogglePlayback: Paused = {State.Paused}");
        }

        private static IEnumerator Restart()
        {
            Audio.LocalReader.Position = 0;
            Audio.RemoteReader.Position = 0;
            
            Audio.LocalWaveOut.Play();
            Audio.RemoteWaveOut.Play();

            while (Audio.LocalWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}