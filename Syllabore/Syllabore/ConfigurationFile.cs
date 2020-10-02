using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Syllabore
{
    public static class ConfigurationFile
    {

        public static void Save(NameGenerator generator, string path)
        {
            string result = JsonSerializer.Serialize<NameGenerator>(generator, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(path, result);
        }

        public static NameGenerator Load(string path)
        {
            string result = File.ReadAllText(path);
            return JsonSerializer.Deserialize<NameGenerator>(result);
        }

    }
}
