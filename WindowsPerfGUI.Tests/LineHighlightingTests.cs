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
// SERVICES? LOSS OF USE, DATA, OR PROFITS? OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System.Threading;
using System.Windows.Media;
using NUnit.Framework;
using WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting;

namespace WindowsPerfGUI.Tests.ToolWindows.SamplingExplorer.LineHighlighting
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ColorGeneratorTests
    {
        [Test]
        [Description("Testing return values of the color generator")]
        [TestCase(0, 3, ExpectedResult = "00ff00")]
        [TestCase(50, 3, ExpectedResult = "feff00")]
        [TestCase(100, 3, ExpectedResult = "ff0000")]
        [TestCase(150, 3, ExpectedResult = "ff0000")] // Test beyond 100%
        public string GenerateColor_ReturnsExpectedColor_ForGivenPercentage(
            double percentage,
            int colorResolution
        )
        {
            var brush =
                ColorGenerator.GenerateColor(percentage, colorResolution) as SolidColorBrush;
            Assert.That(brush, Is.Not.Null);
            var color = brush.Color;
            return $"{color.R:x2}{color.G:x2}{color.B:x2}";
        }

        [Test]
        [Description("Testing out of range color resolution values")]
        [TestCase(-1, ExpectedResult = true)]
        [TestCase(257, ExpectedResult = true)]
        [TestCase(3, ExpectedResult = true)]
        public bool GenerateColor_UsesDefaultResolution_WhenOutOfRange(int colorResolution)
        {
            var defaultBrush =
                ColorGenerator.GenerateColor(50, ColorGenerator.DEFAULT_COLOR_RESOLUTION)
                as SolidColorBrush;
            var testBrush = ColorGenerator.GenerateColor(50, colorResolution) as SolidColorBrush;

            Assert.That(defaultBrush, Is.Not.Null);
            Assert.That(testBrush, Is.Not.Null);

            // Comparing colors to check if the resolution fell back to default
            return defaultBrush.Color.Equals(testBrush.Color);
        }
    }
}
