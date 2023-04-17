using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// Contains probalistic settings in a <see cref="SyllableGenerator"/>.
    /// </summary>
    public class GeneratorProbability
    {
        public double? ChanceStartingSyllableLeadingVowelExists { get; set; }
        public double? ChanceStartingSyllableLeadingVowelIsSequence { get; set; }
        public double? ChanceLeadingConsonantExists { get; set; }
        public double? ChanceLeadingConsonantIsSequence { get; set; }
        public double? ChanceVowelExists { get; set; }
        public double? ChanceVowelIsSequence { get; set; }
        public double? ChanceTrailingConsonantExists { get; set; }
        public double? ChanceTrailingConsonantIsSequence { get; set; }

        public GeneratorProbability() { }

    }
}
