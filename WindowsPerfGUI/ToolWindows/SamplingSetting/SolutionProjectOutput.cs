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

using EnvDTE;
using EnvDTE80;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsPerfGUI.ToolWindows.SamplingSetting
{
    public static class SolutionProjectOutput
    {
        private static DTE2 _dte;
        public static string SelectedConfigName = "";
        public static string SelectedPlatformName = "";
        public static string SelectedConfigLabel = "";
        private static (string, string) EnumerateConfigurations()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string mainOutput = "";
            string pdbOutput = "";

            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            Array projects = (Array)_dte.ActiveSolutionProjects;
            EnvDTE.Project project = (EnvDTE.Project)projects.GetValue(0);
            Configuration config = project.ConfigurationManager.ActiveConfiguration;
            SelectedConfigName = config.ConfigurationName;
            SelectedPlatformName = config.PlatformName;
            SelectedConfigLabel = $"({SelectedConfigName} | {SelectedPlatformName})";
            if (config != null)
            {
                // Process each configuration
                // Find an outputgroup with at least one file.
                OutputGroups groups = config.OutputGroups;
                foreach (OutputGroup group in groups)
                {
                    if (group.FileCount < 1) continue;
                    if (group.CanonicalName != "Built" && group.CanonicalName != "Symbols") continue;
                    string mainFile = "";
                    foreach (string fURL in (Array)group.FileURLs)
                    {
                        mainFile = Regex.Replace(fURL, "file:///", "");
                        break;
                    }
                    if (group.CanonicalName == "Built")
                    {
                        mainOutput = $"\"{mainFile}\"";
                    }
                    if (group.CanonicalName == "Symbols")
                    {
                        pdbOutput = $"\"{mainFile}\"";
                    }
                }
            }
            return (mainOutput, pdbOutput);
        }
        public static async Task<string> GetProjectOutputAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            (string mainOutput, string pdbOutput) = EnumerateConfigurations();
            if (string.IsNullOrEmpty(pdbOutput) || getFilePathWithoutExtension(mainOutput) == getFilePathWithoutExtension(pdbOutput))
            {
                return mainOutput;
            }
            else
            {
                return $"--pe_file {mainOutput} --pdb_file {pdbOutput}";
            }
        }

        private static string getFilePathWithoutExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return filePath;
            return filePath.Remove(filePath.Length - 4);
        }
    }
}
