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
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils.CommandBuilder;

namespace WindowsPerfGUI.ToolWindows.SamplingSetting
{
    public class SamplingSettings : CommandSettings
    {
        public static SamplingSettingsForm samplingSettingsFrom;

        public static string[] GenerateCommandLineArgsArray(
            SamplingSettingsForm _samplingSettingsFrom
        )
        {
            if (_samplingSettingsFrom != null)
            {
                samplingSettingsFrom = _samplingSettingsFrom;
            }
            if (samplingSettingsFrom == null)
                throw new ArgumentNullException(nameof(samplingSettingsFrom));
            List<string> argsList = new List<string>();
            ValidateSettings();
            AppendElementsToList(argsList, "record");
            if (samplingSettingsFrom.IsSPEEnabled)
            {
                string SPEFilters = string.Join("=1,", samplingSettingsFrom.SamplingEventList);
                if (samplingSettingsFrom.SamplingEventList.Count > 0) SPEFilters = $"{SPEFilters}=1";
                AppendElementsToList(
                argsList,
                "-e",
                $"{WperfList.SPE_EVENT_BASE_NAME}{WperfList.SPE_EVENT_SEPARATOR}{SPEFilters}{WperfList.SPE_EVENT_SEPARATOR}"
            );
            }
            else
            {
                AppendElementsToList(
                argsList,
                "-e",
                string.Join(",", samplingSettingsFrom.SamplingEventList)
            );
            }

            AppendElementsToList(
                argsList,
                "-c",
                samplingSettingsFrom.CPUCore?.coreNumber.ToString()
            );

            AppendElementsToList(argsList, "--annotate");
            if (samplingSettingsFrom.ShouldDisassemble) AppendElementsToList(argsList, "--disassemble");

            AppendElementsToList(argsList, "--timeout", samplingSettingsFrom.Timeout);
            AppendElementsToList(argsList, "-v", "--json");

            if (samplingSettingsFrom.ForceLock) AppendElementsToList(argsList, "--force-lock");
            if (samplingSettingsFrom.KernelMode) AppendElementsToList(argsList, "-k");

            AppendElementsToList(argsList, "--pdb_file", samplingSettingsFrom.PdbFile);
            AppendElementsToList(argsList, "--");
            AppendElementsToList(argsList, samplingSettingsFrom.FilePath);
            AppendElementsToList(argsList, samplingSettingsFrom.ExtraArgs);
            ArgsArray = argsList.ToArray();
            return ArgsArray;
        }

        private static void ValidateSettings()
        {
            AreSettingsFilled = !(
                (samplingSettingsFrom.SamplingEventList.Count < 1 && !samplingSettingsFrom.IsSPEEnabled)
                || string.IsNullOrEmpty(samplingSettingsFrom.FilePath)
                || string.IsNullOrEmpty(samplingSettingsFrom.CPUCore?.coreNumber.ToString())
            );
        }

        public static string GenerateCommandLinePreview()
        {
            string[] argsArray = GenerateCommandLineArgsArray(samplingSettingsFrom);
            return $"wperf {string.Join(" ", argsArray)}";
        }
    }
}
