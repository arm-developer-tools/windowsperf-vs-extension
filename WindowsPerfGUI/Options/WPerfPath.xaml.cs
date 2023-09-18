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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;

namespace WindowsPerfGUI.Options
{
    /// <summary>
    /// Interaction logic for WPerfOptions.xaml
    /// </summary>
    public partial class WPerfPath : UserControl
    {
        public WPerfPath()
        {
            InitializeComponent();
        }
        internal WPerfPathPage wPerfPathPage;
        public void Initialize()
        {
            PathInput.Text = WPerfOptions.Instance.WperfPath;
            if (WPerfOptions.Instance.IsWperfInitialized && WPerfOptions.Instance.WperfCurrentVersion != null && WPerfOptions.Instance.WperfList != null)
            {
                SetWperfVersion(WPerfOptions.Instance.WperfCurrentVersion);
                SetPredefinedEventsAndMetrics(WPerfOptions.Instance.WperfList);
            }
            WPerfOptions.Instance.Save();
        }

        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            WPerfOptions.Instance.WperfPath = PathInput.Text;
            WperfClientFactory wperf = new WperfClientFactory();
            try
            {
                (WperfVersion versions, string error) = wperf.GetVersion();
                if (error != "") throw new Exception(error);
                SetWperfVersion(versions, shouldForce: true);

                (WperfList wperfList, string errorWperfList) = wperf.GetEventList();
                if (errorWperfList != "") throw new Exception(errorWperfList);
                SetPredefinedEventsAndMetrics(wperfList, shouldForce: true);
            }
            catch (Exception ex)
            {
                VS.MessageBox.ShowError(ex.Message);
            }
        }

        const int OFFSET_ROW = 2;
        private void ClearMainStack()
        {
            MainStack.Children.RemoveRange(OFFSET_ROW + 1, MainStack.Children.Count - OFFSET_ROW);
        }
        private void SetWperfVersion(WperfVersion wperfVersion, bool shouldForce = false)
        {
            ClearMainStack();

            foreach (var item in wperfVersion.Components.Select((component, i) => new { i, component }))
            {
                int offset = OFFSET_ROW + item.i;

                string component = item.component.Component;
                string componentVersion = item.component.ComponentVersion;
                string gitVersion = item.component.GitVersion;

                MainStack.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                TextBlock componentLabel = TextBlockFactory(component, offset);
                TextBlock componentVersionLabel = TextBlockFactory($"{componentVersion} ({gitVersion})", offset, true);

                MainStack.Children.Add(componentLabel);
                MainStack.Children.Add(componentVersionLabel);
            }
            if (shouldForce)
            {
                WPerfOptions.Instance.IsWperfInitialized = true;
                WPerfOptions.Instance.WperfCurrentVersion = wperfVersion;
                WPerfOptions.Instance.Save();
            }
        }
        private void SetPredefinedEventsAndMetrics(WperfList wperfList, bool shouldForce = false)
        {
            PredefinedEventsDataGrid.ItemsSource = wperfList.PredefinedEvents;
            PredefinedMetricsDataGrid.ItemsSource = wperfList.PredefinedMetrics;
            if (shouldForce)
            {
                WPerfOptions.Instance.WperfList = wperfList;
                WPerfOptions.Instance.Save();
            }
        }
        public static TextBlock TextBlockFactory(string text, int offset, bool isRight = false)
        {
            TextBlock textBlock = new TextBlock() { Text = text, Margin = new Thickness(5, 5, 5, 5) };
            textBlock.SetValue(Grid.RowProperty, offset);
            if (isRight)
            {
                textBlock.SetValue(Grid.ColumnProperty, 1);
            }
            else
            {
                textBlock.SetValue(Grid.ColumnProperty, 0);
            }
            return textBlock;
        }
    }
}
