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
using WindowsPerfGUI.Utils.ListSearcher;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace WindowsPerfGUI.Tests.Utils.ListSearcher
{
    public class ListSearcherTests
    {
        [Test()]
        [Description("Tests the response type of the Search results")]
        [TestCase("fire", ExpectedResult = 1)]
        [TestCase("fox", ExpectedResult = 1)]
        [TestCase("Firefox", ExpectedResult = 1)]
        [TestCase("FIREFOX", ExpectedResult = 1)]
        [TestCase("firefox", ExpectedResult = 1)]
        [TestCase("fifox", ExpectedResult = 0)]
        [TestCase("o", ExpectedResult = 2)]
        [TestCase("", ExpectedResult = 3)]
        public int BasicTest(string searchString)
        {
            var fields = new List<SearchList>();
            fields.Add(new SearchList { Name = "Name", Application = "Firefox" });
            fields.Add(new SearchList { Name = "Other", Application = "Chrome" });
            fields.Add(new SearchList { Name = "Yet another", Application = "Discrpd" });

            var sut = new ListSearcher<SearchList>(fields, new SearchOptions<SearchList> { IsCaseSensitve=false, GetValue = x => x.Application });


            var result = sut.Search(searchString);
            TestContext.WriteLine(result);
            TestContext.WriteLine(searchString);
            return result.Count;
        }
    }
    public class SearchList
    {
        public string Name { get; set; }
        public string Application { get; set; }
    }
}
