using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public class Syllabore
    {
        private ISyllableModel Model { get; set; }
        private INameValidator Validator { get; set; }
        private Random Random { get; set; }
        public int MinimumSyllables { get; set; }
        public int MaximumSyllables { get; set; }

        public Syllabore(ISyllableModel model)
        {
            this.Model = model;
            this.MinimumSyllables = 2;
            this.MaximumSyllables = 2;
            this.Random = new Random();
        }

        public Syllabore(ISyllableModel model, INameValidator validator) : this(model)
        {
            if (validator != null)
            {
                this.Validator = validator;
            }
        }

        public string Next()
        {
            var syllableLength = this.Random.Next(this.MinimumSyllables, this.MaximumSyllables + 1);
            return this.Next(syllableLength);
        }

        public string Next(int syllableLength)
        {
            var output = new StringBuilder();
            var validNameGenerated = false;

            while (!validNameGenerated)
            {
                output.Clear();
                for (int i = 0; i < syllableLength; i++)
                {
                    output.Append(Model.NextSyllable());
                }

                if (this.Validator != null)
                {
                    validNameGenerated = this.Validator.IsValidName(output.ToString());
                }
                else
                {
                    validNameGenerated = true;
                }
            }

            return output.ToString().Substring(0, 1) + output.ToString().Substring(1).ToLower();
        }



    }
}
