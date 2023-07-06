# WindowsPerfGUI Documentation

[[_TOC_]]

## WperfClient

`WperfClient` is the SDK that is used to communicate with the `wperf.exe` process. It is used to send commands to the `wperf.exe` process and to receive its output.

- [WperfClient class](wperf-client.md).

## Utility Classes

- [OutputHanlder](utility-classes/output-handler.md)
  - This class is used to handle the output of any process. It is used to capture it and streamline its processing.
- [ProcessRunner](utility-classes/process-runner.md)
  - This class is used to run the wperf.exe process. It is used to run the process and to handle its output.

## WperfOutputs

`WperfOutputs` is a collection of classes that contains the types of the `JSON` output from the `wperf.exe` process. It is used to deserialize the `JSON` output from the `wperf.exe` process.

- [WperfVersion](wperf-outputs\wperf-version.md)
  - This class is used to represent the version information for components in the Windows Performance (Wperf) system. It is capable of parsing this information from a JSON string.
