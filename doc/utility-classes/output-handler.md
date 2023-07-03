# OutputHandler

## Description

OutputHandler is a class that provides a simple way handle `DataReceivedEventHandler` and plug to the `StdOutput` / `StdError` of process.

## Methods / Properties

- `OutputCb` is callback function that is executed whenever the process outputs something.
- `Output` A `List<string>` that contains the output of the process.
- **constructor**: the constructor takes no parameters.

## Example usage

```csharp
var StdOutput = new OutputHandler();
var StdError = new OutputHandler();
var process = new Process();
{
  StartInfo = {
    FileName = "ping.exe",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    RedirectStandardInput = true,
    Arguments = "-t 127.0.0.1"
  }
}
process.OutputDataReceived += new DataReceivedEventHandler(StdOutput.OutputhHandler);
process.ErrorDataReceived += new DataReceivedEventHandler(StdError.OutputhHandler);
process.Start();

// if you want to react to all the incoming data
StdOutput.OutputCb = (string data) => {
  Debug.WriteLine(data);
};
StdError.OutputCb = (string data) => {
  Debug.WriteLine(data);
};
```
