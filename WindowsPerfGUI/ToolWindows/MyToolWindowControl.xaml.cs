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


using System.Windows.Controls;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingExplorer;
using WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting;

namespace WindowsPerfGUI
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
        }
        static FormattedSamplingResults formattedSamplingResults = new FormattedSamplingResults();
        private void HighlightEditor(SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE)
            {
                foreach (SamplingSection child in samplingSection.Children)
                {
                    HighlightEditor(child);
                }
            }
            if (!samplingSection.IsFileExists)
            {
                return;
            }
            HighlighterDict.AddFileToHighlight(samplingSection);
        }

        private void Test()
        {
            formattedSamplingResults.FormatSamplingResults(WperfSampling.FromJson("{\r\n\"sampling\": {\r\n\"pe_file\": \"C:\\\\Users\\\\nader\\\\OneDrive\\\\Desktop\\\\sampling\\\\windowsPerfSample\\\\WindowsPerfSample.exe\",\r\n\"pdb_file\": \"C:\\\\Users\\\\nader\\\\OneDrive\\\\Desktop\\\\sampling\\\\windowsPerfSample\\\\WindowsPerfSample.pdb\",\r\n\"sample_display_row\": 50,\r\n\"samples_generated\": 60,\r\n\"samples_dropped\": 0,\r\n\"base_address\": 140697426990076,\r\n\"runtime_delta\": 140692058210304,\r\n\"modules\":[{\"name\":\"KERNEL32.DLL\",\"address\":140726913466368,\"path\":\"C:\\\\Windows\\\\System32\\\\KERNEL32.DLL\"},{\"name\":\"KERNELBASE.dll\",\"address\":140726849830912,\"path\":\"C:\\\\Windows\\\\System32\\\\KERNELBASE.dll\"},{\"name\":\"VCRUNTIME140D.dll\",\"address\":140721964122112,\"path\":\"C:\\\\Windows\\\\SYSTEM32\\\\VCRUNTIME140D.dll\"},{\"name\":\"WindowsPerfSample.exe\",\"address\":140697426919424,\"path\":\"C:\\\\Users\\\\nader\\\\OneDrive\\\\Desktop\\\\sampling\\\\windowsPerfSample\\\\WindowsPerfSample.exe\"},{\"name\":\"ntdll.dll\",\"address\":140726927622144,\"path\":\"C:\\\\Windows\\\\SYSTEM32\\\\ntdll.dll\"},{\"name\":\"ucrtbased.dll\",\"address\":140721524113408,\"path\":\"C:\\\\Windows\\\\SYSTEM32\\\\ucrtbased.dll\"}],\r\n\"modules_info\": [{\"sections\":[{\"section\":\".textbss\",\"offset\":4096,\"virtual_size\":65536},{\"section\":\".text\",\"offset\":69632,\"virtual_size\":30657},{\"section\":\".rdata\",\"offset\":102400,\"virtual_size\":6791},{\"section\":\".data\",\"offset\":110592,\"virtual_size\":528},{\"section\":\".pdata\",\"offset\":114688,\"virtual_size\":5360},{\"section\":\".idata\",\"offset\":122880,\"virtual_size\":2559},{\"section\":\".msvcjmc\",\"offset\":126976,\"virtual_size\":275},{\"section\":\".00cfg\",\"offset\":131072,\"virtual_size\":337},{\"section\":\".rsrc\",\"offset\":135168,\"virtual_size\":1084},{\"section\":\".reloc\",\"offset\":139264,\"virtual_size\":431}],\"module\":\"WindowsPerfSample.exe\",\"pdb_file\":\"C:\\\\Users\\\\nader\\\\OneDrive\\\\Desktop\\\\sampling\\\\windowsPerfSample\\\\WindowsPerfSample.pdb\",\"pe_name\":\"C:\\\\Users\\\\nader\\\\OneDrive\\\\Desktop\\\\sampling\\\\windowsPerfSample\\\\WindowsPerfSample.exe\"}],\r\n\"events\": [{\"type\":\"vfp_spec\",\"samples\":[{\"overhead\":65,\"count\":39,\"symbol\":\"simd_hot\"},{\"overhead\":30,\"count\":18,\"symbol\":\"df_hot\"},{\"overhead\":3.33333,\"count\":2,\"symbol\":\"__CheckForDebuggerJustMyCode\"},{\"overhead\":1.66667,\"count\":1,\"symbol\":\"unknown\"}],\"interval\":200000,\"printed_sample_num\":4,\r\n\"pcs\":[{\"address\":140697426997464,\"count\":9},{\"address\":140697426997508,\"count\":7},{\"address\":140697426997500,\"count\":5},{\"address\":140697426997480,\"count\":4},{\"address\":140697426997468,\"count\":4},{\"address\":140697426997488,\"count\":2},{\"address\":140697426997460,\"count\":2},{\"address\":140697426997496,\"count\":2},{\"address\":140697426997492,\"count\":1},{\"address\":140697426997516,\"count\":1},{\"address\":140697426997188,\"count\":14},{\"address\":140697426997176,\"count\":1},{\"address\":140697426997216,\"count\":1},{\"address\":140697426997196,\"count\":1},{\"address\":140697426997192,\"count\":1},{\"address\":140697426998368,\"count\":1},{\"address\":140697426998452,\"count\":1},{\"address\":140726745318480,\"count\":1}],\r\n\"annotate\": [{\"function_name\": \"simd_hot\",\r\n\"source_code\":[{\"line_number\":53,\"hits\":37,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\"},{\"line_number\":52,\"hits\":2,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\"}]},\r\n{\"function_name\": \"df_hot\",\r\n\"source_code\":[{\"line_number\":15732480,\"hits\":18,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\"}]},\r\n{\"function_name\": \"__CheckForDebuggerJustMyCode\",\r\n\"source_code\":[{\"line_number\":23,\"hits\":1,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\"},{\"line_number\":27,\"hits\":1,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\"}]}]}\r\n]}\r\n}"));
            formattedSamplingResults.RootSampledEvent.ForEach(e =>
            {
                HighlightEditor(e);
            });
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Test();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            HighlighterDict.Clear();
        }
    }
}