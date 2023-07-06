# ProcessRunner

[[_TOC_]]

## Summary

The `ProcessRunner` class provides an interface for running and managing system processes. It resides within the `WindowsPerfGUI.Utils.SDK` namespace and contains methods for starting, stopping, and managing system processes both asynchronously and synchronously.

## Properties

- OutputHandler StdOutput: An output handler for standard output of the process.
- OutputHandler StdError: An output handler for error output of the process.
- Task \_BackgroundProcessTask: Represents the ongoing process task when running asynchronously.
- Process \_BackgroundProcess: The actual system process that this class manages.
- string \_Path: The path to the executable that the ProcessRunner will manage.
- IntPtr? \_ProcessorAffinity: Specifies a processor for the system to run the process on.

## Methods

- `ProcessRunner(string path)`: Constructor that takes the path of the executable as a parameter.
- `ProcessRunner(string path, IntPtr? processorAffinity)`: Constructor that takes the path of the executable and an optional processor affinity.
- `Task StartBackgroundProcessAsync(params string[] args)`: Starts the process in the background with optional command-line arguments. Returns a task - representing the ongoing process.
- `void StopProcess(bool force = false)`: Stops the process. If force is true, the process will be killed immediately, otherwise a graceful shutdown will be attempted first.
- `(string stdError, string stdOutput) StartAwaitedProcess(string[] args)`: Starts the process and waits for it to finish before returning the standard - output and error output as a tuple.

**PS:** a graceful shutdown is done by sending a <kbd>Ctrl</kbd> + <kbd>c</kbd> (`SIGINT`) signal to the process.

## Example usage

### StartBackgroundProcessAsync

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
            _Process.StartBackgroundProcessAsync("-t", "127.0.0.1");
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

In this example, a new `ProcessRunner` is created to manage an executable specified by "ping.exe". The process is then started with "-t" and "127.0.0.1" as arguments. Any standard output from the process is then written to the console. Finally, the process is stopped after 3 seconds of execution.

### StartAwaitedProcess

```csharp
// create a new ProcessRunner instance
ProcessRunner pr = new ProcessRunner("path_to_executable");

// start the process with arguments and wait for it to finish
(string stdError, string stdOutput) result = pr.StartAwaitedProcess(new string[] { "arg1", "arg2" });

// print standard output and error
Console.WriteLine("Standard Output: " + result.stdOutput);
Console.WriteLine("Standard Error: " + result.stdError);
```

In this example, a ProcessRunner instance is created to manage an executable specified by "path_to_executable". The StartAwaitedProcess method is then called with "arg1" and "arg2" as arguments. This method blocks until the process has finished executing, at which point it returns the standard output and error. These are then written to the console.
