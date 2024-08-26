# WindowsPerf GUI

WindowsPerfGUI is a Visual Studio extension that aims to provide a GUI for [Windows Perf](https://gitlab.com/Linaro/WindowsPerf/windowsperf)

## Table of content

- [WindowsPerf GUI](#windowsperf-gui)
  - [Table of content](#table-of-content)
  - [A Glimpse of the UI](#a-glimpse-of-the-ui)
  - [Getting Started](#getting-started)
    - [Install WindowsPerf](#install-windowsperf)
    - [Install the extension](#install-the-extension)
    - [Setting up the wperf path](#setting-up-the-wperf-path)
  - [Full documentation](#full-documentation)
  - [Contributing](#contributing)
  - [License](#license)
  - [Articles](#articles)

## A Glimpse of the UI

The **WindowsPerf GUI** extension is composed of several key features, each designed to streamline the user experience:

![An overview of the UI](https://gitlab.com/-/project/47090392/uploads/8fa3be9ccd52623a00ed4469bc8366f6/image__11_.png)

- **WindowsPerf Configuration**: Connect directly to `wperf.exe` for a seamless integration. Configuration is accessible via `Tools -> Options -> Windows Perf -> WindowsPerf Path`.
- **Host Data**: Understand your environment with `Tools -> WindowsPerf Host Data`, offering insights into tests run by WindowsPerf and their outcomes.
- **Output Logging**: All commands executed through the GUI are meticulously logged, ensuring transparency and aiding in performance analysis.
- **Sampling UI**: Customize your sampling experience by selecting events, setting frequency and duration, choosing programs for sampling, and comprehensively analyzing results.
- **Counting Settings UI**: Build a `wperf stat` command from scratch using our configurator, then view the output on the IDE or open it on WPA

![Counting UI overview](https://gitlab.com/-/project/47090392/uploads/d86f6b0c9ce28e772f41bf0c1dbaa4cb/image.png)

## Getting Started

### Install WindowsPerf

You can find latest WindowsPerf installation instructions in [INSTALL.md](https://gitlab.com/Linaro/WindowsPerf/windowsperf/-/blob/main/INSTALL.md?ref_type=heads).

### Install the extension

You can download and install the extension from the Visual Studio marketplace, or using the `Extensions -> Manage Extensions` menu from within the IDE.

### Setting up the wperf path

In order to set the path to the `wperf` executable, go to `Tools -> Options -> Windows Perf -> Wperf Path` and set the **absolute** path to the `wperf.exe` executable and then on the **Validate** button. If all is well, you should see the version of `wperf.exe` as well as `wperf-driver`, It will also populate the list of available events and metrics in `wperf. If not, you will see an error message.
PS: as long as this step has not been fulfilled, the extension will not work.

## Full documentation

To get started with the project, see our [Wiki page](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/wikis/home).

## Contributing

To contribute to the project follow our [Contributing Guidelines](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/blob/v2.0.0/CONTRIBUTING.md?ref_type=tags).

## License

All code in this repository is licensed under the [BSD 3-Clause License](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/blob/v2.0.0/LICENSE?ref_type=tags)

## Articles

- [Launching WindowsPerf Visual Studio Extension v2.1.0](https://www.linaro.org/blog/launching--windowsperf-visual-studio-extension-v210/)
- [Introducing the WindowsPerf GUI: the Visual Studio 2022 extension](https://www.linaro.org/blog/introducing-the-windowsperf-gui-the-visual-studio-2022-extension/)
- [Introducing 1.0.0-beta release of WindowsPerf Visual Studio extension](https://www.linaro.org/blog/introducing-1-0-0-beta-release-of-windowsperf-visual-studio-extension/)
