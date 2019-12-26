# Syllabore
A C# library for generating fantasy names. Name generation is done by randomly constructing syllables and joining them. Syllables are formed from a customizable pool of vowels and consonants. Names are checked against predefined constraints to improve the quality of output.

## Quick Start
The recommended method for creating name generators is through an XML definition file and the *XmlFileLoader* class. If you're looking for a quick way to try this library without loading external files, you can use the standalone provider and validator:
```csharp
public static void Main(string[] args)
{
    var provider = new StandaloneSyllableProvider();
    var validator = new StandaloneNameValidator();

    var names = new NameGenerator(provider, validator);

    for (int i = 0; i < 100; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```

## Configuring Output

Name generator settings can be defined in an XML definition file then loaded through the *XmlFileLoader* class:

```csharp
public static void Main(string[] args)
{
    var file = new XmlFileLoader("data/basic.xml");
    file.Load();

    var names = file.GetNameGenerator("BasicNameGenerator");

    for (int i = 0; i < 10; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```
An XML definition file provides valid vowels, consonants, character sequences, invalid patterns, and letter frequency. Here is an example XML file that corresponds to the *StandaloneSyllableProvider* and *StandaloneNameValidator*:
```xml
<syllabore>
  <define name="BasicNameGenerator">
    <components>
      <add type="Vowels" values="a e i o u"/>
      <add type="VowelSequences" values="ae ea ai ia au ay ie oi ou ey"/>
      <add type="StartingConsonants" values="b c d f g h j k l m n p q r s t v w x y z"/>
      <add type="StartingConsonantSequences" values="ch sh bl cl fl pl gl br cr"/>
      <add type="StartingConsonantSequences" values="dr pr tr th sc sp st sl spr"/>
      <add type="EndingConsonants" values="b c d f g h k l m n p r s t v x y"/>
      <add type="EndingConsonantSequences" values="ck st sc ng nk rsh lsh rk rst nct xt"/>
    </components>
    <constraints>
      <constrain when="NameEndsWith" values="j p q v w z"/>
      <constrain when="NameMatchesRegex" regex="([a-zA-Z])\1\1"/>
    </constraints>
    <probability>
      <set type="StartingVowelProbability" value="0.10"/>
      <set type="StartingConsonantSequenceProbability" value="0.20" />
      <set type="VowelSequenceProbability" value = "0.20" />
      <set type="EndingConsonantProbability" value = "0.10" />
      <set type="EndingConsonantSequenceProbability" value = "0.10" />
    </probability>
  </define>
</syllabore>
```

## Sample Output with Basic Provider and Validator
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

# License

MIT License

Copyright (c) 2019 Kevin Sacro

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

