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

namespace WindowsPerfGUI.ToolWindows.CountingSetting
{
    public static class CountingSettings
    {
        public static string[] ArgsArray;
        public static bool IsSampling = false;
        public static bool AreSettingsFilled = false;
        public static CountingSettingsForm countingSettingsForm;

        public static string[] GenerateCommandLineArgsArray(
            CountingSettingsForm _countingSettingsForm
        )
        {
            if (_countingSettingsForm != null)
            {
                countingSettingsForm = _countingSettingsForm;
            }
            if (countingSettingsForm == null)
                throw new ArgumentNullException(nameof(countingSettingsForm));
            List<string> argsList = new List<string>();
            ValidateSettings();
            AppendElementsToList(argsList, "stat");
            AppendElementsToList(
                argsList,
                "-e",
                string.Join(",", countingSettingsForm.CountingEventList)
            );
            AppendElementsToList(argsList, "--pdb_file", countingSettingsForm.PdbFile);
            AppendElementsToList(argsList, "--");
            AppendElementsToList(argsList, countingSettingsForm.FilePath);
            AppendElementsToList(argsList, countingSettingsForm.ExtraArgs);

            ArgsArray = argsList.ToArray();
            return ArgsArray;
        }

        private static void ValidateSettings()
        {
            // TODO: update ValidateSettings as the UI finalizes
            AreSettingsFilled = true;
        }

        private static List<string> AppendElementsToList(List<string> source, params string[] args)
        {
            bool areAllTruthy = true;
            List<string> tempList = new List<string>();
            foreach (string arg in args)
            {
                tempList.Add(arg);
                if (string.IsNullOrWhiteSpace(arg))
                {
                    areAllTruthy = false;
                }
            }
            if (areAllTruthy)
            {
                source.AddRange(tempList);
            }
            return source;
        }

        public static string GenerateCommandLinePreview()
        {
            string[] argsArray = GenerateCommandLineArgsArray(countingSettingsForm);
            return $"wperf {string.Join(" ", argsArray)}";
        }
    }
}
