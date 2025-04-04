# Overview
This is a guide for building a name generator that might be suitable for planets. This guide's resulting generator will output names like:
```
Zadol
Hadarden
Risar
Tasuda
```
## Table of Contents
* [Part 1: Creating a name generator](#part-1)
* [Part 2: Using filtering](#part-2)
* [Part 3: Using weighting](#part-3)

# Part 1
We'll create a new class for our planet namer, but derive from Syllabore's built-in `NameGenerator` to save time. Then we can do our customization inside the constructor for simplicity.

```C#
    public class PlanetGenerator : NameGenerator
    {
        public PlanetGenerator()
        {
            // We'll do stuff in here
        }
    }
```

To start off, we'll make our custom generator:
1. Allow names to be made of two or three syllables
2. Make use of all vowels and consonants in the English alphabet

We need to instantiate a `SyllableGenerator` to declare the letters we want. Our first iteration will look like so:
```C#
    public class PlanetGenerator : NameGenerator
    {
        public PlanetGenerator()
        {
            this.UsingSyllableCount(2, 3);

            var s = new SyllableGenerator();
                    .WithVowels("aieou");
                    .WithConsonants("bcdfghjklmnpqrstvwxyz");

            this.UsingSyllables(s);
        }
    }
```

Currently, the consonants we've defined will be used for both leading or trailing positions. Some letters will look awkward in trailing positions, so instead of using `WithConsonants()` we'll use `WithLeadingConsonants()` and `WithTrailingConsonants()` for finer control:
```C#
var s = new SyllableGenerator();
        .WithVowels("aieou");
        .WithLeadingConsonants("bcdfghklmnpqrstvxyz");
        .WithTrailingConsonants("cdfgklmnprstv");
```

Finally, let's ensure that our generator makes use of a trailing consonant 50% of the time when creating a syllable.
```C#
s.WithProbability(x => x.OfTrailingConsonants(0.50));
```
Putting it all together, our custom generator now looks like so:
```C#
    public class PlanetGenerator : NameGenerator
    {
        public PlanetGenerator()
        {
            this.UsingSyllableCount(2, 3);

            var s = new SyllableGenerator()
                    .WithVowels("aieou")
                    .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                    .WithTrailingConsonants("cdfgklmnprstv")
                    .WithProbability(x => x.OfTrailingConsonants(0.50));

            this.UsingSyllables(s);
        }
    }
```
Or in a more compact way:
```C#
    public class PlanetGenerator : NameGenerator
    {
        public PlanetGenerator()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));
        }
    }
```
In its current state, our generator outputs names like:
```
Rarevik
Terxolfos
Koqomzof
```
Which is a little too wild for our tastes, so in the next step we'll focus on filtering out patterns we're not interested in.

# Part 2
We can use a `NameFilter` to define substrings or patterns we do not want to appear in generator output. For starters, while the letters `f`, `g`, `j`, and `v` are ok to be used to at the end of a syllable (the trailing position), they look awkward when they end the entire name.

We'll want to avoid these endings and we can use a `NameFilter` in our constructor to do this:
```C#
        public PlanetGenerator()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "j", "v");

            this.UsingFilter(f);
        }
```
Names with three or more consonants in a row are also difficult to read. Our `NameFilter` allows us to also identify patterns we want to avoid through regular expressions:
```C#
f.DoNotAllow("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row
```
For no other reason than aesthetics, let's go ahead and prevent the following patterns from also appearing:
* The letter `q` followed by anything other than a `u`
* The letter `w` preceded by anything besides `t`, `s`, `a`, or `o`
* Weird double consonant combinations like `zz` and `xx`
* The letter `y` followed by anything other than a vowel
* The letter `p` followed by anything other than a vowel or select consonants

Putting that all together, our constructor now looks like so:
```C#
    public class PlanetGenerator : NameGenerator
    {

        public PlanetGenerator()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "j", "v");
            f.DoNotAllow("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row

            f.DoNotAllow("(q[^u])"); // Q must always be followed by a u
            f.DoNotAllow("([^tsao]w)"); // W must always be preceded with a t, s, a, or o

            // Some awkward looking combinations
            f.DoNotAllowSubstring("pn", "zz", "yy", "xx");
            f.DoNotAllow("(y[^aeiou])"); // Avoids things like yt, yw, yz, etc.
            f.DoNotAllow("(p[^aeioustrlh])"); // Avoids things like pb, pq, pz, etc.

            this.UsingFilter(f);
        }

    }
```
And we can restructure this with method chaining to make it like so:
```C#
    public class PlanetGeneratorV2_4 : NameGenerator
    {

        public PlanetGeneratorV2_4()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            this.UsingFilter(x => x
                .DoNotAllowEnding("f", "g", "j", "v")
                .DoNotAllow("([^aieou]{3})")
                .DoNotAllow("(q[^u])")
                .DoNotAllow("([^tsao]w)")
                .DoNotAllowSubstring("pn", "zz", "yy", "xx")
                .DoNotAllow("(y[^aeiou])")
                .DoNotAllow("(p[^aeioustrlh])"));
        }

    }
```
Our generator now outputs names like
```
Vela
Zenistan
Bemil
```
# Part 3
Our generator looks ready to go, but we can make use of one more feature that Syllabore supports: grapheme weighting.

When we define vowels or consonants, by default they are given equal probability of being used to construct a syllable. Syllabore allows us to set weights to individual vowels or consonants (also known as graphemes) to affect how often they surface in syllables.

Returning to our constructor, we can set weights simply by calling `Weight()` after letters are defined like so:
```C#
this.UsingSyllables(x => x
    .WithVowels("aie").Weight(2)
    .WithVowels("ou").Weight(1)
    .WithLeadingConsonants("bcdfghlmnprstvy").Weight(2)
    .WithLeadingConsonants("qkxz").Weight(1)
    .WithTrailingConsonants("dlmnprst").Weight(2)
    .WithTrailingConsonants("cdfkvg").Weight(1)
    .WithProbability(x => x.OfTrailingConsonants(0.50)));
```
# Closing
The final state of our custom name generator is:
```C#
public class PlanetGenerator: NameGenerator
    {
        public PlanetGenerator()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aie").Weight(2)
                .WithVowels("ou").Weight(1)
                .WithLeadingConsonants("bcdfghlmnprstvy").Weight(2)
                .WithLeadingConsonants("qkxz").Weight(1)
                .WithTrailingConsonants("dlmnprst").Weight(2)
                .WithTrailingConsonants("cdfkvg").Weight(1)
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            this.UsingFilter(x => x
                .DoNotAllowEnding("f", "g", "j", "v")
                .DoNotAllow("([^aieou]{3})")
                .DoNotAllow("(q[^u])")
                .DoNotAllow("([^tsao]w)")
                .DoNotAllowSubstring("pn", "zz", "yy", "xx")
                .DoNotAllow("(y[^aeiou])")
                .DoNotAllow("(p[^aeioustrlh])"));
        }

    }
```
And to use it, we simply instantiate it and make a call to `Next()`
```C#
var g = new PlanetGenerator();
Console.WriteLine(g.Next());
```