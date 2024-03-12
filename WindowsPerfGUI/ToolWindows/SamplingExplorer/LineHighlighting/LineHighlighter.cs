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

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsPerfGUI.Options;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting
{
    /// <summary>
    /// LineHighlighter places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class LineHighlighter
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer _layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView _view;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private int _colorResolution;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen _pen = new Pen(new SolidColorBrush(Colors.Transparent), 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="LineHighlighter"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public LineHighlighter(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            _layer = view.GetAdornmentLayer("LineHighlighter");
            this._colorResolution = SamplingManager.Instance.HighlighterColorResolution;

            this._view = view;
            this._view.LayoutChanged += OnLayoutChanged;
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // Clears previous set adornments
            string filePath = _view.TextBuffer.GetFileName();
            // Highlights the lines
            if (filePath == null)
            {
                return;
            }
            if (HighlighterDict.PreviousFilePaths.Count > 0 && HighlighterDict.PreviousFilePaths.Contains(filePath))
            {
                foreach (var line in HighlighterDict.PreviousFilesToHighlight[filePath].LinesToHighlight)
                {
                    ClearLineAdornments((int)line.LineNumber - 1);
                }
            }
            // Clears everything if there are no files to highlight
            if (HighlighterDict.FilePaths.Count == 0)
            {
                ClearHighlighter();
                return;
            }

            // If the open file is not in the list of files to highlight, do nothing
            if (!HighlighterDict.FilePaths.Contains(filePath))
            {
                return;
            }

            {
                foreach (var line in HighlighterDict.FilesToHighlight[filePath].LinesToHighlight)
                {
                    GetHighlightData(filePath, line.LineNumber, out string text, out Brush brush);

                    _ = HighlightLineAsync((int)line.LineNumber - 1, brush, text);
                }
            }
        }

        private void GetHighlightData(string filePath, long lineNumber, out string text, out Brush brush)
        {
            List<LineToHighlight> lines = HighlighterDict.FilesToHighlight[filePath].LinesToHighlight.Where(el => el.LineNumber == lineNumber).ToList();
            double overhead = lines.Average(el => el.Overhead);
            text = string.Join(", ", lines.Select(GetHighlightText).ToArray());
            brush = ColorGenerator.GenerateColor(overhead, this._colorResolution);
            if (lines.Count > 1)
            {
                text = $"{Math.Round(overhead, 2)}% ({text})";
            }
            text = $"// {text}";
        }

        private string GetHighlightText(LineToHighlight line)
        {
            return $"{Math.Round(line.Overhead, 2)}% with {line.Hits} {(line.Hits > 1 ? "hits" : "hit")} ({line.EventName}:{line.Frequency})";
        }



        private SnapshotSpan? GetSpanFromLineNumber(int lineNumber)
        {
            ITextSnapshotLine lineToHighlight;
            // This is to handle out of range line numbers 
            try
            {
                lineToHighlight = _view.TextSnapshot.GetLineFromLineNumber(lineNumber);
            }
            catch (ArgumentOutOfRangeException) { return null; }

            if (lineToHighlight == null) { return null; }

            SnapshotSpan span = new(_view.TextSnapshot, Span.FromBounds(lineToHighlight.Start, lineToHighlight.EndIncludingLineBreak));
            return span;
        }

        public void ClearLineAdornments(int lineNumber)
        {
            SnapshotSpan? span = GetSpanFromLineNumber(lineNumber);

            if (!span.HasValue) { return; }
            _layer.RemoveAdornmentsByVisualSpan(span.Value);

        }

        public void ClearHighlighter()
        {
            _layer.RemoveAllAdornments();
        }


        /// <summary>
        /// Highlights a line number with a given color
        /// </summary>
        /// <param name="lineNumber">Line to add the adornments</param>
        /// <param name="brush">The color to use for highlighting</param>
        /// <param name="highlightText">Text to be displayed</param>
        public async Task HighlightLineAsync(int lineNumber, Brush brush, string highlightText)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            SnapshotSpan? span = GetSpanFromLineNumber(lineNumber);
            if (!span.HasValue) { return; }

            IWpfTextViewLineCollection textViewLines = _view.TextViewLines;


            Geometry geometry = textViewLines.GetMarkerGeometry(span.Value);
            if (geometry == null) { return; }

            var drawing = new GeometryDrawing(brush, _pen, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();

            var image = new Image
            {
                Source = drawingImage,
            };

            Canvas.SetLeft(image, geometry.Bounds.Left);
            Canvas.SetTop(image, geometry.Bounds.Top);
            _layer.RemoveAdornmentsByVisualSpan(span.Value);
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span.Value, null, image, null);

            var lineToHighlight = _view.TextSnapshot.GetLineFromLineNumber(lineNumber);

            DTE2 _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            Properties propertiesList = _dte.get_Properties("FontsAndColors", "TextEditor");
            var fontSize = (short)propertiesList.Item("FontSize").Value;
            var fontFamily = (string)propertiesList.Item("FontFamily").Value;

            var textBlock = new TextBlock
            {
                Foreground = brush,
                FontFamily = new FontFamily(fontFamily),
                FontSize = fontSize,
                Text = highlightText
            };
            Canvas.SetLeft(textBlock, geometry.Bounds.Right + 5);
            Canvas.SetTop(textBlock, geometry.Bounds.Top + Math.Abs(fontSize - geometry.Bounds.Height) * .5);
            _layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, lineToHighlight.Extent, null, textBlock, null);
        }
    }
}
