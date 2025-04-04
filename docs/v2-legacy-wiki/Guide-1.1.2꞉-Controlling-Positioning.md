## Consonant Positioning
If you are configuring the ```SyllableGenerator``` directly, you will have the ability to influence consonant positioning. 

Recall that a syllable consists of three parts:
 * A vowel (or _nucleus_)
 * An optional consonant before the vowel (also called an _onset_)
 * An optional consonant after the vowel (also called a _coda_)

When tailoring the characters used in name generation, you should specify the consonants occurring before and after a vowel separately:
```csharp
var g = new NameGenerator()
        .UsingSyllables(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("str")    // Onsets
            .WithTrailingConsonants("mnl")); // Codas
```
In this example, the characters `s` `t` `r` can start a syllable while the characters `m` `n` `l` can end a syllable. Calling ```Next()``` on this generator will produce names like:
```
Sara
Taen
Ralte
```

In Syllabore, _onsets_ are called _leading consonants_ and _codas_ are called _trailing consonants_.