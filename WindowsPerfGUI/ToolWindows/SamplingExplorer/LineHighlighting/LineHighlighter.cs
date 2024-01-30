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

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsPerfGUI.Utils;

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
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen pen = new Pen(new SolidColorBrush(Colors.Transparent), 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="LineHighlighter"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public LineHighlighter(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            layer = view.GetAdornmentLayer("LineHighlighter");

            this.view = view;
            this.view.LayoutChanged += OnLayoutChanged;
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
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // Clears previous set adornmanets
            string filePath = view.TextBuffer.GetFileName();
            if (HighlighterDict.PreviousFilePaths.Count > 0 && HighlighterDict.PreviousFilePaths.Contains(filePath))
            {
                foreach (var line in HighlighterDict.PreviousFilesToHighlight[filePath].LinesToHighlight)
                {
                    ClearLineAdornements((int)line.LineNumber - 1);
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

            // Highlights the lines
            foreach (var line in HighlighterDict.FilesToHighlight[filePath].LinesToHighlight)
            {
                Brush brush = ColorGenerator.GenerateColor(line.Overhead);

                HighlightLine((int)line.LineNumber - 1, brush);
            }
        }

        private SnapshotSpan? GetSpanFromLineNumber(int lineNumber)
        {
            ITextSnapshotLine lineToHighlight;
            // This is to handle out of range line numbers 
            try
            {
                lineToHighlight = view.TextSnapshot.GetLineFromLineNumber(lineNumber);
            }
            catch (ArgumentOutOfRangeException) { return null; }

            if (lineToHighlight == null) { return null; }

            SnapshotSpan span = new SnapshotSpan(view.TextSnapshot, Span.FromBounds(lineToHighlight.Start, lineToHighlight.EndIncludingLineBreak));
            return span;
        }

        public void ClearLineAdornements(int lineNumber)
        {
            SnapshotSpan? span = GetSpanFromLineNumber(lineNumber);

            if (!span.HasValue) { return; }
            layer.RemoveAdornmentsByVisualSpan(span.Value);

        }

        public void ClearHighlighter()
        {
            layer.RemoveAllAdornments();
        }

        /// <summary>
        /// Highlights a line number with a given color
        /// </summary>
        /// <param name="lineNumber">Line to add the adornments</param>
        /// <param name="_brush">The color to use for highlighting</param>
        public void HighlightLine(int lineNumber, Brush _brush)
        {
            SnapshotSpan? span = GetSpanFromLineNumber(lineNumber);
            if (!span.HasValue) { return; }

            IWpfTextViewLineCollection textViewLines = view.TextViewLines;


            Geometry geometry = textViewLines.GetMarkerGeometry(span.Value);
            if (geometry == null) { return; }

            var drawing = new GeometryDrawing(_brush, pen, geometry);
            drawing.Freeze();

            var drawingImage = new DrawingImage(drawing);
            drawingImage.Freeze();

            var image = new Image
            {
                Source = drawingImage,
            };

            Canvas.SetLeft(image, geometry.Bounds.Left);
            Canvas.SetTop(image, geometry.Bounds.Top);
            layer.RemoveAdornmentsByVisualSpan(span.Value);
            layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span.Value, null, image, null);
        }
    }
}
