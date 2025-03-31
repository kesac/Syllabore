using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Syllabore.Json
{
    /// <summary>
    /// A convenience class for reading and writing 
    /// a <see cref="NameGenerator"/> to disk and back.
    /// </summary>
    public class NameGeneratorSerializer
    {
        /// <summary>
        /// A type of <see cref="JsonConverter{T}"/> used to 
        /// ensure properties with an interface type are serialized
        /// as the concrete class type instead.
        /// <para>
        /// This converter is used by <see cref="NameGeneratorSerializer"/>
        /// internally.
        /// </para>
        /// </summary>
        private class InterfaceConverter<T> : JsonConverter<T>
        {
            /// <summary>
            /// Required to implement <see cref="JsonConverter{T}"/>, but not used by this converter.
            /// </summary>
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // NameGeneratorSerializer does not use this when reading JSON
                throw new InvalidCastException(nameof(InterfaceConverter<T>) + " can only be used when writing JSON.");
            }

            /// <summary>
            /// Writes the value as its concrete type.
            /// </summary>
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                // Calling GetType() on value will return the concrete type
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }

        /// <summary>
        /// A list of characters that will not be escaped in Json output.
        /// (Carriage return, newline, and quotation mark.)
        /// </summary>
        private static readonly char[] AllowedCharacters = { '\r', '\n', '\u0022' };

        /// <summary>
        /// Allows characters and unicode ranges.
        /// </summary>
        public TextEncoderSettings EncoderSettings { get; set; }

        /// <summary>
        /// If true, json output will be indented and easier to read.
        /// </summary>
        public bool WriteIndented { get; set; }

        /// <summary>
        /// Initializes a new <see cref="NameGeneratorSerializer"/> with
        /// basic encoder settings.
        /// </summary>
        public NameGeneratorSerializer()
        {
            EncoderSettings = new TextEncoderSettings();
            EncoderSettings.AllowRanges(UnicodeRanges.BasicLatin);
            EncoderSettings.AllowCharacters(AllowedCharacters);
            WriteIndented = true;
        }

        /// <summary>
        /// Writes the specified <see cref="NameGenerator"/> to a json file.
        /// </summary>
        public void Serialize(NameGenerator generator, string filepath)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = this.WriteIndented,
                Encoder = JavaScriptEncoder.Create(EncoderSettings)
            };

            // Some of NameGenerator's properties are an interface type.
            // These converters ensure they are serialized as their concrete type instead.
            options.Converters.Add(new InterfaceConverter<ISyllableGenerator>());
            options.Converters.Add(new InterfaceConverter<INameTransformer>());
            options.Converters.Add(new InterfaceConverter<INameFilter>());

            var meta = new SerializedNameGenerator()
            {
                Types = GetTypeInformation(generator),
                Value = generator
            };

            string json = JsonSerializer.Serialize<SerializedNameGenerator>(meta, options);
            File.WriteAllText(filepath, json);
        }

        // NameGenerator has properties with interfaces as their types.
        // Normal deserialization won't work. This class's Serialize() method
        // writes the concrete types into the json file so we look for those concrete
        // types when deserializing.
        /// <summary>
        /// Reads a json file and turns it into a <see cref="NameGenerator"/>.
        /// This method expects the json file to have been written by
        /// the <see cref="Serialize"/> method of this class.
        /// </summary>
        public NameGenerator Deserialize(string filepath)
        {
            var json = File.ReadAllText(filepath);
            var result = new NameGenerator();

            using (var doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;

                // Each json file has two major sections:
                // - Value, which contains the actual name generator JSON
                // - Types, which contains the class names needed to deserialize the name generator JSON properly
                //          we need class names because some name generator properties are interface types
                var extractedTypes = root.TryGetProperty(nameof(SerializedNameGenerator.Types), out JsonElement typesElement);
                var extractedValue = root.TryGetProperty(nameof(SerializedNameGenerator.Value), out JsonElement valueElement);
                var extractedSyllableGenerators = valueElement.TryGetProperty(nameof(SerializedNameGenerator.Value.SyllableGenerators), out JsonElement syllableGeneratorsElement);

                if (!extractedTypes || !extractedValue || !extractedSyllableGenerators)
                {
                    throw new JsonException("The json file is not in the expected format.");
                }

                var types = JsonSerializer.Deserialize<NameGeneratorTypeInformation>(typesElement.GetRawText());

                // For syllable generators, we must:
                // - Extract class names from 'Types' and resolve the names into actual C# Type objects
                // - Deserialize portions of the NameGenerator JSON using the Type objects
                // - Assign the deserialized objects to the appropriate properties of NameGenerator
                foreach (var syllableGeneratorTypeName in types.SyllableGeneratorTypeNames)
                {
                    var syllableGeneratorType = Type.GetType(syllableGeneratorTypeName.Value);
                    var syllablePositionName = syllableGeneratorTypeName.Key.ToString();

                    if (syllableGeneratorsElement.TryGetProperty(syllablePositionName, out JsonElement generatorElement)
                        && generatorElement.ValueKind != JsonValueKind.Null)
                    {
                        var generator = (ISyllableGenerator)JsonSerializer.Deserialize(generatorElement.GetRawText(), syllableGeneratorType);
                        result.SyllableGenerators[syllableGeneratorTypeName.Key] = generator;
                    }
                }

                // Transformers and filters are handled the same way as syllable generators:
                // - Extract class names from 'Types' and resolve the names into actual C# Type objects
                // - Deserialize portions of the NameGenerator JSON using the Type objects
                // - Assign the deserialized objects to the transformer and filter properties of NameGenerator
                if (types.NameTransformerTypeName != null)
                {
                    if(valueElement.TryGetProperty(nameof(result.NameTransformer), out JsonElement transformerElement) 
                        && transformerElement.ValueKind != JsonValueKind.Null)
                    {
                        var transformerType = Type.GetType(types.NameTransformerTypeName);
                        result.NameTransformer = (INameTransformer)JsonSerializer.Deserialize(transformerElement.GetRawText(), transformerType);
                    }
                }

                if (types.NameFilterType != null)
                {
                    if (valueElement.TryGetProperty(nameof(result.NameFilter), out JsonElement filterElement)
                        && filterElement.ValueKind != JsonValueKind.Null)
                    {
                        var filterType = Type.GetType(types.NameFilterType);
                        result.NameFilter = (INameFilter)JsonSerializer.Deserialize(filterElement.GetRawText(), filterType);
                    }
                }

                // The remaining properties of NameGenerator don't use interface types
                // so we can deserialize them normally
                if (valueElement.TryGetProperty(nameof(result.MinimumSize), out JsonElement minSizeElement))
                {
                    result.MinimumSize = minSizeElement.GetInt32();
                }

                if (valueElement.TryGetProperty(nameof(result.MaximumSize), out JsonElement maxSizeElement))
                {
                    result.MaximumSize = maxSizeElement.GetInt32();
                }

                if (valueElement.TryGetProperty(nameof(result.MaximumRetries), out JsonElement maxRetriesElement))
                {
                    result.MaximumRetries = maxRetriesElement.GetInt32();
                }

            }
            
            return result;
        }

        /// <summary>
        /// Collects type information of <see cref="NameGenerator"/> properties"/> to be
        /// included in serialized json. This information is important when deserializing 
        /// the json back into a <see cref="NameGenerator"/> object.
        /// </summary>
        private NameGeneratorTypeInformation GetTypeInformation(NameGenerator generator)
        {
            var types = new NameGeneratorTypeInformation();
            types.SyllableGeneratorTypeNames = new Dictionary<SyllablePosition, string>();

            foreach (var pair in generator.SyllableGenerators)
            {
                var position = pair.Key;
                var syllableGenerator = pair.Value;
                types.SyllableGeneratorTypeNames[position] = syllableGenerator.GetType().FullName;
            }

            if (generator.NameTransformer != null)
            {
                types.NameTransformerTypeName = generator.NameTransformer.GetType().FullName;
            }

            if (generator.NameFilter != null)
            {
                types.NameFilterType = generator.NameFilter.GetType().FullName;
            }

            return types;
        }

    }
}
