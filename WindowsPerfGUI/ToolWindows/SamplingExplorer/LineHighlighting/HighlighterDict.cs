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

using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting
{
    internal class LineToHighlight
    {
        public long LineNumber = 0;
        public double Overhead = 0;
        public string EventName = string.Empty;
        public string Frequency = string.Empty;
        public ulong Hits = 0;
    }

    internal class FileToHighlight
    {
        public readonly List<LineToHighlight> LinesToHighlight = new();
    }

    static class HighlighterDict
    {
        // This will hold the highlighted lines from the previous sampling
        // This is needed to clear the previous adornments before mounting the new ones
        public static HashSet<string> PreviousFilePaths = new();
        public static Dictionary<string, FileToHighlight> PreviousFilesToHighlight = new();

        public static readonly HashSet<string> FilePaths = new();

        public static Dictionary<string, FileToHighlight> FilesToHighlight { get; } =
            new Dictionary<string, FileToHighlight>();

        public static void AddFileToHighlight(
            SamplingSection samplingSection,
            bool useAbsoluteOverhead = false
        )
        {
            if (
                samplingSection.LineNumber == null
                || samplingSection.Overhead == null
                || samplingSection.Overhead == 0
            )
                return;
            FilePaths.Add(samplingSection.Name.ToLower());

            FilesToHighlight.TryGetValue(samplingSection.Name.ToLower(), out FileToHighlight fileToHighlight);

            if (fileToHighlight == null)
            {
                fileToHighlight = new FileToHighlight();
            }

            fileToHighlight.LinesToHighlight.Add(
                new LineToHighlight()
                {
                    LineNumber = (long)samplingSection.LineNumber,
                    Overhead = useAbsoluteOverhead
                        ? (double)samplingSection.AbsoluteOverhead
                        : (double)samplingSection.Overhead,
                    EventName = samplingSection.Parent.Parent.Name,
                    Frequency = samplingSection.Parent.Parent.Frequency,
                    Hits = (ulong)samplingSection.Hits
                }
            );

            FilesToHighlight[samplingSection.Name.ToLower()] = fileToHighlight;
            StartHighlightingAsync().FireAndForget();
        }

        public static async Task StartHighlightingAsync()
        {
            DocumentView activeDocument;
            try
            {
                activeDocument = await VS.Documents.GetActiveDocumentViewAsync();
            }
            catch (Exception)
            {
                return;
            }
            IWpfTextView _view = activeDocument?.TextView;
            IAdornmentLayer _layer = _view.GetAdornmentLayer("LineHighlighter");
            LineHighlighter.RefreshTextHighlights(_view, _layer, 3);
        }

        public static void Clear()
        {
            if (FilePaths.Count == 0)
            {
                FilePaths.Clear();
                FilesToHighlight.Clear();
                return;
            }

            PreviousFilePaths = new HashSet<string>(FilePaths);
            PreviousFilesToHighlight = new Dictionary<string, FileToHighlight>(FilesToHighlight);

            FilePaths.Clear();
            FilesToHighlight.Clear();
        }
    }
}
