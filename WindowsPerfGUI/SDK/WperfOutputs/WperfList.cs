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
using Newtonsoft.Json;

namespace WindowsPerfGUI.SDK.WperfOutputs
{
    public partial class WperfList
    {
        [JsonProperty("Predefined_Events")]
        public List<PredefinedEvent> PredefinedEvents { get; set; }

        public (PredefinedEvent, int) GetPredefinedEventFromAliasName(string aliasName)
        {
            foreach (var predefinedEvent in PredefinedEvents.Select((value, i) => new { value, i }))
            {
                if (predefinedEvent.value.AliasName == aliasName)
                {
                    return (predefinedEvent.value, predefinedEvent.i);
                }
            }
            return (null, -1);
        }

        [JsonProperty("Predefined_Metrics")]
        public List<PredefinedMetric> PredefinedMetrics { get; set; }
    }

    public partial class PredefinedEvent
    {
        [JsonProperty("Alias_Name")]
        public string AliasName { get; set; }

        [JsonProperty("Raw_Index")]
        public string RawIndex { get; set; }

        [JsonProperty("Event_Type")]
        public string EventType { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{AliasName} | {Description}";
        }
    }

    public partial class PredefinedMetric
    {
        [JsonProperty("Metric")]
        public string Metric { get; set; }

        [JsonProperty("Events")]
        public string Events { get; set; }

        [JsonProperty("Formula")]
        public string Formula { get; set; }

        [JsonProperty("Unit")]
        public string Unit { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
        public override string ToString()
        {
            return $"{Metric} | {{{Events}}}";
        }
    }

    public partial class WperfList
    {
        public static WperfList FromJson(string json)
        {
            WperfList eventList = JsonConvert.DeserializeObject<WperfList>(
                json,
                JsonSettings.Settings
            );
            var sortedPredefinedEvents = new List<PredefinedEvent>();
            eventList.PredefinedEvents.Sort((a, b) => a.AliasName.CompareTo(b.AliasName));
            return eventList;
        }
    }
}
