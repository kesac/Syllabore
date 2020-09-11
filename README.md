# Syllabore
Syllabore is a fantasy name generator and class library. Name generation is accomplished by constructing individual syllables then sequencing them to form names. Syllables are generated from *SyllableProviders* that maintain their own pool of vowels and consonants. *NameValidators* can be added to the name generation process which adds the extra step of validating syllable and letter sequences during generation to avoid undesirable letter combinations and improve the quality of output.

[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)


## Quick Start
If you're looking for a quick way to try this library without dealing with customizing vowel/consonant pools or loading configuration files, just instantiate *NameGenerator* as-is:
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
A vanilla *NameGenerator* will default to using *StandaloneSyllableProvider* for syllable generation. It will also not have a name validator to improve output quality.

Normally the *NameGenerator* constructor takes a a provider and validator. There are "standalone" classes included in this library for quick and dirty use. It is recommended you create your own by implementing the *ISyllableProvider* and *INameValidator* interfaces, or deriving from *ConfigurableSyllableProvider* and *ConfigurableNameValidator*

The following example creates a generator identical to the first example except the validator is being used to check for awkward name endings:
```csharp
var provider = new StandaloneSyllableProvider();
var validator = new StandaloneNameValidator();

var g = new NameGenerator(provider, validator);
```



## Configuring Output

Name generator configuration can be captured in an XML file then loaded through the *XmlFileLoader* class:

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
A syllabore file defines valid vowels, consonants, character sequences, invalid patterns, and sequence frequency. Here is an example XML file that corresponds to the *StandaloneSyllableProvider* and *StandaloneNameValidator*:
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

## Sample Output with StandaloneSyllableProvider and StandaloneNameValidator
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
## But I don't like XML
If you don't want to deal with XML, you can also build a name generator programmatically. Here is a quick and dirty example:
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
    .SetSyllableLength(3);
```
(This example would create names like Tetara, Resata, Rerere, etc.)

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

