package audio

/*
#cgo LDFLAGS: -lwinmm
#include <windows.h>
#include <mmsystem.h>
*/
import "C"
import (
	"fmt"
	"strconv"
	"strings"
	"unsafe"
)

var InputDevice, OutputDevice, OutputPipe string

type DeviceData struct {
	Number int
	Name   string
}

type DeviceRegistry struct {
	WaveIn  []DeviceData
	WaveOut []DeviceData
}

// ask me about this if you see it. there's a story attached.

func ListDevices() DeviceRegistry {
	var reg DeviceRegistry

	// Output (waveOut) devices
	numOut := int(C.waveOutGetNumDevs())
	for i := 0; i < numOut; i++ {
		var caps C.WAVEOUTCAPS
		if C.waveOutGetDevCaps(
			C.UINT64(i),
			(*C.WAVEOUTCAPS)(unsafe.Pointer(&caps)),
			C.UINT(unsafe.Sizeof(caps)),
		) == C.MMSYSERR_NOERROR {
			name := C.GoString(&caps.szPname[0])
			reg.WaveOut = append(reg.WaveOut, DeviceData{Number: i, Name: name})
		}
	}

	// Input (waveIn) devices
	numIn := int(C.waveInGetNumDevs())
	for i := 0; i < numIn; i++ {
		var caps C.WAVEINCAPS
		if C.waveInGetDevCaps(
			C.UINT64(i),
			(*C.WAVEINCAPS)(unsafe.Pointer(&caps)),
			C.UINT(unsafe.Sizeof(caps)),
		) == C.MMSYSERR_NOERROR {
			name := C.GoString(&caps.szPname[0])
			reg.WaveIn = append(reg.WaveIn, DeviceData{Number: i, Name: name})
		}
	}

	return reg
}

func CollectDeviceNames(devices DeviceRegistry) {
	var outputNum, inputNum, outpipeNum int

	fmt.Print("Enter the output device number matching your speakers/headphones' name: ")
	_, err := fmt.Scanln(&outputNum)

	if err != nil {
		panic(err)
	}

	fmt.Print("Enter the input device number matching your microphone's name: ")
	_, err = fmt.Scanln(&inputNum)

	if err != nil {
		panic(err)
	}

	fmt.Print("Enter the output device number matching your virtual cable input's name: ")
	_, err = fmt.Scanln(&outpipeNum)

	if err != nil {
		panic(err)
	}

	OutputDevice = findName(devices.WaveOut, outputNum)
	InputDevice = findName(devices.WaveIn, inputNum)
	OutputPipe = findName(devices.WaveOut, outpipeNum)
}

func findName(waveOut []DeviceData, targetNum int) string {
	for _, device := range waveOut {
		if device.Number == targetNum {
			return device.Name
		}
	}

	panic("Could not find device with number `" + strconv.Itoa(targetNum) + "`")
}

func WriteDeviceTable(devices DeviceRegistry) {
	fmt.Print("\n[SoundBending Installer] Detected audio devices:\n\n")
	fmt.Println("  /--------+----+----------------------------------------------\\")
	fmt.Println("  | Type   | ## | Name                                         |")
	fmt.Println("  |--------|----|----------------------------------------------|")

	for _, device := range devices.WaveIn {
		devnumFormatted := strconv.Itoa(device.Number)

		if len(devnumFormatted) == 1 {
			devnumFormatted = devnumFormatted + " "
		}

		devnameFormatted := "`" + device.Name + "`"

		devnameFormatted += strings.Repeat(" ", len("                   Name                     ")-len(devnameFormatted))

		fmt.Println("  | Input  | " + devnumFormatted + " | " + devnameFormatted + " |")
	}

	fmt.Println("  |--------|----|----------------------------------------------| ")

	for _, device := range devices.WaveOut {
		devnumFormatted := strconv.Itoa(device.Number)

		if len(devnumFormatted) == 1 {
			devnumFormatted += " "
		}

		devnameFormatted := "`" + device.Name + "`"

		devnameFormatted += strings.Repeat(" ", len("                   Name                     ")-len(devnameFormatted))

		fmt.Println("  | Output | " + devnumFormatted + " | " + devnameFormatted + " |")
	}

	fmt.Println("  \\--------+----+----------------------------------------------/")

	fmt.Println()
}
