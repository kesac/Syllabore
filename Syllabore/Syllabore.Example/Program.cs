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

            for (int i = 0; i < 10; i++)
            {
                System.Console.WriteLine(names.Next());
            }


            var file = new XmlFileLoader("data/basic.xml");
            file.Load();

            var names2 = file.GetNameGenerator("BasicNameGenerator");

            for (int i = 0; i < 10; i++)
            {
                System.Console.WriteLine(names2.Next());
            }
        }
    }
}
