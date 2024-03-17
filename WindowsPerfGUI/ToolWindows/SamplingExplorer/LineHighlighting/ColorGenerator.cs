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

using System.Windows.Media;
using Microsoft.VisualStudio.Imaging;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting
{
    public static class ColorGenerator
    {
        public const int DEFAULT_COLOR_RESOLUTION = 3;
        public const int MIN_COLOR_RESOLUTION = 3;
        public const int MAX_COLOR_RESOLUTION = 256;

        const int GREEN_HUE = 120,
            YELLOW_HUE = 60,
            RED_HUE = 0;

        /// <summary>
        /// Get a color from a hue, saturation and lightness
        /// </summary>
        /// <param name="hue">from 0 to 360</param>
        /// <param name="saturation">from 0 to 1</param>
        /// <param name="lightness">from 0 to 1</param>
        /// <returns><see cref="Color"/></returns>
        private static Color GetColorFromHSL(double hue, double saturation, double lightness)
        {
            var hslColor = new HslColor(hue, saturation, lightness);
            var rgbColor = hslColor.ToColor();
            return Color.FromArgb(80, rgbColor.R, rgbColor.G, rgbColor.B);
        }

        private static Color GetColorFromPercentage(double percentage, int colorResolution)
        {
            if (colorResolution < MIN_COLOR_RESOLUTION || colorResolution > MAX_COLOR_RESOLUTION)
                colorResolution = DEFAULT_COLOR_RESOLUTION ;

            double percentageChunk = 100.0 / (double)colorResolution;
            double hueChunk = (double)(GREEN_HUE - RED_HUE) / ((double)colorResolution - 1);
            int amountOfChunks = (int)Math.Min(Math.Floor(percentage / percentageChunk), colorResolution - 1);
            return GetColorFromHSL(GREEN_HUE - (hueChunk * amountOfChunks) + RED_HUE, 1, 0.5);
        }

        /// <summary>
        /// Takes a percentage from 0 to 100 and returns one of 3 colors red, yellow or green
        /// anything beyond 100 will return red
        /// </summary>
        /// <param name="percentage">values from 0 to 100</param>
        /// <returns></returns>
        public static Brush GenerateColor(double percentage, int colorResolution)
        {
            return new SolidColorBrush(GetColorFromPercentage(percentage, colorResolution));
        }
    }
}
