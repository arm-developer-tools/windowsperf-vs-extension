// BSD 3-Clause License
//
// Copyright (c) 2024, Arm Limited
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its
//    contributors may be used to endorse or promote products derived from
//    this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


using Microsoft.VisualStudio.Threading;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindowsPerfGUI.Utils.SDK
{
    /// <summary>
    /// The `ProcessRunner` class provides an interface for running and managing system processes. 
    /// It resides within the `WindowsPerfGUI.Utils.SDK` namespace and contains methods for starting, stopping, 
    /// and managing system processes both asynchronously and synchronously.
    /// </summary>
    class ProcessRunner
    {
        #region Process Control
        private enum CtrlTypes : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

        delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);
        #endregion Process Control

        #region Properties
        /// <summary>
        /// An output handler for standard output of the process.
        /// </summary>
        public OutputHandler StdOutput;

        /// <summary>
        /// OutputHandler StdError: An output handler for error output of the process.
        /// </summary>
        public OutputHandler StdError;

        /// <summary>
        /// Represents the ongoing process task when running asynchronously.
        /// </summary>
        private Task _BackgroundProcessTask;

        /// <summary>
        /// The actual system process that this class manages.
        /// </summary>
        private Process _BackgroundProcess;

        /// <summary>
        /// The path to the executable that the ProcessRunner will manage.
        /// </summary>
        private readonly string _Path;

        /// <summary>
        /// Specifies a processor for the system to run the process on.
        /// </summary>
        private readonly IntPtr? _ProcessorAffinity;

        private CancellationTokenSource _BackgroundProcessCancelationToken;

        private bool _HasProcessStarted = false;

        #endregion Properties

        /// <summary>
        /// Constructor that takes the path of the executable as a parameter.
        /// </summary>
        /// <param name="path">A string pointing to the process that you want to run </param>
        public ProcessRunner(string path)
        {
            _Path = path;
            StdOutput = new OutputHandler();
            StdError = new OutputHandler();
        }
        /// <summary>
        /// Constructor that takes the path of the executable and an optional processor affinity.
        /// </summary>
        /// <param name="path">A string pointing to the process that you want to run </param>
        /// <param name="processorAffinity">The processor number that you want to pin that process to.</param>
        public ProcessRunner(string path, IntPtr? processorAffinity)
        {
            _Path = path;
            StdOutput = new OutputHandler();
            StdError = new OutputHandler();
            _ProcessorAffinity = processorAffinity;
        }

        /// <summary>
        /// Starts the process in the background with optional command-line arguments. Returns a task - representing the ongoing process.
        /// </summary>
        /// <param name="args">an array of strings that will be appended to the process as command line arguments</param>
        /// <returns>void</returns>
        /// <example>
        /// <code>
        ///          internal class Program
        ///             {
        ///                 private static ProcessRunner _Process;
        ///                 static void start()
        ///                 {
        ///                     // set the affinity to core 3
        ///                     IntPtr affinity = (IntPtr)(1 << 3);
        ///                     // using the ping.exe as an example
        ///                     _Process = new ProcessRunner("ping.exe", affinity);
        ///                     // passing the -t 127.0.0.1 as argument to ping.exe and starting the process
        ///                     _Process.StartBackgroundProcessAsync("-t", "127.0.0.1");
        ///                     Console.WriteLine("process Started");
        ///         
        ///                 }
        ///                 static void stop()
        ///                 {
        ///                     // gracefully stopping the process
        ///                     _Process.StopProcess();
        ///                     Console.WriteLine("process should be stopped");
        ///         
        ///                 }
        ///                 static void Main(string[] args)
        ///                 {
        ///                     start();
        ///         
        ///                     _Process.StdOutput.OutputhCb = (string data) =>
        ///                     {
        ///                         // capturing the output of the process in this callback function
        ///                         Console.WriteLine(data);
        ///                         return data;
        ///                     };
        ///                     // running the process for 3 seconds
        ///                     Thread.Sleep(3000);
        ///                     // shutting down the process
        ///                     stop();
        ///                     Console.WriteLine("Hello World!");
        ///                 }
        ///             }
        /// </code>
        /// </example>
        public Task StartBackgroundProcessAsync(params string[] args)
        {
            _BackgroundProcessCancelationToken = new CancellationTokenSource();
            _BackgroundProcessTask = Task.Run(
                () => _StartBackgroundProcessAsync(args),
                _BackgroundProcessCancelationToken.Token
            );
            return _BackgroundProcessTask;
        }

        /// <summary>
        /// Stops the process. If force is true, the process will be killed immediately, otherwise a graceful shutdown will be attempted first.
        /// a graceful shutdown is done by sending a <kbd>Ctrl</kbd> + <kbd>c</kbd> (`SIGINT`) signal to the process.
        /// </summary>
        /// <param name="force">If true, the process will be killed immediately</param>
        public void StopProcess(bool force = false)
        {
            const int waitForExitTimeout = 2000;
            if (force)
            {
                ForceKillProcess();
                return;
            }
            if (!_HasProcessStarted || _BackgroundProcess == null || _BackgroundProcess.HasExited)
            {
                return;
            }
            _BackgroundProcessCancelationToken.Cancel(true);
            if (!AttachConsole((uint)_BackgroundProcess.Id))
            {
                return;
            }

            // Disable Ctrl-C handling for our program
            SetConsoleCtrlHandler(null, true);

            // Sent Ctrl-C to the attached console
            GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);
            // Wait for the graceful end of the process.
            // Must wait here. If we don't wait and re-enable Ctrl-C handling below too fast, we might terminate ourselves.
            _BackgroundProcess.WaitForExit(waitForExitTimeout);

            // if the process did not exit by the end of the 'waitForExitTimeout' we force kill the process
            ForceKillProcess();

            FreeConsole();

            SetConsoleCtrlHandler(null, false);
        }
        /// <summary>
        /// Starts the process and waits for it to finish before returning the standard - output and error output as a tuple.
        /// </summary>
        /// <param name="args">an array of strings that will be appended to the process as command line arguments</param>
        /// <returns>the standard output and error output as a tuple</returns>
        /// <exception cref="Exception"></exception>
        /// <example>
        ///     <code>
        ///       // create a new ProcessRunner instance
        ///       ProcessRunner pr = new ProcessRunner("path_to_executable");
        ///       
        ///       // start the process with arguments and wait for it to finish
        ///       (string stdError, string stdOutput) result = pr.StartAwaitedProcess(new string[] { "arg1", "arg2" });
        ///       
        ///       // print standard output and error
        ///       Console.WriteLine("Standard Output: " + result.stdOutput);
        ///       Console.WriteLine("Standard Error: " + result.stdError);
        ///     </code>
        /// </example>
        public (string stdOutput, string stdError) StartAwaitedProcess(string[] args)
        {
            InitProcess(args);
            try
            {
                _BackgroundProcess.Start();
                _HasProcessStarted = true;
            }
            catch (Exception e)
            {
                _BackgroundProcess = null;
                _HasProcessStarted = false;
                throw new Exception(
                    $"Failed to start process {_Path} with arguments {string.Join(" ", args)}",
                    e
                );
            }
            if (_ProcessorAffinity != null)
            {
                _BackgroundProcess.ProcessorAffinity = (IntPtr)_ProcessorAffinity;
            }
            string stdOutput = "";
            string stdError = "";

            while (!_BackgroundProcess.HasExited)
            {
                stdOutput += _BackgroundProcess.StandardOutput.ReadToEnd();
                stdError += _BackgroundProcess.StandardError.ReadToEnd();
            }
            _BackgroundProcess.WaitForExit();
            return (stdOutput, stdError);
        }

        private async Task _StartBackgroundProcessAsync(string[] args)
        {
            InitProcess(args);
            StdOutput.ClearOutput();
            StdError.ClearOutput();
            _BackgroundProcess.OutputDataReceived += new DataReceivedEventHandler(
                StdOutput.OutputhHandler
            );
            _BackgroundProcess.ErrorDataReceived += new DataReceivedEventHandler(
                StdError.OutputhHandler
            );
            try
            {
                _BackgroundProcess.Start();
                _HasProcessStarted = true;
            }
            catch (Exception e)
            {
                _BackgroundProcess = null;
                _HasProcessStarted = false;
                throw new Exception(
                    $"Failed to start process {_Path} with arguments {string.Join(" ", args)}",
                    e
                );
            }

            if (_ProcessorAffinity != null)
            {
                _BackgroundProcess.ProcessorAffinity = (IntPtr)_ProcessorAffinity;
            }
            _BackgroundProcess.BeginOutputReadLine();
            _BackgroundProcess.BeginErrorReadLine();
            await _BackgroundProcess.WaitForExitAsync();
            ForceKillProcess();
        }

        private void InitProcess(string[] args)
        {
            if (_BackgroundProcess != null && !_BackgroundProcess.HasExited)
                throw new Exception("Process already running");
            string now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
            _BackgroundProcess = new Process()
            {
                StartInfo =
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            WorkingDirectory = System.IO.Path.GetTempPath(),
            FileName = _Path,
            Arguments = string.Join(" ", args)
        },
                EnableRaisingEvents = true,
            };
        }

        private void ForceKillProcess()
        {
            _BackgroundProcessCancelationToken?.Cancel(true);
            if (_HasProcessStarted && _BackgroundProcess != null && !_BackgroundProcess.HasExited)
            {
                _BackgroundProcess.CancelOutputRead();
                _BackgroundProcess.CancelErrorRead();
                _BackgroundProcess.Kill();
            }
        }

        ~ProcessRunner()
        {
            // kill the running process before garbage collection
            ForceKillProcess();
        }
    }
}
