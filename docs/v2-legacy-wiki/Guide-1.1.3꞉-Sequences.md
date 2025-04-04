## Clusters/Sequences
 * A syllable's _nucleus_ or vowel can be a *cluster* of more than character (eg. "team", "rain", "look")
 * Similarly, an _onset_ or _leading consonant_ can be a cluster (eg. "chat", "wheel", "shot")
 * Finally, a _coda_ or _trailing consonant_ can be a cluster (eg. "beach", "mark", "tell")

In Syllabore, _clusters_ are called _sequences_ and you can define them separately from normal vowels and consonants if you wish to use them:
```csharp
var g = new NameGenerator()
        .UsingSyllables(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("str")
            .WithTrailingConsonants("mnl")
            .WithVowelSequences("ou", "ui")              // Vowel clusters, separate with commas
            .WithLeadingConsonantSequences("wh", "fr")   // Onset clusters
            .WithTrailingConsonantSequences("ld","rd")); // Coda clusters
```
Or in a slightly more compact way:
```csharp
var g = new NameGenerator()
        .UsingSyllables(x => x
            .WithVowels("ae").Sequences("ou", "ui")
            .WithLeadingConsonants("str").Sequences("wh", "fr")
            .WithTrailingConsonants("mnl").Sequences("ld", "rd"));
```
Calling ```Next()``` on this generator will produce names like:
```
Soura
Tesard
Raldren
```