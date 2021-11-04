using Syllabore.Json;
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

                // You can use name generators without any customization
                var g = new NameGenerator();

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                // A "default" name generator will use all vowels and
                // consonants of the English alphabet with no custom weighting
                // or filtering. Expect the output to be wild and exotic.

            }

            Separator();

            {
                // To tailor name generation, first think about
                // what vowels and consonants you want to use:
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("ae")
                            .WithConsonants("strmnl"));
            }
            {
                // Consonants can also be defined based
                // a desired positioning:
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str")
                            .WithTrailingConsonants("mnl"));

                // In a syllable, leading consonants appear before
                // the vowel and trailing consonants appear after

            }
            {
                // You can also introduce vowel sequences or consonant
                // sequences that have a chance of being substituted in
                // for a normal vowel or consonant: 
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("ae")
                            .WithVowelSequences("ou", "ui")
                            .WithLeadingConsonants("str")
                            .WithLeadingConsonantSequences("wh", "fr")
                            .WithTrailingConsonants("mnl")
                            .WithTrailingConsonantSequences("ld","rd"));

                // Note that probabilities can be controlled. There is an
                // example of this further down.

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                // This example is the same as the previous one except
                // it uses context-aware calls to Sequences() 
                g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("ae").Sequences("ou", "ui")
                        .WithLeadingConsonants("str").Sequences("wh", "fr")
                        .WithTrailingConsonants("mnl").Sequences("ld", "rd"));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {
                // The NameGenerator also allows you to define how often
                // sequence substitution occurs, how often leading or trailing consonants
                // occur, etc.
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str")
                            .WithTrailingConsonants("mnl")
                            .WithProbability(x => x
                                .LeadingConsonantExists(1.0)
                                .TrailingConsonantExists(0.20)));

                // In this example, the name generator will use syllables that always have
                // a leading consonant, but only have a trailing one 20% of the
                // time.

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {
                // You can also give custom weights to vowels
                // and consonants. A higher weight means a higher
                // frequency compared those with lower weights.
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("a").Weight(4)
                            .WithVowels("e").Weight(1)
                            .WithLeadingConsonants("str"));

                // In this example, the vowel 'a' will appear 4 times
                // more likely than an 'e'

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {

                // Filters can be used to improve output, by preventing
                // specific substrings or patterns from occuring:
                var g = new NameGenerator()
                            .UsingFilter(x => x
                                .DoNotAllowEnding("j","p","q","w")             // Invalidate these awkward endings
                                .DoNotAllowPattern(@"(\w)\1\1")                // Invalidate any sequence of 3 or more identical letters
                                .DoNotAllowPattern(@"([^aeiouAEIOU])\1\1\1")); // Invalidate any sequence of 4 or more consonants

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {

                // Transformers can be used to apply a transform
                // to a name during the generation process:
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str"))
                        .UsingTransformer(x => x
                            .Select(1).Chance(0.5)
                            .WithTransform(x => x.AppendSyllable("gard")).Weight(2)
                            .WithTransform(x => x.AppendSyllable("dar")))
                        .UsingSyllableCount(3);

                // In this example, the name generation is creating
                // variations of the name *gard and *dar, with the former
                // being twice more likely to be generated.

                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();

            }

            Separator();

            {
                
                /*
                var g = new NameGenerator().UsingTransformer(new VowelMutator());

                for(int i = 0; i < 3; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);

                    for (int j = 0; j < 4; j++)
                    {
                        var variation = g.Transform(name);
                        Console.WriteLine(variation);

                    }
                }
                */
            }

            Separator();

            {

                // This example shows a custom provider, transformer, and filter:
                Console.WriteLine();
                var g = new NameGenerator()
                        .UsingProvider(p => p
                            .WithVowels("aeoy")
                            .WithLeadingConsonants("vstlr")
                            .WithTrailingConsonants("zrt")
                            .WithVowelSequences("ey", "ay", "oy"))
                        .UsingTransformer(m => m
                            .Select(1).Chance(0.99)
                            .WithTransform(x => x.ReplaceSyllable(0, "Gran"))
                            .WithTransform(x => x.ReplaceSyllable(0, "Bri"))
                            .WithTransform(x => x.InsertSyllable(0, "Deu").AppendSyllable("gard")).Weight(2)
                            .WithTransform(x => x.When(-2, "[aeoyAEOY]$").ReplaceSyllable(-1, "opolis"))
                            .WithTransform(x => x.When(-2, "[^aeoyAEOY]$").ReplaceSyllable(-1, "polis")))
                        .UsingFilter(v => v
                            .DoNotAllow("yv", "yt", "zs")
                            .DoNotAllowPattern(
                                @".{12,}",
                                @"(\w)\1\1",              // Prevents any letter from occuring three times in a row
                                @".*([y|Y]).*([y|Y]).*",  // Prevents double y
                                @".*([z|Z]).*([z|Z]).*")) // Prevents double z
                        .UsingSyllableCount(2, 4);

                NameGeneratorConfig.Save(g, "city-name-generator.txt");
                var g2 = NameGeneratorConfig.Load("city-name-generator.txt");

                for (int i = 0; i < 50; i++)
                {
                    var name = g2.NextName();
                    Console.WriteLine(name);
                }

                Console.WriteLine();

            }

            Separator();

            {
                // An example without using method chaining

                var provider = new SyllableProvider();
                provider.WithVowels("a", "e", "o", "y");
                provider.WithLeadingConsonants("v", "s", "t", "l", "r");
                provider.WithTrailingConsonants("z", "r", "t");
                provider.WithVowelSequences("ey", "ay", "oy");
                // provider.DisallowLeadingConsonantSequences(); // They will be off if nothing is defined
                // provider.DisallowTrailingConsonantSequences();  // They will be off if nothing is defined

                var shifter = new VowelMutator("a", "e", "o", "y");

                var filter = new NameFilter();
                filter.DoNotAllowPattern(@"(\w)\1\1");
                filter.DoNotAllowPattern(@"([^aeoyAEOY])\1");
                filter.DoNotAllowPattern(@".*([y|Y]).*([y|Y]).*");
                filter.DoNotAllowPattern(@".*([z|Z]).*([z|Z]).*");
                filter.DoNotAllowPattern(@"(zs)");
                filter.DoNotAllowPattern(@"(y[v|t])");

                var g = new NameGenerator(provider, shifter, filter);
                g.UsingSyllableCount(2, 3);

            }

            Separator();

            {
                // Creating variations of a single name
                var name = new Name("syl", "la", "bore");
                var mutator = new NameTransformer()
                                .Join(new DefaultNameTransformer())
                                .Join(new VowelMutator());

                for(int i = 0; i < 20; i++)
                {
                    Console.WriteLine(mutator.Transform(name));
                }
            }

            Separator();

            {
                var g = new NameGenerator()
                        .UsingProvider(new SyllableSet(1, 1, 4)
                            .WithVowels("ae").Weight(2)
                            .WithVowels("iou")
                            .WithLeadingConsonants("str").Weight(2)
                            .WithLeadingConsonants("lmncvbyzwkd"))
                        .UsingSyllableCount(2,4);

                // In this example, the name generation is creating
                // variations of the name *gard and *dar, with the former
                // being twice more likely to be generated.

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();
            }

            Separator();

            {
                var g = new JPGenerator();

                for (int i = 0; i < 20; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {
                // Fantasy-sounding names, maybe
                var g = new FantasyGenerator();

                for (int i = 0; i < 20; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

        }

        public static void Separator()
        {
            Console.WriteLine("----------------------");
        }

    }
}
