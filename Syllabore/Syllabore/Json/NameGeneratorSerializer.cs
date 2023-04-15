using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// A list of characters that will not be escaped in Json output.
        /// (Carriage return, newline, and quotation mark.)
        /// </summary>
        private static readonly char[] AllowedCharacters = { '\r', '\n', '\u0022' };

        public Type ProviderType { get; set; }
        public Type TransformerType { get; set; }
        public Type FilterType { get; set; }

        public NameGeneratorSerializer()
        {
            this.ProviderType = typeof(SyllableGenerator);
            this.TransformerType = typeof(NameTransformer);
            this.FilterType = typeof(NameFilter);
        }

        /// <summary>
        /// Uses the specified type when serializing or deserializing 
        /// <see cref="ISyllableGenerator"/> property <c>Provider</c> of
        /// <see cref="NameGenerator"/>. By default, the type 
        /// used is <see cref="SyllableGenerator"/>.
        /// </summary>
        public NameGeneratorSerializer UsingProviderType(Type type)
        {
            this.ProviderType = type;
            return this;
        }

        /// <summary>
        /// Uses the specified type when serializing or deserializing 
        /// <see cref="INameTransformer"/> property <c>Transformer</c> of
        /// <see cref="NameGenerator"/>. By default, the type 
        /// used is <see cref="NameTransformer"/>.
        /// </summary>
        public NameGeneratorSerializer UsingTransformerType(Type type)
        {
            this.TransformerType = type;
            return this;
        }

        /// <summary>
        /// Uses the specified type when serializing or deserializing 
        /// <see cref="INameFilter"/> property <c>Filter</c> of
        /// <see cref="NameGenerator"/>. By default, the type 
        /// used is <see cref="NameFilter"/>.
        /// </summary>
        public NameGeneratorSerializer UsingFilterType(Type type)
        {
            this.FilterType = type;
            return this;
        }

        private JsonSerializerOptions GetSerializerOptions()
        {
            var encoding = new TextEncoderSettings();
            encoding.AllowRanges(UnicodeRanges.BasicLatin);
            encoding.AllowCharacters(AllowedCharacters);

            var options = new JsonSerializerOptions() { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(encoding)
            };

            return options;
        }

        /// <summary>
        /// Writes the specified NameGenerator to disk as a JSON file to the specified file path.
        /// </summary>
        public void Serialize(NameGenerator generator, string filepath)
        {    
            Save(generator, filepath, this.ProviderType, this.TransformerType, this.FilterType);
        }

        /// <summary>
        /// Writes the specified NameGenerator to disk as a JSON file to the specified file path.
        /// The NameGenerator's provider, transformer, and filter will be saved as the specified types.
        /// </summary>
        private void Save(NameGenerator generator, string path, Type provider, Type transformer, Type filter)
        {
            var options = GetSerializerOptions();
            options.Converters.Add(new JsonPropertyCast<ISyllableGenerator>(provider));
            options.Converters.Add(new JsonPropertyCast<INameTransformer>(transformer));
            options.Converters.Add(new JsonPropertyCast<INameFilter>(filter));

            string result = JsonSerializer.Serialize<NameGenerator>(generator, options);
            File.WriteAllText(path, result);
        }

        /// <summary>
        /// Reads a JSON file at the specified path and returns a <see cref="NameGenerator"/> based on that file.
        /// </summary>
        public NameGenerator Deserialize(string filepath)
        {
            return Load(filepath, this.ProviderType, this.TransformerType, this.FilterType);
        }

        /// <summary>
        /// Reads a JSON file at the specified path and returns a <see cref="NameGenerator"/> based on that file.
        /// The NameGenerator's provider, transformer, and filter will be instantiated as the specified types.
        /// </summary>
        private NameGenerator Load(string path, Type provider, Type transformer, Type filter)
        {
            var options = GetSerializerOptions();
            options.Converters.Add(new JsonPropertyCast<ISyllableGenerator>(provider));
            options.Converters.Add(new JsonPropertyCast<INameTransformer>(transformer));
            options.Converters.Add(new JsonPropertyCast<INameFilter>(filter));

            string result = File.ReadAllText(path);
            var g = JsonSerializer.Deserialize<NameGenerator>(result, options);

            // The parent property is not serialized because it would create a cycle
            // and so needs to be set again.
            if(g.Provider is SyllableGenerator)
            {
                var p = (SyllableGenerator)g.Provider;
                p.Probability.StartingSyllable.Parent = p.Probability;
            }
            
            return g;
        }

    }
}
