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

            Separator();

            {
                // Example of customizing name generator
                // with a custom vowel and consonant pool
                var p = new SyllableProvider();
                p.WithVowels("aei");
                p.WithConsonants("trs");

                var g = new NameGenerator();
                g.UsingProvider(p);

                Console.WriteLine(g.Next());
                

                // Same example, but with method chaining.
                // The rest of the examples will use this style.

                g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("aei")
                        .WithConsonants("trs"));

                Console.WriteLine(g.Next());
                
            }

            Separator();

            {
                // Example of differentiating between leading and
                // trailing consonants
                var g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("aei")
                        .WithLeadingConsonants("vmnl")
                        .WithTrailingConsonants("tsrc"));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {
                // Example of introducing sequences:
                // Vowels and consonants have a probability
                // of turning into a vowel sequences and consonant
                // sequences respectively
                var g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("aeiou")
                        .WithVowelSequences("oo", "ea")
                        .WithLeadingConsonants("vmnl")
                        .WithLeadingConsonantSequences("wh", "tr")
                        .WithTrailingConsonants("tsrc")
                        .WithTrailingConsonantSequences("st","rd"));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                // Same example, but using context-aware AndSequences()

                g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("aeiou").Sequences("oo", "ea")
                        .WithLeadingConsonants("vmnl").Sequences("wh", "tr")
                        .WithTrailingConsonants("tsrc").Sequences("st", "rd"));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {
                // An example of manipulating general frequency
                // of leading and trailing consonants. In this example,
                // the name generator will use syllables that always have
                // a leading consonant, but only have a trailing one 20% of the
                // time.
                var g = new NameGenerator()
                        .UsingProvider(x => x
                            .WithVowels("aei")
                            .WithLeadingConsonants("vmnl")
                            .WithTrailingConsonants("tsrc")
                            .WithProbability(x => x
                                .LeadingConsonantExists(1.0)
                                .TrailingConsonantExists(0.20)));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {

                // Normally the constructor takes a SyllableProvider
                // and NameValidator. There are "Standalone" classes
                // available for quick and dirty use. It is recommended
                // you create your own by using ISyllableProvider/INameValidator
                // or inheriting from ConfigurableSyllableProvider/ConfigurableNameValidator.

                var provider = new DefaultSyllableProvider();
                var validator = new NameValidator()
                        .DoNotAllowPattern(@"[j|p|q|w]$")             // Invalidate these awkward endings
                        .DoNotAllowPattern(@"(\w)\1\1")               // Invalidate any sequence of 3 or more identical letters
                        .DoNotAllowPattern(@"([^aeiouAEIOU])\1\1\1"); // Invalidate any sequence of 4 or more consonants
                
                var g = new NameGenerator(provider, validator);

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {

                // You can choose to build name generators programmatically.
                var g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithLeadingConsonants("str")
                        .WithVowels("ae"))
                    .LimitSyllableCount(3);

                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();

            }

            Separator();

            {
                // Creating variations of a single name
                var g = new NameGenerator().UsingMutator(new VowelMutator());

                for(int i = 0; i < 3; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);

                    for (int j = 0; j < 4; j++)
                    {
                        var variation = g.Mutate(name);
                        Console.WriteLine(variation);

                    }
                }
            }

            Separator();

            {
                Console.WriteLine();
                var g = new NameGenerator()
                    .UsingProvider(p => p
                        .WithVowels("aeoy")
                        .WithLeadingConsonants("vstlr")
                        .WithTrailingConsonants("zrt")
                        .WithVowelSequences("ey", "ay", "oy"))
                    .UsingMutator(m => m
                        .WithMutation(x => x.ReplaceSyllable(0, "Gran"))
                        .WithMutation(x => x.ReplaceSyllable(0, "Bri"))
                        .WithMutation(x => x.InsertSyllable(0, "Deu").AppendSyllable("gard")).Weight(2)
                        .WithMutation(x => x.When(-2, "[aeoyAEOY]$").ReplaceSyllable(-1, "opolis"))
                        .WithMutation(x => x.When(-2, "[^aeoyAEOY]$").ReplaceSyllable(-1, "polis"))
                        .WithMutationCount(1))
                    .UsingValidator(v => v
                        .DoNotAllowPattern(
                            @".{12,}",
                            @"(\w)\1\1",             // Prevents any letter from occuring three times in a row
                            @".*([y|Y]).*([y|Y]).*", // Prevents double y
                            @".*([z|Z]).*([z|Z]).*", // Prevents double z
                            @"(zs)",                 // Prevents "zs"
                            @"(y[v|t])"))            // Prevents "yv" and "yt"
                    .LimitMutationChance(0.99)
                    .LimitSyllableCount(2, 4);

                ConfigurationFile.Save(g, "city-name-generator.txt");
                var g2 = ConfigurationFile.Load("city-name-generator.txt");

                for (int i = 0; i < 50; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);
                }

                Console.WriteLine();

            }

            Separator();

            {
                var provider = new SyllableProvider();
                provider.WithVowels("a", "e", "o", "y");
                provider.WithLeadingConsonants("v", "s", "t", "l", "r");
                provider.WithTrailingConsonants("z", "r", "t");
                provider.WithVowelSequences("ey", "ay", "oy");
                // provider.DisallowLeadingConsonantSequences(); // They will be off if nothing is defined
                // provider.DisallowTrailingConsonantSequences();  // They will be off if nothing is defined

                var shifter = new VowelMutator("a", "e", "o", "y");

                var validator = new NameValidator();
                validator.DoNotAllowPattern(@"(\w)\1\1");
                validator.DoNotAllowPattern(@"([^aeoyAEOY])\1");
                validator.DoNotAllowPattern(@".*([y|Y]).*([y|Y]).*");
                validator.DoNotAllowPattern(@".*([z|Z]).*([z|Z]).*");
                validator.DoNotAllowPattern(@"(zs)");
                validator.DoNotAllowPattern(@"(y[v|t])");

                var g = new NameGenerator(provider, shifter, validator);
                g.LimitSyllableCount(2, 3);

            }

            Separator();

            {
                var name = new Name("syl", "la", "bore");
                var mutator = new NameMutator()
                                .Join(new DefaultNameMutator())
                                .Join(new VowelMutator());

                for(int i = 0; i < 20; i++)
                {
                    Console.WriteLine(mutator.Mutate(name));
                }
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
