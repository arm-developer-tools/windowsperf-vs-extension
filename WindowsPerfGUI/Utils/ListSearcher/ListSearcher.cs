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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsPerfGUI.Utils.ListSearcher
{
    public class ListSearcher<T>
    {
        private readonly List<T> _records;
        private readonly SearchOptions<T> _options;

#nullable enable
        public ListSearcher(IEnumerable<T> records, SearchOptions<T>? options = null)
        {
            _records = records.ToList();
            _options = options ?? new SearchOptions<T>();
        }

#nullable disable

        public List<T> Search(string searchText)
        {
            var results = new List<T>();
            if (!_options.IsCaseSensitve)
            {
                searchText = searchText.ToLower();
            }
            Parallel.ForEach(
                _records,
                record =>
                {
                    var recordValue =
                        _options.GetValue != null
                            ? _options.GetValue(record)
                            : record?.ToString() ?? "";
                    if (!_options.IsCaseSensitve)
                    {
                        recordValue = recordValue.ToLower();
                    }
                    if (recordValue.Contains(searchText))
                        results.Add(record);
                }
            );

            return results;
        }
    }
}

public class SearchOptions<T>
{
    public bool IsCaseSensitve { get; init; } = false;
#nullable enable
    public Func<T, string>? GetValue { get; set; }
#nullable disable
}
