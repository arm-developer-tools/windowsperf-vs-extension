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

using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WindowsPerfGUI.Components.TreeListView;
using WindowsPerfGUI.Options;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting;
using WindowsPerfGUI.ToolWindows.SamplingSetting;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer
{
    public partial class SamplingExplorerControl : UserControl
    {
        /// <summary>
        /// Settings for the the number of available colors in the highlighter.
        /// </summary>
        private int _colorResolution;

        readonly WperfClientFactory wperfClient = new();
        static FormattedSamplingResults formattedSamplingResults = new FormattedSamplingResults();
#if DEBUG
        string rootSample =
            "{\"sampling\":{\"pe_file\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\ARM64EC\\\\Debug\\\\WindowsPerfSample.exe\",\"pdb_file\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\ARM64EC\\\\Debug\\\\WindowsPerfSample.pdb\",\"sample_display_row\":50,\"samples_generated\":246,\"samples_dropped\":0,\"base_address\":140694649523592,\"runtime_delta\":140689280761224,\"modules\":[{\"name\":\"KERNEL32.DLL\",\"address\":140709666422784,\"path\":\"C:\\\\WINDOWS\\\\System32\\\\KERNEL32.DLL\"},{\"name\":\"KERNELBASE.dll\",\"address\":140709651611648,\"path\":\"C:\\\\WINDOWS\\\\System32\\\\KERNELBASE.dll\"},{\"name\":\"VCRUNTIME140D.dll\",\"address\":140708881629184,\"path\":\"C:\\\\WINDOWS\\\\SYSTEM32\\\\VCRUNTIME140D.dll\"},{\"name\":\"WindowsPerfSample.exe\",\"address\":140694649503744,\"path\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\ARM64EC\\\\Debug\\\\WindowsPerfSample.exe\"},{\"name\":\"apphelp.dll\",\"address\":140709640601600,\"path\":\"C:\\\\WINDOWS\\\\SYSTEM32\\\\apphelp.dll\"},{\"name\":\"ntdll.dll\",\"address\":140709724291072,\"path\":\"C:\\\\WINDOWS\\\\SYSTEM32\\\\ntdll.dll\"},{\"name\":\"ucrtbased.dll\",\"address\":140706884026368,\"path\":\"C:\\\\WINDOWS\\\\SYSTEM32\\\\ucrtbased.dll\"},{\"name\":\"xtajit64.dll\",\"address\":140709704105984,\"path\":\"C:\\\\WINDOWS\\\\System32\\\\xtajit64.dll\"}],\"modules_info\":[{\"sections\":[{\"section\":\".text\",\"offset\":4096,\"virtual_size\":45686},{\"section\":\".hexpthk\",\"offset\":53248,\"virtual_size\":16},{\"section\":\".rdata\",\"offset\":57344,\"virtual_size\":16984},{\"section\":\".data\",\"offset\":77824,\"virtual_size\":1804},{\"section\":\".pdata\",\"offset\":81920,\"virtual_size\":3112},{\"section\":\".msvcjmc\",\"offset\":86016,\"virtual_size\":16},{\"section\":\".a64xrm\",\"offset\":90112,\"virtual_size\":16},{\"section\":\".rsrc\",\"offset\":94208,\"virtual_size\":480},{\"section\":\".reloc\",\"offset\":98304,\"virtual_size\":396}],\"module\":\"WindowsPerfSample.exe\",\"pdb_file\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\ARM64EC\\\\Debug\\\\WindowsPerfSample.pdb\",\"pe_name\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\ARM64EC\\\\Debug\\\\WindowsPerfSample.exe\"}],\"events\":[{\"type\":\"ld_spec\",\"samples\":[{\"overhead\":97.8541,\"count\":228,\"symbol\":\"simd_hot:WindowsPerfSample.exe\"},{\"overhead\":2.14592,\"count\":5,\"symbol\":\"unknown\"}],\"interval\":67108864,\"printed_sample_num\":2,\"pcs\":[{\"address\":140694649522528,\"count\":82},{\"address\":140694649522532,\"count\":56},{\"address\":140694649522468,\"count\":10},{\"address\":140694649522536,\"count\":9},{\"address\":140694649522560,\"count\":8},{\"address\":140694649522564,\"count\":8},{\"address\":140694649522524,\"count\":7},{\"address\":140694649522492,\"count\":6},{\"address\":140694649522456,\"count\":4},{\"address\":140694649522552,\"count\":4},{\"address\":140708784023816,\"count\":1},{\"address\":140709302620384,\"count\":1},{\"address\":140709724618176,\"count\":1},{\"address\":140708783249492,\"count\":1},{\"address\":140709717920900,\"count\":1}],\"annotate\":[{\"function_name\":\"simd_hot:WindowsPerfSample.exe\",\"source_code\":[{\"line_number\":53,\"hits\":203,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"4960\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"492c\",\"instruction\":\"3b 40 b9 08              \\tpushq\\t$0x8b9403b              # imm = 0x8B9403B\"},{\"address\":\"4931\",\"instruction\":\"40                       \\tjge\\t0x140004973 <.text+0x3973>\"},{\"address\":\"4933\",\"instruction\":\"xchgl\\t%ebx, %eax\"},{\"address\":\"4934\",\"instruction\":\"00                       \\tmovl\\t%eax, (%rax)\"},{\"address\":\"4936\",\"instruction\":\"d2 09                    \\tadcb\\t$0x9, %dl\"},{\"address\":\"4939\",\"instruction\":\"09                       \\tjge\\t0x140004944 <.text+0x3944>\"},{\"address\":\"493b\",\"instruction\":\"wait\"},{\"address\":\"493c\",\"instruction\":\"0f 40 f9 08              \\tpushq\\t$0x8f9400f              # imm = 0x8F9400F\"},{\"address\":\"4941\",\"instruction\":\"09                       \\taddl\\t%ecx, (%rcx)\"},{\"address\":\"4943\",\"instruction\":\"69 3b                    \\tmovl\\t0x3b(%rcx), %ebp\"},{\"address\":\"4946\",\"instruction\":\"b9 29 7d 40 93           \\tmovl\\t$0x93407d29, %ecx       # imm = 0x93407D29\"},{\"address\":\"494c\",\"instruction\":\"00                       \\tmovb\\t(%rax), %al\"},{\"address\":\"494e\",\"instruction\":\"d2 2a                    \\tadcb\\t$0x2a, %dl\"},{\"address\":\"4951\",\"instruction\":\"0a                       \\tjge\\t0x14000495d <.text+0x395d>\"},{\"address\":\"4953\",\"instruction\":\"wait\"},{\"address\":\"4954\",\"instruction\":\"17 40 f9 29 01           \\timull\\t$0x129f940, (%rdi), %edx # imm = 0x129F940\"},{\"address\":\"495a\",\"instruction\":\"8b 08 01 40 b9           \\torb\\t-0x46bffef8(%rbx), %cl\"},{\"address\":\"4960\",\"instruction\":\"01                       \\tsubl\\t%eax, (%rcx)\"},{\"address\":\"4962\",\"instruction\":\"b9 08 01 09 0b           \\tmovl\\t$0xb090108, %ecx        # imm = 0xB090108\"},{\"address\":\"4968\",\"instruction\":\"3b 40 b9 29 7d           \\timull\\t$0x7d29b940, (%rbx), %edi # imm = 0x7D29B940\"},{\"address\":\"496e\",\"instruction\":\"93                       \\txchgl\\t%ebx, %eax\"},{\"address\":\"4970\",\"instruction\":\"00                       \\tmovb\\t(%rax), %al\"},{\"address\":\"4972\",\"instruction\":\"d2 2a                    \\tadcb\\t$0x2a, %dl\"},{\"address\":\"4975\",\"instruction\":\"0a                       \\tjge\\t0x140004981 <.text+0x3981>\"},{\"address\":\"4977\",\"instruction\":\"wait\"},{\"address\":\"4978\",\"instruction\":\"07 40 f9 29 01           \\timull\\t$0x129f940, (%rdi), %eax # imm = 0x129F940\"},{\"address\":\"497e\",\"instruction\":\"8b 28 01 00 b9           \\torb\\t-0x46fffed8(%rbx), %cl\"},{\"address\":\"4984\",\"instruction\":\"ff                       \\tjrcxz\\t0x140004985 <.text+0x3985>\"},{\"address\":\"4986\",\"instruction\":\"17                       \\tcallq\\t*(%rdi)\"},{\"address\":\"4988\",\"instruction\":\"03 00 91 fb              \\tmovl\\t$0xfb910003, %edi       # imm = 0xFB910003\"},{\"address\":\"498d\",\"instruction\":\"<unknown>\"},{\"address\":\"498e\",\"instruction\":\"f9                       \\tstc\"},{\"address\":\"4990\",\"instruction\":\"13                       \\tnotb\\t(%rbx)\"},{\"address\":\"4992\",\"instruction\":\"f9                       \\tstc\"},{\"address\":\"4994\",\"instruction\":\"hlt\"},{\"address\":\"4995\",\"instruction\":\"pushq\\t%rdi\"},{\"address\":\"4996\",\"instruction\":\"a9 fd 7b c3 a8           \\ttestl\\t$0xa8c37bfd, %eax       # imm = 0xA8C37BFD\"},{\"address\":\"499c\",\"instruction\":\"03 5f                    \\trolb\\t$0x5f, (%rbx)\"},{\"address\":\"499f\",\"instruction\":\"<unknown>\"}]}},{\"line_number\":52,\"hits\":25,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"4924\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"4904\",\"instruction\":\"00                       \\torb\\t%al, (%rax)\"},{\"address\":\"4906\",\"instruction\":\"52 68 3b                 \\tadcb\\t$0x3b, 0x68(%rdx)\"},{\"address\":\"490a\",\"instruction\":\"b9 04 00 00 14           \\taddb\\t%bh, 0x14000004(%rcx)\"},{\"address\":\"4910\",\"instruction\":\"3b 40 b9 08              \\tpushq\\t$0x8b9403b              # imm = 0x8B9403B\"},{\"address\":\"4915\",\"instruction\":\"00 11 68 3b              \\taddl\\t$0x3b681100, %eax       # imm = 0x3B681100\"},{\"address\":\"491a\",\"instruction\":\"b9 09 e2 84 52           \\taddb\\t%bh, 0x5284e209(%rcx)\"},{\"address\":\"4920\",\"instruction\":\"3b 40 b9 1f              \\tpushq\\t$0x1fb9403b             # imm = 0x1FB9403B\"},{\"address\":\"4925\",\"instruction\":\"09                       \\taddl\\t%ecx, (%rcx)\"},{\"address\":\"4927\",\"instruction\":\"0a 03                    \\timull\\t$0x3, (%rdx), %ecx\"},{\"address\":\"492a\",\"instruction\":\"54 68 3b                 \\taddb\\t%dl, 0x3b(%rax,%rbp,2)\"}]}}]}]},{\"type\":\"vfp_spec\",\"samples\":[{\"overhead\":84.6154,\"count\":11,\"symbol\":\"df_hot:WindowsPerfSample.exe\"},{\"overhead\":7.69231,\"count\":1,\"symbol\":\"simd_hot:WindowsPerfSample.exe\"},{\"overhead\":7.69231,\"count\":1,\"symbol\":\"unknown\"}],\"interval\":100000,\"printed_sample_num\":3,\"pcs\":[{\"address\":140694649522044,\"count\":4},{\"address\":140694649522092,\"count\":3},{\"address\":140694649522124,\"count\":2},{\"address\":140694649522000,\"count\":1},{\"address\":140694649522056,\"count\":1},{\"address\":140694649522460,\"count\":1},{\"address\":140708295216952,\"count\":1}],\"annotate\":[{\"function_name\":\"df_hot:WindowsPerfSample.exe\",\"source_code\":[{\"line_number\":37,\"hits\":4,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"477c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"4768\",\"instruction\":\"1f                       \\t<unknown>\"},{\"address\":\"476a\",\"instruction\":\"fd                       \\tstd\"},{\"address\":\"476c\",\"instruction\":\"1f                       \\t<unknown>\"},{\"address\":\"476e\",\"instruction\":\"fd                       \\tstd\"},{\"address\":\"4770\",\"instruction\":\"08                       \\toutb\\t%al, $0x8\"},{\"address\":\"4772\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"4774\",\"instruction\":\"00 00 f0 07              \\tpushq\\t$0x7f00000              # imm = 0x7F00000\"},{\"address\":\"4779\",\"instruction\":\"40 fd                    \\taddl\\t%eax, -0x3(%rax)\"},{\"address\":\"477c\",\"instruction\":\"28                       \\toutl\\t%eax, $0x28\"},{\"address\":\"477e\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"4780\",\"instruction\":\"00 00 f0 07              \\tpushq\\t$0x7f00000              # imm = 0x7F00000\"},{\"address\":\"4785\",\"instruction\":\"00                       \\taddl\\t%eax, (%rax)\"},{\"address\":\"4787\",\"instruction\":\"std\"}]}},{\"line_number\":38,\"hits\":4,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"47ac\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"4788\",\"instruction\":\"17                       \\t<unknown>\"},{\"address\":\"478a\",\"instruction\":\"fd                       \\tstd\"},{\"address\":\"478c\",\"instruction\":\"07                       \\toutb\\t%al, $0x7\"},{\"address\":\"478e\",\"instruction\":\"5c e7 08                 \\taddb\\t%bl, 0x8(%rdi,%riz,8)\"},{\"address\":\"4792\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"4794\",\"instruction\":\"1f                       \\t<unknown>\"},{\"address\":\"4796\",\"instruction\":\"fd                       \\tstd\"},{\"address\":\"4798\",\"instruction\":\"07 00                    \\t<unknown>\"},{\"address\":\"479b\",\"instruction\":\"popq\\t%rsp\"},{\"address\":\"479c\",\"instruction\":\"18                       \\t<unknown>\"},{\"address\":\"479e\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"47a0\",\"instruction\":\"28                       \\toutb\\t%al, $0x28\"},{\"address\":\"47a2\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"47a4\",\"instruction\":\"00 00 f0 07              \\tpushq\\t$0x7f00000              # imm = 0x7F00000\"},{\"address\":\"47a9\",\"instruction\":\"40 fd                    \\taddl\\t%eax, -0x3(%rax)\"},{\"address\":\"47ac\",\"instruction\":\"08                       \\toutl\\t%eax, $0x8\"},{\"address\":\"47ae\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"47b0\",\"instruction\":\"00 00 f0 07              \\tpushq\\t$0x7f00000              # imm = 0x7F00000\"},{\"address\":\"47b5\",\"instruction\":\"00                       \\taddl\\t%eax, (%rax)\"},{\"address\":\"47b7\",\"instruction\":\"std\"}]}},{\"line_number\":40,\"hits\":2,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"47cc\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"47c4\",\"instruction\":\"00 00 f0 00              \\tpushq\\t$0xf00000               # imm = 0xF00000\"},{\"address\":\"47c9\",\"instruction\":\"40 fd                    \\taddl\\t%eax, -0x3(%rax)\"},{\"address\":\"47cc\",\"instruction\":\"<unknown>\"},{\"address\":\"47cd\",\"instruction\":\"00                       \\taddb\\t%al, (%rax)\"},{\"address\":\"47cf\",\"instruction\":\"08                       \\tadcb\\t$0x8, %al\"}]}},{\"line_number\":36,\"hits\":1,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"4750\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"4750\",\"instruction\":\"00 00 f0 07              \\tpushq\\t$0x7f00000              # imm = 0x7F00000\"},{\"address\":\"4755\",\"instruction\":\"40 fd                    \\taddl\\t%eax, -0x3(%rax)\"},{\"address\":\"4758\",\"instruction\":\"17                       \\t<unknown>\"},{\"address\":\"475a\",\"instruction\":\"fd                       \\tstd\"},{\"address\":\"475c\",\"instruction\":\"18                       \\toutl\\t%eax, $0x18\"},{\"address\":\"475e\",\"instruction\":\"1e                       \\t<unknown>\"},{\"address\":\"4760\",\"instruction\":\"00 00 f0 07              \\tpushq\\t$0x7f00000              # imm = 0x7F00000\"},{\"address\":\"4765\",\"instruction\":\"00                       \\taddl\\t%eax, (%rax)\"},{\"address\":\"4767\",\"instruction\":\"std\"}]}}]},{\"function_name\":\"simd_hot:WindowsPerfSample.exe\",\"source_code\":[{\"line_number\":52,\"hits\":1,\"filename\":\"C:\\\\Users\\\\Alaa\\\\devProjects\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"491c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"c\",\"instruction\":\"coff-x86-64\"},{\"address\":\"4904\",\"instruction\":\"00                       \\torb\\t%al, (%rax)\"},{\"address\":\"4906\",\"instruction\":\"52 68 3b                 \\tadcb\\t$0x3b, 0x68(%rdx)\"},{\"address\":\"490a\",\"instruction\":\"b9 04 00 00 14           \\taddb\\t%bh, 0x14000004(%rcx)\"},{\"address\":\"4910\",\"instruction\":\"3b 40 b9 08              \\tpushq\\t$0x8b9403b              # imm = 0x8B9403B\"},{\"address\":\"4915\",\"instruction\":\"00 11 68 3b              \\taddl\\t$0x3b681100, %eax       # imm = 0x3B681100\"},{\"address\":\"491a\",\"instruction\":\"b9 09 e2 84 52           \\taddb\\t%bh, 0x5284e209(%rcx)\"},{\"address\":\"4920\",\"instruction\":\"3b 40 b9 1f              \\tpushq\\t$0x1fb9403b             # imm = 0x1FB9403B\"},{\"address\":\"4925\",\"instruction\":\"09                       \\taddl\\t%ecx, (%rcx)\"},{\"address\":\"4927\",\"instruction\":\"0a 03                    \\timull\\t$0x3, (%rdx), %ecx\"},{\"address\":\"492a\",\"instruction\":\"54 68 3b                 \\taddb\\t%dl, 0x3b(%rax,%rbp,2)\"}]}}]}]}]}}";
#endif

        private void HandleSamplingFinished(
            object sender,
            (WperfSampling serializedOutput, string stdError) e
        )
        {
            SamplingSettingsMonikerButton.IsEnabled = true;
            StartSamplingMonikerButton.IsEnabled = true;
            StopSamplingMonikerButton.IsEnabled = false;
            if (!string.IsNullOrEmpty(e.stdError))
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.ErrorWindowsPerfCLI, e.stdError);
                return;
            }

            if (e.serializedOutput == null)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.NoWindowsPerfCliData, e.stdError);
                return;
            }

            formattedSamplingResults.FormatSamplingResults(
                e.serializedOutput,
                $"{SamplingExplorerLanguagePack.ExecutedAt} {DateTime.Now.ToShortTimeString()}"
            );
            if (_tree.Model == null)
                _tree.Model = formattedSamplingResults;
            _tree.UpdateTreeList();
        }

        public SamplingExplorerControl()
        {
            InitializeComponent();
#if DEBUG
            formattedSamplingResults.FormatSamplingResults(WperfSampling.FromJson(rootSample));
#endif
            _tree.Model = formattedSamplingResults;
            wperfClient.OnSamplingFinished += HandleSamplingFinished;
            StopSamplingMonikerButton.IsEnabled = false;
            this._colorResolution = SamplingManager.Instance.HighlighterColorResolution;
        }

        private void SettingsMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WPerfOptions.Instance.IsWperfInitialized)
            {
                VS.MessageBox.ShowError(
                    ErrorLanguagePack.NotInititiatedWperfErrorLine1,
                    ErrorLanguagePack.NotInititiatedWperfErrorLine2
                );
                return;
            }

            SamplingSettingDialog samplingSettingsDialog = new();
            samplingSettingsDialog.Title = SamplingSettingsLanguagePack.WindowTitle;
            samplingSettingsDialog.ShowDialog();
        }

        private void StartSamplingMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WPerfOptions.Instance.IsWperfInitialized)
                return;
            if (!SamplingSettings.AreSettingsFilled)
            {
                VS.MessageBox.ShowError(
                    ErrorLanguagePack.UncompleteSamplingSettingsLine1,
                    ErrorLanguagePack.UncompleteSamplingSettingsLine2
                );
                return;
            }

            if (SamplingSettings.IsSampling)
            {
                VS.MessageBox.ShowError(
                    ErrorLanguagePack.RunningSamplingOverlapLine1,
                    ErrorLanguagePack.RunningSamplingOverlapLine2
                );
                return;
            }

            wperfClient.StartSamplingAsync().FireAndForget();
            SamplingSettingsMonikerButton.IsEnabled = false;
            StartSamplingMonikerButton.IsEnabled = false;
            StopSamplingMonikerButton.IsEnabled = true;
        }

        private void StopSamplingMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            wperfClient.StopSampling();
            SamplingSettingsMonikerButton.IsEnabled = true;
            StartSamplingMonikerButton.IsEnabled = true;
            StopSamplingMonikerButton.IsEnabled = false;
        }

        private void HighlightEditor(SamplingSection samplingSection)
        {
            if (
                samplingSection.SectionType
                != SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE
            )
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

        private void _tree_SelectionChanged(
            object sender,
            System.Windows.Controls.SelectionChangedEventArgs e
        )
        {
            TreeList treeList = sender as TreeList;
            SummaryStack.Children.Clear();
            if (treeList == null || treeList.SelectedItem == null)
                return;
            SamplingSection selecteditem =
                (treeList.SelectedItem as TreeNode).Tag as SamplingSection;
            switch (selecteditem.SectionType)
            {
                case SamplingSection.SamplingSectionType.ROOT:
                    CreateRootSummary(SummaryStack.Children, selecteditem);
                    HighlighterDict.Clear();
                    HighlightEditor(selecteditem);
                    break;
                case SamplingSection.SamplingSectionType.SAMPLE_EVENT:
                    CreateEventSummary(SummaryStack.Children, selecteditem);
                    HighlighterDict.Clear();
                    HighlightEditor(selecteditem);
                    break;
                case SamplingSection.SamplingSectionType.SAMPLE_FUNCTION:
                    CreateFunctionSummary(SummaryStack.Children, selecteditem);
                    break;

                case SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE:
                    CreateSourceCodeSummary(SummaryStack.Children, selecteditem);
                    break;
                default:
                    break;
            }
        }

        private void CreateSourceCodeSummary(
            UIElementCollection children,
            SamplingSection samplingSection
        )
        {
            if (
                samplingSection.SectionType
                != SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE
            )
            {
                return;
            }

            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.FilePath}: ")),
                        new Run(samplingSection.Name)
                    },
                    _localFontSizes.lg,
                    _localFontWeights.bold
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.LineNumber}: ")),
                        new Run(samplingSection.LineNumber.ToString())
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Hits}: ")),
                        new Run($"{samplingSection.Hits}")
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Overhead}: ")),
                        new Run($"{samplingSection.OverheadPercentage}")
                    },
                    _localFontSizes.md
                )
            );
            StackPanel asseblyPanel = GenerateAssemblyStackPanel(samplingSection);
            if (asseblyPanel != null)
                children.Add(asseblyPanel);
        }

        const int AVERAGE_LINE_HEIGHT = 30;

        private StackPanel GenerateAssemblyStackPanel(SamplingSection samplingSection)
        {
            if (samplingSection.Assemblies == null || samplingSection.Assemblies.Count == 0)
            {
                return null;
            }

            StackPanel asseblyPanel = new StackPanel();

            Grid asseblyTitleGrid = GenerateAssemblyLineGrid(
                "Address",
                "Instruction",
                isTitle: true
            );

            asseblyPanel.Children.Add(asseblyTitleGrid);

            Brush highlightColor = ColorGenerator.GenerateColor(samplingSection.Overhead ?? 0, this._colorResolution);
            int offset = 1;
            bool stopOffsetCount = false;
            foreach (ExtendedAssembly assemblyLine in samplingSection.Assemblies)
            {
                Grid asseblyLineGrid = GenerateAssemblyLineGrid(
                    firstColumn: assemblyLine.Address,
                    secondColumn: assemblyLine.Instruction,
                    textColor: assemblyLine.IsHighlighted ? highlightColor : null
                );

                asseblyPanel.Children.Add(asseblyLineGrid);

                if (assemblyLine.IsHighlighted)
                    stopOffsetCount = true;

                if (!stopOffsetCount)
                    offset++;
            }

            int numberOfVisibleLines = (int)AssemblyScrollViewer.ActualHeight / AVERAGE_LINE_HEIGHT;
            int highlightedTextPosition = (offset + 4) * AVERAGE_LINE_HEIGHT;
            if ((offset + 5) > numberOfVisibleLines)
            {
                int centeredPositionForHighlightedText =
                    highlightedTextPosition - (numberOfVisibleLines * AVERAGE_LINE_HEIGHT / 2);
                AssemblyScrollViewer.ScrollToVerticalOffset(centeredPositionForHighlightedText);
            }
            else
            {
                AssemblyScrollViewer.ScrollToVerticalOffset(0);
            }

            return asseblyPanel;
        }

        private Grid GenerateAssemblyLineGrid(
            string firstColumn,
            string secondColumn,
            bool isTitle = false,
            Brush textColor = null
        )
        {
            Grid asseblyTitleGrid = new Grid();
            asseblyTitleGrid.ColumnDefinitions.Add(
                new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Pixel) }
            );
            asseblyTitleGrid.ColumnDefinitions.Add(
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
            );

            TextBlock addressTitleTB = GenerateTextBlock(
                InlineText: new List<Inline>()
                {
                    isTitle ? new Bold(new Run(firstColumn)) : new Run(firstColumn)
                },
                fontSize: isTitle ? _localFontSizes.lg : _localFontSizes.md,
                textColor: textColor
            );
            FontFamily _fontFamily = new FontFamily("Consolas");
            addressTitleTB.FontFamily = _fontFamily;
            addressTitleTB.SetValue(Grid.ColumnProperty, 0);

            asseblyTitleGrid.Children.Add(addressTitleTB);

            TextBlock instructionTitleTB = GenerateTextBlock(
                InlineText: new List<Inline>() { new Bold(new Run(secondColumn)) },
                fontSize: isTitle ? _localFontSizes.lg : _localFontSizes.md,
                textColor: textColor
            );
            instructionTitleTB.SetValue(Grid.ColumnProperty, 1);
            instructionTitleTB.FontFamily = _fontFamily;

            asseblyTitleGrid.Children.Add(instructionTitleTB);
            return asseblyTitleGrid;
        }

        private void CreateFunctionSummary(
            UIElementCollection children,
            SamplingSection samplingSection
        )
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_FUNCTION)
            {
                return;
            }

            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.FunctionName}: ")),
                        new Run(samplingSection.Name)
                    },
                    _localFontSizes.lg,
                    _localFontWeights.bold
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Interval}: ")),
                        new Run(samplingSection.Parent.Frequency)
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Hits}: ")),
                        new Run($"{samplingSection.Hits}")
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Overhead}: ")),
                        new Run($"{samplingSection.OverheadPercentage}")
                    },
                    _localFontSizes.md
                )
            );
        }

        private void CreateEventSummary(
            UIElementCollection children,
            SamplingSection samplingSection
        )
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_EVENT)
            {
                return;
            }

            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.EventName}: ")),
                        new Run(samplingSection.Name)
                    },
                    _localFontSizes.lg,
                    _localFontWeights.bold
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Interval}: ")),
                        new Run(samplingSection.Frequency)
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(
                            new Run($"{SamplingExplorerLanguagePack.NumberOfCollectedSamples}: ")
                        ),
                        new Run($"{samplingSection.Hits}")
                    },
                    _localFontSizes.md
                )
            );
        }

        private void CreateRootSummary(
            UIElementCollection children,
            SamplingSection samplingSection
        )
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.ROOT)
            {
                return;
            }

            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")),
                        new Run(samplingSection.Name)
                    },
                    _localFontSizes.lg,
                    _localFontWeights.bold
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.PeFile}: ")),
                        new Run(samplingSection.PeFile)
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.PdbFile}: ")),
                        new Run(samplingSection.PdbFile)
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.SamplesGenerated}: ")),
                        new Run($"{samplingSection.Hits}")
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingExplorerLanguagePack.SamplesDropped}: ")),
                        new Run($"{samplingSection.SamplesDropped}")
                    },
                    _localFontSizes.md
                )
            );
            children.Add(
                GenerateTextBlock(
                    new List<Inline>() { new Run($"{SamplingExplorerLanguagePack.Modules}: ") },
                    _localFontSizes.md,
                    _localFontWeights.bold
                )
            );

            foreach (var module in samplingSection.Modules)
            {
                children.Add(
                    GenerateTextBlock(
                        new List<Inline>()
                        {
                            new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")),
                            new Run(module.Name)
                        },
                        layer: 1
                    )
                );
                children.Add(
                    GenerateTextBlock(
                        new List<Inline>()
                        {
                            new Bold(new Run($"{SamplingExplorerLanguagePack.FilePath}: ")),
                            new Run(module.Path)
                        },
                        layer: 2
                    )
                );
            }

            children.Add(
                GenerateTextBlock(
                    new List<Inline>() { new Run($"{SamplingExplorerLanguagePack.EventList}: ") },
                    _localFontSizes.md,
                    _localFontWeights.bold
                )
            );

            foreach (var sampledEvent in samplingSection.Children)
            {
                children.Add(
                    GenerateTextBlock(
                        new List<Inline>()
                        {
                            new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")),
                            new Run(sampledEvent.Name)
                        },
                        layer: 1
                    )
                );
                children.Add(
                    GenerateTextBlock(
                        new List<Inline>()
                        {
                            new Bold(new Run($"{SamplingSettingsLanguagePack.Frequency} ")),
                            new Run(sampledEvent.Frequency)
                        },
                        layer: 2
                    )
                );
            }
        }

        private enum _localFontSizes
        {
            sm = 12,
            md = 14,
            lg = 16,
        }

        private enum _localFontWeights
        {
            normal,
            bold,
        }

        private TextBlock GenerateTextBlock(
            List<Inline> InlineText,
            _localFontSizes fontSize = _localFontSizes.sm,
            _localFontWeights fontWeight = _localFontWeights.normal,
            int layer = 0,
            Brush textColor = null
        )
        {
            FontWeight _fontWeight =
                fontWeight == _localFontWeights.normal ? FontWeights.Normal : FontWeights.Bold;
            double marginTop =
                fontSize == _localFontSizes.sm
                    ? 5
                    : fontSize == _localFontSizes.md
                        ? 10
                        : 15;
            Thickness _margin = new Thickness(layer * 15, marginTop, 0, 0);
            TextBlock textBlock = new TextBlock()
            {
                FontSize = (double)fontSize,
                FontWeight = _fontWeight,
                Margin = _margin
            };
            textBlock.Inlines.AddRange(InlineText);
            if (textColor != null)
            {
                textBlock.Foreground = textColor;
            }

            return textBlock;
        }

        private void _tree_MouseDoubleClick(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            var treeList = sender as TreeList;
            if (treeList.SelectedItem == null)
                return;
            SamplingSection selecteditem =
                (treeList.SelectedItem as TreeNode).Tag as SamplingSection;
            if (!selecteditem.IsFileExists)
                return;
            string filePath = selecteditem.Name;
            OpenVSDocument(filePath, (int)selecteditem.LineNumber);
        }

        private void OpenVSDocument(string filePath, int lineNumber)
        {
            _ = Task.Run(async () =>
            {
                DocumentView docView;
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                docView = await VS.Documents.OpenAsync(filePath);

                SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
                docView.TextView.Caret.MoveTo(
                    position.Snapshot.GetLineFromLineNumber(lineNumber - 1).End
                );
            });
        }

        private void ClearListMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            formattedSamplingResults.ClearSampling();
            _tree.UpdateTreeList();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            string filePath = textBlock.Text;
            SamplingSection selecteditem =
                (_tree?.SelectedItem as TreeNode)?.Tag as SamplingSection;
            bool isFileExists = File.Exists(filePath);
            if (!isFileExists)
                return;
            int lineNumber = selecteditem?.LineNumber != null ? (int)selecteditem?.LineNumber : 0;
            OpenVSDocument(filePath, lineNumber);
        }
    }
}
