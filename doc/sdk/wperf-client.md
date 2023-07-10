# WperfClient Class Documentation

## Summary

The `WperfClient` class is part of the `WindowsPerfGUI.SDK` namespace and provides functionality for interacting with the WindowsPerf (Wperf) process. This class allows you to run commands against Wperf and retrieve the output.

## Properties

- `string Path`: This property holds the path to the Wperf executable. It should be set before any commands are executed.
- `bool IsInitilized`: This private property indicates whether the `WperfClient` has been initialized. Initialization occurs before the first command execution.

## Methods

### WperfClient Class

- `void InitProcess()`: This protected method initializes the `WperfClient` for command execution. It creates a new `ProcessRunner` instance that is used to run the commands.

- `(string stdOutput, string stdError) ExecuteAwaitedCommand(params string[] args)`: This private method runs a Wperf command, specified by the `args` parameter, and waits for the command to complete. It returns the standard output and standard error from the command.

- `(WperfVersion output, string stdError) GetVersion()`: This public method executes the `wperf -version -json` command and returns the output. The standard output is deserialized into a `WperfVersion` object and returned, along with the standard error. [Read more...](../wperf-outputs/wperf-versiom.md)

- `(WperfList output, string stdError) GetEventList()`: This public method executes the `wperf list -json` command and returns the output. The standard output is deserialized into a `WperfList` object and returned, along with the standard error. [Read more...](../wperf-outputs/wperf-list.md)

- `(WperfTest output, string stdError) GetTest()`: This public method executes the `wperf test -json` command and returns the output. The standard output is deserialized into a `WperfTest` object and returned, along with the standard error. [Read more...](../wperf-outputs/wperf-test.md)

## Usage Example

```csharp
// create a new WperfClient instance
WperfClient client = new WperfClient();

// set the path to the Wperf executable
client.Path = "wperf.exe";

// retrieve the version information
(WperfVersion version, string error) = client.GetVersion();

// print version information for each component
foreach (Version component in version.Version)
{
    Console.WriteLine($"Component: {component.Component}, Version: {component.ComponentVersion}");
}

// print any errors
if (!string.IsNullOrEmpty(error))
{
    Console.WriteLine("Error: " + error);
}
```

In this example, a `WperfClient` instance is created and the `Path` property is set to the location of the Wperf executable. The `GetVersion` method is then called to retrieve version information, which is printed to the console. If any errors occurred during execution, they are also written to the console.
