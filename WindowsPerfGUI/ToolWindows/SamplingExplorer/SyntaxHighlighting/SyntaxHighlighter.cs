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
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer.SyntaxHighlighting
{
    public class SyntaxHighlighter
    {
        private static SolidColorBrush DrawingcolorToSolidBrush(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        public static List<Inline> HighlightCode(
            string codeLine,
            Rule[] rules,
            System.Drawing.Color defaultColor,
#pragma warning disable CS8632
            Action<Run>? configureHighlightedRun = null
#pragma warning restore CS8632
        )
        {
            List<Inline> inlines = [];
            SolidColorBrush defaultColorBrush = DrawingcolorToSolidBrush(defaultColor);

            int lastIndex = 0;
            foreach (
                Match match in Regex.Matches(
                    codeLine,
                    $"{string.Join("|", rules.Select((rule) => rule.pattern))}"
                )
            )
            {
                if (match.Index > lastIndex)
                {
                    inlines.Add(
                        new Run(codeLine.Substring(lastIndex, match.Index - lastIndex))
                        {
                            Foreground = defaultColorBrush
                        }
                    );
                }

                System.Drawing.Color tokenColor = defaultColor;

                foreach (var rule in rules)
                {
                    if (Regex.IsMatch(match.Value, rule.pattern))
                        tokenColor = rule.color;
                }

                Run runToAdd =
                    new(match.Value) { Foreground = DrawingcolorToSolidBrush(tokenColor) };
                configureHighlightedRun?.Invoke(runToAdd);
                inlines.Add(runToAdd);
                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < codeLine.Length)
            {
                inlines.Add(
                    new Run(codeLine.Substring(lastIndex)) { Foreground = defaultColorBrush }
                );
            }

            return inlines;
        }
    }
}
