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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WindowsPerfGUI.ToolWindows.SamplingSetting;

namespace WindowsPerfGUI.Components
{
    /// <summary>
    /// Interaction logic for ButtonGrid.xaml
    /// </summary>
    public partial class ButtonGrid : UserControl
    {
        readonly UniformGrid grid = new() { Columns = 8 };

        public List<CpuCoreElement> ItemsSource
        {
            get { return (List<CpuCoreElement>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(List<CpuCoreElement>),
            typeof(ButtonGrid),
            new PropertyMetadata(null, OnItemsSourceChanged)
        );

        public SolidColorBrush SelectedColor
        {
            get { return (SolidColorBrush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor",
                typeof(SolidColorBrush),
                typeof(ButtonGrid),
                new PropertyMetadata(new SolidColorBrush(Colors.Green))
            );

        public ObservableCollection<CpuCoreElement> SelectedItems
        {
            get { return (ObservableCollection<CpuCoreElement>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(ObservableCollection<CpuCoreElement>),
                typeof(ButtonGrid),
                new PropertyMetadata(null, OnItemsSourceChanged)
            );

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
            "FilePath",
            typeof(string),
            typeof(ButtonGrid),
            new PropertyMetadata("", OnFilePathChanged)
        );

        public ButtonGrid()
        {
            InitializeComponent();
            AddChild(grid);
        }

        private static void OnItemsSourceChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            ButtonGrid buttonGrid = (ButtonGrid)d;
            buttonGrid.UpdateButtons();
        }

        private static void OnFilePathChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            ButtonGrid buttonGrid = (ButtonGrid)d;
            buttonGrid.UpdateSelection((string)e.NewValue);
        }

        private void UpdateSelection(string newValue)
        {
            if (SelectedItems == null)
                return;
            if (newValue != "" && SelectedItems.Count > 1)
            {
                var itemToKeep = SelectedItems.ToList().OrderBy(el => el.coreNumber).First();
                SelectedItems.Clear();
                SelectedItems.Add(itemToKeep);
            }
            this.UpdateButtons();
        }

        private void UpdateButtons()
        {
            grid.Children.Clear();
            if (ItemsSource == null)
                return;
            foreach (var cpu in ItemsSource)
            {
                var button = new CustomButtonControl { Content = cpu.coreNumber };
                if (SelectedItems != null && SelectedItems.Contains(cpu))
                {
                    button.Background = SelectedColor;
                }
                button.Click += (sender, e) => Button_Click(sender, e, cpu);
                grid.Children.Add(button);
            }
            if (ItemsSource.Count < 16)
            {
                grid.Columns = 4;
                return;
            }
            if (ItemsSource.Count < 32)
            {
                grid.Columns = 6;
                return;
            }
        }

        private void Button_Click(object sender, EventArgs e, CpuCoreElement cpu)
        {
            if (SelectedItems == null)
                return;
            bool isSelected = SelectedItems.Contains(cpu);
            if (isSelected)
            {
                SelectedItems.Remove(cpu);
            }
            else
            {
                if (FilePath != "")
                {
                    SelectedItems.Clear();
                }
                SelectedItems.Add(cpu);
            }
            UpdateButtons();
        }
    }
}
