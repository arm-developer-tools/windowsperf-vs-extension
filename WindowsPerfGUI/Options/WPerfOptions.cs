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


using System.ComponentModel;
using System.Runtime.InteropServices;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.Options
{
    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class WPerfPathOptions : BaseOptionPage<WPerfOptions> { }
    }

    public class WPerfOptions : BaseOptionModel<WPerfOptions>
    {
        [Category("Windows Perf")]
        [DisplayName("WindowsPerf path")]
        [Description("The path for the wperf.exe file")]
        [DefaultValue(true)]
        public string WperfPath { get; set; } = "wperf.exe";
        public bool WperfVersionCheckIgnore { get; set; } = false;
        public bool IsWperfInitialized { get; set; } = false;
        public bool HasSPESupport { get; set; } = false;
        public bool UseDefaultSearchDirectory { get; set; } = true;
        public string WPAPluginSearchDirectory { get; set; }
        public WperfVersion WperfCurrentVersion { get; set; }
        public WperfList WperfList { get; set; }

        public void UpdateWperfVersion(WperfVersion wperfVersion)
        {
            IsWperfInitialized = true;
            WperfCurrentVersion = wperfVersion;
        }

        public void UpdateWperfOptions(WperfVersion wperfVersion, WperfList wperfList)
        {
            UpdateWperfVersion(wperfVersion);
            if (wperfList != null)
            {
                WperfList = wperfList;
            }
            Save();
        }

        public void UpdateWperfOptions(WperfVersion wperfVersion, bool hasSPESupport)
        {
            UpdateWperfVersion(wperfVersion);

            HasSPESupport = hasSPESupport;
            WperfDefaults.HasSPESupport = hasSPESupport;
            Save();
        }

#nullable enable
        public void UpdateWperfOptions(
            WperfVersion wperfVersion,
            WperfList? wperfList,
            bool? hasSPESupport
        )
        {
            UpdateWperfVersion(wperfVersion);
            if (wperfList != null)
            {
                WperfList = wperfList;
            }
            if (hasSPESupport != null)
            {
                HasSPESupport = (bool)hasSPESupport;
            }
            Save();
        }
#nullable disable
    }
}
