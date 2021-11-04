using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syllabore.Json
{

    /// <summary>
    /// A convenience class for reading and writing NameGenerator configurations to disk and back.
    /// </summary>
    public static class NameGeneratorConfig
    {
        /// <summary>
        /// Writes the specified NameGenerator to disk as a JSON file to the specified file path.
        /// The NameGenerator's provider, transformer, and filter are assumed be of type <see cref="SyllableProvider"/>,
        /// <see cref="NameTransformer"/>, and <see cref="NameFilter"/> respectively.
        /// </summary>
        public static void Save(NameGenerator generator, string path)
        {    
            Save(generator, path, typeof(SyllableProvider), typeof(NameTransformer), typeof(NameFilter));
        }

        /// <summary>
        /// Writes the specified NameGenerator to disk as a JSON file to the specified file path.
        /// The NameGenerator's provider, transformer, and filter will be saved as the specified types.
        /// </summary>
        public static void Save(NameGenerator generator, string path, Type provider, Type transformer, Type filter)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new JsonPropertyCast<ISyllableProvider>(provider));
            options.Converters.Add(new JsonPropertyCast<INameTransformer>(transformer));
            options.Converters.Add(new JsonPropertyCast<INameFilter>(filter));

            string result = JsonSerializer.Serialize<NameGenerator>(generator, options);
            File.WriteAllText(path, result);
        }

        /// <summary>
        /// Reads a JSON file at the specified path and returns a <see cref="NameGenerator"/> based on that file.
        /// The NameGenerator's provider, transformer, and filter will be instantiated as a <see cref="SyllableProvider"/>,
        /// <see cref="NameTransformer"/>, and <see cref="NameFilter"/> respectively.
        /// </summary>
        public static NameGenerator Load(string path)
        {
            return Load(path, typeof(SyllableProvider), typeof(NameTransformer), typeof(NameFilter));
        }

        /// <summary>
        /// Reads a JSON file at the specified path and returns a <see cref="NameGenerator"/> based on that file.
        /// The NameGenerator's provider, transformer, and filter will be instantiated as the specified types.
        /// </summary>
        public static NameGenerator Load(string path, Type provider, Type transformer, Type filter)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new JsonPropertyCast<ISyllableProvider>(provider));
            options.Converters.Add(new JsonPropertyCast<INameTransformer>(transformer));
            options.Converters.Add(new JsonPropertyCast<INameFilter>(filter));

            string result = File.ReadAllText(path);
            var g = JsonSerializer.Deserialize<NameGenerator>(result, options);

            // The parent property is not serialized because it would create a cycle
            // and so needs to be set again.
            if(g.Provider is SyllableProvider)
            {
                var p = (SyllableProvider)g.Provider;
                p.Probability.StartingSyllable.Parent = p.Probability;
            }
            
            return g;
        }

    }
}
