package files

import (
	"fmt"
	"installer/audio"
	"io"
	"os"
	"strings"
)

var RumbleRoot string

func SetupRumblePath() {
	fmt.Print("!! Please enter your RUMBLE root folder (this is usually under steamapps/common/): ")

	_, err := fmt.Scanln(&RumbleRoot)

	RumbleRoot = strings.ReplaceAll(RumbleRoot, "\\", "/")

	if RumbleRoot[:len(RumbleRoot)-1] != "/" {
		RumbleRoot += "/"
	}

	if err != nil {
		panic(err)
	}

	sbPath := RumbleRoot + "UserData/SoundBending/"

	err = os.Mkdir(sbPath, os.ModeDir|os.ModePerm)

	if err != nil && !os.IsExist(err) {
		panic(err)
	}

	err = os.Mkdir(sbPath+"Sfx", os.ModeDir|os.ModePerm)

	if err != nil && !os.IsExist(err) {
		panic(err)
	}

	err = os.Mkdir(sbPath+"Sounds", os.ModeDir|os.ModePerm)

	if err != nil && !os.IsExist(err) {
		panic(err)
	}

	file, err := os.Create(sbPath + "Config.json")

	if err != nil {
		if os.IsExist(err) {
			return
		}

		panic(err)
	}

	defer func(file *os.File) {
		err := file.Close()
		if err != nil {
			panic(err)
		}
	}(file)

	_, err = file.WriteString(`
{
    "Services": ["Soundboard", "HighNoon", "NoMute", "NamebendingIntegration"],

    "Devices": {
        "Input": "` + audio.InputDevice + `",
        "Output": "` + audio.OutputDevice + `",
        "PipeOutput": "` + audio.OutputPipe + `"
    },

    "NamebendingIntegration": {
        "Prefer": "customBentName",
        "Strict": true
    },

    "LogLevel": 2
}`)

	if err != nil {
		panic(err)
	}
}

func PopulateUserLibs() {
	entries, err := os.ReadDir("./UserLibs")

	if err != nil {
		panic(err)
	}

	for _, entry := range entries {
		oldFile, entryErr := os.OpenFile("./UserLibs/"+entry.Name(), os.O_RDONLY, os.ModePerm)

		if entryErr != nil {
			if os.IsExist(entryErr) {
				continue
			}

			panic(entryErr)
		}

		newFile, entryErr := os.Create(RumbleRoot + "UserLibs/" + entry.Name())

		if entryErr != nil {
			panic(entryErr)
		}

		copyTo(oldFile, newFile)

		entryErr = newFile.Close()

		if entryErr != nil {
			panic(entryErr)
		}
	}
}

func copyTo(from *os.File, to *os.File) {
	const BufSize = 4096

	buf := make([]byte, BufSize)

	for {
		n, err := from.Read(buf)

		if err == nil {
			_, writeErr := to.Write(buf[:n])

			if writeErr != nil {
				panic(writeErr)
			}

			continue
		}

		if err != io.EOF {
			panic(err)
		}

		break
	}
}

func PopulateSfx() {
	entries, err := os.ReadDir("./Sfx")

	if err != nil {
		panic(err)
	}

	for _, entry := range entries {
		oldFile, entryErr := os.OpenFile("./Sfx/"+entry.Name(), os.O_RDONLY, os.ModePerm)

		if entryErr != nil {
			if os.IsExist(entryErr) {
				continue
			}

			panic(entryErr)
		}

		newFile, entryErr := os.Create(RumbleRoot + "UserData/SoundBending/Sfx/" + entry.Name())

		if entryErr != nil {
			panic(entryErr)
		}

		copyTo(oldFile, newFile)

		entryErr = newFile.Close()

		if entryErr != nil {
			panic(entryErr)
		}
	}
}

func PopulateMods() {
	oldFile, err := os.OpenFile("./Mods/SoundBending.dll", os.O_RDONLY, os.ModePerm)

	if err != nil {
		if os.IsExist(err) {
			err = oldFile.Truncate(0)

			if err != nil {
				panic(err)
			}
		}

		panic(err)
	}

	newFile, err := os.Create(RumbleRoot + "Mods/SoundBending.dll")

	if err != nil {
		panic(err)
	}

	copyTo(oldFile, newFile)

	err = newFile.Close()

	if err != nil {
		panic(err)
	}
}
