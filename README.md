# SoundBending v1.0.0
*Bends sound. duh.*

## The future
I will be adding an optional ModUI integration, some visual/physical stuff (show file name when switching sounds, extra controls on arm, mute status on healthbar, etc.), a sound API for other devs to use, and hopefully handrolling an audio pipe so the user doesn't have to download one separately. Also, improvements to the logging tool and more testing, I guess.

## Installation
I strongly recommend that you use the installer. All you need to do is download `\Installer\Bin\`, then run `\Installer\Bin\installer.exe`. If that doesn't work, or you're deadset on manual installation, you can follow these steps (8 total):

1) Download the `\Bin\` and `\Sfx\` folders.
2) Put the file `\Bin\SoundBending.dll` into `(Rumble Path)\Mods\`.
3) Put all the files in `\Bin\UserLibs\` into `(Rumble Path)\UserLibs\`.
4) Ensure you have `RumbleModdingAPI` from UlvakSkillz in your `(Rumble Path)\Mods\` folder.
5) Run RUMBLE and do the T-Pose. This will populate your `(Rumble Path)\UserData\` folder with a subfolder called `SoundBending`, containing an `Sfx` folder, a `Sounds` folder, and two files: `Config.json` and `Latest.log`. Do not close the game until you've been in the gym for a few seconds.
6) Move all files from the original `\Sfx\` that you downloaded into the folder `(Rumble Path)\UserData\SoundBending\Sfx\`.

Look at your `Config.json`. You should see the following:
```json
{
    "Services": ["Soundboard", "HighNoon", "NoMute", "NamebendingIntegration"],

    "Devices": {
        "Input": "PLEASE FILL THIS FROM LOG INFO ",
        "Output": "PLEASE FILL THIS FROM LOG INFO ",
        "PipeOutput": "PLEASE FILL THIS FROM LOG INFO "
    },

    "NamebendingIntegration": {
        "Prefer": "customBentName",
        "Strict": true
    },

    "LogLevel": 2
}
```

You'll want to read your logfile. It'll be stored at `(Rumble Path)\UserData\SoundBending\Latest.log` and will look something like the following at the top (if not, just contact me, you're cooked lol):

```
[===============| SoundBending V1.0.0 -- Latest.log -- opened at 11:39 PM |===============]
| [SoundBending.Managers.Env] > ReloadConfiguration: Configuration loaded with log level 2
| [SoundBending.Managers.Env] Prepare: Environment initialized.
| |   [SoundBending.Managers.Log] Prepare: Logfile handle ready
| |   [SoundBending.Managers.State] Prepare: State initialized with sound list Altars Of Apostasy; Buddy Holly; Domain Expansion
| |   >> [SoundBending.Managers.Audio] Prepare / Device List;
| |   |   Input  | `CABLE Output (VB-Audio Virtual `
| |   |   Input  | `Headset Microphone (Oculus Virt`
| |   |          | 
| |   |   Output | `Headphones (Realtek(R) Audio)`
| |   |   Output | `CABLE Input (VB-Audio Virtual C`
| |   << [SoundBending.Managers.Audio] Prepare / Device List;
| |   [SoundBending.Managers.Audio] > FindInput: Found device `Headset Microphone (Oculus Virt`!
| |   [SoundBending.Managers.Audio] > FindOutput: Found device `LG ULTRAGEAR (NVIDIA High Defin`!
| |   [SoundBending.Managers.Audio] > FindOutput: Found device `CABLE Input (VB-Audio Virtual C`!
| |   [SoundBending.Managers.Audio] > GetReaderOrAdd: Found a reader for `D:\SteamLibrary\steamapps\common\RUMBLE\UserData\SoundBending\Sfx\dummy.mp3`!
| |   [SoundBending.Managers.Audio] > GetReaderOrAdd: Found a reader for `D:\SteamLibrary\steamapps\common\RUMBLE\UserData\SoundBending\Sfx\dummy.mp3`!
| |   [SoundBending.Managers.Audio] > GetReaderOrAdd: Found a reader for `D:\SteamLibrary\steamapps\common\RUMBLE\UserData\SoundBending\Sfx\dummy.mp3`!
| |   [SoundBending.Managers.Audio] Prepare: Audio initialized
| |   [SoundBending.Managers.Input] Prepare: Input initialized
| |   [SoundBending.Managers.SoundboardActions] Prepare: Soundboard actions initialized (noop)
| |   [SoundBending.Managers.Audio] > Feed (Coroutine): Feeding source #2 -> sink #9
| |   [SoundBending.Managers.Audio] > Feed (Coroutine): Active!
| |   [SoundBending.Services.HighNoon] Setup: HealthChange listeners active
| |   [SoundBending.Services.NoMute] Setup: Mute button disabled
| |   [SoundBending.Services.Soundboard] Setup: noop lol
| |   >> / Activity check;
| |   |   HighNoon: ACTIVE
| |   |   NoMute: ACTIVE
| |   |   NamebendingIntegration: inactive
| |   |   Soundboard: ACTIVE
| |   << / Activity check;
| |   SoundBending active!
| << [SoundBending.Mod] > Startup;
```

7) You'll want to focus on the `Input`/`Output` table near the top. Find your headphones/speakers, microphone, and virtual cable input (this will be under `Output`). Copy + Paste each one into its respective place in your `Config.json`.
You should end up with a configuration file similar to this:

```json
{
    "Services": ["Soundboard", "HighNoon", "NoMute", "NamebendingIntegration"],

    "Devices": {
        "Input": "Headset Microphone (Oculus Virt",
        "Output": "LG ULTRAGEAR (NVIDIA High Defin",
        "PipeOutput": "CABLE Input (VB-Audio Virtual C"
    },

    "NamebendingIntegration": {
        "Prefer": "customBentName",
        "Strict": true
    },

    "LogLevel": 2
}
```

8) You'll also want to set your virtual cable output (listed under `Input` in the logfile) as your microphone in Steam/Oculus.

------

Now, here's the specific information for each *service*, which you can think of as a smaller mod inside SoundBending.

Soundboard: This is far and away the largest service. It will let you play arbitrary `.mp3` files from your `(Rumble Path)\UserData\SoundBending\Sounds\` folder, cycle between them, control their volumes, and have them play on scene load. First, make sure it's listed under `"Services"` in your configuration file. Also, if `"NamebendingIntegration"` is active, autoplay won't work with this service. Next, to use it, you'll press the following controls (on the Quest 3, for all other headsets good luck. it should work?):

```
1) Toggle Control Lock:                       A            + X
2) Pause/Resume:                              Left Trigger + Right Trigger + Left Grip
3) Next Track:                                Left Trigger + Right Trigger + X
4) Toggle Mute (Like a better push-to-talk!): Left Trigger + Right Trigger + Y
5) Volume Up:                                 Left Trigger + Right Trigger + B
6) Volume Down:                               Left Trigger + Right Trigger + A
7) Toggle Autoplay (on scene load):           Left Trigger + Right Trigger + Right Grip
```

HighNoon: You'll only need to activate it in `"Services"`. Once all players in a session reach 1 HP, you'll hear some funny music (provided there's more than just one player lol)

NoMute: Same as HighNoon. Disables the mute button. Very simple.

NamebendingIntegration (NOT YET IMPLEMENTED): This one's a little more complicated. It allows you to play sounds on scene load *based on your specific NameBending name or title variation* .

To actually use it, though: 

1) You'll have to add a folder, `\soundbendingSounds\`, under your NameBending user data folder, then place whatever `.mp3` files you want to use for it there.
2) Go to your SoundBending configuration file. Change the strictness and preference settings to your liking. (Preference is whether you'd rather play sounds from `customBentName` or `customBentTitle`, strictness is whether or not to swap files if no sound is listed in the current variation of your preferred file. If true, this will not happen.
3) For each variation that you want a sound to play on in each file, add a `soundbendingSound` property in the variation, like so:

```json
{
	"doRandomVariations": false,
	
	"variations":
	[
		{
			"altText": "<#F22>Rdm<#966>8417",
			"frameDuration": 62,

			"soundbendingSound": "Buddy Holly",
			
			"frames":
			{
				"0": "<#F22>Rdm<#966>8417",
				"6": "<#E22>Rdm<#966>8417",
				"7": "<#D22>Rdm<#966>8417",
				"8": "<#C22>Rdm<#966>8417",
				"9": "<#C22>Rdm<#966>8417"
			}
		}
	]
}
```

SoundBending will assume there is a `.mp3` file called `"Buddy Holly"` under `(Rumble Path)\UserData\NameBending\soundbendingSounds\`, and will play it if this variation is used by NameBending and if the preference/strictness settings allow it.

- Note: Your configuration reloads on scene change. If you want to enable/disable a service or change any settings, just change them then go to a gym/park/match and it'll handle everything automatically. No need to restart the game.

------

Now you should be good to go! Enjoy using SoundBending!

- A note for anyone looking at the source: read `SourceDocs.md`, it'll contain everything you need to know to get started.
