//
//          Holds all program state used across files
//

using System.IO;
using System.Linq;

namespace Managers
{
    public static class State
    {
        private static bool initialized;
        
        public static void Prepare()
        {
            var soundToFind = ":VOID:";
            
            if (initialized)
            {
                soundToFind = CurrentSound;
            }
            
            var files = Directory.GetFiles(Env.SoundRoot);

            for (int i = 0; i < files.Length; i++)
            {
                string f = files[i];

                files[i] = new string(f
                    .Skip(f.LastIndexOf('\\') + 1)
                    .TakeWhile(c => c != '.')
                    .ToArray());
            }

            Sounds = files;

            if (!initialized)
            {
                initialized = true;
            }
            else
            {
                soundIndex = Sounds.ToList().IndexOf(soundToFind);
            }
            
            Log.Loud("[SoundBending.Managers.State] Prepare: State initialized with sound list " + string.Join("; ", Sounds));
        }
        
        public static bool SoundChanged = true;
        public static bool Paused;

        public static bool PlayOnLoad = true;

        public static float Volume = 1.0f;

        public static bool Locked;

        public static bool Muted;

        public static string[] Sounds { get; private set; }
        private static int soundIndex;

        public static string CurrentSound => Sounds[soundIndex];

        public static void NextSound()
        {
            if (soundIndex < Sounds.Count() - 1)
            {
                soundIndex++;
                return;
            }

            soundIndex = 0;
        }
    }
}