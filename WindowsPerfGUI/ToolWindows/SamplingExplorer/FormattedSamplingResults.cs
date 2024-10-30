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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WindowsPerfGUI.Components.TreeListView;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer
{
    public class FormattedSamplingResults : NotifyPropertyChangedImplementor, ITreeModel
    {
        private List<SamplingSection> rootSampledEvent;
        public List<SamplingSection> RootSampledEvent
        {
            get { return rootSampledEvent; }
            set
            {
                rootSampledEvent = value;
                OnPropertyChanged();
            }
        }

        public FormattedSamplingResults()
        {
            RootSampledEvent = new List<SamplingSection>();
        }

        public void ClearSampling()
        {
            HighlighterDict.Clear();
            RootSampledEvent = new List<SamplingSection>();
        }
        private ulong RecalculateOverhead(SamplingSection rootSamplingSection)
        {
            ulong hits = 0;
            foreach (SamplingSection samplingEvents in rootSamplingSection.Children)
            {
                hits += (ulong)samplingEvents.Hits;
            }

            return hits;
        }
        private ulong CalculateAllSamplesGenerated(WperfSampling rootSampling)
        {
            ulong hits = 0;
            foreach (SampledEvent sampledEvent in rootSampling.SamplingSummary.SampledEvents)
            {
                hits += (ulong)CalculateSampleHits(sampledEvent);
            }

            return hits;
        }
        double CalculateSampleHits(SampledEvent sampledEvent)
        {
            double allHits = 0;
            foreach (SampleResult sample in sampledEvent.SampleList)
            {
                allHits += sample.Count;
            }
            return allHits;
        }

        public void FormatSamplingResults(
            WperfSampling wperSamplingOutput,
            string rootName = "Root"
        )
        {
            var rootSample = new SamplingSection()
            {
                PdbFile = wperSamplingOutput.SamplingSummary.PdbFile,
                PeFile = wperSamplingOutput.SamplingSummary.PeFile,
                SamplesDropped = wperSamplingOutput.SamplingSummary.SamplesDropped,
                Modules = wperSamplingOutput.SamplingSummary.Modules,
                Hits = wperSamplingOutput.SamplingSummary.SamplesGenerated,
                Overhead = 100,
                AbsoluteOverhead = 100,
                Name = rootName,
                Layer = 0,
                Children = new List<SamplingSection>(),
                Parent = null,
                SectionType = SamplingSection.SamplingSectionType.ROOT,
                StringifiedJsonOutput = wperSamplingOutput.ToJson(),
            };
            RootSampledEvent.Add(rootSample);

            if (rootSample.Hits == 0)
                rootSample.Hits = CalculateAllSamplesGenerated(wperSamplingOutput);


            double allHits = (double)rootSample.Hits;

            foreach (SampledEvent sampledEvent in wperSamplingOutput.SamplingSummary.SampledEvents)
            {
                double eventHits = CalculateSampleHits(sampledEvent);

                SamplingSection eventSection = new SamplingSection()
                {
                    Name = sampledEvent.Type,
                    Children = new List<SamplingSection>(),
                    Hits = (ulong)eventHits,
                    Layer = 1,
                    SectionType = SamplingSection.SamplingSectionType.SAMPLE_EVENT,
                    Frequency = sampledEvent.Interval.ToString(),
                    Parent = rootSample,
                    Overhead = CalculatePercentage(eventHits, allHits),
                    AbsoluteOverhead = CalculatePercentage(eventHits, allHits),
                };

                rootSample.Children.Add(eventSection);
                foreach (SampleResult sample in sampledEvent.SampleList)
                {
                    SamplingSection samplesSection = new SamplingSection()
                    {
                        Children = new List<SamplingSection>(),
                        Name = sample.Symbol,
                        Hits = sample.Count,
                        Overhead = sample.Overhead,
                        AbsoluteOverhead = CalculatePercentage(sample.Count, allHits),
                        Layer = 2,
                        SectionType = SamplingSection.SamplingSectionType.SAMPLE_FUNCTION,
                        Parent = eventSection,
                    };
                    eventSection.Children.Add(samplesSection);
                    Annotate annotatedSample = sampledEvent.Annotate.Find(
                        (x) => x.FunctionName == sample.Symbol
                    );
                    if (annotatedSample == null)
                        continue;
                    foreach (SourceCode annotationSourceCode in annotatedSample.SourceCode)
                    {
                        SamplingSection annotationSection = new SamplingSection()
                        {
                            Name = annotationSourceCode.Filename,
                            Hits = annotationSourceCode.Hits,
                            Overhead = CalculatePercentage(
                                annotationSourceCode.Hits,
                                (double)samplesSection.Hits
                            ),
                            AbsoluteOverhead = CalculatePercentage(
                                annotationSourceCode.Hits,
                                allHits
                            ),
                            Layer = 3,
                            SectionType = SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE,
                            Parent = samplesSection,
                            LineNumber = annotationSourceCode.LineNumber,
                            IsFileExists = File.Exists(annotationSourceCode.Filename),
                            Assemblies = annotationSourceCode
                                .DisassembledLine.Assembly.Select(
                                    assemblyLine =>
                                        new ExtendedAssembly()
                                        {
                                            Address = assemblyLine.Address,
                                            Instruction = assemblyLine.Instruction,
                                            IsHighlighted =
                                                assemblyLine.Address
                                                == annotationSourceCode.InstructionAddress
                                        }
                                )
                                .ToList()
                        };
                        samplesSection.Children.Add(annotationSection);
                    }
                }
            }



            OnPropertyChanged("RootSampledEvent");
        }

        private double CalculatePercentage(double value, double total)
        {
            return Math.Min(value / total * 100, 100);
        }

        public IEnumerable GetChildren(object parent)
        {
            return parent == null
                ? RootSampledEvent
                : (IEnumerable)((SamplingSection)parent).Children;
        }

        public bool HasChildren(object parent)
        {
            return ((SamplingSection)parent).Children?.Count > 0;
        }
    }
}
