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

using System.Collections.ObjectModel;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.ToolWindows.SamplingSetting
{
    public class SamplingEventConfiguration
    {
        private string samplingEvent;

        public string SamplingEvent
        {
            get { return samplingEvent; }
            set { samplingEvent = value; }
        }
        private string samplingFrequency;
#nullable enable
        public string? SamplingFrequency
        {
            get { return samplingFrequency; }
            set { samplingFrequency = value; }
        }
#nullable disable
        override public string ToString()
        {
            return string.IsNullOrWhiteSpace(SamplingFrequency) ? SamplingEvent : $"{SamplingEvent}:{SamplingFrequency}";
        }
    }
    public class SamplingSettingsForm : NotifyPropertyChangedImplementor
    {
        private bool customProcessRadioButton;

        public bool CustomProcessRadioButton
        {
            get { return customProcessRadioButton; }
            set
            {
                customProcessRadioButton = value;
                OnPropertyChanged();
                if (value == true)
                {
                    FilePath = "";
                }
            }
        }
        private bool currentProjectProcessRadioButton;

        public bool CurrentProjectProcessRadioButton
        {
            get { return currentProjectProcessRadioButton; }
            set
            {
                currentProjectProcessRadioButton = value;
                OnPropertyChanged();
                if (value == true)
                {
                    _ = Task.Run(async () => FilePath = await SolutionProjectOutput.GetProjectOutputAsync());
                }
            }
        }


        private string commandLinePreview;

        public string CommandLinePreview
        {
            get { return commandLinePreview; }
            set
            {
                commandLinePreview = value;
                OnPropertyChanged();

            }
        }

        private CpuCoreElement cpuCore;

        public CpuCoreElement CPUCore
        {
            get { return cpuCore; }
            set
            {
                cpuCore = value;
                OnPropertyChanged("CPUCore");
                CommandLinePreview = SamplingSettings.GenerateCommandLinePreview();
            }
        }

        private string samplingFrequency;

        public string SamplingFrequency
        {
            get { return samplingFrequency; }
            set
            {
                samplingFrequency = value;
            }
        }

        private PredefinedEvent samplingEvent;

        public PredefinedEvent SamplingEvent
        {
            get { return samplingEvent; }
            set
            {
                samplingEvent = value;
            }
        }

        private ObservableCollection<SamplingEventConfiguration> samplingEventList;

        public ObservableCollection<SamplingEventConfiguration> SamplingEventList
        {
            get { return samplingEventList; }
            set
            {
                samplingEventList = value;
                OnPropertyChanged();
                CommandLinePreview = SamplingSettings.GenerateCommandLinePreview();
            }
        }


        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (value == null) return;
                if (value.StartsWith("\"") || value.StartsWith("--pe_file")) filePath = value;
                else filePath = $"\"{value}\"";

                OnPropertyChanged();
                CommandLinePreview = SamplingSettings.GenerateCommandLinePreview();
            }
        }
        private string samplingTimeout;

        public string SamplingTimeout
        {
            get { return samplingTimeout; }
            set
            {
                samplingTimeout = value;
                OnPropertyChanged();
                CommandLinePreview = SamplingSettings.GenerateCommandLinePreview();
            }
        }

        private string extraArgs;

        public string ExtraArgs
        {
            get { return extraArgs; }
            set
            {
                extraArgs = value;
                OnPropertyChanged();
                CommandLinePreview = SamplingSettings.GenerateCommandLinePreview();
            }
        }

        public SamplingSettingsForm()
        {
            customProcessRadioButton = true;
            // We deliberatly set the private version of `samplingEventList`
            // to not trigger the OnPropertyChanged event and generateCommandLinePreview
            // that depend on the init of SamplingSettings.samplingSettingsFrom
            if (SamplingEventList == null) samplingEventList = new ObservableCollection<SamplingEventConfiguration>();
            samplingEventList.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged("SamplingEventList");
                CommandLinePreview = SamplingSettings.GenerateCommandLinePreview();
            };
            if (SamplingSettings.samplingSettingsFrom != null)
            {
                SamplingSettingsForm samplingSettingsForm = SamplingSettings.samplingSettingsFrom;
                FilePath = samplingSettingsForm.FilePath;
                SamplingFrequency = samplingSettingsForm.SamplingFrequency;
                SamplingEvent = samplingSettingsForm.SamplingEvent;
                SamplingTimeout = samplingSettingsForm.SamplingTimeout;
                CPUCore = samplingSettingsForm.CPUCore;
                ExtraArgs = samplingSettingsForm.ExtraArgs;
                SamplingEventList = samplingSettingsForm.SamplingEventList;
            }
            SamplingSettings.samplingSettingsFrom = this;
        }
    }
}
