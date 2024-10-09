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

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using System.Windows;
using System.Windows.Controls;

namespace WindowsPerfGUI.Components
{
    /// <summary>
    /// Like the button control, the MonikerButton control is a styled button that inherets from the native `System.Windows.Controls.Button` class. 
    /// The only difference is that it hosts an image (from the KnownMonikers) instead of text.
    /// </summary>
    /// <example>
    /// Include the Components and the ImageCataloge namespaces in your XAML file
    ///     <code>
    ///         xmlns:Components="clr-namespace:WindowsPerfGUI.Components"
    ///         xmlns:catalog="clr-namespace:Microsoft.VisualStudio.Imaging;assembly=Microsoft.VisualStudio.ImageCatalog"
    ///     </code>
    /// Then use the control in your page
    ///     <code>
    ///         <Components:CustomMonikerButtonControl
    ///                x:Name="StopSamplingMonikerButton"
    ///                Margin="0,4,1,4"
    ///                Click="StopSamplingMonikerButton_Click"
    ///                MonikerName="{x:Static catalog:KnownMonikers.Stop}" />
    ///     </code>
    /// </example>
    public class CustomMonikerButtonControl : Button
    {
        /// <summary>
        /// This is the MonikerName property that is used to set the image of the button.
        /// <seealso cref="Microsoft.VisualStudio.Imaging.KnownMonikers"/>
        /// </summary>
        public ImageMoniker MonikerName
        {
            get { return (ImageMoniker)GetValue(MonikerNameProperty); }
            set { SetValue(MonikerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MonikerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonikerNameProperty = DependencyProperty.Register(
            nameof(MonikerName),
            typeof(ImageMoniker),
            typeof(CustomMonikerButtonControl),
            new PropertyMetadata(KnownMonikers.Play)
        );

        static CustomMonikerButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CustomMonikerButtonControl),
                new FrameworkPropertyMetadata(typeof(CustomMonikerButtonControl))
            );
        }

        private readonly CrispImage _monikerImage = new();

        protected override void OnInitialized(EventArgs e)
        {
            _monikerImage.Moniker = MonikerName;
            _monikerImage.Width = 16;
            _monikerImage.Height = 16;
            _monikerImage.Grayscale = !this.IsEnabled;
            Content = _monikerImage;
            base.OnInitialized(e);
            this.IsEnabledChanged += ToggleGrayScaleMoniker;
        }

        private void ToggleGrayScaleMoniker(object sender, DependencyPropertyChangedEventArgs e)
        {
            _monikerImage.Grayscale = !this.IsEnabled;
        }
    }
}
