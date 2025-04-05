using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Json
{
    /// <summary>
    /// Contains a <see cref="NameGenerator"/> plus the <see cref="Type"/> information of its properties.
    /// This class is used by <see cref="NameGeneratorSerializer"/> when serializing or deserializing
    /// a <see cref="NameGenerator"/>.
    /// </summary>
    public class SerializedNameGenerator
    {
        /// <summary>
        /// The type information to serialize.
        /// </summary>
        public NameGeneratorTypeInformation Types { get; set; }

        /// <summary>
        /// The <see cref="NameGenerator"/> to serialize.
        /// </summary>
        public NameGenerator Value { get; set; }

        /// <summary>
        /// Creates an empty <see cref="SerializedNameGenerator"/>.
        /// </summary>
        public SerializedNameGenerator(){ }
    }
}
