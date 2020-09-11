using System;
using System.Collections.Generic;
using System.Linq;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            { 
                // Quickest way to use Syllabore's name generator
                // without specifying any configuration. This instance
                // will default to using StandaloneSyllableProvider for
                // name generator and will not use any NameValidator to
                // improve output.
                var g = new NameGenerator();

                for (int i = 0; i < 10; i++)
                {
                    System.Console.WriteLine(g.Next());
                }
            }
            {

                // Normally the constructor takes a SyllableProvider
                // and NameValidator. There are "Standalone" classes
                // available for quick and dirty use. It is recommended
                // you create your own by using ISyllableProvider/INameValidator
                // or inheriting from ConfigurableSyllableProvider/ConfigurableNameValidator.

                var provider = new StandaloneSyllableProvider();
                var validator = new StandaloneNameValidator();

                var g = new NameGenerator(provider, validator);
                Console.WriteLine(g.Next());
            }

            {
                // Configuration of syllable providers and name validators
                // can be captured in a dedicated XML file then loaded
                // through the XmlFileLoader.

                var file = new XmlFileLoader("data/basic.xml").Load();
                var g = file.GetNameGenerator("SoftNameGenerator").SetSyllableCount(2, 4);

                Console.WriteLine();
                for (int i = 0; i < 10; i++)
                {
                    System.Console.WriteLine(g.Next());
                }
                Console.WriteLine();
            }
            {

                // If you don't like XML, you can choose to
                // build name generators programmatically.
                var g = new NameGenerator()
                    .SetProvider(new ConfigurableSyllableProvider()
                        .AddLeadingConsonant("s", "t", "r")
                        .AddVowel("a", "e")
                        .SetVowelSequenceProbability(0.20)
                        .AddTrailingConsonant("z")
                        .SetTrailingConsonantProbability(0.10)
                        .AllowVowelSequences(false)
                        .AllowLeadingConsonantSequences(false)
                        .AllowTrailingConsonantSequences(false))
                    .SetValidator(new ConfigurableNameValidator()
                        .AddRegexConstraint("zzz")
                        .AddRegexConstraint("[q]+"))
                    .SetSyllableCount(3);

                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            {
                // Creating variations of a single name
                var provider = new StandaloneSyllableProvider();
                var names = new NameGenerator(provider);
                var set = new HashSet<string>();
                var syllableShifter = new SyllableShifter(provider);
                var vowelShifter = new VowelShifter(provider.GetAllVowels());
                names.SetSyllableCount(2);

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

                foreach (var name in set.ToList().OrderBy(x => x))
                {
                    System.Console.WriteLine(name);
                }
            }

        }
    }
}
