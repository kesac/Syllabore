﻿using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using System.Diagnostics;
using Syllabore.Fluent;
using Archigen;
using System.Text.Unicode;
using System.Text;
using System.Globalization;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunCustomGenerator(new HighFantasyNames().GetGenerator());
            PrintSeparator();

            RunCustomGenerator(new SpaceshipNames().GetGenerator());
            PrintSeparator();

            RunCustomGenerator(new SimilarSoundingNames().GetNonFluentGenerator());
            PrintSeparator();

            RunCustomGenerator(new FuturisticCityNames().GetGenerator());
            PrintSeparator();

            RunCustomGenerator(new HarshSoundingNames().GetGenerator());
            PrintSeparator();

            RunCustomGenerator(new SofterSoundingNames().GetGenerator());
            PrintSeparator();

            RunSimpleConstructorExample();
            PrintSeparator();

            RunSerializationExample();
            RunExecutionTimeCheck(300000);

        }


        private static void RunCustomGenerator(IGenerator<string> generator)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(generator.Next());
            }
        }

        private static void RunSimpleConstructorExample()
        {
            
            var n1 = new NameGenerator("srnl", "ae");
                
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(n1.Next());
            }

            PrintSeparator();

            var n2 = new NameGenerator("srnl", "ae", "lmt");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(n2.Next());
            }

        }

        private static void RunExecutionTimeCheck(int attempts)
        {
            var stopwatch = Stopwatch.StartNew();
            var v2 = new NameGenerator("ae", "srnl");

            for (int i = 0; i < attempts; i++)
            {
                v2.Next();
            }

            stopwatch.Stop();
            var v2time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Time for {attempts} name generations:");
            Console.WriteLine($"{v2time}ms");

        }

        private static void RunSerializationExample()
        {
            var generator = new NameGenerator("Àe", "🜧🙂🂅ヅ")
                .Transform(x => x.Insert(0, "a"))
                .Filter("z");

            generator.SetSyllables(SyllablePosition.Inner, new SyllableSet("ko"));

            var serializer = new NameGeneratorSerializer();
            serializer.Serialize(generator, "name-generator.json");
            var back = serializer.Deserialize("name-generator.json");

        }

        public static void PrintSeparator()
        {
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine();
        }

    }
}
