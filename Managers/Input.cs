using UnityEngine.InputSystem;
using System.Linq;

//
//      Manages inputs from the controller via Unity
//

namespace Managers
{
    static class Input
    {
        public static void Prepare()
        {
            rightTrigger.AddBinding("<XRController>{RightHand}/trigger");
            leftTrigger.AddBinding("<XRController>{LeftHand}/trigger");

            taudioLeftGrip.AddBinding("<XRController>{LeftHand}/gripButton");

            tmuteLeftSecondary.AddBinding("<XRController>{LeftHand}/secondaryButton");

            nafLeftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");

            volupRightSecondary.AddBinding("<XRController>{RightHand}/secondaryButton");

            voldownRightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");

            tautoRightGrip.AddBinding("<XRController>{RightHand}/gripButton");

            tlockRightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");
            tlockLeftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");

            triggersMap.Enable();
            taudioMap.Enable();
            tmuteMap.Enable();
            nafMap.Enable();
            volupMap.Enable();
            voldownMap.Enable();
            tautoMap.Enable();
            tlockMap.Enable();
            
            Log.Loud("[SoundBending.Managers.Input] Prepare: Input initialized");
        }
        
        
        public static bool AllActive(params Binding[] inputs)
        {
            Log.Open("[SoundBending.Managers.Input] > AllActive;");
            
            foreach (var input in inputs)
            {
                if (!input.Active())
                {
                    Log.Quiet("maps `" + 
                              string.Join("; ", inputs.Select(i => i.map.name).ToList()) + 
                              "` returning false");
                    
                    return false;
                }
            }
            
            Log.Loud("maps `" + 
                     string.Join("; ", inputs.Select(i => i.map.name).ToList()) + 
                     "` returning true");
            
            Log.Close("[SoundBending.Managers.Input] > AllActive;");
            
            return true;
        }


        public struct Binding
        {
            internal readonly InputActionMap map;
            private bool armed;

            // Tweak these to taste:
            private const float PressThreshold   = 0.8f;
            private const float ReleaseThreshold = 0.2f;

            public Binding(InputActionMap map)
            {
                this.map = map;
                armed    = true;
            }

            public bool Active()
            {
                bool allHigh = true;
                bool allLow  = true;

                // read every action in the map
                foreach (var action in map.actions)
                {
                    float v = action.ReadValue<float>();

                    if (v < PressThreshold)
                        allHigh = false;

                    // all-low check: we only re-arm when *every* action is back down
                    if (v > ReleaseThreshold)
                        allLow = false;

                    // quick-exit
                    if (!allHigh && !allLow)
                        break;
                }

                // if we're armed, and we see all triggers past the pressThreshold...
                if (armed && allHigh)
                {
                    armed = false;
                    Log.Quiet("[SoundBending.Managers.Input] > Binding.Active: map `" + map.name + "` pressed");
                    return true;
                }

                // once everything is back down under the releaseThreshold, re-arm
                if (!armed && allLow)
                {
                    Log.Quiet("[SoundBending.Managers.Input] > Binding.Active: map `" + map.name + "` rearming");
                    armed = true;
                }

                return false;
            }
        }
        

        static InputActionMap triggersMap = new InputActionMap("Triggers");
        static InputAction rightTrigger = triggersMap.AddAction("Right Trigger");
        static InputAction leftTrigger = triggersMap.AddAction("Left Trigger");

        public static Binding Triggers { get; } = new Binding(triggersMap);


        static InputActionMap taudioMap = new InputActionMap("Toggle Playback");
        static InputAction taudioLeftGrip = taudioMap.AddAction("Left Grip");

        public static Binding Playback { get; } = new Binding(taudioMap);


        static InputActionMap tmuteMap = new InputActionMap("Toggle Mute");
        static InputAction tmuteLeftSecondary = tmuteMap.AddAction("Left Secondary");

        public static Binding Mute { get; } = new Binding(tmuteMap);


        static InputActionMap nafMap = new InputActionMap("Next Audio File");
        static InputAction nafLeftPrimary = nafMap.AddAction("Left Primary");

        public static Binding NextFile { get; } = new Binding(nafMap);


        static InputActionMap volupMap = new InputActionMap("Volume Up");
        static InputAction volupRightSecondary = volupMap.AddAction("Right Secondary");

        public static Binding VolumeUp { get; } = new Binding(volupMap);


        static InputActionMap voldownMap = new InputActionMap("Volume Down");
        static InputAction voldownRightPrimary = voldownMap.AddAction("Right Primary");

        public static Binding VolumeDown { get; } = new Binding(voldownMap);


        static InputActionMap tautoMap = new InputActionMap("Toggle Play On Load");
        static InputAction tautoRightGrip = tautoMap.AddAction("Right Grip");

        public static Binding Autoplay { get; } = new Binding(tautoMap);


        static InputActionMap tlockMap = new InputActionMap("Toggle Control Lock");
        static InputAction tlockRightPrimary = tlockMap.AddAction("Right Primary");
        static InputAction tlockLeftPrimary = tlockMap.AddAction("Left Primary");

        public static Binding Lock { get; } = new Binding(tlockMap);
    }
}

