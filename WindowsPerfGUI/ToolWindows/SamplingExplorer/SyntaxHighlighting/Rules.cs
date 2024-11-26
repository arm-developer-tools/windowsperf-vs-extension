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

using WindowsPerfGUI.Options;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer.SyntaxHighlighting
{
    public struct Rule
    {
        public string pattern;
        public System.Drawing.Color color;

        public Rule(string pattern, System.Drawing.Color color)
        {
            this.pattern = pattern;
            this.color = color;
        }
    }

    static class Rules
    {
        public static Rule[] GetCPPRules()
        {
            var keywordPattern =
                @"\b(int|char|double|float|void|class|struct|public|private|protected|const|unsigned|signed|static|virtual|inline|explicit)\b";
            var keywordColor = SamplingManager.Instance.KeywordColor;

            var symbolsPattern = @"\(+|\)+|\*+";
            var symbolColor = SamplingManager.Instance.SymbolColor;

            var modulesPattern = @":\w+\.\w+\b";
            var moduleColor = SamplingManager.Instance.ModuleColor;

            return
            [
                new Rule(keywordPattern, keywordColor),
                new Rule(symbolsPattern, symbolColor),
                new Rule(modulesPattern, moduleColor),
            ];
        }

        public static Rule[] GetARM64AssemblyRules()
        {
            var symbolicPattern = @"<.+>";
            var symbolicColor = SamplingManager.Instance.SymbolicNotationColor;

            var commentsPattern = @"(\/\/|%%|;|@).+";
            var commentsColor = SamplingManager.Instance.CommentColor;

            var updatePattern = @"!";
            var updateColor = SamplingManager.Instance.UpdateModifierColor;

            var separatorPattern = @",";
            var separatorColor = SamplingManager.Instance.SeparatorColor;

            var groupPattern = @"(\[|]|{|})";
            var groupColor = SamplingManager.Instance.GroupColor;

            var immediateValuePattern = @"#\w+\b";
            var immediateValueColor = SamplingManager.Instance.ImmediateValueColor;

            return
            [
                new Rule(symbolicPattern, symbolicColor),
                new Rule(groupPattern, groupColor),
                new Rule(updatePattern, updateColor),
                new Rule(separatorPattern, separatorColor),
                new Rule(immediateValuePattern, immediateValueColor),
                new Rule(commentsPattern, commentsColor),
            ];
        }
    }
}
