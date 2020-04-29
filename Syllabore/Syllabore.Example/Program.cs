using System;
using System.Collections.Generic;
using System.Linq;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Creating a vanilla name generator with a validator
            // with no configuration or loading of definition files
            var provider = new StandaloneSyllableProvider();
            var validator = new StandaloneNameValidator();

            var names = new NameGenerator(provider, validator);

            for (int i = 0; i < 10; i++)
            {
                System.Console.WriteLine(names.Next());
            }

            System.Console.WriteLine();

            // Creating a name generator from an XML definiton file
            var file = new XmlFileLoader("data/basic.xml");
            file.Load();

            var names2 = file.GetNameGenerator("SoftNameGenerator");

            for (int i = 0; i < 10; i++)
            {
                System.Console.WriteLine(names2.Next());
            }

            System.Console.WriteLine();

            // Creating variations of a single name
            var set = new HashSet<string>();
            var syllableShifter = new SyllableShifter(provider);
            var vowelShifter = new VowelShifter(provider.GetAllVowels());
            names.MinimumSyllables = 2;
            names.MaximumSyllables = 2;
            
            for (int i = 0; i < 5; i++)
            {
                var sourceName = names.NextName();
                set.Add(sourceName.ToString());

                for (int j = 0; j < 1; j++)
                {
                    set.Add(syllableShifter.NextVariation(sourceName).ToString());
                }

                for (int j = 0; j < 5; j++)
                {
                    set.Add(vowelShifter.NextVariation(sourceName).ToString());
                }
            }

            foreach(var name in set.ToList().OrderBy(x => x))
            {
                System.Console.WriteLine(name);
            }

        }
    }
}
