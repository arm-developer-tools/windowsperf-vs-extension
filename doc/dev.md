# Windows Perf GUI Code Docs

## Table of Contents

- [Windows Perf GUI Code Docs](#windows-perf-gui-code-docs)
  - [Table of Contents](#table-of-contents)
  - [WperfClient](#wperfclient)
  - [Utility Classes](#utility-classes)

## WperfClient

`WperfClient` is the SDK that is used to communicate with the `wperf.exe` process. It is used to send commands to the `wperf.exe` process and to receive its output.

## Utility Classes

- [OutputHanlder](utility-classes/output-handler.md)
  - This class is used to handle the output of any process. It is used to capture it and streamline its processing.
- [ProcessRunner](utility-classes/process-runner.md)
  - This class is used to run the wperf.exe process. It is used to run the process and to handle its output.
