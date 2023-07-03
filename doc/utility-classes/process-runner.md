# ProcessRunner

## Summary

A utility class for running processes, gracefully shutting them down and capturing their output.

## Methods / Properties

- Constructor: the constructor takes two parameters:
  - `string path` the path for the executable process that we need to run.
  - `intPtr processorAffinity` that represents the processor affinity for the process (MUST be binary, for core number 3 ==> (1 << 3 )).
- `StdOutput` is an [OutputHandler](output-handler.md) that handles the output of the process.
- `StdError` is an [OutputHandler](output-handler.md) that handles the error output of the process.
- `StartProcessAsync` is an async method that starts the process and returns a `Task` that represents the process.
  - It takes and array of strings as arguments to be passed to the spawning process.
- `StopProcess` is a method that stops the process.
  - It takes an optional boolean parameter that indicates whether to force kill the process or to do it gracefully. It defaults to `false` (gracefull shutdown).

**PS:** a graceful shutdown is done by sending a <kbd>Ctrl</kbd> + <kbd>c</kbd> (`SIGINT`) signal to the process.

## Example usage

```csharp
 internal class Program
    {
        private static ProcessRunner _Process;
        static void start()
        {
            // set the affinity to core 3
            IntPtr affinity = (IntPtr)(1 << 3);
            // using the ping.exe as an example
            _Process = new ProcessRunner("ping.exe", affinity);
            // passing the -t 127.0.0.1 as argument to ping.exe and starting the process
            _Process.StartProcessAsync("-t 127.0.0.1");
            Console.WriteLine("process Started");

        }
        static void stop()
        {
            // gracefully stopping the process
            _Process.StopProcess();
            Console.WriteLine("process should be stopped");

        }
        static void Main(string[] args)
        {
            start();

            _Process.StdOutput.OutputhCb = (string data) =>
            {
                // capturing the output of the process in this callback function
                Console.WriteLine(data);
                return data;
            };
            // running the process for 3 seconds
            Thread.Sleep(3000);
            // shutting down the process
            stop();
            Console.WriteLine("Hello World!");
        }
    }
```
