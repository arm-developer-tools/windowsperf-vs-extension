# WindowsPerfGUI Documentation

[[_TOC_]]

## How to install WindowsPerfGUI extension for Visual Studio

First, you need to have the binary of the extension. and then you can install it by clicking on `WindowsPerfGUI.vsix` and then click on `Install`.

PS: you need to make sure that any previous version of the extension is uninstalled and that Visual Studio is closed before installing the extension.

## WperfClient

`WperfClient` is the SDK that is used to communicate with the `wperf.exe` process. It is used to send commands to the `wperf.exe` process and to receive its output.

- [WperfClient class](sdk/wperf-client.md).

## Utility Classes

- [OutputHanlder](utility-classes/output-handler.md)
  - This class is used to handle the output of any process. It is used to capture it and streamline its processing.
- [ProcessRunner](utility-classes/process-runner.md)
  - This class is used to run the wperf.exe process. It is used to run the process and to handle its output.

## WperfOutputs

`WperfOutputs` is a collection of classes that contains the types of the `JSON` output from the `wperf.exe` process. It is used to deserialize the `JSON` output from the `wperf.exe` process.

- [WperfVersion](wperf-outputs/wperf-version.md)
  - This class is used to represent the version information for components in the WindowsPerf (`wperf`) system using `wperf -version`.
- [WperfList](wperf-outputs/wperf-list.md)
  - The `WperfList` class is a part of the `WindowsPerfGUI.SDK.WperfOutputs` namespace. It represents a collection of predefined events and metrics using `wperf list`.
- [WperfTest](wperf-outputs/wperf-test.md)
  - This class is a part of the `WindowsPerfGUI.SDK.WperfOutputs` namespace. It represents a collection of test results that represents additional data about the host using `wperf test`.
