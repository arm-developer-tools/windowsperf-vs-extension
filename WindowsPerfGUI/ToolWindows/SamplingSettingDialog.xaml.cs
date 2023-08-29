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
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI
{
    public partial class SamplingSettingDialog : DialogWindow
    {
        private readonly CpuCores cpuCores = new();
        public SamplingSettingDialog()
        {
            InitializeComponent();
            CpuCoreComboBox.ItemsSource = cpuCores.CpuCoreList;
            EventComboBox.ItemsSource = WPerfOptions.Instance.WperfList.PredefinedEvents;
            SamplingFrequencyComboBox.ItemsSource = SamplingFrequency.SamplingFrequencyList;

        }


        private void CustomButtonControl_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SyncSamplingSettings();
            // TODO: verify that all the mandatory fields have been filled before closing
            if (!Utils.SamplingSettings.AreSettingsFilled)
            {
                VS.MessageBox.ShowError("To start sampling you need to have at least",
                    "The executable file path and the event name as well as the core selected!"
                    );
                return;
            }
            this.Close();
        }

        private void SyncSamplingSettings()
        {
            Utils.SamplingSettings.FilePath = SamplingSourcePathFilePicker.FilePathTextBox.Text;
            Utils.SamplingSettings.CPUCore = cpuCores.CpuCoreList[CpuCoreComboBox.SelectedIndex];
            Utils.SamplingSettings.SamplingEvent = WPerfOptions.Instance.WperfList.PredefinedEvents[EventComboBox.SelectedIndex];
            Utils.SamplingSettings.SamplingFrequency = SamplingFrequency.SamplingFrequencyList[SamplingFrequencyComboBox.SelectedIndex];
            Utils.SamplingSettings.SamplingTimeout = SamplingTimeoutTextBox.Text;
            UpdateSamplingCommandCallTextBox();

        }
        private void UpdateSamplingCommandCallTextBox()
        {
            Utils.SamplingSettings.GenerateCommandLineArgsArray();
            SamplingCommandCallTextBox.Text = Utils.SamplingSettings.GenerateCommandLinePreview();
        }
        private void EventComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Utils.SamplingSettings.SamplingEvent = WPerfOptions.Instance.WperfList.PredefinedEvents[EventComboBox.SelectedIndex];
            UpdateSamplingCommandCallTextBox();
        }

        private void SamplingFrequencyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Utils.SamplingSettings.SamplingFrequency = SamplingFrequency.SamplingFrequencyList[SamplingFrequencyComboBox.SelectedIndex];
            UpdateSamplingCommandCallTextBox();
        }

        private void SamplingTimeoutTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Utils.SamplingSettings.SamplingTimeout = SamplingTimeoutTextBox.Text;
            UpdateSamplingCommandCallTextBox();
        }

        private void CpuCoreComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Utils.SamplingSettings.CPUCore = cpuCores.CpuCoreList[CpuCoreComboBox.SelectedIndex];
            UpdateSamplingCommandCallTextBox();
        }
        private void FilePickerTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Utils.SamplingSettings.FilePath = SamplingSourcePathFilePicker.FilePathTextBox.Text;
            UpdateSamplingCommandCallTextBox();
        }
    }
}
