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


global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using System.Diagnostics;
global using Task = System.Threading.Tasks.Task;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsPerfGUI.Options;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingExplorer;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideToolWindow(
        typeof(MyToolWindow.Pane),
        Style = VsDockStyle.Tabbed,
        Window = WindowGuids.SolutionExplorer
    )]
    [ProvideToolWindow(
        typeof(SamplingExplorer.Pane),
        Style = VsDockStyle.Tabbed,
        Window = WindowGuids.SolutionExplorer
    )]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.WindowsPerfGUIString)]
    [ProvideOptionPage(
        typeof(WPerfPathPage),
        "WindowsPerf",
        "WindowsPerf Path",
        0,
        0,
        true,
        SupportsProfiles = true
    )]
    [ProvideOptionPage(
        typeof(SamplingManagerOptionsProvider.SamplingManagerOptions),
        "WindowsPerf",
        "Sampling Manager",
        0,
        0,
        true,
        SupportsProfiles = true
    )]
    [ProvideOptionPage(
        typeof(WPAOptionsPage),
        "WindowsPerf",
        "WPA Options",
        0,
        0,
        true,
        SupportsProfiles = true
    )]
    public sealed class WindowsPerfGUIPackage : ToolkitPackage
    {
        public static OutputWindowPane WperfOutputWindow { get; set; }

        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress
        )
        {
            await this.RegisterCommandsAsync();
            WperfOutputWindow = await OutputWindowPane.CreateAsync(
                "WindowsPerf Output",
                lazyCreate: true
            );
            this.RegisterToolWindows();
        }

        protected override async Task OnAfterPackageLoadedAsync(CancellationToken cancellationToken)
        {
            bool shouldIgnoreWperfVersion = WPerfOptions.Instance.WperfVersionCheckIgnore;
            WperfClientFactory wperfClient = new();
            
            try
            {
                (WperfVersion versions, string stdVersionError) = wperfClient.GetVersion();
                (WperfTest wperfTest, _) = wperfClient.GetTest();

                bool speSupport = wperfClient.CheckIsSPESupported(versions, wperfTest);
                WperfDefaults.HasSPESupport = speSupport;
                
                if (!string.IsNullOrEmpty(stdVersionError)) 
                {
                    WPerfOptions.Instance.IsWperfInitialized = false;
                    throw new Exception("Unable to get WindowsPerf version"); 
                }

                (WperfList wperfList, string stdListError) = wperfClient.GetEventList();

                WPerfOptions.Instance.UpdateWperfOptions(versions, wperfList, speSupport);

                if (!shouldIgnoreWperfVersion)
                {
                    string wperfVersion = versions.Components.FirstOrDefault().ComponentVersion;
                    if (wperfVersion != WperfDefaults.WPERF_MIN_VERSION)
                        await VS.MessageBox.ShowWarningAsync(
                            string.Format(
                                ErrorLanguagePack.MinimumVersionMismatch,
                                WperfDefaults.WPERF_MIN_VERSION
                            )
                        );
                }
            }

            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            await base.OnAfterPackageLoadedAsync(cancellationToken);
        }
    }
}
