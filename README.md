# Syllabore
Syllabore is a fantasy name generator and class library. 

Name generation is accomplished by generating syllables from vowel-consonant pools and sequencing them into names. Syllabore does not use lists of pre-made names.

These are concepts Syllabore uses when generating names:
* **Providers** - used to randomly generate syllables. Providers maintain vowel-consonant pools used to construct syllables from scratch
* **Mutators** - an optional mechanism to randomly adjust *or mutate* names during the generation process. This is useful in iterating or evolving a name by replacing syllables, swapping a vowel for another vowel, adding new components to the name, etc.
* **Validators** - an optional mechanism to validate syllable or letter sequences during name generation. A name generator will only output names that pass through its  validator. This is useful in avoiding undesirable letter combinations and improve the quality of output

[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)


## Quick Start
A generator is setup to work without any additional configuration. Just instantiate ```NameGenerator``` as-is and start calling ``Next()``:
```csharp
var g = new NameGenerator();

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(g.Next());
}

```
A default ```NameGenerator``` uses ```DefaultSyllableProvider``` for syllable generation, will not have a mutation step, and will not have a name validator.

## Customizing Name Generation
You can customize a name generator programmatically. Here is a basic three-syllable name generator:
```csharp
var g = new NameGenerator()
    .UsingProvider(x => x
        .WithLeadingConsonants("str")
        .WithVowels("ae"))
    .LimitSyllableCount(3);
```
This example would create names like:
```
Tetara
Resata
Resere
Sasata
```

Here is a more complicated name generator that could be suitable for naming cities:
```csharp
var g = new NameGenerator()
    .UsingProvider(p => p
        .WithVowels("aeoy")
        .WithLeadingConsonants("vstlr")
        .WithTrailingConsonants("zrt")
        .WithVowelSequences("ey", "ay", "oy"))
    .UsingMutator(m => m
        .WithMutation(x => { x.Syllables[0] = "gran"; })
        .WithMutation(x => x.Syllables.Add("opolis")).When(x => x.EndsWithConsonant())
        .WithMutation(x => x.Syllables.Add("polis")).When(x => x.EndsWithVowel())
        .WithMutationCount(1))
    .UsingValidator(v => v
        .Invalidate(
            @"(\w)\1\1",             // no letters three times in a row
            @"([^aeoyAEOY])\1",      // no consonants twice in a row
            @".*([y|Y]).*([y|Y]).*", // two y's in same name
            @".*([z|Z]).*([z|Z]).*", // two z's in same name
            @"(zs)",                 // this just looks weird
            @"(y[v|t])"))            // this also looks weird 
    .LimitMutationChance(0.50)
    .LimitSyllableCount(2, 3);
```
This example would create names like:
```
Resepolis
Varosy
Sola 
Grantero
```

## Capturing configuration in XML
Name generator configurations can be captured in an XML file then loaded through an ```XmlFileLoader``` class:

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
A syllabore XML file defines valid vowels, consonants, character sequences, invalid patterns, and sequence frequency. Here is an example XML file that corresponds to the ```DefaultSyllableProvider``` implementation, but also checks for awkward letter sequences and name endings:
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
This example would create names like:
```
Naci, Xogud, Beqae, Crovo, Garu, Lultu, Laibu, Glowia, Goscuc, Tevu
Zimvu, Druhhest, Sumae, Vumnih, Gefa, Duvu, Qiclou, Najost, Lidfo, Godrest
Bebin, Zaprey, Thopea, Wiqa, Clunust, Jodo, Jita
```

## Mutators and Creating Variations
You can use Syllabore to produce variations of a specific name by accessing mutators directly. Here is a quick example of producing variations:
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
In this example, the ```NameGenerator``` defaults to using ```DefaultNameMutator``` for its mutator. Mutation will not occur on calls to ```NextName()``` because a default ```NameGenerator``` has a mutation chance to 0%. You can increase the percentage or call ```NextVariation()``` directly to create a variation of a name.

Here is another example of configuring the mutation step of a ```NameGenerator```:
```csharp
var g = new NameGenerator()
    .UsingMutator(new VowelMutator())
    .LimitMutationChance(0.25);
```
In this example, the name generator has been given a custom mutation step in which one vowel of one syllable in a name is changed to different vowel. This is set to occur 25% of the time whenever ```Next()``` is called.

# Installation
The easiest way to add this to your project is through NuGet Package Manager (search for "Syllabore"). Visit https://www.nuget.org/packages/Syllabore/ for details. 

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

