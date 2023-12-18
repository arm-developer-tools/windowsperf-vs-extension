# vs-extension

## What is WindowsPerfGUI

WindowsPerfGUI is a Visual Studio extension that aims to provide a GUI for [Windows Perf](https://gitlab.com/Linaro/WindowsPerf/windowsperf)

## How to use Windows Perf

### Setting up the wperf path

In order to set the path to the `wperf` executable, go to `Tools -> Options -> Windows Perf -> Wperf Path` and set the **absolute** path to the `wperf.exe` executable and then on the **Validate** button. If all is well, you should see the version[^1] of `wperf.exe` as well as `wperf-driver`, It will also populate the list of available events and metrics[^2] in `wperf. If not, you will see an error message.
PS: as long as this step has not been fulfilled, the extension will not work.

## Project Structure

```bash
└───WindowsPerfGUI
    ├───Commands    (Commands for the extension)
    ├───Properties  (Contains the AssemblyInfo.cs file that describes the application metadata)
    ├───Resources   (Contains the icons for the extension as well as all the locals)
    ├───ToolWindows (Contains the code for the tool window)
    ├───Options     (Contains The settings page for the extension)
    ├───SDK         (Contains a wrapper around wperf the cli)
    ├───Utils       (Contains utility functions and classes)
    ├───Components  (Contains a set of reusable components)
    └───Themes      (Contains the main style for the generic components)
```

## Wiki page

To get started with the project, see our [Wiki page](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/wikis/home).

## Contributing

To contribute to the project follow our [Contributing Guidelines](CONTRIBUTING.md).

## License

All code in this repository is licensed under the [BSD 3-Clause License](LICENSE)

## Articles

- [Introducing the WindowsPerf GUI: the Visual Studio 2022 extension](https://www.linaro.org/blog/introducing-the-windowsperf-gui-the-visual-studio-2022-extension/)
- [Introducing 1.0.0-beta release of WindowsPerf Visual Studio extension](https://www.linaro.org/blog/introducing-1-0-0-beta-release-of-windowsperf-visual-studio-extension/)
