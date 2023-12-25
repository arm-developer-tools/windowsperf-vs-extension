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
using System.Windows.Controls;


namespace WindowsPerfGUI
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
        }
        private DTE2 _dte;

        public void EnumerateConfigurations()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            foreach (EnvDTE.Project project in _dte.Solution.Projects)
            {
                Configuration config = project.ConfigurationManager.ActiveConfiguration;
                if (config != null)
                {
                    // Process each configuration
                    string configName = config.ConfigurationName;
                    Debug.WriteLine(configName);
                    Debug.WriteLine(config.PlatformName);
                    string msg = "";
                    // Find an outputgroup with at least one file.  
                    OutputGroups groups = config.OutputGroups;
                    foreach (OutputGroup group in groups)
                    {
                        Debug.WriteLine(group.CanonicalName + " " + configName + " " + config.PlatformName);
                        if (group.FileCount < 1) continue;
                        msg += "\nOutputGroup Canonical: " + group.CanonicalName;
                        msg += "\nOutputGroup DisplayName: " + group.DisplayName;
                        msg += "\nOutputGroup Description: " + group.Description;
                        msg += "\nNumber of Associated Files: " + group.FileCount.ToString();
                        msg += "\nAssociated File Names: ";
                        foreach (String str in (Array)group.FileNames)
                        {
                            msg += "\n" + str;
                        }
                        msg += "\nFiles Built in OutputGroup: ";
                        foreach (String fURL in (Array)group.FileURLs)
                        {
                            msg += "\n" + fURL;
                        }
                        Debug.WriteLine(msg);
                    }

                    // Do something with the configuration name
                }
            }
        }
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                EnumerateConfigurations();
            });
        }
    }
}