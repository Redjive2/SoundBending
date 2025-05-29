using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        private static Dictionary<string, Mp3FileReader> localPool = new Dictionary<string, Mp3FileReader>();
        private static Dictionary<string, Mp3FileReader> remotePool = new Dictionary<string, Mp3FileReader>();
        private static Dictionary<string, Mp3FileReader> sfxPool = new Dictionary<string, Mp3FileReader>();

        // Stores a reference to one of the readers in its respective reader pool.
        public static Mp3FileReader LocalReader;
        public static Mp3FileReader RemoteReader;
        public static Mp3FileReader SfxReader;

        public static Mp3FileReader GetReaderOrAdd(ref Dictionary<string, Mp3FileReader> pool, string path)
        {
            if (pool.ContainsKey(path))
            {
                Log.Quiet($"[SoundBending.Managers.Audio] > GetReaderOrAdd: Found a reader for `{path}`!");
                
                Mp3FileReader reader = pool[path];
                reader.Position = 0;

                return reader;
            }

            if (File.Exists(path))
            {
                pool[path] = new Mp3FileReader(path);
                
                Log.Loud($"[SoundBending.Managers.Audio] > GetReaderOrAdd: No reader for `{path}`; adding it to the pool");
                
                return pool[path];
            }
            
            Log.Loud($"[SoundBending.Managers.Audio] > GetReaderOrAdd: Failed to find a file at `{path}`; returning dummy.mp3");
            return new Mp3FileReader(Env.SfxRoot + "dummy.mp3");
        }
        
        
        public static void Prepare()
        {
            Log.Open("[SoundBending.Managers.Audio] Prepare / Device List;");
            
            for (var i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capabilities = WaveIn.GetCapabilities(i);
                Log.Force("Input  | `" + capabilities.ProductName + "`");
            }
            
            Log.Force("       | ");
            
            for (var i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);
                Log.Force("Output | `" + capabilities.ProductName + "`");
            }
            
            Log.Close("[SoundBending.Managers.Audio] Prepare / Device List;");
            
            Populate(ref localPool, Env.SoundRoot);
            Populate(ref remotePool, Env.SoundRoot);
            Populate(ref sfxPool, Env.SfxRoot);
            
            MicIn = FindInput(Env.InputDevice);
            SpeakerOut = FindOutput(Env.OutputDevice);
            InputCableOut = FindOutput(Env.OutputPipe);

            LocalWaveOut = new WaveOutEvent();
            LocalWaveOut.DeviceNumber = SpeakerOut;

            RemoteWaveOut = new WaveOutEvent();
            RemoteWaveOut.DeviceNumber = InputCableOut;

            SfxWaveOut = new WaveOutEvent();
            SfxWaveOut.DeviceNumber = SpeakerOut;
            
            // Just so all these values aren't null
            
            LocalReader = GetReaderOrAdd(ref sfxPool, Env.SfxRoot + "dummy.mp3");
            RemoteReader = GetReaderOrAdd(ref sfxPool, Env.SfxRoot + "dummy.mp3");
            SfxReader = GetReaderOrAdd(ref sfxPool, Env.SfxRoot + "dummy.mp3");
            
            LocalVolumeProvider = new VolumeWaveProvider16(LocalReader);
            RemoteVolumeProvider = new VolumeWaveProvider16(RemoteReader);
            
            LocalWaveOut.Init(LocalVolumeProvider);
            RemoteWaveOut.Init(RemoteVolumeProvider);
            
            LocalWaveOut.Play();
            RemoteWaveOut.Play();
            
            Log.Loud("[SoundBending.Managers.Audio] Prepare: Audio initialized");
        }
        
        public static void Deinit()
        {
            // noop lol
            Log.Loud("[SoundBending.Managers.Audio] Deinit: noop lol");
        }


        public static void Populate(ref Dictionary<string, Mp3FileReader> pool, string root)
        {
            string[] fileList = Directory.GetFiles(root, "*.mp3", SearchOption.AllDirectories);

            foreach (string fileName in fileList)
            {
                pool.Add(fileName, new Mp3FileReader(fileName));
            }
        }


        private static int FindOutput(string productName)
        {
            for (var i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);

                if (capabilities.ProductName == productName)
                {
                    Log.Quiet("[SoundBending.Managers.Audio] > FindOutput: Found device `" + productName + "`!");
                    return i;
                }
            }

            Log.Loud("[SoundBending.Managers.Audio] > FindOutput: Could not find device `" + productName + "`");
            return -1;
        }

        private static int FindInput(string productName)
        {
            for (var i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capabilities = WaveIn.GetCapabilities(i);

                if (capabilities.ProductName == productName)
                {
                    Log.Quiet("[SoundBending.Managers.Audio] > FindInput: Found device `" + productName + "`!");
                    return i;
                }
            }

            Log.Loud("[SoundBending.Managers.Audio] > FindInput: Could not find device `" + productName + "`");
            return -1;
        }
        

        public static void PlaySfx(string sfxName)
        {
            Log.Quiet("[SoundBending.Managers.Audio] > PlaySfx: Playing sfx `" + sfxName + "`");
            MelonCoroutines.Start(PlaySfxCoroutine(Env.SfxRoot + sfxName + ".mp3"));
        }

        private static IEnumerator PlaySfxCoroutine(string path)
        {
            SfxReader = GetReaderOrAdd(ref sfxPool, path);

            SfxWaveOut.Dispose();
            SfxWaveOut.Init(SfxReader);
            SfxWaveOut.Play();

            while (SfxWaveOut.PlaybackState == PlaybackState.Playing)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        


        public static void PlaySound(string soundName) => Log.Wrap(
        "[SoundBending.Managers.Audio] > PlaySound", "Playing sound `" + soundName + "`", null, () => {
            string path = Env.SoundRoot + soundName + ".mp3";
            
            PlayLocal(path);
            PlayRemote(path);
        });


        public static void PlayLocal(string path)
        {
            Log.Quiet("[SoundBending.Managers.Audio] > PlayLocal: Playing file at path `" + path + "`");
            MelonCoroutines.Start(PlayLocalCoroutine(path));
        }

        private static IEnumerator PlayLocalCoroutine(string path)
        {
            LocalReader = GetReaderOrAdd(ref localPool, path);
            LocalVolumeProvider = new VolumeWaveProvider16(LocalReader)
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
            Log.Quiet("[SoundBending.Managers.Audio] > PlayRemote: Playing file at path `" + path + "`");
            MelonCoroutines.Start(PlayRemoteCoroutine(path));
        }

        private static IEnumerator PlayRemoteCoroutine(string path)
        {
            RemoteReader = GetReaderOrAdd(ref remotePool, path);
            
            RemoteVolumeProvider = new VolumeWaveProvider16(RemoteReader)
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
            Log.Loud($"[SoundBending.Managers.Audio] > Feed (Coroutine): Feeding source #{sourceDeviceNumber} -> sink #{sinkDeviceNumber}");
            
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
            
            Log.Loud("[SoundBending.Managers.Audio] > Feed (Coroutine): Active!");

            // Keep MelonLoader’s coroutine alive while audio is playing
            while (true)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}