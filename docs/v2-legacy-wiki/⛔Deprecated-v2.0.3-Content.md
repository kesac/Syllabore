## Putting It All Together
Here is a more complicated name generator that could be suitable for naming cities:
```csharp
var g = new NameGenerator()
        .UsingProvider(p => p
            .WithVowels("aeoy")
            .WithLeadingConsonants("vstlr") // Only used to start a syllable
            .WithTrailingConsonants("zrt")  // Only used to end a syllable
            .WithVowelSequences("ey", "ay", "oy"))
        .UsingTransformer(m => m
            .Select(1).Chance(0.99) // 99% chance to choose 1 transform
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
Grantero
```

Check out the [wiki](https://github.com/kesac/Syllabore/wiki) for more guides!