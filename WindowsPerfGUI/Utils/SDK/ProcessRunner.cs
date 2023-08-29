// BSD 3-Clause License
//
// Copyright (c) 2022, Arm Limited
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
        public OutputHandler StdOutput;

        public OutputHandler StdError;

        private Task _BackgroundProcessTask;

        private Process _BackgroundProcess;

        private readonly string _Path;

        private readonly IntPtr? _ProcessorAffinity;

        private CancellationTokenSource _BackgroundProcessCancelationToken;

        private bool _HasProcessStarted = false;

        #endregion Properties

        public ProcessRunner(string path)
        {
            _Path = path;
            StdOutput = new OutputHandler();
            StdError = new OutputHandler();
        }

        public ProcessRunner(string path, IntPtr? processorAffinity)
        {
            _Path = path;
            StdOutput = new OutputHandler();
            StdError = new OutputHandler();
            _ProcessorAffinity = processorAffinity;
        }

        public Task StartBackgroundProcessAsync(params string[] args)
        {
            _BackgroundProcessCancelationToken = new CancellationTokenSource();
            _BackgroundProcessTask = Task.Run(() => _StartBackgroundProcessAsync(args), _BackgroundProcessCancelationToken.Token);
            return _BackgroundProcessTask;
        }

        public void StopProcess(bool force = false)
        {
            const int waitForExitTimeout = 2000;
            if (force) { ForceKillProcess(); return; }
            if (_BackgroundProcess.HasExited) { return; }
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
        public (string stdError, string stdOutput) StartAwaitedProcess(string[] args)
        {
            InitProcess(args);
            try
            {
                _BackgroundProcess.Start();
                _HasProcessStarted = true;
            }
            catch (Exception e)
            {

                throw new Exception($"Failed to start process {_Path} with arguments {string.Join(" ", args)}", e);
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
            _BackgroundProcess.OutputDataReceived += new DataReceivedEventHandler(StdOutput.OutputhHandler);
            _BackgroundProcess.ErrorDataReceived += new DataReceivedEventHandler(StdError.OutputhHandler);
            _BackgroundProcess.Start();
            if (_ProcessorAffinity != null)
            {
                _BackgroundProcess.ProcessorAffinity = (IntPtr)_ProcessorAffinity;
            }
            _BackgroundProcess.BeginOutputReadLine();
            _BackgroundProcess.BeginErrorReadLine();
            await _BackgroundProcess.WaitForExitAsync();

        }

        private void InitProcess(string[] args)
        {
            if (_BackgroundProcess != null && !_BackgroundProcess.HasExited) throw new Exception("Process already running");
            _BackgroundProcess = new Process()
            {
                StartInfo = {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
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
