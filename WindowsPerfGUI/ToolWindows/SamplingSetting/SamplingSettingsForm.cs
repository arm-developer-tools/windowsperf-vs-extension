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
using WindowsPerfGUI.Utils;
using WindowsPerfGUI.Utils.CommandBuilder;

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
    public override string ToString()
    {
      return string.IsNullOrWhiteSpace(SamplingFrequency)
          ? SamplingEvent
          : $"{SamplingEvent}:{SamplingFrequency}";
    }
  }

  public class SamplingSettingsForm : CommandSettingsForm
  {
    private bool isEventSelectionEnabled;

    public bool IsEventSelectionEnabled
    {
      get { return isEventSelectionEnabled; }
      set
      {
        isEventSelectionEnabled = value;
        OnPropertyChanged();
      }
    }

    private bool sampleDisplayLong;

    public bool SampleDisplayLong
    {
      get { return sampleDisplayLong; }
      set
      {
        sampleDisplayLong = value;
        OnPropertyChanged();
        CommandLinePreview = GenerateCommandLinePreview();
      }
    }

    private bool isSPEEnabled;

    public bool IsSPEEnabled
    {
      get { return isSPEEnabled; }
      set
      {
        isSPEEnabled = value;
        OnPropertyChanged();
      }
    }
    private bool shouldDisassemble = true;

    public bool ShouldDisassemble
    {
      get { return shouldDisassemble; }
      set
      {
        shouldDisassemble = value;
        OnPropertyChanged();
        CommandLinePreview = GenerateCommandLinePreview();
      }
    }

    private ObservableCollection<SamplingEventConfiguration> samplingEventList = new();

    public ObservableCollection<SamplingEventConfiguration> SamplingEventList
    {
      get { return samplingEventList; }
      set
      {
        samplingEventList = value;
        OnPropertyChanged();
        IsEventSelectionEnabled =
           value.Count
           < WperfDefaults.TotalGPCNum;
        OnPropertyChanged("IsEventSelectionEnabled");
        CommandLinePreview = GenerateCommandLinePreview();
      }
    }

    internal override string GenerateCommandLinePreview() { return SamplingSettings.GenerateCommandLinePreview(); }

    public SamplingSettingsForm()
    {
      // We deliberatly set the private version of `samplingEventList`
      // to not trigger the OnPropertyChanged event and generateCommandLinePreview
      // that depend on the init of SamplingSettings.samplingSettingsFrom

      if (SamplingSettings.samplingSettingsFrom != null)
      {
        SamplingSettingsForm samplingSettingsForm = SamplingSettings.samplingSettingsFrom;
        FilePath = samplingSettingsForm.FilePath;
        SelectedEventFrequency = samplingSettingsForm.SelectedEventFrequency;
        SelectedEvent = samplingSettingsForm.SelectedEvent;
        Timeout = samplingSettingsForm.Timeout;
        CPUCore = samplingSettingsForm.CPUCore;
        ExtraArgs = samplingSettingsForm.ExtraArgs;
        SamplingEventList = samplingSettingsForm.SamplingEventList;
        RawEvents = samplingSettingsForm.RawEvents;
        ForceLock = samplingSettingsForm.ForceLock;
        IsSPEEnabled = samplingSettingsForm.IsSPEEnabled;
        ShouldDisassemble = samplingSettingsForm.ShouldDisassemble;
        SampleDisplayLong = samplingSettingsForm.SampleDisplayLong;
      }
      SamplingSettings.samplingSettingsFrom = this;
      SamplingSettings.samplingSettingsFrom.SamplingEventList.CollectionChanged +=
          CollectionUpdater("SamplingEventList");
    }
  }
}
