using System;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var provider = new StandaloneSyllableProvider();
            var validator = new StandaloneNameValidator();

            var names = new NameGenerator(provider, validator);

            for (int i = 0; i < 100; i++)
            {
                System.Console.WriteLine(names.Next());
            }


            var loader = new XmlLoader("data/basic.xml");

        }
    }
}
