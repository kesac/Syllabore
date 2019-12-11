using System;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ISyllableModel model = new BasicSyllableModel();
            INameValidator validator = new BasicNameValidator();

            Syllabore names = new Syllabore(model, validator);
            for(int i = 0; i < 50; i++)
            {
                System.Console.WriteLine(names.Next());
            }
        }
    }
}
