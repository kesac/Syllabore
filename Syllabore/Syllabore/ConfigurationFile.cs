using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syllabore
{

    /// <summary>
    /// A convenience class for reading and writing NameGenerator configurations to disk and back.
    /// </summary>
    public static class ConfigurationFile
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = true };

        /// <summary>
        /// Writes the specified NameGenerator to disk as a JSON file to the specified file path.
        /// </summary>
        public static void Save(NameGenerator generator, string path)
        {    
            string result = JsonSerializer.Serialize<NameGenerator>(generator, Options);
            File.WriteAllText(path, result);
        }

        /// <summary>
        /// Reads a JSON file at the specified path and returns a NameGenerator based on that file.
        /// </summary>
        public static NameGenerator Load(string path)
        {
            string result = File.ReadAllText(path);
            var g = JsonSerializer.Deserialize<NameGenerator>(result, Options);

            // The parent property is not serialized because it would create a cycle
            // and so needs to be set again.
            g.Provider.Probability.StartingSyllable.Parent = g.Provider.Probability;

            return g;
        }

    }
}
