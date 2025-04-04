# Overview
This is a guide for creating a generator suitable for spaceships in the format "`<military-like prefix>` `<space-sounding-name>`". The resulting generator will output names like:
```
NNX Tanorus
VSS Citara
AVS Sonovium
```
## Table of Contents
* [Part 1: Initializing a composite generator](#part-1)
* [Part 2: Shaping the prefix](#part-2)
* [Part 3: Improving prefix output](#part-3)
* [Part 4: Shaping ship names](#part-4)
* [Part 5: Adding specific suffixes](#part-1)


# Part 1
The style of name we're looking for will require two generators: one for the acronym part then a separate one for the actual name. We can begin by constructing a composite generator that uses two `NameGenerators` internally.

```C#
    public class SpaceshipGenerator
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        public SpaceshipGenerator()
        {
            _prefixGenerator = new NameGenerator();
            _shipGenerator = new NameGenerator();
        }

        public string Next()
        {
            var prefix = _prefixGenerator.Next().ToUpper();
            var ship = _shipGenerator.Next();

            return String.Format("{0} {1}", prefix, ship);
        }
    }
```
Every call to `Next()` on an instance of our custom class will return random names like:
```
RUMOU Zaivi
CHAUWA Troflau
PLUCHI Hoivea
```
It's a start, but we'll want to shape the output by customizing our two `NameGenerators`. Note that a default `NameGenerator` uses [all consonants and vowels in the English language](https://github.com/kesac/Syllabore/wiki/Defaults). This is why our custom class is capable of generating names without any customization yet.

# Part 2
Let's start shaping the prefix. For the prefix, we're only interested in generating 3-letter military-sounding acronyms. We don't really need all letters in the alphabet to do this, so we'll choose a subset that look cool when put side by side. For simplicity, we'll change the syllable probabilities so that only consonants are used. This lets us define all our letters as consonants out of simplicity.
```C#
        public SpaceshipGenerator()
        {
            _prefixGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithConsonants("UVXSHMLAMN")
                    .WithProbability(x => x
                        .OfVowels(0.0) // These methods take a value 0.0 through 1.0 indicating probability
                        .OfLeadingConsonants(1.0) // So a 1.0 equates to 100% usage
                        .OfTrailingConsonants(0.0))); // And 0.0 equates to 0% usage

            _shipGenerator = new NameGenerator();
        }
```
Making the changes above changes our output to look like the following:
```
SS Yuspai
ML Wiaclu
NS Raki
```
Which is sort of ok, but the prefix can still be made better.

# Part 3
Let's improve the prefix by doing the following things:
1. Make cooler letters get used more often than less cool letters
2. Set the prefix length to 3 letters because it looks nicer
3. Filter out prefixes composed of only one letter since we're not interested in something like "AAA"

We can introduce these improvements like so:
```C#
        public SpaceshipGenerator()
        { 
            _prefixGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithConsonants("SHMLAMN").Weight(1)
                    .WithConsonants("UVX").Weight(2) // These letters will appear twice more likely than others
                    .WithProbability(x => x
                        .OfVowels(0.0)
                        .OfLeadingConsonants(1.0)
                        .OfTrailingConsonants(0.0)))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1\1")) // Regex for three consecutive same letters
                .UsingSyllableCount(3); // In our case, this changes prefix length, not syllable count

            _shipGenerator = new NameGenerator();
        }
```
Using this in our custom class, we can now get names with proper 3-letter suffixes like:
```
VLU Pleatrae
ASX Glipe
AMV Ouria
```

# Part 4
We now begin shaping the name of the ship. We'll try to have some thematic cohesion in the names and we'll also try making it roughly space sounding.  

The way to do this is to ensure we are selecting a specific subset of vowels and consonants for the generator to use. A smaller pool of vowels and consonants means a more cohesive, but less creative output. We'll ensure the pool is not too small. 

For our core vowels, we'll use only three:
* a
* o
* i

And for the vowels e and u, we'll only permit them to show up in vowel sequences:
* ei
* ia
* ou
* eu

For our consonants, we'll make a few stylistic decisions:
* We'll avoid using 'b' and use 'p' in its place
* We'll avoid using 'k' and use 'c' in its place
* We'll allow harsher sounding consonants, but make them show up less

Putting that all together, we can modify the ship generator like so:
```C#
            _shipGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels("aoi") // Don't need to separate vowels with commas if they're not sequences
                    .WithVowelSequences("ei", "ia", "ou", "eu") // For sequences though, we'll obviously need the commas
                    .WithLeadingConsonants("rstlmn").Weight(4) // Weights are relative to each other
                    .WithLeadingConsonants("cdgp").Weight(2)) // So this line says "2 out of 6" or 33% of the time
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1")) // Regex again, for two of the same letters consecutively
                .UsingSyllableCount(3);
```
Our generator can now produces names like:
```
VSS Citara
HHM Raseiri
NMX Lovina
```

# Part 5
Sometimes it can be beneficial to insert deterministic patterns into names if we have them in mind. Suffixes like `-rus`, `-des`, and `-vium` are easy ways to make the names even more space sounding. We can insert these suffixes in 50% of output using a transformer:

```C#
            _shipGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels("aoi")
                    .WithVowelSequences("ei", "ia", "ou", "eu")
                    .WithLeadingConsonants("rstlmn").Weight(4)
                    .WithLeadingConsonants("cdgp").Weight(2))
                .UsingTransform(0.5, new TransformSet() // Only allow transform to be used 50% of the time
                    .RandomlySelect(1) // Only apply one transform
                    .WithTransform(x => x.ReplaceSyllable(-1, "des")) // Index -1 is the last position
                    .WithTransform(x => x.ReplaceSyllable(-1, "rus"))
                    .WithTransform(x => x.ReplaceSyllable(-1, "vium")))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1"))
                .UsingSyllableCount(3);
```
Our generator can now produce names like:
```
MSX Igito
HSA Tisavium
SAU Solarus
```

# Closing
The final state of our custom class is now this:
```C#
    public class SpaceshipGenerator
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        public SpaceshipGenerator()
        {
            _prefixGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithConsonants("SHMLAMN").Weight(1)
                    .WithConsonants("UVX").Weight(2)
                    .WithProbability(x => x
                        .OfVowels(0.0)
                        .OfLeadingConsonants(1.0)
                        .OfTrailingConsonants(0.0)))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1\1"))
                .UsingSyllableCount(3);

            _shipGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels("aoi")
                    .WithVowelSequences("ei", "ia", "ou", "eu")
                    .WithLeadingConsonants("rstlmn").Weight(4)
                    .WithLeadingConsonants("cdgp").Weight(2))
                .UsingTransform(0.5, new TransformSet() // Only allow transform to be used 50% of the time
                    .RandomlySelect(1) // Only apply one transform
                    .WithTransform(x => x.ReplaceSyllable(-1, "des")) // Index -1 is the last position
                    .WithTransform(x => x.ReplaceSyllable(-1, "rus"))
                    .WithTransform(x => x.ReplaceSyllable(-1, "vium")))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1"))
                .UsingSyllableCount(3);
        }

        public string Next()
        {
            var prefix = _prefixGenerator.Next().ToUpper();
            var ship = _shipGenerator.Next();

            return String.Format("{0} {1}", prefix, ship);
        }

    }
```

To use it, we create an instance of our generator and then call `Next()` like so:

```C#
var g = new SpaceshipGenerator();
Console.WriteLine(g.Next());
```