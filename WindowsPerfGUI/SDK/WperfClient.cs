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

using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils.SDK;

namespace WindowsPerfGUI.SDK
{
    internal class WperfClient
    {
        public string Path { get; set; }
        private bool IsInitilized { get; set; }

        private ProcessRunner WperfPorcess;
        public WperfClient() { }

        public Action<string> OutputWindowTextWriter { get; set; }

        protected void InitProcess()
        {
            if (IsInitilized)
            {
                return;
            }

            WperfPorcess = new ProcessRunner(Path);
            IsInitilized = true;
        }

        private (string stdOutput, string stdError) ExecuteAwaitedCommand(params string[] args)
        {
            InitProcess();

            (string stdOutput, string stdError) = WperfPorcess.StartAwaitedProcess(args);

            if (OutputWindowTextWriter == null) return (stdOutput, stdError);

            OutputWindowTextWriter("Executing wperf command:");
            OutputWindowTextWriter($"{Path} {string.Join(" ", args)}");
            OutputWindowTextWriter("wperf command output:");
            OutputWindowTextWriter("=============================================================");
            OutputWindowTextWriter(stdOutput);
            OutputWindowTextWriter("=============================================================");

            if (stdError == "") return (stdOutput, stdError);

            OutputWindowTextWriter("wperf command error:");
            OutputWindowTextWriter("=============================================================");
            OutputWindowTextWriter(stdError);
            OutputWindowTextWriter("=============================================================");

            return (stdOutput, stdError);
        }
        /// <summary>
        /// This returns the WPerf and Wperf driver installed version
        /// it runs the command wperf -version -json
        /// </summary>
        /// <returns>
        ///     Tuple<![CDATA[<WperfVersion serializedOutput, string stdError>]]>
        /// </returns>
        public (WperfVersion output, string stdError) GetVersion()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("-version", "-json");
            WperfVersion serializedOutput = WperfVersion.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }
        /// <summary>
        /// This returns the list of Wperf's predefined events and metrics
        /// it runs the command wperf list -v -json
        /// </summary> 
        /// <returns></returns>
        public (WperfList output, string stdError) GetEventList()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("list", "-v", "-json");
            WperfList serializedOutput = WperfList.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }

        /// <summary>
        /// This returns the additional data about the host
        /// it runs the command wperf test -json
        /// </summary>
        /// <returns></returns>
        public (WperfTest output, string stdError) GetTest()
        {
            (string stdOutput, string stdError) = ExecuteAwaitedCommand("test", "-json");

            WperfTest serializedOutput = WperfTest.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }

        public async Task StartSamplingAsync()
        {
            string[] samplingArgs = Utils.SamplingSettings.GenerateCommandLineArgsArray();
            Utils.SamplingSettings.IsSampling = true;
            await WperfPorcess.StartBackgroundProcessAsync(samplingArgs);
            (WperfSampling serializedOutput, string stdError) = StopSampling();
            OnSamplingFinished?.Invoke(this, (serializedOutput, stdError));
            //WperfSampling serializedOutput = WperfSampling.FromJson(stdOutput);
            //return (serializedOutput, stdError);
        }

        public EventHandler<(WperfSampling serializedOutput, string stdError)> OnSamplingFinished { get; set; }
        public (WperfSampling serializedOutput, string stdError) StopSampling()
        {
            WperfPorcess.StopProcess();
            Utils.SamplingSettings.IsSampling = false;
            string stdOutput = string.Join("", WperfPorcess.StdOutput.Output);
            string stdError = string.Join("", WperfPorcess.StdError.Output);
            WperfSampling serializedOutput = WperfSampling.FromJson(stdOutput);
            return (serializedOutput, stdError);
        }
    }
}
