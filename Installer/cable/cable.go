package cable

import (
	"archive/zip"
	"fmt"
	"io"
	"net/http"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
)

func RunSetupProgram() {
	cmd := exec.Command("powershell", "-Command", "Start-Process", "-FilePath", "\"./VBCable/VBCABLE_setup.exe\"", "-Verb", "runAs")
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	cmd.Stdin = os.Stdin

	err := cmd.Run()

	if err != nil {
		panic(err)
	}
}

func DownloadZipFile() {
	file, err := os.Create("./VBCable_Archive.zip")

	if err != nil {
		if os.IsExist(err) {
			return
		}

		panic(err)
	}

	defer func(zip *os.File) {
		err := zip.Close()
		if err != nil {
			panic(err)
		}
	}(file)

	response, err := http.Get("https://download.vb-audio.com/Download_CABLE/VBCABLE_Driver_Pack45.zip")

	if err != nil {
		panic(err)
	}

	defer func(Body io.ReadCloser) {
		closeErr := Body.Close()
		if closeErr != nil {
			panic(closeErr)
		}
	}(response.Body)

	_, err = io.Copy(file, response.Body)

	if err != nil {
		panic(err)
	}
}

func ExtractZipFile() {
	err := os.Mkdir("./VBCable", 0755)

	if err != nil {
		if os.IsExist(err) {
			return
		}

		panic(err)
	}

	reader, err := zip.OpenReader("./VBCable_Archive.zip")

	if err != nil {
		panic(err)
	}

	defer func(reader *zip.ReadCloser) {
		err := reader.Close()
		if err != nil {
			panic(err)
		}
	}(reader)

	err = unzipSource("./VBCable_Archive.zip", "./VBCable")

	if err != nil {
		panic(err)
	}
}

func unzipSource(source, destination string) error {
	// 1. Open the zip file
	reader, err := zip.OpenReader(source)
	if err != nil {
		return err
	}

	defer func(reader *zip.ReadCloser) {
		err := reader.Close()
		if err != nil {
			panic(err)
		}
	}(reader)

	// 2. Get the absolute destination path
	destination, err = filepath.Abs(destination)
	if err != nil {
		return err
	}

	// 3. Iterate over zip files inside the archive and unzip each of them
	for _, f := range reader.File {
		err := unzipFile(f, destination)
		if err != nil {
			return err
		}
	}

	return nil
}

func unzipFile(f *zip.File, destination string) error {
	// 4. Check if file paths are not vulnerable to Zip Slip
	filePath := filepath.Join(destination, f.Name)
	if !strings.HasPrefix(filePath, filepath.Clean(destination)+string(os.PathSeparator)) {
		return fmt.Errorf("invalid file path: %s", filePath)
	}

	// 5. Create directory tree
	if f.FileInfo().IsDir() {
		if err := os.MkdirAll(filePath, os.ModePerm); err != nil {
			return err
		}
		return nil
	}

	if err := os.MkdirAll(filepath.Dir(filePath), os.ModePerm); err != nil {
		return err
	}

	// 6. Create a destination file for unzipped content
	destinationFile, err := os.OpenFile(filePath, os.O_WRONLY|os.O_CREATE|os.O_TRUNC, f.Mode())
	if err != nil {
		return err
	}
	defer func(destinationFile *os.File) {
		err := destinationFile.Close()
		if err != nil {
			panic(err)
		}
	}(destinationFile)

	// 7. Unzip the content of a file and copy it to the destination file
	zippedFile, err := f.Open()
	if err != nil {
		return err
	}
	defer func(zippedFile io.ReadCloser) {
		err := zippedFile.Close()
		if err != nil {
			panic(err)
		}
	}(zippedFile)

	if _, err := io.Copy(destinationFile, zippedFile); err != nil {
		return err
	}
	return nil
}
