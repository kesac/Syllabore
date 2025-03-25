using Syllabore.Json;
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
    public interface Example
    {
        IGenerator<string> GetGenerator();
    }

    public class Program
    {
        public static void Main(string[] args)
        {

            RunCustomGenerator(new HighFantasyNames());
            RunCustomGenerator(new SpaceshipNames());
            RunCustomGenerator(new SimilarSoundingNames());
            RunCustomGenerator(new FuturisticCityNames());
            RunCustomGenerator(new HarshSoundingNames());
            RunCustomGenerator(new SofterSoundingNames());
            RunCustomGenerator(new AlienSoundingNames());
            RunCustomGenerator(new BossNames());
   
            RunSimpleConstructorExample();
            PrintSeparator();

            RunSerializationExample();
            RunExecutionTimeCheck(300000);
        }


        private static void RunCustomGenerator(Example example)
        {
            Console.WriteLine(string.Format("[{0}]", example.GetType().Name));

            var generator = example.GetGenerator();

            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(generator.Next());
            }

            PrintSeparator();
        }

        private static void RunSimpleConstructorExample()
        {
            Console.WriteLine("[SimpleConstructor]");
            var n1 = new NameGenerator("srnl", "ae");
                
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(n1.Next());
            }
        }

        private static void RunExecutionTimeCheck(int attempts)
        {
            Console.WriteLine("[ExecutionTimeCheck]");
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
            // Console.WriteLine();
            // Console.WriteLine("---");
            Console.WriteLine();
        }

    }
}
