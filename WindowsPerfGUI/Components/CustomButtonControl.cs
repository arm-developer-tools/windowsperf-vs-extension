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

using System.Windows;
using System.Windows.Controls;

namespace WindowsPerfGUI.Components
{
  /// <summary>
  /// Custom Button control is a styled button that inherets from the native `System.Windows.Controls.Button` class.
  /// </summary>
  /// <example>
  ///     Include the Components namespace in your XAML file
  ///     <code>
  ///         xmlns:Components="clr-namespace:WindowsPerfGUI.Components"
  ///     </code>
  ///     Then use the control in your page
  ///     <code>
  ///         <Components:CustomButtonControl Grid.Row="4"
  ///                                         Margin="0,10,0,10"
  ///                                         Padding="20,5,20,5"
  ///                                         HorizontalAlignment="Right"
  ///                                         Click="SaveButton_Click"
  ///                                         Content="Save" />
  ///     </code>
  /// </example>
  public class CustomButtonControl : Button
  {
    static CustomButtonControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(
          typeof(CustomButtonControl),
          new FrameworkPropertyMetadata(typeof(CustomButtonControl))
      );
    }
  }
}
