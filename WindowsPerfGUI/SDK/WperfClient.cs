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

using System.Collections.Generic;
using System.Linq;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.CountingSetting;
using WindowsPerfGUI.ToolWindows.SamplingSetting;
using WindowsPerfGUI.Utils.SDK;

namespace WindowsPerfGUI.SDK
{
    internal class WperfClient
    {
        protected string Path { get; set; }
        private bool IsInitialized { get; set; }

        private ProcessRunner _wperfProcess;

        public WperfClient() { }

        protected Action<string> OutputWindowTextWriter { get; set; }

        public void Reinitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            _wperfProcess = new ProcessRunner(Path);
            IsInitialized = true;
        }

        private void LogToOutput(string stdOutput, string stdError, params string[] args)
        {
            if (OutputWindowTextWriter == null)
                return;

            OutputWindowTextWriter("Executing wperf command:");
            OutputWindowTextWriter($"{Path} {string.Join(" ", args)}");
            OutputWindowTextWriter("wperf command output:");
            OutputWindowTextWriter("=============================================================");
            OutputWindowTextWriter(stdOutput);
            OutputWindowTextWriter("=============================================================");

            if (stdError == "")
                return;

            OutputWindowTextWriter("wperf command error:");
            OutputWindowTextWriter("=============================================================");
            OutputWindowTextWriter(stdError);
            OutputWindowTextWriter("=============================================================");
        }

        protected void InitProcess()
        {
            if (IsInitialized)
            {
                return;
            }

            _wperfProcess = new ProcessRunner(Path);
            IsInitialized = true;
        }

        private (string stdOutput, string stdError) ExecuteAwaitedCommand(params string[] args)
        {
            InitProcess();

            (string stdOutput, string stdError) = _wperfProcess.StartAwaitedProcess(args);

            LogToOutput(stdOutput, stdError, args);

            return (stdOutput, stdError);
        }

        /// <summary>
        /// This returns the WPerf and Wperf driver installed version
        /// it runs the command wperf -version --json
        /// </summary>
        /// <returns>
        ///     Tuple<![CDATA[<WperfVersion serializedOutput, string stdError>]]>
        /// </returns>
        public (WperfVersion output, string stdError) GetVersion()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("--version", "--json");
            WperfVersion serializedOutput = WperfVersion.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }

        /// <summary>
        /// This returns the list of Wperf's predefined events and metrics
        /// it runs the command wperf list -v --json
        /// </summary>
        /// <returns></returns>
        public (WperfList output, string stdError) GetEventList()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("list", "-v", "--json");
            WperfList serializedOutput = WperfList.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }

        /// <summary>
        /// This returns the additional data about the host
        /// it runs the command wperf test --json
        /// </summary>
        /// <returns></returns>
        public (WperfTest output, string stdError) GetTest()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("test", "--json");

            WperfTest serializedOutput = WperfTest.FromJson(stdOutput);
            try
            {
                if (stdError != "")
                    throw new Exception(stdError);

                WperfDefaults.Frequency = serializedOutput
                    .TestResults.Find(el => el.TestName == "pmu_device.sampling.INTERVAL_DEFAULT")
                    ?.Result;
                string gpc_num = serializedOutput
                    .TestResults.Find(el => el.TestName == "PMU_CTL_QUERY_HW_CFG [total_gpc_num]")
                    ?.Result;
                WperfDefaults.TotalGPCNum = Convert.ToInt32(gpc_num, 16);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                VS.MessageBox.ShowError(ErrorLanguagePack.WperfPathChanged);
            }
            return (serializedOutput, stdError);
        }

        public async Task StartSamplingAsync()
        {
            string[] samplingArgs = SamplingSettings.GenerateCommandLineArgsArray(
                SamplingSettings.samplingSettingsFrom
            );
            SamplingSettings.IsSampling = true;
            try
            {
                await _wperfProcess.StartBackgroundProcessAsync(samplingArgs);
                (WperfSampling serializedOutput, string stdError) = StopSampling();
                OnSamplingFinished?.Invoke(this, (serializedOutput, stdError));
            }
            catch (Exception e)
            {
                SamplingSettings.IsSampling = false;
                throw e;
            }
        }

        public async Task StartCountingAsync()
        {
            string[] countingArgs = CountingSettings.GenerateCommandLineArgsArray(
                CountingSettings.countingSettingsForm
            );
            CountingSettings.IsCounting = true;
            CountingSettings.countingSettingsForm.GenerateNewOutputPath();
            List<string> countingArgsList = countingArgs.ToList();
            int indexToInsertAt = countingArgsList.FindIndex(el => el.StartsWith("--json"));
            countingArgsList.Insert(
                indexToInsertAt,
                $"--output {CountingSettings.countingSettingsForm.OutputPath}"
            );
            try
            {
                await _wperfProcess.StartBackgroundProcessAsync(countingArgsList.ToArray());
                (object serializedOutput, string stdError) = StopCounting();
                OnCountingFinished?.Invoke(this, (serializedOutput, stdError));
            }
            catch (Exception e)
            {
                CountingSettings.IsCounting = false;
                throw e;
            }
        }

        public EventHandler<(
            WperfSampling serializedOutput,
            string stdError
        )> OnSamplingFinished { get; set; }

        public EventHandler<(object _, string stdError)> OnCountingFinished { get; set; }

        public (WperfSampling serializedOutput, string stdError) StopSampling()
        {
            _wperfProcess.StopProcess();
            SamplingSettings.IsSampling = false;
            string stdOutput = string.Join("", _wperfProcess.StdOutput.Output);
            string stdError = string.Join("", _wperfProcess.StdError.Output);
            WperfSampling serializedOutput = WperfSampling.FromJson(stdOutput);
            LogToOutput(
                stdOutput,
                stdError,
                SamplingSettings.GenerateCommandLineArgsArray(SamplingSettings.samplingSettingsFrom)
            );
            return (serializedOutput, stdError);
        }

        public (object serializedOutput, string stdError) StopCounting()
        {
            _wperfProcess.StopProcess();
            CountingSettings.IsCounting = false;
            string stdOutput = string.Join("", _wperfProcess.StdOutput.Output);
            string stdError = string.Join("", _wperfProcess.StdError.Output);
            WperfCounting serializedOutput = WperfCounting.FromJson(stdOutput);
            LogToOutput(
                stdOutput,
                stdError,
                CountingSettings.GenerateCommandLineArgsArray(CountingSettings.countingSettingsForm)
            );
            return (serializedOutput, stdError);
        }
    }
}
