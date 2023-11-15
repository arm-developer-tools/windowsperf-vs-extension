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
using WindowsPerfGUI.Components.TreeListView;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI
{
    public partial class SamplingExplorerControl : UserControl
    {
        readonly WperfClientFactory wperfClient = new();
        static FormattedSamplingResults formattedSamplingResults = new FormattedSamplingResults();
        private void HandleSamplingFinished(object sender, (WperfSampling serializedOutput, string stdError) e)
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
            formattedSamplingResults.FormatSamplingResults(e.serializedOutput, $"{SamplingExplorerLanguagePack.ExecutedAt} {DateTime.Now.ToShortTimeString()}");
            if (_tree.Model == null) _tree.Model = formattedSamplingResults;
            _tree.UpdateTreeList();
        }

        public SamplingExplorerControl()
        {
            InitializeComponent();
            _tree.Model = formattedSamplingResults;
            wperfClient.OnSamplingFinished += HandleSamplingFinished;
            StopSamplingMonikerButton.IsEnabled = false;
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
            if (!WPerfOptions.Instance.IsWperfInitialized) return;
            if (!SamplingSettings.AreSettingsFilled)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.UncompleteSamplingSettingsLine1,
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

        private void _tree_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TreeList treeList = sender as TreeList;
            SummaryStack.Children.Clear();
            if (treeList == null || treeList.SelectedItem == null) return;
            SamplingSection selecteditem = (treeList.SelectedItem as TreeNode).Tag as SamplingSection;
            switch (selecteditem.SectionType)
            {
                case SamplingSection.SamplingSectionType.ROOT:
                    CreateRootSummary(SummaryStack.Children, selecteditem);
                    break;
                case SamplingSection.SamplingSectionType.SAMPLE_EVENT:
                    CreateEventSummary(SummaryStack.Children, selecteditem);
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
        private void CreateSourceCodeSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE)
            {
                return;
            }

            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.FilePath}: ")), new Run(samplingSection.Name) }, _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.LineNumber}: ")), new Run(samplingSection.LineNumber.ToString()) }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Hits}: ")), new Run($"{samplingSection.Hits}") }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Overhead}: ")), new Run($"{samplingSection.OverheadPercentage}") }, _localFontSizes.md));
        }
        private void CreateFunctionSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_FUNCTION)
            {
                return;
            }

            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.FunctionName}: ")), new Run(samplingSection.Name) }, _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Interval}: ")), new Run(samplingSection.Parent.Frequency) }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Hits}: ")), new Run($"{samplingSection.Hits}") }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Overhead}: ")), new Run($"{samplingSection.OverheadPercentage}") }, _localFontSizes.md));
        }
        private void CreateEventSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_EVENT)
            {
                return;
            }
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.EventName}: ")), new Run(samplingSection.Name) }, _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Interval}: ")), new Run(samplingSection.Frequency) }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.NumberOfCollectedSamples}: ")), new Run($"{samplingSection.Hits}") }, _localFontSizes.md));

        }

        private void CreateRootSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.ROOT)
            {
                return;
            }
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")), new Run(samplingSection.Name) }, _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.PeFile}: ")), new Run(samplingSection.PeFile) }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.PdbFile}: ")), new Run(samplingSection.PdbFile) }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.SamplesGenerated}: ")), new Run($"{samplingSection.Hits}") }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.SamplesDropped}: ")), new Run($"{samplingSection.SamplesDropped}") }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Run($"{SamplingExplorerLanguagePack.Modules}: ") }, _localFontSizes.md, _localFontWeights.bold));

            foreach (var module in samplingSection.Modules)
            {
                children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")), new Run(module.Name) }, layer: 1));
                children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.FilePath}: ")), new Run(module.Path) }, layer: 2));
            }

            children.Add(GenerateTextBlock(new List<Inline>() { new Run($"{SamplingExplorerLanguagePack.EventList}: ") }, _localFontSizes.md, _localFontWeights.bold));

            foreach (var sampledEvent in samplingSection.Children)
            {
                children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")), new Run(sampledEvent.Name) }, layer: 1));
                children.Add(GenerateTextBlock(new List<Inline>() { new Bold(new Run($"{SamplingSettingsLanguagePack.Frequency} ")), new Run(sampledEvent.Frequency) }, layer: 2));
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
        private TextBlock GenerateTextBlock(List<Inline> InlineText, _localFontSizes fontSize = _localFontSizes.sm, _localFontWeights fontWeight = _localFontWeights.normal, int layer = 0)
        {
            FontWeight _fontWeight = fontWeight == _localFontWeights.normal ? FontWeights.Normal : FontWeights.Bold;
            double marginTop = fontSize == _localFontSizes.sm ? 5 : fontSize == _localFontSizes.md ? 10 : 15;
            Thickness _margin = new Thickness(layer * 15, marginTop, 0, 0);
            TextBlock textBlock = new TextBlock() { FontSize = (double)fontSize, FontWeight = _fontWeight, Margin = _margin };
            textBlock.Inlines.AddRange(InlineText);
            return textBlock;

        }
        private void _tree_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeList = sender as TreeList;

            SamplingSection selecteditem = (treeList.SelectedItem as TreeNode).Tag as SamplingSection;
            if (!selecteditem.IsFileExists) return;
            string filePath = selecteditem.Name;
            OpenVSDocument(filePath, (int)selecteditem.LineNumber);
        }
        private void OpenVSDocument(string filePath, int lineNumber)
        {
            _ = Task.Run(async () =>
            {
                DocumentView docView;
                docView = await VS.Documents.OpenAsync(filePath);
                SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
                docView.TextView.Caret.MoveTo(position.Snapshot.GetLineFromLineNumber(lineNumber - 1).End);
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
            SamplingSection selecteditem = (_tree?.SelectedItem as TreeNode)?.Tag as SamplingSection;
            bool isFileExists = File.Exists(filePath);
            if (!isFileExists) return;
            int lineNumber = selecteditem?.LineNumber != null ? (int)selecteditem?.LineNumber : 0;
            OpenVSDocument(filePath, lineNumber);
        }
    }
}
