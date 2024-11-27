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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.Tests.Parsers
{
    public class JsonParserTests
    {
        private string GetTestInput(string fileName)
        {
            string directory = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location
            );
            return File.ReadAllText(Path.Combine(directory, "Parsers", fileName));
        }

        [Test()]
        [Description("Tests JSON parsing for wperf count output")]
        public void ParseCount()
        {
            string countingString = GetTestInput("count_05.json");

            bool isWperfCounting = JsonTypeChecker.IsJsonType<WperfCounting>(countingString);
            Assert.That(isWperfCounting, Is.True);

            WperfCounting counting = WperfCounting.FromJson(countingString);

            Assert.That(counting.Core.PerformanceCounters.Length, Is.EqualTo(10));
            Assert.That(
                counting.Core.PerformanceCounters[0].PerformanceCounter.Length,
                Is.EqualTo(16)
            );
            Assert.That(counting.Core.TsMetric.TelemetrySolutionMetrics.Length, Is.EqualTo(70));
        }

        [Test()]
        [Description("Tests JSON parsing for wperf record output")]
        public void ParseRecord()
        {
            string samplingString = GetTestInput("sample_04.json");

            bool isWperfSampling = JsonTypeChecker.IsJsonType<WperfSampling>(samplingString);
            Assert.That(isWperfSampling, Is.True);

            WperfSampling sampling = WperfSampling.FromJson(samplingString);

            Assert.That(sampling.SamplingSummary.Modules.Count, Is.EqualTo(15));
            Assert.That(sampling.SamplingSummary.SampledEvents.Count, Is.EqualTo(1));
            Assert.That(sampling.SamplingSummary.SampledEvents[0].Type, Is.EqualTo("ld_spec"));
            Assert.That(sampling.SamplingSummary.SampledEvents[0].Annotate.Count, Is.EqualTo(26));
            Assert.That(
                sampling.SamplingSummary.SampledEvents[0].Annotate[0].SourceCode[0].LineNumber,
                Is.EqualTo(3591)
            );
        }

        public static IEnumerable<TestCaseData> TestSPEParseData
        {
            get
            {
                yield return new TestCaseData("spe_01.json", (0, 70d, (long)20));
                yield return new TestCaseData("spe_02.json", (6, 66.6667d, (long)39));
                yield return new TestCaseData("spe_03.json", (7, 40d, (long)29));
            }
        }

        [Test()]
        [Description("Tests JSON parsing for wperf spe output")]
        [TestCaseSource(nameof(TestSPEParseData))]
        public void ParseSPE(
            string input,
            (int annotateCount, double firstSampleOverhead, long thirdCounterValue) values
        )
        {
            string speString = GetTestInput(input);

            bool isWperfSPE = JsonTypeChecker.IsJsonType<WperfSPE>(speString);
            Assert.That(isWperfSPE, Is.True);

            WperfSPE spe = WperfSPE.FromJson(speString);

            Assert.That(
                spe.Sampling.SamplingSummary.SampledEvents[0].Annotate.Count,
                Is.EqualTo(values.annotateCount)
            );
            Assert.That(
                spe.Sampling.SamplingSummary.SampledEvents[0].SampleList[0].Overhead,
                Is.EqualTo(values.firstSampleOverhead)
            );
            Assert.That(
                spe.Counting.Core.PerformanceCounters[0].PerformanceCounter[2].CounterValue,
                Is.EqualTo(values.thirdCounterValue)
            );
        }
    }
}
