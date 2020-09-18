using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                    Console.WriteLine(g.Next());
                }
            }
            {

                // Normally the constructor takes a SyllableProvider
                // and NameValidator. There are "Standalone" classes
                // available for quick and dirty use. It is recommended
                // you create your own by using ISyllableProvider/INameValidator
                // or inheriting from ConfigurableSyllableProvider/ConfigurableNameValidator.

                var provider = new DefaultSyllableProvider();
                var validator = new ConfigurableNameValidator()
                        .Invalidate(@"[j|p|q|w]$")             // Invalidate these awkward endings
                        .Invalidate(@"(\w)\1\1")               // Invalidate any sequence of 3 or more identical letters
                        .Invalidate(@"([^aeiouAEIOU])\1\1\1"); // Invalidate any sequence of 4 or more consonants
                
                var g = new NameGenerator(provider, validator);

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            {
                // Configuration of syllable providers and name validators
                // can be captured in a dedicated XML file then loaded
                // through the XmlFileLoader.

                var file = new XmlFileLoader("data/basic.xml").Load();
                var g = file.GetNameGenerator("SoftNameGenerator").LimitSyllableCount(2, 4);

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
                    .UsingProvider(x => x
                        .WithLeadingConsonants("str")
                        .WithVowels("ae")
                        .WithTrailingConsonants("yz")
                        .WithProbability(y => y
                            .OfVowelSequences(0.20)
                            .OfTrailingConsonants(0.10)))
                    .UsingValidator(x => x
                        .Invalidate("zzz")
                        .Invalidate("[q]+"))
                    .LimitSyllableCount(3);

                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();

            }

            {
                // Creating variations of a single name
                var g = new NameGenerator()
                    .UsingMutator(new MutatorCollection()
                        .Add(new DefaultNameMutator())
                        .Add(new VowelMutator()));

                for(int i = 0; i < 3; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);

                    for (int j = 0; j < 4; j++)
                    {
                        var variation = g.NextVariation(name);
                        Console.WriteLine(variation);

                    }
                }
            }


            {
                Console.WriteLine();
                var g = new NameGenerator()
                    .UsingProvider(p => p
                        .WithVowels("aeoy")
                        .WithLeadingConsonants("vstlr")
                        .WithTrailingConsonants("zrt")
                        .WithVowelSequences("ey", "ay", "oy"))
                    .UsingMutator(m => m
                        .WithMutation(x => { x.Syllables[0] = "Gran"; })
                        .WithMutation(x => { x.Syllables[x.Syllables.Count - 1] = "gard"; }).When(x => x.EndsWithVowel())
                        .WithMutation(x => x.Syllables.Add("opolis")).When(x => x.EndsWithConsonant())
                        .WithMutation(x => x.Syllables.Add("polis")).When(x => x.EndsWithVowel())
                        .WithMutationCount(1))
                    .LimitMutationChance(0.99)
                    .LimitSyllableCount(2, 3)
                    .UsingValidator(v => v
                        .Invalidate(
                            @"(\w)\1\1", // no triples
                            @"([^aeoyAEOY])\1", // no two consonant sequence
                            @".*([y|Y]).*([y|Y]).*", // two y's in same word
                            @".*([z|Z]).*([z|Z]).*", // two z's in same word
                            @"(zs)", // looks wierd
                            @"(y[v|t])") // looks wierd 
                        );


                for (int i = 0; i < 50; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);
                }

                Console.WriteLine();

            }
            {
                var provider = new ConfigurableSyllableProvider();
                provider.WithVowels("a", "e", "o", "y");
                provider.WithLeadingConsonants("v", "s", "t", "l", "r");
                provider.WithTrailingConsonants("z", "r", "t");
                provider.WithVowelSequences("ey", "ay", "oy");
                provider.DisallowLeadingConsonantSequences();
                provider.DisallowTrailingConsonantSequences();

                var shifter = new VowelMutator("a", "e", "o", "y");

                var validator = new ConfigurableNameValidator();
                validator.Invalidate(@"(\w)\1\1");
                validator.Invalidate(@"([^aeoyAEOY])\1");
                validator.Invalidate(@".*([y|Y]).*([y|Y]).*");
                validator.Invalidate(@".*([z|Z]).*([z|Z]).*");
                validator.Invalidate(@"(zs)");
                validator.Invalidate(@"(y[v|t])");

                var g = new NameGenerator(provider, shifter, validator);
                g.LimitSyllableCount(2, 3);

            }

        }
    }
}
