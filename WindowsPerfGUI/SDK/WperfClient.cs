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
    /// <summary>
    /// This class is responsible for running WindowsPerf commands and handling their output.
    /// It uses the ProcessRunner class to run the commands and parse the output.
    /// <example>
    /// <code >
    ///  // create a new WperfClient instance
    ///  WperfClient client = new WperfClient();
    ///
    ///  // set the path to the Wperf executable
    ///  client.Path = "wperf.exe";
    ///
    ///  // retrieve the version information
    ///  (WperfVersion version, string error) = client.GetVersion();
    ///
    ///  // print version information for each component
    ///  foreach (Version component in version.Version)
    ///  {
    ///      Console.WriteLine($"Component: {component.Component}, Version: {component.ComponentVersion}");
    ///  }
    ///
    ///  // print any errors
    ///  if (!string.IsNullOrEmpty(error))
    ///  {
    ///      Console.WriteLine("Error: " + error);
    ///  }
    /// </code>
    /// </example>
    /// </summary>
    internal class WperfClient
    {
        // Path to the WindowsPerf executable.
        protected string Path { get; set; }
        private bool IsInitialized { get; set; }

        // Helper class for running external processes (WindowsPerf commands in this case).
        private ProcessRunner _wperfProcess;

        /// Constructor for WperfClient, does not initialize the process.

        public WperfClient() { }

        // Delegate used to output messages to the Visual Studio output window.
        protected Action<string> OutputWindowTextWriter { get; set; }

        /// <summary>
        /// Reinitializes the WindowsPerf process.
        /// </summary>
        public void Reinitialize()
        {
            if (!IsInitialized)
            {
                return;
            }
            _wperfProcess = new ProcessRunner(Path);
            IsInitialized = true;
        }

        /// <summary>
        /// Logs the output and errors from a WidnowsPerf command to the Visual Studio output window.
        /// </summary>
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

        /// <summary>
        /// This protected method initializes the `WperfClient` for command execution.
        /// It creates a new `ProcessRunner` instance that is used to run the commands.
        /// </summary>
        protected void InitProcess()
        {
            if (IsInitialized)
            {
                return;
            }

            _wperfProcess = new ProcessRunner(Path);
            IsInitialized = true;
        }

        /// <summary>
        ///This private method runs a Wperf command, specified by the `args` parameter, and waits for the command to complete.
        ///It returns the standard output and standard error from the command.
        /// </summary>
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
        /// This returns whether or not SPE is supported on this machine
        /// it runs the command wperf -version --json and wperf test --json
        /// it then checks for "spe_device.version_name" in the test results and the feature string in the version results
        /// </summary>
        /// <returns>
        ///     bool
        /// </returns>
        public bool CheckIsSPESupported(
            WperfVersion versionSerializedOutput,
            WperfTest testSerializedOutput
        )
        {
            try
            {
                foreach (var component in versionSerializedOutput.Components)
                {
                    if (!component.FeatureString.Contains("+spe"))
                    {
                        return false;
                    }
                }

                TestResult speDeviceConf = testSerializedOutput.TestResults.Find(el =>
                    el.TestName == "spe_device.version_name"
                );
                if (speDeviceConf == null)
                {
                    return false;
                }

                WperfDefaults.SPEFeatureName = speDeviceConf.Result;
                return speDeviceConf.Result.StartsWith("FEAT_SPE");
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the list of predefined WindowsPerf events, metrics and groups of metrics
        /// by running the command `wperf list -v --json`.
        /// </summary>
        /// <returns>A tuple containing the parsed list of events and any errors.</returns>
        public (WperfList output, string stdError) GetEventList()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("list", "-v", "--json");
            WperfList serializedOutput = WperfList.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }

        /// <summary>
        /// Retrieves additional data about the host system by running the command `wperf test --json`.
        /// </summary>
        /// <returns>A tuple containing the parsed test results and any errors.</returns>
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

        /// <summary>
        /// Gets manual information for given event
        /// </summary>
        public (WperfManOutput output, string stdError) GetManOutput(string eventName)
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("man", eventName, "--json");
            WperfManOutput serializedOutput = WperfManOutput.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }

        /// <summary>
        /// Starts the WindowsPerf sampling process asynchronously.
        /// </summary>
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

        /// <summary>
        /// Generates a new output path for WindowsPerf count results, to be stored in a temporary directory.
        /// </summary>
        static void GenerateNewOutputPath()
        {
            string now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
            OutputPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "wperf-stat-output-" + now
            );
        }

        /// <summary>
        /// This public method executes the
        /// `wperf stat<...args> --json` command in the background..
        /// </summary>
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

        /// <summary>
        /// Parses the counting results from a JSON file and returns a list of events and their corresponding number of hits.
        /// </summary>
        public static List<CountingEvent> GetCountingEventsFromJSONFile(string filePath)
        {
            List<CountingEvent> countingEvents = new();

            if (string.IsNullOrEmpty(filePath))
                return countingEvents;

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

        /// <summary>
        /// Processes a single WindowsPerf counting event and adds it to a countingEvents list.
        /// </summary>
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
                    int index = countingEvents.FindIndex(el =>
                        el.CoreNumber == core.CoreNumber && el.Name == rawCountingEvent.EventName
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

        // Event handlers for when sampling, SPE sampling, or counting are finished.
        /// <summary>
        /// This public event is raised when the sampling process is finished. The standard
        /// output is deserialized into a `WperfSampling` object and returned,
        /// along with the standard error.
        /// </summary>
        public EventHandler<(
            WperfSampling serializedOutput,
            string stdError
        )> OnSamplingFinished { get; set; }

        public EventHandler<(
            WperfSPE serializedOutput,
            string stdError
        )> OnSPESamplingFinished { get; set; }

        /// <summary>
        /// This public event is raised when the counting process is finished. The standard
        /// output is deserialized into a `List<CountingEvents>` list and returned,
        /// along with the standard error.
        /// </summary>
        public EventHandler<(
            List<CountingEvent> countingEvents,
            string stdError
        )> OnCountingFinished { get; set; }

        /// <summary>
        /// This public method greacefully stops the sampling command and returns the output.
        /// The standard output is deserialized into a `WperfSampling` object and returned,
        /// along with the standard error.
        /// </summary>
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

        /// <summary>
        /// Stops the SPE sampling process and returns the SPE sampling data and any errors.
        /// </summary>
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

        /// <summary>
        /// This public method greacefully stops the counting command and returns the output.
        /// The standard output is deserialized into a `List<CountingEvents>` list and returned,
        /// along with the standard error.
        /// </summary>
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
            List<CountingEvent> countingEvents = GetCountingEventsFromJSONFile(
                OutputPath + ".json"
            );
            return (countingEvents, stdError);
        }
    }
}
