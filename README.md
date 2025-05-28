# SoundBending
*Bends sound. duh.*

## Installation
### If you're on windows x86_64 (which you most likely are) I recommend you download and use the installer. It will set everything up for you with minimal interference; all you have to do is download the `\Installer\Bin\` folder and run `installer.exe`.

### If you're deadset on installing manually (or having installer issues), you'll need to follow a few steps:

1) Download the `\Bin\` and `\Sfx\` folders.
2) Put the file `\Bin\SoundBending.dll` into `(Rumble Path)\Mods\`.
3) Put all the files in `\Bin\UserLibs\` into `(Rumble Path)\UserLibs\`
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
| [SoundBending.Managers.Env] > ReloadConfiguration: Configuration loaded with log level 2
| [SoundBending.Managers.Env] Prepare: Environment initialized.
| |   [SoundBending.Managers.Log] Prepare: noop lol
| |   [SoundBending.Managers.State] Prepare: State initialized with sound list Altars Of Apostasy; Buddy Holly; Domain Expansion
| |   >> [SoundBending.Managers.Audio] Prepare / Device List;
| |   |   WaveIn  | `CABLE Output (VB-Audio Virtual `
| |   |   WaveIn  | `Headset Microphone (Oculus Virt`
| |   |   WaveIn  | `Microphone (Yeti Classic)`
| |   |           | 
| |   |   WaveOut | `Headphones (Realtek(R) Audio)`
| |   |   WaveOut | `CABLE Input (VB-Audio Virtual C`
| |   << [SoundBending.Managers.Audio] Prepare / Device List;
| |   [SoundBending.Managers.Audio] Prepare: Audio initialized
| |   [SoundBending.Managers.Input] Prepare: Input initialized
| |   [SoundBending.Managers.SoundboardActions] Prepare: Soundboard actions initialized (noop)
| |   >> [SoundBending.Mod] > RefreshServices;
| |   |   [SoundBending.Managers.Env] > ReloadConfiguration: Configuration loaded with log level 2
| |   |   [SoundBending.Managers.State] Prepare: State initialized with sound list Altars Of Apostasy; Buddy Holly; Domain Expansion
| |   |   [SoundBending.Services.HighNoon] Setup: HealthChange listeners active
| |   |   [SoundBending.Services.NoMute] Setup: Mute button disabled
| |   |   [SoundBending.Services.Soundboard] Setup: noop lol
| |   |   [SoundBending.Services.HighNoon] SceneLoaded: State reset
| |   |   [SoundBending.Services.NoMute] SceneLoaded: noop lol
| |   << [SoundBending.Mod] > RefreshServices;
| |   >> / Activity check;
| |   |   HighNoon: ACTIVE
| |   |   NoMute: ACTIVE
| |   |   NamebendingIntegration: inactive
| |   |   Soundboard: ACTIVE
| |   << / Activity check;
| |   SoundBending active!
| << [SoundBending.Mod] > Startup;
```

You'll want to focus on the `WaveIn`/`WaveOut` table near the top. Find your headphones/speakers, microphone, and virtual cable input (this will be under `WaveOut`). Copy + Paste each one into its respective place in your `Config.json`.
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

Now, here's the specific installation information for each *service*, which you can think of as a smaller mod inside SoundBending.

- Soundboard: This is far and away the largest service. It will let you play arbitrary `.mp3` files from your `(Rumble Path)\UserData\SoundBending\Sounds\` folder, cycle between them, control their volumes, and have them play on scene load. First, make sure it's listed under `"Services"` in your configuration file. Also, if `"NamebendingIntegration"` is active, autoplay won't work with this service. Now, to use it, you'll press the following controls (on the Quest 3, for all other headsets good luck. it should work?):

```
1) Toggle Control Lock:                       A            + X
2) Pause/Resume:                              Left Trigger + Right Trigger + Left Grip
3) Next Track:                                Left Trigger + Right Trigger + X
4) Toggle Mute (Like a better push-to-talk!): Left Trigger + Right Trigger + Y
5) Volume Up:                                 Left Trigger + Right Trigger + B
6) Volume Down:                               Left Trigger + Right Trigger + A
7) Toggle Autoplay (on scene load):           Left Trigger + Right Trigger + Right Grip
```

HighNoon: You'll only need to activate it in `"Services"`. Once all players in a session reach 1 HP, you'll hear some funny music.

NoMute: Same as HighNoon. Disables the mute button. Very simple.

NamebendingIntegration: This one's a little more complicated. It allows you to play sounds on scene load *based on your specific NameBending name or title variation* .

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

SoundBending will assume there is a file called `"Buddy Holly.mp3"` under `(Rumble Path)\UserData\NameBending\soundbendingSounds\`, and will play it if this variation is used by NameBending and if the preference/strictness settings allow it.



