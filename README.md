[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)

![](https://i.imgur.com/qUMcu2tm.png) 

### What is this?
 * **Syllabore** is a fantasy name generator and class library 
 * It does **not** use pre-made name lists and does **not** access generative AI services
 * It can be embedded into any .NET program and used 100% offline

### How are names generated?
 * Name generation is accomplished by generating syllables from vowel-consonant pools and sequencing them into names
 * **Syllabore** is an example of procedural generation


# Quick Start
```csharp
var g = new NameGenerator();
Console.WriteLine(g.Next());
```
Every call to ``Next()`` on a ```NameGenerator```  will return a different name. An uncustomized name generator will use [all consonants and vowels in the English language](https://github.com/kesac/Syllabore/wiki/Defaults), no transformers, and no filters by default. 

# Tailoring Syllables
Modify a name generator's ```SyllableProvider``` to customize vowels and consonants used in syllable generation:
```csharp
var g = new NameGenerator()
        .UsingProvider(x => x
            .WithVowels("ae")
            .WithConsonants("strmnl"));     
```
This example will create names like:
```
Lena
Salna
Rasse
```
See the [wiki](https://github.com/kesac/Syllabore/wiki/v2.x-Customizing-Syllable-Providers) for more examples on customizing providers. The topics of consonant positioning, vowel/consonant sequences, and grapheme weights are not shown here.

# Using Transformers
Transformers can be used to apply a transform to a name during the generation process:
```csharp
var g = new NameGenerator()
        .UsingProvider(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("str"))
        .UsingTransformer(x => x
            .Select(1).Chance(0.5)
            .WithTransform(x => x.AppendSyllable("gard")).Weight(2)
            .WithTransform(x => x.AppendSyllable("dar")))
        .UsingSyllableCount(3);
```
This example ensures names end up with a specific suffix 50% of the time:
```
Satagard
Resadar
Teregard
```

# Using Filters
Filters can be used to improve output, by preventing specific substrings or patterns from occuring:
```csharp
var g = new NameGenerator()
            .UsingFilter(x => x
                .DoNotAllowEnding("j","p","q","w")
                .DoNotAllowPattern(@"(\w)\1\1")
                .DoNotAllowPattern(@"([^aeiouAEIOU])\1\1\1"));
```
This example avoids awkward sounding endings, avoids any sequence of 3 or more identical letters, and avoids any sequence of 4 or more consonants.

# Putting It All Together
Here is a more complicated name generator that could be suitable for naming cities:
```csharp
var g = new NameGenerator()
        .UsingProvider(p => p
            .WithVowels("aeoy")
            .WithLeadingConsonants("vstlr")
            .WithTrailingConsonants("zrt")
            .WithVowelSequences("ey", "ay", "oy"))
        .UsingTransformer(m => m
            .Select(1).Chance(0.99)
            .WithTransform(x => x.ReplaceSyllable(0, "Gran"))
            .WithTransform(x => x.ReplaceSyllable(0, "Bri"))
            .WithTransform(x => x.InsertSyllable(0, "Deu").AppendSyllable("gard")).Weight(2)
            .WithTransform(x => x.When(-2, "[aeoyAEOY]$").ReplaceSyllable(-1, "opolis"))
            .WithTransform(x => x.When(-2, "[^aeoyAEOY]$").ReplaceSyllable(-1, "polis")))
        .UsingFilter(v => v
            .DoNotAllow("yv", "yt", "zs")
            .DoNotAllowPattern(
                @".{12,}",
                @"(\w)\1\1",              // Prevents any letter from occuring three times in a row
                @".*([y|Y]).*([y|Y]).*",  // Prevents double y
                @".*([z|Z]).*([z|Z]).*")) // Prevents double z
        .UsingSyllableCount(2, 4);
```
This example would create names like:
```
Resepolis
Varosy
Sola 
Grantero
```

# Serialization
The easiest way to capture name generator settings is to just serialize a ```NameGenerator``` object into a json file. You can use the ```NameGeneratorSerializer``` class for this purpose which has a method of dealing with polymorphic deserialization:

```csharp
var g = new NameGenerator();
var s = new NameGeneratorSerializer();

// Write the name generator to disk
s.Serialize(g, "name-generator.json");

// Load the json file and generate a new name from it
var g2 = s.Deserialize("name-generator.json");
Console.WriteLine(g2.Next());
```
# Advanced Use
[See the wiki for more information.](https://github.com/kesac/Syllabore/wiki)

# Installation
Syllabore is available as a NuGet package. You can install it from your [NuGet package manager in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) (search for "Syllabore") or by running the following command in your NuGet package manager console:
```
Install-Package Syllabore
```

# License
```
MIT License

Copyright (c) 2019-2023 Kevin Sacro

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
