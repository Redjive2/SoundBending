# Source Documentation
This file contains all relevant information for someone to start looking at the source code.

------

## What am I even looking at?
The general flow of code in this mod is as follows: 

```
SoundBending.Mod -> Services.(Some Service).(Setup | Run | SceneLoaded | Cleanup) -> Managers.(Some Manager).(Some Utility Method)
            |
            ------> Managers.(Some Manager).(Prepare | Deinit)
```

As you can see from the (bad) diagram, There are three namespaces: SoundBending, Services, and Components, all arranged in a strict hierarchy. The top-level namespace, SoundBending, only contains a single MelonMod child class called `Mod`.

`Mod` is responsible for orchestrating all of the handler functions (Setup, Run, SceneLoaded, Cleanup; Prepare, Deinit) and respecting configuration data (**managed** by the Environment **manager**).
It coordinates all of the services: HighNoon, NoMute, NamebendingIntegration, and Soundboard. Each of these is basically a small sub-mod of SoundBending, and can be treated as such.

- Now is a good time to note that all of the code outside of `Mod` is wrapped in a static class unique to the file. This means you can effectively treat each file as a separate object (NOT a class or namespace).
  Think packages in Golang or ESModules in JavaScript.

Each manager, on the other hand, is better thought of as a little library inside of the mod. They *only* run code "on their own" at program startup and shutdown, and only necessary bookkeeping code.

------

## What does each thing do?
Each service is adequately described in the readme.

The managers are a little more nebulous, since they don't directly do anything in game.

1) Audio: Plays audio and hosts shared utilities for sounds. Pools every sound on startup and adds more as needed.
2) Environment: Ensures the configuration data is readily available; holds folder and file paths for ease of use; Ensures that the `(Rumble Path)\UserData\SoundBending\` folder exists and has the right stuff inside.
   Can function independently of `Managers.Log`.
3) Input: Manages user inputs. Provides a `Binding` struct that manages a single `InputActionMap` and an `AllActive` utility function. That's it.
4) Logging: My favorite one, for sure - handles all logging for SoundBending, providing a very human-readable and searchable depth-based logging tool. Very useful, but maybe overkill for such a simple (for now) mod.
   Makes debugging a breeze, though.
5) SoundboardActions: Because there's so much code for the soundboard service, all of the actual code that *does* things has been placed in here, like pausing/unpausing or changing the volume.
   The input detection & control flow are still in the service, though.
6) State: Just holds all the shared state. Very simple.

- Also, of note, the NameBending service has its own Manager group since it's a little more independent than the other services. Those are stored under `Services\NBManagers\` next to the other services.

------

## Why would you ever do this?
Because I like writing code more than making useful mods, so I overarchitected and overengineered the mod.

That, and I had to find a way to comfortably include a bunch number of unrelated features while keeping a shared codebase. It's actually quite nice to work with, too.

------

## Where do I start?
At `SoundBending.cs`, in `SoundBending.Mod.OnSceneWasInitialized`, then `SoundBending.Mod.Startup`. After that, just look around the file and find the functions it calls.
