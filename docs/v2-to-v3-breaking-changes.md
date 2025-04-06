# Overview
This document describes breaking changes between Syllabore's v2 and v3 releases. This document does not contain v3 docs. You can find v3 docs in the gitbook here: [https://sacro.gitbook.io/syllabore](https://sacro.gitbook.io/syllabore).

# Breaking Change 1: Name Generator Constructors
In v2, you could configure a `NameGenerator` without explicitly configuring `SyllableGenerators` through quick setup constructors.

In v3, quick setup constructors still exist, but:
* The order of arguments has swapped
* Consonants must be specified as leading or trailing. It no longer uses them automatically for both positions.

❌ This example no longer generates names like `Lena` or `Rasa`. Instead it will generate names like `Aren` or `Elan`: 
```C#
var g = new NameGenerator("ae", "srnl");
Console.WriteLine(g.Next());
```

✅ This is because the first and second arguments have swapped positions. To generate names like `Lena` or `Rasa`, you will need to do the following:
```C#
var names = new NameGenerator("srnl", "ae");
```

🆕 The consonant symbols will only be used in the leading position. To specify trailing consonants in quick setup constructors, provide them as a third argument:
```C#
var names = new NameGenerator("srnl", "ae", "lmt");
```

The example above will generate names like `Lemnal` or `Salnat`.

**Rationale**
- You can now specify trailing consonants in quick setup constructors
- It's easier to read these constructors and guess what names will be produced without running them
- For example, the v3 `NameGenerator` below always produces the names "Tuk", "Tuktuk", or  "Tuktuktuk"
```C#
var names = new NameGenerator("t", "u", "k").SetSize(1, 3);
```
# Breaking Change 2: Implicit Generator Settings
In v2, you could instantiate a `NameGenerator` and use it right away without specifying the symbols or `SyllableGenerators` to use. This is because a `DefaultSyllableGenerator` would be used by default.

In v3, a ```NameGenerator``` can **only be used after specifying symbols** to use or by setting up a `SyllableGenerator`.
 
❌ This example no longer generates names by default: 
```C#
var g = new NameGenerator();
Console.WriteLine(g.Next());
```
 
✅ You must specify symbols through quick setup:
```C#
var names = new NameGenerator("srnl", "ae");
```

✅ Or specify symbols for each syllable position and symbol position:
```C#
var names = new NameGenerator()
    .Start(x => x     // The starting syllable of a name
        .First("st")  // Leading consonants
        .Middle("eo") // Vowels
        .Last("mn"))  // Trailing consonants
    .Inner(x => x     // The "body" of a name
        .First("pl")
        .Middle("ia"))
    .End(x => x       // The ending syllable of a name
        .CopyInner()) // Use the same symbols as inner syllables
    .SetSize(3);      // Makes names 3 syllables long
```


**Rationale**
- `DefaultSyllableGenerator` was intended to be a convenience, but it gave `NameGenerator` hidden behaviours which made it tricky to use or design the library around
* For example, an unconfigured v2 `NameGenerator` would use all vowels in the English alphabet. Configuring it to use less *vowels* would cause the `DefaultSyllableGenerator` to be replaced and this in turn meant all *consonants* would be removed too
- Removing `DefaultSyllableGenerator` makes the library more intuitive to use and makes setup code easier to read

# Breaking Change 3: Simplified weights, positions, and probabilities
In v2, you could control things like the frequency of symbols or the probability a vowel would turn into a sequence. 

In v3, weights, probabilities, and overall positioning has been simplified:
- Vowel and vowel sequences are placed in the same grapheme pool
- Leading consonants and leading consonant clusters are placed in the same pool too
- Trailing consonants and trailing consonant clusters are placed in the pool as well
- Methods related to "final consonants" have been removed
- The probability of a leading or trailing consonant appearing is automatically 100% where there is at least one symbol added for that position

❌ The following advanced configuration of symbol positions, weights, and probabilities no longer works
```C#
var g = new NameGenerator()
        .UsingSyllables(x => x
            .WithVowels("a").Weight(4)
            .WithVowels("e").Weight(1)
            .WithLeadingConsonants("str")
            .WithTrailingConsonants("mnl")
            .WithVowelSequences("ou", "ui")
            .WithLeadingConsonantSequences("wh", "fr"))
		.UsingProbability(x => x
            .OfLeadingConsonants(1.0)     // Onsets appear 100% of the time
            .OfTrailingConsonants(0.20)); // Codas appear 20% of the time;
```

✅ Instead do the following:
```C#
var names = new NameGenerator()
	.Any(x => x
		.First(x => x
			.Add("str")
			.Cluster("wh", "fr"))
		.Middle(x => x
			.Add("a").Weight(4)
			.Add("e")
			.Cluster("ou", "ui"))
		.Last("mnl")
			.Chance(0.20));
```

**Rationale**
- In v2:
	- It was getting hard to understand the different ways positioning, weights, and probabilities could be controlled as they were all controlled in different ways.
	- There were also hidden probabilities used. For example, the `DefaultSyllableGenerator` was set to use leading consonants only 95% of the time.
	- It could be unclear how sequences found their way into a name because probabilities controlled the conversion of symbols into sequences. So even if sequences had a probability of 100%, they could still not appear if the related symbol probability was less than 100%.
- In v3:
	- Weights are now always the main way of controlling symbol frequency
	- Symbols (graphemes in v2) and symbol clusters (sequences in v2) are not separated into different pools so there no need for conversion probabilities
	- Any probabilities you can still control will reliably default to 100% as long as there are symbols to use 

# Breaking Change 4: Moved fluent-style builder methods
In v2, builder methods were in almost every class allowing you to configure complicated NameGenerators on a single line of code.

In v3, builder methods still exist, but:
- They now belong to the `Syllabore.Fluent` namespace
- You must use the `using Syllabore.Fluent` directive to use fluent methods
- They have been renamed to account for the simplified weights, probabilities, and positioning
- Fluent method names have been simplified

**Rationale**
- In V2, the fluent API was verbose because the method names reflected non-fluent use as well
- In V3, the fluent API is now implemented as extension methods. This let's us make the "normal" methods descriptive and the fluent API shorter and simpler

# Other breaking Changes  
- Transform method names have changed
- Filter method names have changed
- NameFormatter method names have changed
- SyllableSet has changed
- Removed deprecated classes and deprecated methods. There are too many breaking changes to preserve backwards compatibility and so there is no longer any need to keep deprecated classes and methods.
- NameGeneratorSerializer was overhauled: the v2 serializer will not be able to deserialize a v3 json file and vice versa - see docs


