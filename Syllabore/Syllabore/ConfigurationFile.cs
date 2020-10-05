using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syllabore
{

    public static class ConfigurationFile
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions() { WriteIndented = true };

        public static void Save(NameGenerator generator, string path)
        {    
            string result = JsonSerializer.Serialize<NameGenerator>(generator, Options);
            File.WriteAllText(path, result);
        }

        public static NameGenerator Load(string path)
        {
            string result = File.ReadAllText(path);
            return JsonSerializer.Deserialize<NameGenerator>(result, Options);
        }

    }
}
