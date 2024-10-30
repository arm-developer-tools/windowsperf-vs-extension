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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using WindowsPerfGUI.Utils.CommandBuilder;

namespace WindowsPerfGUI.ToolWindows.CountingSetting
{
    public class CountingEventConfiguration { }

    public class CountingSettingsForm : CommandSettingsForm
    {
        private ObservableCollection<string> countingEventList = new ObservableCollection<string>();

        public ObservableCollection<string> CountingEventList
        {
            get { return countingEventList; }
            set
            {
                countingEventList = value;
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }

        private ObservableCollection<string> countingMetricList =
            new ObservableCollection<string>();

        public ObservableCollection<string> CountingMetricList
        {
            get { return countingMetricList; }
            set
            {
                countingMetricList = value;
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }
 
        private bool isTimelineSelected;
        public bool IsTimelineSelected
        {
            get { return isTimelineSelected; }
            set
            {
                isTimelineSelected = value;
                if (value == true)
                {
                    TimelineInterval ??= "0.5";
                    TimelineIterations ??= "1";
                }
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }

        private bool noTarget;
        public bool NoTarget
        {
            get { return noTarget; }
            set
            {
                noTarget = value;
                if (value == true)
                {
                    FilePath = "";
                    PdbFile = "";
                }
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }
       
        private string timelineIterations;

        public string TimelineIterations
        {
            get { return timelineIterations; }
            set
            {
                timelineIterations = value;
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }

        private string timelineInterval;

        public string TimelineInterval
        {
            get { return timelineInterval; }
            set
            {
                timelineInterval = value;
                OnPropertyChanged();
                CommandLinePreview = CountingSettings.GenerateCommandLinePreview();
            }
        }

        private bool isCountCollected;

        public bool IsCountCollected
        {
            get { return isCountCollected; }
            set
            {
                isCountCollected = value;
                OnPropertyChanged();
            }
        }

        private List<CountingEvent> countingResult;

        public List<CountingEvent> CountingResult
        {
            get { return countingResult; }
            set
            {
                countingResult = value;
                OnPropertyChanged();
            }
        }
        internal override string GenerateCommandLinePreview() { return CountingSettings.GenerateCommandLinePreview(); }
        public CountingSettingsForm()
        {
            if (CountingSettings.countingSettingsForm != null)
            {
                CountingSettingsForm countingSettingsForm = CountingSettings.countingSettingsForm;
                FilePath = countingSettingsForm.FilePath;
                CPUCores = countingSettingsForm.CPUCores;
                ExtraArgs = countingSettingsForm.ExtraArgs;
                Timeout = countingSettingsForm.Timeout;
                CustomProcessRadioButton = countingSettingsForm.CustomProcessRadioButton;
                CurrentProjectProcessRadioButton =
                    countingSettingsForm.CurrentProjectProcessRadioButton;
                NoTarget = countingSettingsForm.NoTarget;
                IsTimelineSelected = countingSettingsForm.IsTimelineSelected;
                TimelineInterval = countingSettingsForm.TimelineInterval;
                TimelineIterations = countingSettingsForm.TimelineIterations;
                CountingEventList = countingSettingsForm.CountingEventList;
                CountingMetricList = countingSettingsForm.CountingMetricList;
                IsCountCollected = countingSettingsForm.IsCountCollected;
                CountingResult = countingSettingsForm.CountingResult;
            }
            CountingSettings.countingSettingsForm = this;

            CountingSettings.countingSettingsForm.CPUCores.CollectionChanged += CollectionUpdater(
                "CPUCores"
            );
            CountingSettings.countingSettingsForm.CountingEventList.CollectionChanged +=
                CollectionUpdater("CountingEventList");
            CountingSettings.countingSettingsForm.CountingMetricList.CollectionChanged +=
                CollectionUpdater("CountingMetricList");
        }
    }
}
