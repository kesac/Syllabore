## Character Frequency
 * Adjusting character frequency helps fine-tune name generator output
 * Increasing a character's _weight_ increases the frequency it appears
 * By default, all characters have the same weight of 1

To adjust a character's weight, call ```Weight(int)``` when you declare characters through your ```SyllableProvider```:
```csharp
var g = new NameGenerator()
        .UsingSyllables(x => x
            .WithVowels("a").Weight(4)
            .WithVowels("e").Weight(1)
            .WithLeadingConsonants("str"));
```
In this example, the name generator will use vowel ```a``` 4 times more likely than an ```e```.

### Tip❕
 * Weights are used instead of percentages because it is easier to see ratios between characters
 * If you want to use percentages, just make sure your weights add up to 100
 * For example, if you want vowel ```a``` to be chosen 80% of the time and ```e``` 20% of the time, set their weights to 80 and 20 respectively