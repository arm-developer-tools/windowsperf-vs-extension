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

namespace WindowsPerfGUI.SDK.WperfOutputs
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class WperfSampling
    {
        [JsonProperty("sampling")]
        public SamplingSummary SamplingSummary { get; set; }
    }

    public partial class SamplingSummary
    {
        [JsonProperty("pe_file")]
        public string PeFile { get; set; }

        [JsonProperty("pdb_file")]
        public string PdbFile { get; set; }

        [JsonProperty("sample_display_row")]
        public ulong SampleDisplayRow { get; set; }

        [JsonProperty("samples_generated")]
        public ulong SamplesGenerated { get; set; }

        [JsonProperty("samples_dropped")]
        public ulong SamplesDropped { get; set; }

        [JsonProperty("base_address")]
        public ulong BaseAddress { get; set; }

        [JsonProperty("runtime_delta")]
        public ulong RuntimeDelta { get; set; }

        [JsonProperty("modules")]
        public List<Module> Modules { get; set; }

        [JsonProperty("modules_info")]
        public List<ModulesInfo> ModulesInfo { get; set; }

        [JsonProperty("events")]
        public List<SampledEvent> SampledEvents { get; set; }
    }

    public partial class SampledEvent
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("samples")]
        public List<SampleResult> SampleList { get; set; }

        [JsonProperty("interval")]
        public ulong Interval { get; set; }

        [JsonProperty("printed_sample_num")]
        public ulong PrintedSampleNum { get; set; }

        [JsonProperty("pcs")]
        public List<Pc> Pcs { get; set; }

        [JsonProperty("annotate")]
        public List<Annotate> Annotate { get; set; }
    }

    public partial class Annotate
    {
        [JsonProperty("function_name")]
        public string FunctionName { get; set; }

        [JsonProperty("source_code")]
        public List<SourceCode> SourceCode { get; set; }
    }

    public partial class SourceCode
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("line_number")]
        public ulong LineNumber { get; set; }

        [JsonProperty("hits")]
        public ulong Hits { get; set; }

        [JsonProperty("instruction_address")]
        public string InstructionAddress { get; set; }

        [JsonProperty("disassembled_line")]
        public DisassembledLine DisassembledLine { get; set; }
    }

    public partial class DisassembledLine
    {
        [JsonProperty("disassemble")]
        public List<Assembly> Assembly { get; set; }
    }

    public partial class Assembly
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("instruction")]
        public string Instruction { get; set; }
    }

    public partial class Pc
    {
        [JsonProperty("address")]
        public ulong Address { get; set; }

        [JsonProperty("count")]
        public ulong Count { get; set; }
    }

    public partial class SampleResult
    {
        [JsonProperty("overhead")]
        public double Overhead { get; set; }

        [JsonProperty("count")]
        public ulong Count { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }

    public partial class Module
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public ulong Address { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public partial class ModulesInfo
    {
        [JsonProperty("sections")]
        public List<ModuleSection> Sections { get; set; }

        [JsonProperty("pdb_file")]
        public string PdbFile { get; set; }

        [JsonProperty("pe_name")]
        public string PeName { get; set; }
    }

    public partial class ModuleSection
    {
        [JsonProperty("section")]
        public string SectionSection { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("virtual_size")]
        public long VirtualSize { get; set; }
    }

    public partial class WperfSampling
    {
        public static WperfSampling FromJson(string json) =>
            JsonConvert.DeserializeObject<WperfSampling>(json, JsonSettings.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this WperfSampling self)
        {
            return JsonConvert.SerializeObject(self, JsonSettings.Settings);
        }
    }
}
