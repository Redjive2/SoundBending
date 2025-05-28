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
            Log.Loud("[SoundBending.Managers.SoundboardActions] Prepare: Soundboard actions initialized (noop)");
            // noop
        }
        
        public static IEnumerator Resume()
        {
            if (!State.Paused || Audio.LocalWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield break;
            }

            if (State.SoundChanged)
            {
                Audio.PlaySound(State.CurrentSound);
                State.SoundChanged = false;
                yield break;
            }

            Audio.LocalWaveOut.Play();
            Audio.RemoteWaveOut.Play();

            while (Audio.LocalWaveOut.PlaybackState != PlaybackState.Playing)
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

            MelonLogger.Msg($"~~ [SoundBending volup] ~~   volume = {State.Volume}");

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

            MelonLogger.Msg($"~~ [SoundBending voldown] ~~   volume = {State.Volume}");
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

            MelonLogger.Msg($"~~ [SoundBending] ~~   playOnLoad = {State.PlayOnLoad}");
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

            MelonLogger.Msg($"~~ [SoundBending] ~~   muted = {State.Muted}");
        }


        public static void NextAudioFile()
        {
            Audio.PlaySfx("next_audio");

            State.NextSound();

            State.SoundChanged = true;
        }


        public static void TogglePlayback()
        {
            // ================== RESUME ================== //

            if (State.Paused)
            {
                if (Audio.LocalWaveOut?.PlaybackState != PlaybackState.Playing)
                {
                    MelonCoroutines.Start(Resume());
                }

                State.Paused = false;

                MelonLogger.Msg("~~ [SoundBending] ~~   resumed");

                return;
            }

            // ================== STOP ================== //

            Audio.LocalWaveOut?.Pause();
            Audio.RemoteWaveOut?.Pause();

            State.Paused = true;

            MelonLogger.Msg("~~ [SoundBending] ~~   paused");
        }
    }
}