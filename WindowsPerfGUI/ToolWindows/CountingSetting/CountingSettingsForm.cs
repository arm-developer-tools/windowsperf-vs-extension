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
using WindowsPerfGUI.ToolWindows.SamplingSetting;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.ToolWindows.CountingSetting
{
    public class CountingEventConfiguration { }

    public class CountingSettingsForm : NotifyPropertyChangedImplementor
    {
        private ObservableCollection<SamplingEventConfiguration> countingEventList;

        public ObservableCollection<SamplingEventConfiguration> CountingEventList
        {
            get { return countingEventList; }
            set
            {
                countingEventList = value;
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }

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
        private string pdbFile;

        public string PdbFile
        {
            get { return pdbFile; }
            set { pdbFile = value; }
        }

        private string extraArgs;

        public string ExtraArgs
        {
            get { return extraArgs; }
            set
            {
                extraArgs = value;
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (value.StartsWith("\"") || string.IsNullOrEmpty(value))
                    filePath = value;
                else
                    filePath = $"\"{value}\"";

                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
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
                    _ = Task.Run(async () =>
                    {
                        (string mainOutput, string pdbFile) =
                            await SolutionProjectOutput.GetProjectOutputAsync();
                        FilePath = mainOutput;
                        PdbFile = pdbFile;
                    });
                }
            }
        }

        private bool noTarget;
        public bool NoTarget
        {
            get { return noTarget; }
            set
            {
                noTarget = value;
                OnPropertyChanged();
                if (value == true)
                {
                    FilePath = "";
                    PdbFile = "";
                }
            }
        }

        public CountingSettingsForm()
        {
            customProcessRadioButton = true;
            // We deliberatly set the private version of `countingEventList`
            // to not trigger the OnPropertyChanged event and generateCommandLinePreview
            // that depend on the init of CountingSEttings.countingSettingsForm
            if (CountingEventList == null)
                countingEventList = new ObservableCollection<SamplingEventConfiguration>();
            countingEventList.CollectionChanged += (sender, e) =>
            {
                OnPropertyChanged("CountingEventList");
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            };
            if (CountingSettings.countingSettingsForm != null)
            {
                CountingSettingsForm countingSettingsForm = CountingSettings.countingSettingsForm;
                FilePath = countingSettingsForm.FilePath;
                // CountingEvent = countingSettingsForm.SamplingEvent;
                // CountingTimeout = countingSettingsForm.SamplingTimeout;
                // CPUCore = countingSettingsForm.CPUCore;
                ExtraArgs = countingSettingsForm.ExtraArgs;
                CountingEventList = countingSettingsForm.CountingEventList;
            }
            CountingSettings.countingSettingsForm = this;
        }
    }
}
