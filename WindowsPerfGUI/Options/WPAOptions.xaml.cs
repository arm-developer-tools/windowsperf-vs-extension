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

using System.Windows.Controls;
using System.Windows.Forms;
using WindowsPerfGUI.Utils;
using UserControl = System.Windows.Controls.UserControl;

namespace WindowsPerfGUI.Options
{
  /// <summary>
  /// Interaction logic for WPAOptions.xaml
  /// </summary>
  public partial class WPAOptions : UserControl
  {
    public WPAOptions()
    {
      InitializeComponent();
    }

    internal WPAOptionsPage wpaOptionsPage;

    public void Initialize()
    {
      string checkboxLabel = "Use default search directory";
      if (!string.IsNullOrEmpty(WperfDefaults.DefaultWPASearchDir))
      {
        checkboxLabel += $": {WperfDefaults.DefaultWPASearchDir}";
      }
      else
      {
        checkboxLabel += ": Environment variable is not set";
      }

      UseDefaultSearchLocation.Content = checkboxLabel;

      if (!string.IsNullOrEmpty(WperfDefaults.DefaultWPASearchDir))
      {
        EnvironmentVariableNotice.Text =
            $"WPA_ADDITIONAL_SEARCH_DIRECTORIES=\"{WperfDefaults.DefaultWPASearchDir}\"";
      }
      UseDefaultSearchLocation.IsChecked = WPerfOptions.Instance.UseDefaultSearchDirectory;
      CustomSearchDir.IsEnabled = !WPerfOptions.Instance.UseDefaultSearchDirectory;
      SelectDirectoryButton.IsEnabled = !WPerfOptions.Instance.UseDefaultSearchDirectory;
      CustomSearchDir.Text = WPerfOptions.Instance.WPAPluginSearchDirectory;
    }

    private void UseDefaultSearchLocation_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      bool newValue = !WPerfOptions.Instance.UseDefaultSearchDirectory;
      CustomSearchDir.IsEnabled = !newValue;
      SelectDirectoryButton.IsEnabled = !newValue;
      UseDefaultSearchLocation.IsChecked = newValue;
      if (newValue)
      {
        CustomSearchDir.Text = WperfDefaults.DefaultWPASearchDir;
      }
      WPerfOptions.Instance.UseDefaultSearchDirectory = newValue;
      WPerfOptions.Instance.Save();
    }

    private void CustomSearchDir_TextChanged(object sender, TextChangedEventArgs e)
    {
      WPerfOptions.Instance.WPAPluginSearchDirectory = CustomSearchDir.Text;
      WPerfOptions.Instance.Save();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      using var fbd = new FolderBrowserDialog()
      {
        SelectedPath = WPerfOptions.Instance.WPAPluginSearchDirectory,
      };
      DialogResult result = fbd.ShowDialog();

      if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
      {
        CustomSearchDir.Text = fbd.SelectedPath;
      }
    }
  }
}
