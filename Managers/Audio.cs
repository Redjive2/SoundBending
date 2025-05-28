using System.Collections;
using MelonLoader;
using NAudio.Wave;
using UnityEngine;

namespace Managers
{
    public static class Audio
    {
        public static WaveOutEvent LocalWaveOut;
        public static WaveOutEvent RemoteWaveOut;
        public static WaveOutEvent SfxWaveOut;

        public static VolumeWaveProvider16 RemoteVolumeProvider;
        public static VolumeWaveProvider16 LocalVolumeProvider;

        public static int MicIn;
        public static int SpeakerOut;
        public static int InputCableOut;


        public static void Prepare()
        {
            Log.Open("[SoundBending.Managers.Audio] Prepare / Device List;");
            
            for (var i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capabilities = WaveIn.GetCapabilities(i);
                Log.Force("WaveIn  | `" + capabilities.ProductName + "`");
            }
            
            Log.Force("        | ");
            
            for (var i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);
                Log.Force("WaveOut | `" + capabilities.ProductName + "`");
            }
            
            Log.Close("[SoundBending.Managers.Audio] Prepare / Device List;");
            
            MicIn = FindInput(Env.InputDevice);
            SpeakerOut = FindOutput(Env.OutputDevice);
            InputCableOut = FindOutput(Env.OutputPipe);

            LocalWaveOut = new WaveOutEvent();
            LocalWaveOut.DeviceNumber = SpeakerOut;

            RemoteWaveOut = new WaveOutEvent();
            RemoteWaveOut.DeviceNumber = InputCableOut;

            SfxWaveOut = new WaveOutEvent();
            SfxWaveOut.DeviceNumber = SpeakerOut;
            
            // just so these values aren't null
            Mp3FileReader dummy = new Mp3FileReader(Env.SfxRoot + "dummy.mp3");
            
            LocalVolumeProvider = new VolumeWaveProvider16(dummy);
            RemoteVolumeProvider = new VolumeWaveProvider16(dummy);
            
            LocalWaveOut.Init(LocalVolumeProvider);
            RemoteWaveOut.Init(RemoteVolumeProvider);
            
            LocalWaveOut.Play();
            RemoteWaveOut.Play();
            
            Log.Loud("[SoundBending.Managers.Audio] Prepare: Audio initialized");
        }


        private static int FindOutput(string productName)
        {
            for (var i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);

                if (capabilities.ProductName == productName)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindInput(string productName)
        {
            for (var i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capabilities = WaveIn.GetCapabilities(i);

                if (capabilities.ProductName == productName)
                {
                    return i;
                }
            }

            return -1;
        }


        public static void PlaySfx(string sfxName)
        {
            MelonCoroutines.Start(playSfxCoroutine(Env.SfxRoot + sfxName + ".mp3"));
        }

        private static IEnumerator playSfxCoroutine(string path)
        {
            var reader = new Mp3FileReader(path);

            SfxWaveOut.Dispose();
            SfxWaveOut.Init(reader);
            SfxWaveOut.Play();

            while (SfxWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        


        public static void PlaySound(string soundName)
        {
            string path = Env.SoundRoot + soundName + ".mp3";
            
            PlayLocal(path);
            PlayRemote(path);
        }


        public static void PlayLocal(string path)
        {
            MelonCoroutines.Start(playLocalCoroutine(path));
        }

        private static IEnumerator playLocalCoroutine(string path)
        {
            var reader = new Mp3FileReader(path);
            LocalVolumeProvider = new VolumeWaveProvider16(reader)
            {
                Volume = State.Volume
            };

            LocalWaveOut.Dispose();
            LocalWaveOut.Init(LocalVolumeProvider);
            LocalWaveOut.Play();

            while (LocalWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        
        
        public static void PlayRemote(string path)
        {
            MelonCoroutines.Start(playRemoteCoroutine(path));
        }

        private static IEnumerator playRemoteCoroutine(string path)
        {
            var reader = new Mp3FileReader(path);
            
            RemoteVolumeProvider = new VolumeWaveProvider16(reader)
            {
                Volume = State.Volume
            };

            RemoteWaveOut.Dispose();
            RemoteWaveOut.Init(RemoteVolumeProvider);
            RemoteWaveOut.Play();

            while (RemoteWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        public static IEnumerator Feed(int sourceDeviceNumber, int sinkDeviceNumber)
        {
            // Set up the microphone input
            var source = new WaveInEvent
            {
                DeviceNumber = sourceDeviceNumber,
                WaveFormat = new WaveFormat(44100, 16, 1)   // 44.1 kHz, 16‑bit, mono
            };

            // Create a buffer to hold incoming audio
            var bufferedWaveProvider = new BufferedWaveProvider(source.WaveFormat)
            {
                BufferDuration = System.TimeSpan.FromMilliseconds(5000),   // up to 5 seconds of audio buffered
                DiscardOnBufferOverflow = false                            // drop old data if consumer lags
            };

            // Set up the speaker output and link it to the buffer
            var sink = new WaveOutEvent
            {
                DeviceNumber = sinkDeviceNumber
            };

            sink.Init(bufferedWaveProvider);               // connect the buffer as the source for playback
            sink.Play();

            // As microphone data arrives, enqueue it for playback
            source.DataAvailable += (_, e) =>
            {
                if (State.Muted)
                {
                    return;
                }

                bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
            };

            // Begin recording from the microphone
            source.StartRecording();

            // Keep MelonLoader’s coroutine alive while audio is playing
            yield return new WaitForFixedUpdate();
        }
    }
}