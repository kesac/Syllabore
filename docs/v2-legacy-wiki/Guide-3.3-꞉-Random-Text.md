# Overview
Syllabore was not intended for random text generation, but it can be configured to do so. This is a short guide for creating a name generator that outputs text like so:
```
KVKPMT5ESPFIPKODC
1R78ICK8LI5OB2OFR
85JRJM0PWA7Y6V
```

# Guide
We'll create a new class for random text generation, but derive from Syllabore's built-in `NameGenerator` to save time. Then we can do our customization inside the constructor for simplicity.

```C#
    public class RandomTextGenerator : NameGenerator
    {
        public RandomTextGenerator()
        {
            // We'll do stuff in here
        }
    }
```

We'll want to do the following things in the constructor:
1. Create a `SyllableGenerator` whose pool of vowels and consonants are identical
2. Set a minimum string length by giving a syllable length

We need to instantiate a `SyllableGenerator` to declare the letters we want in text generation. We'll use all alphanumeric characters in the English language:
```C#
    public class RandomTextGenerator : NameGenerator
    {
        private const string AlphanumericCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public RandomTextGenerator()
        {
            var syllables = new SyllableGenerator()
                .WithConsonants(AlphanumericCharacters)
                .WithVowels(AlphanumericCharacters);

            this.UsingSyllables(syllables);
        }
    }
```
This generates the following text as-is:
```
Ivo3
8vtoj
7tvuu
```

This seems like good start, but we'll want longer strings. Let's configure the syllable count to do this:
```C#
    public class RandomTextGenerator : NameGenerator
    {
        private const string AlphanumericCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public RandomTextGenerator()
        {
            var syllables = new SyllableGenerator()
                .WithConsonants(AlphanumericCharacters)
                .WithVowels(AlphanumericCharacters);

            this.UsingSyllables(syllables);

            this.UsingSyllableCount(8);// Note that syllable count is not the same as string length
        }
    }
```
Our generator now produces text like this:
```
Sm5xk728q1ejs50h
6rengeq2elg2ioobp
O39pc2rm0d6rc3jz
```

To use our generator, we simply instantiate it and make a call to `Next()`. We can also turn the output into all uppercase characters at this step.
```C#
var g = new RandomTextGenerator();
Console.WriteLine(g.Next().ToUpper());
```
This method produces text that matches our desired format:
```
GRCEY71CCI9IPE6K
HT9RP5JP86TWQZNCI4
OXJBRUE4ZP5TF0Y
```
