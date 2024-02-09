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
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer
{
    public class ExtendedAssembly : Assembly
    {
        public bool IsHighlighted = false;
    }
    public class SamplingSection : NotifyPropertyChangedImplementor
    {
        private ulong? hits;

        public ulong? Hits
        {
            get { return hits; }
            set
            {
                hits = value;
                OnPropertyChanged();
            }
        }
        private double? overhead;

        public double? Overhead
        {
            get { return overhead; }
            set
            {
                overhead = value;
                OnPropertyChanged();
                if (value != null)
                    overheadPercentage = RoundToTwoDecimalPlaces(value).ToString() + " %";
            }
        }


        private ulong? lineNumber = null;

        public ulong? LineNumber
        {
            get { return lineNumber; }
            set
            {
                lineNumber = value;
                OnPropertyChanged();
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private int layer;

        public int Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        private SamplingSectionType sectionType;

        public SamplingSectionType SectionType
        {
            get { return sectionType; }
            set { sectionType = value; }
        }

        private bool isFileExists = false;

        public bool IsFileExists
        {
            get { return isFileExists; }
            set { isFileExists = value; }
        }

#nullable enable
        private List<ExtendedAssembly>? assemblies;

        public List<ExtendedAssembly>? Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        private string? overheadPercentage;

        public string? OverheadPercentage
        {
            get { return overheadPercentage; }
        }
        private string? frequency;

        public string? Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }

        private string? pdbFile;

        public string? PdbFile
        {
            get { return pdbFile; }
            set
            {
                pdbFile = value;
                OnPropertyChanged();
            }
        }
        private string? peFile;

        public string? PeFile
        {
            get { return peFile; }
            set
            {
                peFile = value;
                OnPropertyChanged();
            }
        }
        private List<Module>? modules;

        public List<Module>? Modules
        {
            get { return modules; }
            set
            {
                modules = value;
                OnPropertyChanged();
            }
        }

        private ulong? samplesDropped;

        public ulong? SamplesDropped
        {
            get { return samplesDropped; }
            set { samplesDropped = value; }
        }


#nullable disable
        public enum SamplingSectionType
        {
            ROOT,
            SAMPLE_EVENT,
            SAMPLE_FUNCTION,
            SAMPLE_SOURCE_CODE
        }

        private List<SamplingSection> children;

        public List<SamplingSection> Children
        {
            get { return children; }
            set
            {
                children = value;
                OnPropertyChanged();
            }
        }
        private SamplingSection parent;

        public SamplingSection Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                OnPropertyChanged();
            }
        }
        private double RoundToTwoDecimalPlaces(double? value)
        {
            if (value == null) return 0;
            return Math.Round((double)value, 2);
        }


    }
}
