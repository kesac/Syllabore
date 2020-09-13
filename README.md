# Syllabore
Syllabore is a fantasy name generator and class library. Name generation is accomplished by constructing individual syllables then sequencing them to form names. 

These are concepts that Syllabore uses when generating names:
* *SyllableProviders* - used to generate syllables. Providers maintain their own pool of vowels and consonants
* *NameValidators* - used to add an extra step of validating syllable or letter sequences during name generation. This is useful in avoiding undesirable letter combinations and improve the quality of output
* *NameShifters* - used to produce variations on a name. This is useful in iterating or evolving a name by replacing individual syllables or letters

[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)


## Quick Start
The generator is setup to work without any configuration; just instantiate *NameGenerator* as-is:
```csharp
public static void Main(string[] args)
{
    var g = new NameGenerator();

    for (int i = 0; i < 10; i++)
    {
        Console.WriteLine(g.Next());
    }
}
```
A vanilla *NameGenerator* will default to using *DefaultSyllableProvider* for syllable generation. It will also not have a name validator.

## Customizing Name Generation
You can customize a name generator programmatically. Here is a quick and dirty example:
```csharp
var g = new NameGenerator()
    .SetProvider(new ConfigurableSyllableProvider()
        .AddLeadingConsonant("s", "t", "r")
        .AddVowel("a", "e")
        .SetVowelSequenceProbability(0.20)
        .AddTrailingConsonant("z")
        .SetTrailingConsonantProbability(0.10)
        .AllowVowelSequences(false)
        .AllowLeadingConsonantSequences(false)
        .AllowTrailingConsonantSequences(false))
    .SetValidator(new ConfigurableNameValidator()
        .AddRegexConstraint("zzz")
        .AddRegexConstraint("[q]+"))
    .SetSyllableCount(3);

// (This example would create 3-syllable names like Tetara, Resata, Resere, etc.)
```

## Capturing configuration in XML
Name generator configurations can be captured in an XML file then loaded through an *XmlFileLoader* class:

```csharp
public static void Main(string[] args)
{
    var file = new XmlFileLoader("data/basic.xml").Load();
    var names = file.GetNameGenerator("BasicNameGenerator");

    for (int i = 0; i < 10; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```
A syllabore XML file defines valid vowels, consonants, character sequences, invalid patterns, and sequence frequency. Here is an example XML file that corresponds to the *DefaultSyllableProvider* implementation, but also checks for awkward letter sequences and name endings:
```xml
<syllabore>
  <define name="BasicNameGenerator">
    <components>
      <add type="Vowels" values="a e i o u"/>
      <add type="VowelSequences" values="ae ea ai ia au ay ie oi ou ey"/>
      <add type="LeadingConsonants" values="b c d f g h j k l m n p q r s t v w x y z"/>
      <add type="LeadingConsonantSequences" values="ch sh bl cl fl pl gl br cr"/>
      <add type="LeadingConsonantSequences" values="dr pr tr th sc sp st sl spr"/>
      <add type="TrailingConsonants" values="b c d f g h k l m n p r s t v x y"/>
      <add type="TrailingConsonantSequences" values="ck st sc ng nk rsh lsh rk rst nct xt"/>
    </components>
    <constraints>
      <invalid if="NameEndsWith" values="j p q v w z"/>
      <invalid if="NameMatchesRegex" regex="([a-zA-Z])\1\1"/>
    </constraints>
    <probability>
      <set type="LeadingVowelProbability" value="0.10"/>
      <set type="LeadingConsonantSequenceProbability" value="0.20" />
      <set type="VowelSequenceProbability" value="0.20" />
      <set type="TrailingConsonantProbability" value="0.10" />
      <set type="TrailingConsonantSequenceProbability" value="0.10" />
    </probability>
  </define>
</syllabore>
```
## Sample Output
```
Naci
Xogud
Beqae
Crovo
Garu
Lultu
Laibu
Glowia
Goscuc
Tevu
Zimvu
Druhhest
Sumae
Vumnih
Gefa
Duvu
Qiclou
Najost
Lidfo
Godrest
Bebi
Zaprey
Thopea
Wiqa
Clunust
Jodo
Jita
```

## Shifters and Creating Variations
Syllabore can also produce variations of names by using *Shifters*. By default, a *NameGenerator* will use the *DefaultSyllableShifter*. Here is a quick example of producing variations:
```csharp
var g = new NameGenerator();

for(int i = 0; i < 3; i++)
{
    var name = g.NextName();
    Console.WriteLine(name);

    for (int j = 0; j < 4; j++)
    {
        // Variations will be different to the original
        // in that one syllable is replaced with a new
        // randomly generated syllable
        var variation = g.NextVariation(name);
        Console.WriteLine(variation);
    }
}

```
You can create your own custom shifters and configure *NameGenerator* to use them. For example:
```csharp
// Creates a name generator that uses both DefaultSyllableShifter and
// VowelShifter for creating variations of names.
var g = new NameGenerator()
    .SetShifter(new MultiShifter()
        .Using(new DefaultSyllableShifter())
        .Using(new VowelShifter()));
```
# Installation
The easiest way to add this to your project is through NuGet Package Manager (search for "Syllabore"). Visit https://www.nuget.org/packages/Syllabore/ for more details. 

# License

MIT License

Copyright (c) 2019-2020 Kevin Sacro

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

