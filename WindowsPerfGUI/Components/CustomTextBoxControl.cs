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

using System.Windows;
using System.Windows.Controls;

namespace WindowsPerfGUI.Components
{
    public class CustomTextBoxControl : TextBox
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            nameof(Placeholder),
            typeof(string),
            typeof(CustomTextBoxControl),
            new PropertyMetadata(string.Empty)
        );

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(
            nameof(IsEmpty),
            typeof(bool),
            typeof(CustomTextBoxControl),
            new PropertyMetadata(false)
        );

        public bool IsEmpty
        {
            get { return (bool)GetValue(IsEmptyProperty); }
            private set { SetValue(IsEmptyProperty, value); }
        }

        static CustomTextBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CustomTextBoxControl),
                new FrameworkPropertyMetadata(typeof(CustomTextBoxControl))
            );
        }

        protected override void OnInitialized(EventArgs e)
        {
            UpdateIsEmpty();
            this.TextWrapping = TextWrapping.Wrap;
            base.OnInitialized(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            UpdateIsEmpty();
            base.OnTextChanged(e);
        }

        private void UpdateIsEmpty()
        {
            IsEmpty = string.IsNullOrEmpty(Text);
        }
    }
}
