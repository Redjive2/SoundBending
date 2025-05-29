package main

import (
	"fmt"
	"installer/audio"
	"installer/cable"
	"installer/files"
)

func main() {
	fmt.Println(
		"[SoundBending Installer V0.0.1] Downloading VBCable driver pack at `https://download.vb-audio.com/Download_CABLE/VBCABLE_Driver_Pack45.zip` (this may take a moment)")

	cable.DownloadZipFile()

	fmt.Println("[SoundBending Installer] File downloaded! Extracting.")

	cable.ExtractZipFile()

	fmt.Println("[SoundBending Installer] File extracted. Running setup tool.")

	cable.RunSetupProgram()

	fmt.Println("[SoundBending Installer] VBCable setup completed.")

	devices := audio.ListDevices()

	audio.WriteDeviceTable(devices)

	audio.CollectDeviceNames(devices)

	fmt.Printf("[SoundBending Installer] Devices selected: `%s` / `%s` / `%s`\n", audio.OutputDevice, audio.InputDevice, audio.OutputPipe)

	files.SetupRumblePath()

	fmt.Println("[SoundBending Installer] Rumble setup completed.")

	files.PopulateMods()

	fmt.Println("[SoundBending Installer] Mod file copied.")

	files.PopulateSfx()

	fmt.Println("[SoundBending Installer] SFX folder populated.")

	files.PopulateUserLibs()

	fmt.Println("[SoundBending Installer] Required libraries added.")

	fmt.Println("\n!!!   [SoundBending Installer] You may now delete this installer!   !!!")
}
