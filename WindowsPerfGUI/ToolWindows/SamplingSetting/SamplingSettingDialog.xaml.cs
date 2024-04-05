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

using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsPerfGUI.Options;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils.ListSearcher;

namespace WindowsPerfGUI.ToolWindows.SamplingSetting
{
    public partial class SamplingSettingDialog : DialogWindow
    {
        private void PreviewKeyDown_EnhanceComboSearch(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            EventComboBox.IsDropDownOpen = true;
            EventComboBoxPlaceholder.Visibility = Visibility.Hidden;
            if (!string.IsNullOrEmpty(EventComboBox.Text))
            {
                EventComboBox.ItemsSource = FilterEventList(EventComboBox.Text);
            }
            else
            {
                EventComboBoxPlaceholder.Visibility = Visibility.Visible;
                EventComboBox.ItemsSource = WPerfOptions.Instance.WperfList.PredefinedEvents;
            }
        }

        private static List<PredefinedEvent> FilterEventList(string searchText)
        {
            var eventList = WPerfOptions.Instance.WperfList.PredefinedEvents;
            var listSearcher = new ListSearcher<PredefinedEvent>(eventList, new SearchOptions<PredefinedEvent> { IsCaseSensitve = true, GetValue = x => x.AliasName });
            return listSearcher.Search(searchText);
        }

        public SamplingSettingDialog()
        {
            SolutionProjectOutput.GetProjectOutputAsync().FireAndForget();
            InitializeComponent();
            CpuCoreComboBox.ItemsSource = CpuCores.InitCpuCores();
            EventComboBox.ItemsSource = WPerfOptions.Instance.WperfList.PredefinedEvents;
            SamplingFrequencyComboBox.ItemsSource = SamplingFrequency.SamplingFrequencyList;
            ProjectTargetConfigLabel.Content = SolutionProjectOutput.SelectedConfigLabel;
            if (SamplingSettings.samplingSettingsFrom.SamplingEvent == null)
                EventComboBox.SelectedIndex = -1;
            if (SamplingSettings.samplingSettingsFrom.SamplingFrequency == null)
                SamplingFrequencyComboBox.SelectedIndex = -1;
            if (SamplingSettings.samplingSettingsFrom.CPUCore == null)
                CpuCoreComboBox.SelectedIndex = 0;

            if (SamplingSettings.samplingSettingsFrom.FilePath != null)
                SamplingSourcePathFilePicker.FilePathTextBox.Text = SamplingSettings
                    .samplingSettingsFrom
                    .FilePath;
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine(CustomProcessRadioButton.IsChecked);
            Debug.WriteLine(CurrentProjectProcessRadioButton.IsChecked);
            Debug.WriteLine(SamplingSourcePathFilePicker.IsEnabled);
            SyncSamplingSettings();
            if (!SamplingSettings.AreSettingsFilled)
            {
                VS.MessageBox.ShowError(
                    ErrorLanguagePack.UncompleteSamplingSettingsLine1,
                    ErrorLanguagePack.UncompleteSamplingSettingsLine2
                );
                return;
            }

            Close();
        }

        private void SyncSamplingSettings()
        {
            UpdateSamplingCommandCallTextBox();
        }

        private void UpdateSamplingCommandCallTextBox()
        {
            SamplingSettings.GenerateCommandLineArgsArray(SamplingSettings.samplingSettingsFrom);
            SamplingCommandCallTextBox.Text = SamplingSettings.GenerateCommandLinePreview();
        }

        private void FilePickerTextBox_TextChanged(
            object sender,
            System.Windows.Controls.TextChangedEventArgs e
        )
        {
            SamplingSettings.samplingSettingsFrom.FilePath = SamplingSourcePathFilePicker
                .FilePathTextBox
                .Text;
            UpdateSamplingCommandCallTextBox();
        }

        private void AddEventButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var newSamplingEventConfig = new SamplingEventConfiguration()
            {
                SamplingEvent = (EventComboBox.SelectedItem as PredefinedEvent)?.AliasName,
                SamplingFrequency = SamplingFrequencyComboBox.SelectedItem as string
            };
            EventComboBox.SelectedIndex = -1;
            SamplingFrequencyComboBox.SelectedIndex = -1;
            EventComboBox.ItemsSource = WPerfOptions.Instance.WperfList.PredefinedEvents;

            foreach (
                var item in SamplingSettings.samplingSettingsFrom.SamplingEventList.Select(
                    (value, i) => new { i, value }
                )
            )
            {
                if (item.value.SamplingEvent != newSamplingEventConfig.SamplingEvent)
                {
                    continue;
                }

                SamplingSettings.samplingSettingsFrom.SamplingEventList[item.i] =
                    newSamplingEventConfig;
                return;
            }

            SamplingSettings.samplingSettingsFrom.SamplingEventList.Add(newSamplingEventConfig);
        }

        private void AddRawEventButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string input = RawEventsInput.Text;
            if (input == null)
                return;

            string[] inputParts = input.Split(':');

            string eventIndex = inputParts[0];
            string eventFrequency = "0";
            if (inputParts.Length > 1)
            {
                eventFrequency = inputParts[1];
            }

            var indexRegex = new Regex("^r[\\da-f]{1,4}$", RegexOptions.IgnoreCase);
            bool indexMatch = indexRegex.Match(eventIndex).Success;

            bool frequencyMatch = true;
            uint frequency = 0;
            if (eventFrequency != null)
            {
                frequencyMatch = uint.TryParse(eventFrequency, out frequency);
            }

            if (!frequencyMatch || !indexMatch)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.RawEventBadFormat);
                return;
            }
            var eventExists = SamplingSettings.samplingSettingsFrom.SamplingEventList.Any(el =>
                el.SamplingEvent == eventIndex
            );

            if (eventExists)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.RawEventExists);
                return;
            }

            SamplingEventConfiguration newSamplingEventConfig = new SamplingEventConfiguration()
            {
                SamplingEvent = eventIndex,
                SamplingFrequency = frequency > 0 ? frequency.ToString() : null
            };

            SamplingSettings.samplingSettingsFrom.SamplingEventList.Add(newSamplingEventConfig);

            RawEventsInput.Clear();
        }

        private void RemoveEventButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int selectedIndex = SamplingEventListBox.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }

            SamplingSettings.samplingSettingsFrom.SamplingEventList.RemoveAt(
                SamplingEventListBox.SelectedIndex
            );

            SamplingEventListBox.Items.Refresh();
            SamplingEventListBox.SelectedIndex = Math.Min(
                selectedIndex,
                SamplingEventListBox.Items.Count - 1
            );
        }

        private void SamplingEventListBox_SelectionChanged(
            object sender,
            System.Windows.Controls.SelectionChangedEventArgs e
        )
        {
            int eventIndex = -1;
            if (!(SamplingEventListBox.SelectedItems?.Count > 0))
            {
                return;
            }

            string aliasName = (
                SamplingEventListBox.SelectedItems[0] as SamplingEventConfiguration
            )?.SamplingEvent;

            foreach (
                var predefinedEvent in ((List<PredefinedEvent>)EventComboBox.ItemsSource).Select(
                    (value, i) => new { value, i }
                )
            )
            {
                if (predefinedEvent.value.AliasName == aliasName)
                {
                    eventIndex = predefinedEvent.i;
                }
            }

            EventComboBox.SelectedIndex = eventIndex;
            SamplingFrequencyComboBox.SelectedItem = (
                SamplingEventListBox.SelectedItems[0] as SamplingEventConfiguration
            )?.SamplingFrequency;
        }
    }
}
