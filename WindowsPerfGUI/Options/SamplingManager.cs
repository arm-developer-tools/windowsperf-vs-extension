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

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WindowsPerfGUI.Options
{
    internal partial class SamplingManagerOptionsProvider
    {
        [ComVisible(true)]
        public class SamplingManagerOptions : BaseOptionPage<SamplingManager> { }
    }

    public class SamplingManager : BaseOptionModel<SamplingManager>
    {
        [Category("Code Annotations")]
        [DisplayName("Highlighter Color Resolution")]
        [Description(
            "The resolution of the colors to use in the line highlighter. Please chose a value between 3 and 256, defaulting to 3 otherwise."
        )]
        [DefaultValue(3)]
        public int HighlighterColorResolution { get; set; } = 3;

        [Category("Syntax Highlighting")]
        [DisplayName("Disassembly Syntax Highlighting")]
        [Description("Enables or disables disassembly syntax highlighting")]
        public bool EnableDisassemblySyntaxHighlighting { get; set; } = true;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Mnemonic Color")]
        [Description("Sets the color of mnemonics")]
        public Color MnemonicColor { get; set; } = Color.MediumPurple;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Memory Address Color")]
        [Description("Sets the color of memory addresses")]
        public Color AddressColor { get; set; } = Color.LimeGreen;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Comment Color")]
        [Description("Sets the color of inline comments")]
        public Color CommentColor { get; set; } = Color.Gray;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Update Modifier Color")]
        [Description("Sets the color of the \"!\" modifier")]
        public Color UpdateModifierColor { get; set; } = Color.Yellow;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Symbolic Notation Color")]
        [Description("Sets the color of items enclosed in angle brackets.")]
        public Color SymbolicNotationColor { get; set; } = Color.MediumVioletRed;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Argument Separator Color")]
        [Description("Sets the color of the \",\" separator")]
        public Color SeparatorColor { get; set; } = Color.White;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Memory Adressing and Register List Color")]
        [Description("Sets the color of items enclosed in either \"{}\" or \"[]\"")]
        public Color GroupColor { get; set; } = Color.LightGreen;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Immediate Value Color")]
        [Description("Sets the color of immediate values (eg: #0x01)")]
        public Color ImmediateValueColor { get; set; } = Color.SkyBlue;

        [Category("Disassembly Syntax Highlighting Colors")]
        [DisplayName("Register Color")]
        [Description("Sets the color of registers.")]
        public Color RegisterColor { get; set; } = Color.LightPink;

        [Category("Syntax Highlighting")]
        [DisplayName("Annotated Symbols Syntax Highlighting")]
        [Description("Enables or disables annotated symbols syntax highlighting")]
        public bool EnableAnnotatedSymbolsSyntaxHighlighting { get; set; } = true;

        [Category("Annotated Symbols Highlighting Colors")]
        [DisplayName("Keyword Color")]
        [Description("Sets the color of keywords.")]
        public Color KeywordColor { get; set; } = Color.SkyBlue;

        [Category("Annotated Symbols Highlighting Colors")]
        [DisplayName("Symbol Color")]
        [Description("Sets the color of symbols.")]
        public Color SymbolColor { get; set; } = Color.Yellow;

        [Category("Annotated Symbols Highlighting Colors")]
        [DisplayName("Module Color")]
        [Description("Sets the color of modules.")]
        public Color ModuleColor { get; set; } = Color.LightPink;
    }
}
