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
using System.IO;
using System.Linq;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.CountingSetting;
using WindowsPerfGUI.ToolWindows.SamplingSetting;
using WindowsPerfGUI.Utils;
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
        /// This returns wether or not SPE is supported on this machine
        /// it runs the command wperf -version --json and wperf test --json
        /// it then checks for "spe_device.version_name" in the test results and the feature string in the version results
        /// </summary>
        /// <returns>
        ///     bool
        /// </returns>
        public bool CheckIsSPESupported()
        {
            try
            {
                (WperfVersion versionSerializedOutput, _) = GetVersion();
                foreach (var component in versionSerializedOutput.Components)
                {
                    if (!component.FeatureString.Contains("+spe"))
                    {
                        return false;
                    }
                }

                (WperfTest testSerializedOutput, _) = GetTest();

                TestResult speDeviceConf = testSerializedOutput
                    .TestResults.Find(el => el.TestName == "spe_device.version_name");
                if (speDeviceConf == null) { return false; }

                WperfDefaults.SPEFeatureName = speDeviceConf.Result;
                return speDeviceConf.Result.StartsWith("FEAT_SPE");
            }
            catch (Exception)
            {

                return false;
            }


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
                if (SamplingSettings.samplingSettingsFrom.IsSPEEnabled)
                {
                    (WperfSPE serializedOutput, string stdError) = StopSPESampling();
                    OnSPESamplingFinished?.Invoke(this, (serializedOutput, stdError));
                }
                else
                {
                    (WperfSampling serializedOutput, string stdError) = StopSampling();
                    OnSamplingFinished?.Invoke(this, (serializedOutput, stdError));
                }

            }
            catch (Exception e)
            {
                SamplingSettings.IsSampling = false;
                throw e;
            }
        }

        public static string OutputPath;

        static void GenerateNewOutputPath()
        {
            string now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
            OutputPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "wperf-stat-output-" + now
            );
        }

        public async Task StartCountingAsync()
        {
            string[] countingArgs = CountingSettings.GenerateCommandLineArgsArray(
                CountingSettings.countingSettingsForm
            );
            CountingSettings.IsCounting = true;
            GenerateNewOutputPath();
            List<string> countingArgsList = countingArgs.ToList();
            int indexToInsertAt = countingArgsList.FindIndex(el => el.StartsWith("--json"));
            string wperfOutputCommand = $"--output {OutputPath}.json";

            if (CountingSettings.countingSettingsForm.IsTimelineSelected)
            {
                wperfOutputCommand += $" --output-csv {OutputPath}.csv";
            }
            countingArgsList.Insert(indexToInsertAt, wperfOutputCommand);
            try
            {
                await _wperfProcess.StartBackgroundProcessAsync(countingArgsList.ToArray());
                (List<CountingEvent> countingEvents, string stdError) = StopCounting();
                OnCountingFinished?.Invoke(this, (countingEvents, stdError));
            }
            catch (Exception e)
            {
                CountingSettings.IsCounting = false;
                throw e;
            }
        }

        public static List<CountingEvent> GetCountingEventsFromJSONFile(string filePath)
        {
            List<CountingEvent> countingEvents = new();

            if (string.IsNullOrEmpty(filePath)) return countingEvents;

            string jsonContent = File.ReadAllText(filePath);
            bool isTimelineJSON = jsonContent.Contains("timeline");
            if (isTimelineJSON)
            {
                WperfTimeline wperfTimeline = WperfTimeline.FromJson(jsonContent);
                foreach (var count in wperfTimeline.Timeline)
                {
                    ProcessSingleCount(count, countingEvents, true);
                }
            }
            else
            {
                WperfCounting wperfCount = WperfCounting.FromJson(jsonContent);
                ProcessSingleCount(wperfCount, countingEvents);
            }

            return countingEvents;
        }

        private static void ProcessSingleCount(
            WperfCounting count,
            List<CountingEvent> countingEvents,
            bool accumulatePerCoreAndEvent = false
        )
        {
            foreach (CorePerformanceCounter core in count.Core.PerformanceCounters)
            {
                foreach (CorePerformanceCounterItem rawCountingEvent in core.PerformanceCounter)
                {
                    int index = countingEvents.FindIndex(
                        el =>
                            el.CoreNumber == core.CoreNumber
                            && el.Name == rawCountingEvent.EventName
                    );
                    if (accumulatePerCoreAndEvent && index != -1)
                    {
                        countingEvents[index].Value += rawCountingEvent.CounterValue;
                    }
                    else
                    {
                        CountingEvent countingEvent =
                            new()
                            {
                                CoreNumber = core.CoreNumber,
                                Value = rawCountingEvent.CounterValue,
                                Name = rawCountingEvent.EventName,
                                Index = rawCountingEvent.EventIdx,
                                Note = rawCountingEvent.EventNote,
                            };

                        countingEvents.Add(countingEvent);
                    }
                }
            }
        }

        public EventHandler<(
            WperfSampling serializedOutput,
            string stdError
        )> OnSamplingFinished
        { get; set; }

        public EventHandler<(
          WperfSPE serializedOutput,
          string stdError
      )> OnSPESamplingFinished
        { get; set; }

        public EventHandler<(
            List<CountingEvent> countingEvents,
            string stdError
        )> OnCountingFinished
        { get; set; }

        public (WperfSampling serializedOutput, string stdError) StopSampling()
        {
            _wperfProcess.StopProcess();
            SamplingSettings.IsSampling = false;
            string stdOutput = string.Join("\n", _wperfProcess.StdOutput.Output);
            string stdError = string.Join("\n", _wperfProcess.StdError.Output);
            if (SamplingSettings.samplingSettingsFrom.IsSPEEnabled)
            {
                WperfSPE wperfSPE = WperfSPE.FromJson(stdOutput);

            }
            WperfSampling serializedOutput = WperfSampling.FromJson(stdOutput);
            LogToOutput(
                stdOutput,
                stdError,
                SamplingSettings.GenerateCommandLineArgsArray(SamplingSettings.samplingSettingsFrom)
            );
            return (serializedOutput, stdError);
        }
        public (WperfSPE serializedOutput, string stdError) StopSPESampling()
        {
            _wperfProcess.StopProcess();
            SamplingSettings.IsSampling = false;
            string stdOutput = string.Join("\n", _wperfProcess.StdOutput.Output);
            string stdError = string.Join("\n", _wperfProcess.StdError.Output);
            WperfSPE serializedOutput = WperfSPE.FromJson(stdOutput);
            LogToOutput(
                stdOutput,
                stdError,
                SamplingSettings.GenerateCommandLineArgsArray(SamplingSettings.samplingSettingsFrom)
            );
            return (serializedOutput, stdError);
        }
        public (List<CountingEvent> countingEvents, string stdError) StopCounting()
        {
            _wperfProcess.StopProcess();
            CountingSettings.IsCounting = false;
            string stdOutput = string.Join("\n", _wperfProcess.StdOutput.Output);
            string stdError = string.Join("\n", _wperfProcess.StdError.Output);
            LogToOutput(
                stdOutput,
                stdError,
                CountingSettings.GenerateCommandLineArgsArray(CountingSettings.countingSettingsForm)
            );
            List<CountingEvent> countingEvents = GetCountingEventsFromJSONFile(OutputPath + ".json");
            return (countingEvents, stdError);
        }
    }
}
