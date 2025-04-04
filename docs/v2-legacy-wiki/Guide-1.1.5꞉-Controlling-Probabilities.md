## Probabilities
A ```NameGenerator``` will allow you to define how often specific consonants positions occur, how often sequences appear, and more. This is done by calling ```UsingProbability``` to set desired probabilities.

Here is an example of customizing probabilities for leading and trailing consonants:
```csharp
var g = new NameGenerator()
        .UsingSyllables(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("str")
            .WithTrailingConsonants("mnl"))
        .UsingProbability(x => x
            .OfLeadingConsonants(1.0)     // Onsets appear 100% of the time
            .OfTrailingConsonants(0.20)); // Codas appear 20% of the time
```
Probability values must be a ```double``` between 0.0 and 1.0 inclusive. A value of 0.0 corresponds to 0% probability and 1.0 corresponds to 100% probability.

In example above, the ```NameGenerator``` will use syllables that always have a leading consonant, but only have a trailing consonant 20% of the time.

Syllabore allows you to adjust the probabilities for the following settings:
| For each syllable, the probability that... | Method to Call | Default value if characters added, but custom probability not supplied |
| ------- | -------------- | ------------- |
| ...a leading consonant exists | ```OfLeadingConsonants(double)``` | 0.95 |
| ...a leading consonant is a sequence | ```OfLeadingConsonantIsSequence(double)``` | 0.25 |
| ...the vowel exists | ```OfVowels(double)``` | 1.00 |
| ...the vowel is a sequence | ```OfVowelIsSequence(double)``` | 0.25 |
| ...a trailing consonant exists | ```OfTrailingConsonants(double)``` | 0.10 |
| ...a trailing consonant is a sequence | ```OfTrailingConsonantIsSequence(double)``` | 0.25 |

| For each *starting* syllable, the probability that... | Method to Call | Default value if characters added, but custom probability not supplied |
| ------- | -------------- | ------------- |
| ...a leading vowel exists | ```OfLeadingVowelsInStartingSyllable(double)``` | 0.0 |
| ...a leading vowel is a sequence | ```OfLeadingVowelIsSequenceInStartingSyllable(double)``` | 0.0 |
