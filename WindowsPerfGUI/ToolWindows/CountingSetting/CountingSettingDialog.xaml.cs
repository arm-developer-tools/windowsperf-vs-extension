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

using CliWrap;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WindowsPerfGUI.Options;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingSetting;
using WindowsPerfGUI.Utils;
using WindowsPerfGUI.Utils.ListSearcher;
using static WindowsPerfGUI.SDK.WperfOutputs.WperfList;

namespace WindowsPerfGUI.ToolWindows.CountingSetting
{
  public partial class CountingSettingDialog : DialogWindow
  {
    readonly WperfClientFactory wperfClient = new();

    public CountingSettingDialog()
    {
      SolutionProjectOutput.GetProjectOutputAsync().FireAndForget();
      InitializeComponent();
      OpenInWPAButton.IsEnabled = !string.IsNullOrEmpty(WperfClient.OutputPath);
      SaveAsButton.IsEnabled = OpenInWPAButton.IsEnabled;
      StopCountingButton.IsEnabled = false;
      ResetMetricComboBox();
      ResetEventComboBox();
      CpuCoresGrid.ItemsSource = CpuCores.InitCpuCores();
      ProjectTargetConfigLabel.Content = SolutionProjectOutput.SelectedConfigLabel;
      if (CountingSettings.countingSettingsForm.FilePath != null)
        CountingSourcePathFilePicker.FilePathTextBox.Text = CountingSettings
            .countingSettingsForm
            .FilePath;
      if (CountingSettings.countingSettingsForm.SelectedEvent == null)
        EventComboBox.SelectedIndex = -1;
      if (CountingSettings.countingSettingsForm.SelectedMetric == null)
        MetricComboBox.SelectedIndex = -1;
      MetricComboBox.DropDownOpened += (sender, e) =>
      {
        HideMetricComboBoxPlaceholder();
      };
      MetricComboBox.SelectionChanged += (sender, e) =>
      {
        HideMetricComboBoxPlaceholder();
      };
      EventComboBox.DropDownOpened += (sender, e) =>
      {
        HideEventComboBoxPlaceholder();
      };
      EventComboBox.SelectionChanged += (sender, e) =>
      {
        HideEventComboBoxPlaceholder();
      };
      wperfClient.OnCountingFinished += HandleCountingFinished;
    }



    private void HandleCountingFinished(
        object sender,
        (List<CountingEvent> countingEvents, string stdError) e
    )
    {
      StopCountingButton.IsEnabled = false;
      StartCountingButton.IsEnabled = true;
      BuildAndStartCountingButton.IsEnabled = true;
      OpenInWPAButton.IsEnabled = true;
      SaveAsButton.IsEnabled = OpenInWPAButton.IsEnabled;
      StartCountingButton.Content = CountingSettingsLanguagePack.Start;

      if (!string.IsNullOrEmpty(e.stdError))
      {
        VS.MessageBox.ShowError(ErrorLanguagePack.ErrorWindowsPerfCLI, e.stdError);
        return;
      }

      CountingSettings.countingSettingsForm.IsCountCollected = true;
      CountingSettings.countingSettingsForm.CountingResult = e.countingEvents;
    }

    private void UpdateCountingCommandCallTextBox()
    {
      CountingSettings.GenerateCommandLineArgsArray(CountingSettings.countingSettingsForm);
      CountingCommandCallTextBox.Text = CountingSettings.GenerateCommandLinePreview();
    }

    private void FilePickerTextBox_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e
    )
    {
      CountingSettings.countingSettingsForm.FilePath = CountingSourcePathFilePicker
          .FilePathTextBox
          .Text;
      UpdateCountingCommandCallTextBox();
    }

    private void StartCounting_Click(object sender, RoutedEventArgs e)
    {
      if (!WPerfOptions.Instance.IsWperfInitialized)
        return;
      if (!CountingSettings.AreSettingsFilled)
      {
        VS.MessageBox.ShowError(ErrorLanguagePack.IncompleteCountingSettingsLine1);
        return;
      }

      if (CountingSettings.IsCounting)
      {
        VS.MessageBox.ShowError(
            ErrorLanguagePack.RunningCountingOverlapLine1,
            ErrorLanguagePack.RunningCountingOverlapLine2
        );
        return;
      }
      SyncCountingSettings();
      _ = wperfClient
          .StartCountingAsync()
          .ContinueWith(
              async (t) =>
              {
                while (!t.IsCompleted)
                {
                  Thread.Sleep(1000);
                }
                if (t.IsFaulted)
                {
                  await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                  await VS.MessageBox.ShowErrorAsync(ErrorLanguagePack.WperfPathChanged);
                  Trace.WriteLine(t.Exception.Message);
                  wperfClient.Reinitialize();

                  StopCountingButton.IsEnabled = false;
                  StartCountingButton.IsEnabled = true;
                  BuildAndStartCountingButton.IsEnabled = true;

                  StartCountingButton.Content = SamplingSettingsLanguagePack.Save;
                }
              },
              CancellationToken.None,
              (TaskContinuationOptions)TaskCreationOptions.None,
              TaskScheduler.Default
          );

      CountingSettings.countingSettingsForm.CountingResult = null;
      CountingSettings.countingSettingsForm.IsCountCollected = false;
      StopCountingButton.IsEnabled = true;
      StartCountingButton.IsEnabled = false;
      BuildAndStartCountingButton.IsEnabled = false;
      StartCountingButton.Content = "Loading...";
    }

    private void StopCounting_Click(object sender, RoutedEventArgs e)
    {
      wperfClient.StopCounting();
      StopCountingButton.IsEnabled = false;
      StartCountingButton.IsEnabled = true;
      BuildAndStartCountingButton.IsEnabled = true;
    }

    private void SyncCountingSettings()
    {
      UpdateCountingCommandCallTextBox();
    }

    private void CountingEventListBox_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e
    )
    {
      int eventIndex = -1;

      if (!(CountingEventListBox.SelectedItems?.Count > 0))
        return;

      HideEventComboBoxPlaceholder();

      GroupEventButton.IsEnabled = CanGroupEvents();

      string aliasName = CountingEventListBox.SelectedItems[0] as string;
      EventComboBox.ClearFilter();
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
    }


    private void AddEventButton_Click(object sender, RoutedEventArgs e)
    {
      var newCountingEvent = (EventComboBox.SelectedItem as PredefinedEvent)?.AliasName;

      EventComboBox.SelectedIndex = -1;

      foreach (
          var item in CountingSettings.countingSettingsForm.CountingEventList.Select(
              (value, i) => new { i, value }
          )
      )
      {
        if (item.value != newCountingEvent)
        {
          continue;
        }

        CountingSettings.countingSettingsForm.CountingEventList[item.i] = newCountingEvent;
        return;
      }

      CountingSettings.countingSettingsForm.CountingEventList.Add(newCountingEvent);
      EventComboBoxPlaceholder.Visibility = Visibility.Visible;
    }

    private void RemoveEventButton_Click(object sender, RoutedEventArgs e)
    {
      int selectedIndex = CountingEventListBox.SelectedIndex;
      if (selectedIndex < 0)
      {
        return;
      }
      CountingSettings.countingSettingsForm.CountingEventList.RemoveAt(
          CountingEventListBox.SelectedIndex
      );

      CountingEventListBox.Items.Refresh();
      CountingEventListBox.SelectedIndex = Math.Min(
          selectedIndex,
          CountingEventListBox.Items.Count - 1
      );
    }

    private void HideEventComboBoxPlaceholder()
    {
      EventComboBoxPlaceholder.Visibility = Visibility.Hidden;
    }

    private void HideMetricComboBoxPlaceholder()
    {
      MetricComboBoxPlaceholder.Visibility = Visibility.Hidden;
    }

    private void AddRawEventButton_Click(object sender, RoutedEventArgs e)
    {
      string rawEvent = RawEventsInput.Text;
      if (rawEvent == null)
        return;

      var indexRegex = new Regex("^r[\\da-f]{1,4}$", RegexOptions.IgnoreCase);

      if (!indexRegex.Match(rawEvent).Success)
      {
        VS.MessageBox.ShowError(ErrorLanguagePack.RawEventBadFormat);
        return;
      }
      var eventExists = CountingSettings.countingSettingsForm.CountingEventList.Any(el =>
          el == rawEvent
      );

      if (eventExists)
      {
        VS.MessageBox.ShowError(ErrorLanguagePack.RawEventExists);
        return;
      }

      CountingSettings.countingSettingsForm.CountingEventList.Add(rawEvent);

      RawEventsInput.Clear();
    }

    private static List<PredefinedEvent> FilterEventList(string searchText)
    {
      var eventList = WPerfOptions.Instance.WperfList.PredefinedEvents;
      var listSearcher = new ListSearcher<PredefinedEvent>(
          eventList,
          new SearchOptions<PredefinedEvent> { GetValue = x => x.AliasName }
      );
      return listSearcher.Search(searchText);
    }

    private void CustomButtonControl_Click(object sender, RoutedEventArgs e)
    {
      if (!CanGroupEvents())
      {
        return;
      }
      var selectedItems = CountingEventListBox.SelectedItems.Cast<string>().ToList();
      string eventGroup = "{" + string.Join(",", selectedItems) + "}";
      CountingSettings.countingSettingsForm.CountingEventList.Add(eventGroup);
      for (int i = 0; i < selectedItems.Count; i++)
      {
        CountingSettings.countingSettingsForm.CountingEventList.Remove(selectedItems[i]);
      }
      CountingEventListBox.SelectedItems.Add(eventGroup);
    }

    private bool CanGroupEvents()
    {
      if (CountingEventListBox.SelectedItems.Count < 2)
        return false;
      if (CountingEventListBox.SelectedItems.Count > WperfDefaults.TotalGPCNum)
        return false;
      var selectedItems = CountingEventListBox.SelectedItems.Cast<string>().ToList();
      if (
          string.Join(",", selectedItems).Contains("{")
          || string.Join(",", selectedItems).Contains("}")
      )
        return false;
      return true;
    }

    private void CountingMetricListBox_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e
    )
    {
      int metricIndex = -1;
      if (!(CountingMetricListBox.SelectedItems?.Count > 0))
      {
        return;
      }
      MetricComboBoxPlaceholder.Visibility = Visibility.Hidden;

      string metricName = CountingMetricListBox.SelectedItems[0] as string;
      MetricComboBox.ClearFilter();

      foreach (
          var predefinedMetric in ((List<PredefinedMetricAndGroupOfMetrics>)MetricComboBox.ItemsSource).Select(
              (value, i) => new { value, i }
          )
      )
      {
        if (predefinedMetric.value.Metric == metricName)
        {
          metricIndex = predefinedMetric.i;
        }
      }

      MetricComboBox.SelectedIndex = metricIndex;
    }

    private void AddMetricButton_Click(object sender, RoutedEventArgs e)
    {
      var newCountingMetric = (MetricComboBox.SelectedItem as PredefinedMetricAndGroupOfMetrics)?.Metric;

      MetricComboBox.SelectedIndex = -1;

      foreach (
          var item in CountingSettings.countingSettingsForm.CountingMetricList.Select(
              (value, i) => new { i, value }
          )
      )
      {
        if (item.value != newCountingMetric)
        {
          continue;
        }

        CountingSettings.countingSettingsForm.CountingMetricList[item.i] =
            newCountingMetric;
        return;
      }

      CountingSettings.countingSettingsForm.CountingMetricList.Add(newCountingMetric);
      MetricComboBoxPlaceholder.Visibility = Visibility.Visible;
    }
    private void ResetEventComboBox()
    {
      var eventList = new List<PredefinedEvent>(
          WPerfOptions.Instance.WperfList.PredefinedEvents
      );
      EventComboBox.ItemsSource = eventList;
    }
    private void ResetMetricComboBox()
    {
      var metricList = new List<PredefinedMetricAndGroupOfMetrics>(
          WPerfOptions.Instance.WperfList.PredefinedMetricsAndGroupsOfMetrics
      );
      MetricComboBox.ItemsSource = metricList;
    }
    private void RemoveMetricButton_Click(object sender, RoutedEventArgs e)
    {
      int selectedIndex = CountingMetricListBox.SelectedIndex;
      if (selectedIndex < 0)
      {
        return;
      }
      CountingSettings.countingSettingsForm.CountingMetricList.RemoveAt(selectedIndex);

      CountingMetricListBox.Items.Refresh();
      CountingMetricListBox.SelectedIndex = Math.Min(
          selectedIndex,
          CountingMetricListBox.Items.Count - 1
      );
    }

    private List<string> GenerateWPACommand(List<string> args)
    {
      List<string> _args = [];
      if (
          !WPerfOptions.Instance.UseDefaultSearchDirectory
          && !string.IsNullOrEmpty(WPerfOptions.Instance.WPAPluginSearchDirectory)
      )
      {
        _args.Add($"-addsearchdir {WPerfOptions.Instance.WPAPluginSearchDirectory}");
      }

      _args.AddRange(args);
      return _args;
    }

    volatile bool checkingWPAInstallation = false;

    private async Task<bool> CheckWPAInstallationAsync()
    {
      if (checkingWPAInstallation)
      {
        return false;
      }

      try
      {
        checkingWPAInstallation = true;
        StringBuilder outputStringBuilder = new();
        var request = await Cli.Wrap("wpa")
            .WithArguments(GenerateWPACommand(["-listplugins"]))
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(outputStringBuilder))
            .ExecuteAsync();

        checkingWPAInstallation = false;
        string output = outputStringBuilder.ToString();

        if (request.ExitCode != 0)
        {
          await VS.MessageBox.ShowErrorAsync(ErrorLanguagePack.WPANotInstalled);
          return false;
        }

        bool isWPAPluginInstalled =
            !string.IsNullOrEmpty(output) && output.Contains("WindowsPerf WPA Plugin");
        if (!isWPAPluginInstalled)
        {
          await VS.MessageBox.ShowErrorAsync(ErrorLanguagePack.WPAPluginNotInstalled);
        }

        return isWPAPluginInstalled;
      }
      catch (Win32Exception error)
      {
        Trace.WriteLine($"[ERROR]: {error.Message}");
        checkingWPAInstallation = false;
        await VS.MessageBox.ShowErrorAsync(ErrorLanguagePack.WPANotInstalled);
        return false;
      }
    }

    private async void OpenInWPA_Click(object sender, RoutedEventArgs e)
    {
      OpenInWPAButton.IsEnabled = false;
      bool wpaInstalled = await CheckWPAInstallationAsync();

      if (!wpaInstalled)
      {
        OpenInWPAButton.IsEnabled = true;
        return;
      }
      string args = string.Join(
          " ",
          GenerateWPACommand([$"-i \"{WperfClient.OutputPath}.json\""])
      );
      Process process = new();
      ProcessStartInfo startInfo =
          new()
          {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "cmd.exe",
            Arguments = $"/C wpa {args}",
          };

      process.EnableRaisingEvents = true;
      process.StartInfo = startInfo;
      process.Start();
      process.Exited += new EventHandler(WPAProcessExited);

      OpenInWPAButton.IsEnabled = false;
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void BuildAndStartCounting_Click(object sender, RoutedEventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
    {
      if (string.IsNullOrEmpty(CountingSettings.countingSettingsForm.FilePath))
        return;
      Project project = await VS.Solutions.GetActiveProjectAsync();
      BuildAndStartCountingButton.IsEnabled = false;
      StartCountingButton.IsEnabled = false;
      bool buildSucceeded = await project.BuildAsync(BuildAction.Rebuild);
      BuildAndStartCountingButton.IsEnabled = true;
      StartCountingButton.IsEnabled = true;

      if (!buildSucceeded)
        return;

      StartCounting_Click(sender, e);
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private async void WPAProcessExited(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
    {
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
      OpenInWPAButton.IsEnabled = true;
    }

    private void DataGrid_OnAutoGeneratingColumn(
        object sender,
        DataGridAutoGeneratingColumnEventArgs e
    )
    {
      if (e.Column is not DataGridTextColumn textColumn)
      {
        return;
      }

      if (e.PropertyName == "Value")
      {
        var numericCellStyle = FindResource("NumericDataGridCellStyle") as Style;
        textColumn.ElementStyle = numericCellStyle;
      }
    }

    private void LoadFromJSONButton_Click(object sender, RoutedEventArgs e)
    {
      var fileDialog = new OpenFileDialog();
      fileDialog.DefaultExt = "json";
      fileDialog.Filter = "JSON files (*.json)|*.json";

      bool? result = fileDialog.ShowDialog();
      if (result != true)
        return;

      string filename = fileDialog.FileName;
      try
      {
        List<CountingEvent> wperfSampling = WperfClient.GetCountingEventsFromJSONFile(
            filename
        );
        CountingSettings.countingSettingsForm.IsCountCollected = true;
        CountingSettings.countingSettingsForm.CountingResult = wperfSampling;
      }
      catch (Exception err)
      {
        VS.MessageBox.ShowError("Error loading JSON file", err.Message);
      }
    }

    private void OpenSaveAsDialog(string extension)
    {
      SaveFileDialog SaveFileDialog =
          new()
          {
            DefaultExt = extension,
            FileName = WperfClient.OutputPath.Split('\\').Last(),
            Filter = $"{extension.ToUpper()} files (*.{extension})|*.{extension}",
          };
      if (SaveFileDialog.ShowDialog() != true)
      {
        return;
      }

      System.IO.File.Copy(WperfClient.OutputPath + $".{extension}", SaveFileDialog.FileName);
    }

    private void SaveAs_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(WperfClient.OutputPath))
      {
        return;
      }

      OpenSaveAsDialog("json");
      if (System.IO.File.Exists(WperfClient.OutputPath + ".csv"))
      {
        OpenSaveAsDialog("csv");
      }
    }

    private void EventComboBox_GotFocus(object sender, RoutedEventArgs e)
    {
      HideEventComboBoxPlaceholder();
    }

    private void MetricComboBox_GotFocus(object sender, RoutedEventArgs e)
    {
      HideMetricComboBoxPlaceholder();
    }
  }
}
