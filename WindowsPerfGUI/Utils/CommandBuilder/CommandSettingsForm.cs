// BSD 3-Clause License
//
// Copyright (c) 2024, Arm Limited
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
using System.Collections.Specialized;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingSetting;

namespace WindowsPerfGUI.Utils.CommandBuilder
{
    public abstract class CommandSettingsForm : NotifyPropertyChangedImplementor
    {
        private bool kernelMode;
        public bool KernelMode
        {
            get { return kernelMode; }
            set
            {
                kernelMode = value;
                OnPropertyChanged();
                CommandLinePreview = GenerateCommandLinePreview();
            }
        }
        private bool forceLock;
        public bool ForceLock
        {
            get { return forceLock; }
            set
            {
                forceLock = value;
                OnPropertyChanged();
                CommandLinePreview = GenerateCommandLinePreview();
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
                CommandLinePreview = GenerateCommandLinePreview();
            }
        }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.StartsWith("\""))
                    filePath = value;
                else
                    filePath = $"\"{value}\"";

                OnPropertyChanged();
                CommandLinePreview = GenerateCommandLinePreview();
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

        private bool customProcessRadioButton = true;

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
                if (value)
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

        private CpuCoreElement cpuCore;

        public CpuCoreElement CPUCore
        {
            get { return cpuCore; }
            set
            {
                cpuCore = value;
                OnPropertyChanged("CPUCore");
                CommandLinePreview = GenerateCommandLinePreview();
            }
        }

        private ObservableCollection<CpuCoreElement> cpuCores =
            new ObservableCollection<CpuCoreElement>();

        public ObservableCollection<CpuCoreElement> CPUCores
        {
            get { return cpuCores; }
            set
            {
                cpuCores = value;
                OnPropertyChanged();
                CommandLinePreview = GenerateCommandLinePreview();
            }
        }

        private string rawEvents;

        public string RawEvents
        {
            get { return rawEvents; }
            set
            {
                rawEvents = value;
                OnPropertyChanged();
                CommandLinePreview = GenerateCommandLinePreview();
            }
        }
        private PredefinedEvent selectedEvent;

        public PredefinedEvent SelectedEvent
        {
            get { return selectedEvent; }
            set { selectedEvent = value; }
        }

        private string selectedEventFrequency;

        public string SelectedEventFrequency
        {
            get { return selectedEventFrequency; }
            set { selectedEventFrequency = value; }
        }

        private PredefinedMetric selectedMetric;

        public PredefinedMetric SelectedMetric
        {
            get { return selectedMetric; }
            set
            {
                selectedMetric = value;
                OnPropertyChanged();
            }
        }
        private string timeout;

        public string Timeout
        {
            get { return timeout; }
            set
            {
                timeout = value;
                OnPropertyChanged();
                CommandLinePreview = GenerateCommandLinePreview();
            }
        }
        internal NotifyCollectionChangedEventHandler CollectionUpdater(string callerMemberName)
        {
            return (object sender, NotifyCollectionChangedEventArgs e) =>
            {
                OnPropertyChanged(callerMemberName);
                CommandLinePreview = GenerateCommandLinePreview();
            };
        }
        internal abstract string GenerateCommandLinePreview();
    }
}
