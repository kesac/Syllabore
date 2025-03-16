using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Json
{
    /// <summary>
    /// Contains <see cref="Type"/> information for a 
    /// <see cref="NameGenerator"/>'s properties. This class 
    /// is used by <see cref="NameGeneratorSerializer"/>.
    /// </summary>
    public class NameGeneratorTypeInformation
    {
        /// <summary>
        /// The concrete type names of the generators in <see cref="NameGenerator.SyllableGenerators"/>.
        /// </summary>
        public Dictionary<SyllablePosition, string> SyllableGeneratorTypeNames { get; set; }

        /// <summary>
        /// The concrete type name of <see cref="NameGenerator.NameTransformer"/>.
        /// </summary>
        public string NameTransformerTypeName { get; set; }

        /// <summary>
        /// The concrete type name of <see cref="NameGenerator.NameFilter"/>.
        /// </summary>
        public string NameFilterType { get; set; }

        /// <summary>
        /// Creates an empty <see cref="NameGeneratorTypeInformation"/>.
        /// </summary>
        public NameGeneratorTypeInformation()
        {
            SyllableGeneratorTypeNames = new Dictionary<SyllablePosition, string>();
        }

    }

}
