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

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace WindowsPerfGUI.Components
{
    public class FilterableComboBox : CustomComboBoxControl
    {
        /// <summary>
        /// If true, on lost focus or enter key pressed, checks the text in the combobox. If the text is not present
        /// in the list, it leaves it blank.
        /// </summary>
        public bool OnlyValuesInList
        {
            get => (bool)GetValue(OnlyValuesInListProperty);
            set => SetValue(OnlyValuesInListProperty, value);
        }
        public static readonly DependencyProperty OnlyValuesInListProperty =
            DependencyProperty.Register(nameof(OnlyValuesInList), typeof(bool), typeof(FilterableComboBox));

        /// <summary>
        /// Selected item, changes only on lost focus or enter key pressed
        /// </summary>
        public object EffectivelySelectedItem
        {
            get => (bool)GetValue(EffectivelySelectedItemProperty);
            set => SetValue(EffectivelySelectedItemProperty, value);
        }
        public static readonly DependencyProperty EffectivelySelectedItemProperty =
            DependencyProperty.Register(nameof(EffectivelySelectedItem), typeof(object), typeof(FilterableComboBox));

        private string CurrentFilter = string.Empty;
        private bool TextBoxFreezed;
        protected TextBox EditableTextBox => GetTemplateChild("PART_EditableTextBox") as TextBox;
        private UserChange<bool> IsDropDownOpenUC;

        /// <summary>
        /// Triggers on lost focus or enter key pressed, if the selected item changed since the last time focus was lost or enter was pressed.
        /// </summary>
        public event Action<FilterableComboBox, object> SelectionEffectivelyChanged;

        public FilterableComboBox()
        {
            IsDropDownOpenUC = new UserChange<bool>(v => IsDropDownOpen = v);
            DropDownOpened += FilteredComboBox_DropDownOpened;

            IsEditable = true;
            IsTextSearchEnabled = true;
            StaysOpenOnEdit = true;
            IsReadOnly = false;

            Loaded += (s, e) =>
            {
                if (EditableTextBox != null)
                    new TextBoxBaseUserChangeTracker(EditableTextBox).UserTextChanged += FilteredComboBox_UserTextChange;
            };

            SelectionChanged += (_, _) => shouldTriggerSelectedItemChanged = true;

            SelectionEffectivelyChanged += (_, o) => EffectivelySelectedItem = o;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Down && !IsDropDownOpen)
            {
                IsDropDownOpen = true;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                ClearFilter();
                Text = "";
                IsDropDownOpen = true;
            }
            else if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                CheckSelectedItem();
                TriggerSelectedItemChanged();
            }
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewLostKeyboardFocus(e);
            CheckSelectedItem();
            if ((e.OldFocus == this || e.OldFocus == EditableTextBox) && e.NewFocus != this && e.NewFocus != EditableTextBox)
                TriggerSelectedItemChanged();
        }

        private void CheckSelectedItem()
        {
            if (OnlyValuesInList)
                Text = SelectedItem?.ToString() ?? "";
        }

        private bool shouldTriggerSelectedItemChanged = false;
        private void TriggerSelectedItemChanged()
        {
            if (shouldTriggerSelectedItemChanged)
            {
                SelectionEffectivelyChanged?.Invoke(this, SelectedItem);
                shouldTriggerSelectedItemChanged = false;
            }
        }

        public void ClearFilter()
        {
            if (string.IsNullOrEmpty(CurrentFilter)) return;
            CurrentFilter = "";
            CollectionViewSource.GetDefaultView(ItemsSource).Refresh();
        }

        private void FilteredComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (IsDropDownOpenUC.IsUserChange)
                ClearFilter();
        }

        private void FilteredComboBox_UserTextChange(object sender, EventArgs e)
        {
            if (TextBoxFreezed) return;
            var tb = EditableTextBox;
            if (tb.SelectionStart + tb.SelectionLength == tb.Text.Length)
                CurrentFilter = tb.Text.Substring(0, tb.SelectionStart).ToLower();
            else
                CurrentFilter = tb.Text.ToLower();
            RefreshFilter();
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += FilterItem;
            }

            if (oldValue != null)
            {
                var view = CollectionViewSource.GetDefaultView(oldValue);
                if (view != null) view.Filter -= FilterItem;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        private void RefreshFilter()
        {
            if (ItemsSource == null) return;

            var view = CollectionViewSource.GetDefaultView(ItemsSource);
            FreezTextBoxState(() =>
            {
                var isDropDownOpen = IsDropDownOpen;
                //always hide because showing it enables the user to pick with up and down keys, otherwise it's not working because of the glitch in view.Refresh()
                IsDropDownOpenUC.Set(false);
                view.Refresh();

                if (!string.IsNullOrEmpty(CurrentFilter) || isDropDownOpen)
                    IsDropDownOpenUC.Set(true);

                if (SelectedItem == null)
                {
                    foreach (var itm in ItemsSource)
                        if (itm.ToString() == Text)
                        {
                            SelectedItem = itm;
                            break;
                        }
                }
            });
        }

        private void FreezTextBoxState(Action action)
        {
            TextBoxFreezed = true;
            var tb = EditableTextBox;
            var text = Text;
            var selStart = tb.SelectionStart;
            var selLen = tb.SelectionLength;
            action();
            Text = text;
            tb.SelectionStart = selStart;
            tb.SelectionLength = selLen;
            TextBoxFreezed = false;
        }

        private bool FilterItem(object value)
        {
            if (value == null) return false;
            if (CurrentFilter.Length == 0) return true;

            return value.ToString().ToLower().Contains(CurrentFilter);
        }

        private class TextBoxBaseUserChangeTracker
        {
            private bool IsTextInput { get; set; }

            public TextBoxBase TextBoxBase { get; set; }
            private List<Key> PressedKeys = new List<Key>();
            public event EventHandler UserTextChanged;
            private string LastText;

            public TextBoxBaseUserChangeTracker(TextBoxBase textBoxBase)
            {
                TextBoxBase = textBoxBase;
                LastText = TextBoxBase.ToString();

                textBoxBase.PreviewTextInput += (s, e) =>
                {
                    IsTextInput = true;
                };

                textBoxBase.TextChanged += (s, e) =>
                {
                    var isUserChange = PressedKeys.Count > 0 || IsTextInput || LastText == TextBoxBase.ToString();
                    IsTextInput = false;
                    LastText = TextBoxBase.ToString();
                    if (isUserChange)
                        UserTextChanged?.Invoke(this, e);
                };

                textBoxBase.PreviewKeyDown += (s, e) =>
                {
                    switch (e.Key)
                    {
                        case Key.Back:
                        case Key.Space:
                            if (!PressedKeys.Contains(e.Key))
                                PressedKeys.Add(e.Key);
                            break;
                    }
                };

                textBoxBase.PreviewKeyUp += (s, e) =>
                {
                    if (PressedKeys.Contains(e.Key))
                        PressedKeys.Remove(e.Key);
                };

                textBoxBase.LostFocus += (s, e) =>
                {
                    PressedKeys.Clear();
                    IsTextInput = false;
                };
            }
        }

        private class UserChange<T>
        {
            private Action<T> action;

            public bool IsUserChange { get; private set; } = true;

            public UserChange(Action<T> action)
            {
                this.action = action;
            }

            public void Set(T val)
            {
                try
                {
                    IsUserChange = false;
                    action(val);
                }
                finally
                {
                    IsUserChange = true;
                }
            }
        }
    }
}
