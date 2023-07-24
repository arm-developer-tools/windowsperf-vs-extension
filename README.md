# vs-extension

## What is WindowsPerfGUI

WindowsPerfGUI is a Visual Studio extension that aims to provide a GUI for [Windows Perf](https://gitlab.com/Linaro/WindowsPerf/windowsperf)

## How to use Windows Perf

### Setting up the wperf path

In order to set the path to the `wperf` executable, go to `Tools -> Options -> Windows Perf -> Wperf Path` and set the **absolute** path to the `wperf.exe` executable and then on the **Validate** button. If all is well, you should see the version of `wperf.exe` as well as `wperf-driver`. If not, you will see an error message.
PS: as long as this step has not been fulfilled, the extension will not work.

![Update settings tutorial](doc/resources/update-settings.gif)

## What can you do with Visual Studio extension for WindowsPerfGUI

Under construction...

## Project Structure

```bash
└───WindowsPerfGUI
    ├───Commands    (Commands for the extension)
    ├───Properties  (Contains the AssemblyInfo.cs file that describes the application metadata)
    ├───Resources   (Contains the icons for the extension)
    └───ToolWindows (Contains the code for the tool window)
```

## Authors and acknowledgment

Show your appreciation to those who have contributed to the project.

## Code Documentation

To get started with the project, see our [Code Docs](doc/README.md) page.

## Contributing

To contribute to the project follow our [Contributing Guidelines](CONTRIBUTING.md).

## License

All code in this repository is licensed under the [BSD 3-Clause License](LICENSE)
