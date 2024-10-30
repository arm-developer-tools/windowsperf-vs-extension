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
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
using WindowsPerfGUI.Options;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils.ListSearcher;

namespace WindowsPerfGUI.ToolWindows
{
  public partial class WperfManDialog : DialogWindow
  {
    readonly WperfClientFactory wperfClient = new();

    public WperfManDialog()
    {
      InitializeComponent();
      ResetEventComboBox();
    }

    private void HideEventComboBoxPlaceholder()
    {
      EventComboBoxPlaceholder.Visibility = Visibility.Hidden;
    }

    private void ResetEventComboBox()
    {
      var eventList = new List<PredefinedEvent>(
          WPerfOptions.Instance.WperfList.PredefinedEvents
      );
      EventComboBox.ItemsSource = eventList;
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

    private void EventComboBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      EventComboBox.IsDropDownOpen = true;
      HideEventComboBoxPlaceholder();
      if (!string.IsNullOrEmpty(EventComboBox.Text))
      {
        EventComboBox.ItemsSource = FilterEventList(EventComboBox.Text);
      }
      else
      {
        EventComboBoxPlaceholder.Visibility = Visibility.Visible;
        ResetEventComboBox();
      }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void EventComboBox_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e
    )
    {
      string selectedEvent = (EventComboBox.SelectedItem as PredefinedEvent)?.AliasName;
      if (string.IsNullOrEmpty(selectedEvent))
        return;

      ManOutput.Children.Clear();

      try
      {
        (WperfManOutput wperfManOutput, string stdErr) = wperfClient.GetManOutput(
            selectedEvent
        );

        if (!string.IsNullOrEmpty(stdErr))
        {
          VS.MessageBox.ShowError(stdErr);
          return;
        }
          ;

        foreach (var manResult in wperfManOutput.ManualResults)
        {
          if (manResult.Result.ToLower() == "n/a")
            continue;
          ManOutput.Children.Add(
              new TextBlock()
              {
                Text = manResult.FieldType,
                FontSize = 16,
                FontWeight = FontWeights.Bold
              }
          );
          ManOutput.Children.Add(
              new TextBlock()
              {
                Text = manResult.Result,
                Margin = new Thickness(0, 5, 0, 10),
                FontSize = 14,
              }
          );
        }
      }
      catch (Exception ex)
      {
        VS.MessageBox.ShowError(ex.Message);
      }
    }
  }
}
