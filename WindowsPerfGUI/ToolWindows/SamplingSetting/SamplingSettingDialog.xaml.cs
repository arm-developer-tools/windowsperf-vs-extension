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
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK.WperfOutputs;

namespace WindowsPerfGUI.ToolWindows.SamplingSetting
{
    public partial class SamplingSettingDialog : DialogWindow
    {
        public SamplingSettingDialog()
        {
            InitializeComponent();

            CpuCoreComboBox.ItemsSource = CpuCores.InitCpuCores();
            EventComboBox.ItemsSource = WPerfOptions.Instance.WperfList.PredefinedEvents;
            SamplingFrequencyComboBox.ItemsSource = SamplingFrequency.SamplingFrequencyList;

            if (SamplingSettings.samplingSettingsFrom.SamplingEvent == null) EventComboBox.SelectedIndex = -1;
            if (SamplingSettings.samplingSettingsFrom.SamplingFrequency == null) SamplingFrequencyComboBox.SelectedIndex = -1;
            if (SamplingSettings.samplingSettingsFrom.CPUCore == null) CpuCoreComboBox.SelectedIndex = 0;

            if (SamplingSettings.samplingSettingsFrom.FilePath != null) SamplingSourcePathFilePicker.FilePathTextBox.Text = SamplingSettings.samplingSettingsFrom.FilePath;
        }


        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine(CustomProcessRadioButton.IsChecked);
            Debug.WriteLine(CurrentProjectProcessRadioButton.IsChecked);
            Debug.WriteLine(SamplingSourcePathFilePicker.IsEnabled);
            SyncSamplingSettings();
            if (!SamplingSettings.AreSettingsFilled)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.UncompleteSamplingSettingsLine1,
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

        private void FilePickerTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SamplingSettings.samplingSettingsFrom.FilePath = SamplingSourcePathFilePicker.FilePathTextBox.Text;
            UpdateSamplingCommandCallTextBox();
        }

        private void AddEventButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var newSamplingEventConfig = new SamplingEventConfiguration()
            {
                SamplingEvent = (EventComboBox.SelectedItem as PredefinedEvent).AliasName,
                SamplingFrequency = SamplingFrequencyComboBox.SelectedItem as string
            };
            EventComboBox.SelectedIndex = -1;
            SamplingFrequencyComboBox.SelectedIndex = -1;
            foreach (var item in SamplingSettings.samplingSettingsFrom.SamplingEventList.Select((value, i) => new { i, value }))
            {
                if (item.value.SamplingEvent == newSamplingEventConfig.SamplingEvent)
                {
                    SamplingSettings.samplingSettingsFrom.SamplingEventList[item.i] = newSamplingEventConfig;
                    return;
                }
            }
            SamplingSettings.samplingSettingsFrom.SamplingEventList.Add(newSamplingEventConfig);
        }

        private void RemoveEventButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int selectedIndex = SamplingEventListBox.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }
            SamplingSettings.samplingSettingsFrom.SamplingEventList.RemoveAt(SamplingEventListBox.SelectedIndex);

            SamplingEventListBox.Items.Refresh();
            SamplingEventListBox.SelectedIndex = Math.Min(selectedIndex, SamplingEventListBox.Items.Count - 1);
        }

        private void SamplingEventListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int EventIndex = -1;
            if (SamplingEventListBox.SelectedItems?.Count > 0)
            {
                var aliasName = (SamplingEventListBox.SelectedItems[0] as SamplingEventConfiguration).SamplingEvent;
                foreach (var predefinedEvent in (EventComboBox.ItemsSource as List<PredefinedEvent>).Select((value, i) => new { value, i }))
                {
                    if (predefinedEvent.value.AliasName == aliasName)
                    {
                        EventIndex = predefinedEvent.i;
                    }
                }
                EventComboBox.SelectedIndex = EventIndex;
                SamplingFrequencyComboBox.SelectedItem = (SamplingEventListBox.SelectedItems[0] as SamplingEventConfiguration).SamplingFrequency;
            }
        }
    }
}
