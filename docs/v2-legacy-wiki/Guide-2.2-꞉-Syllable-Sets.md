## Syllable Sets
A ```SyllableSet``` is a special ```SyllableGenerator``` that generates a finite set of syllables. 

This is useful for when there is a desire to produce names that have  thematic cohesion or to simulate names originating from the same fictional geographic region, culture, or historical period.

## Quick Usage
A ```SyllableSet``` can be used without any configuration. It can supplied directly to a ```NameGenerator``` through the constructor or through a call to ```UsingSyllables(ISyllableProvider)```.

```csharp
// Basic usage: both generators are unconfigured, but still usable
var s = new SyllableSet();
var g = new NameGenerator(s);

// This also would work
var g2 = new NameGenerator()
        .UsingSyllables(new SyllableSet());
```

A vanilla ```SyllableSet``` will have the following characteristics:
 * Uses a ```DefaultSyllableGenerator``` to create syllables
 * Uses up to 8 unique syllables suitable for starting a name
 * Uses up to 8 unique syllables suitable for ending a name
 * Uses up to 8 unique syllables to be positioned between starting and ending syllables

## Customizing Characters
Call ```WithGenerator()``` to supply your own ```SyllableGenerator``` and influence how the finite pool of syllables is generated.

Here is an example of customizing your syllables to use a limited set of vowels and consonants:
```csharp
var s = new SyllableSet()
        .WithGenerator(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("lmn"));
```

## Changing Set Sizes
The number of starting syllables, "middle" syllables, and ending syllables is easily changed through the constructor:
```csharp
// This set will have 2 starting syllables, 8 middle syllables, and 2 endings
var s = new SyllableSet(2, 8, 2)
        .WithGenerator(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("strlmn"));

var g = new NameGenerator()
        .UsingSyllables(s)
        .UsingSyllableCount(3);
```
Calling ```Next()``` on the name generator will product names like:
```
Tasane
Nalena
Tarane
```

## Providing Syllables Manually 
If you have specific syllables in mind, you can provide them manually like so:
```csharp
var s = new SyllableSet(2, 8, 2)
        .WithStartingSyllable("ko", "ro") // Specific syllables provided
        .WithEndingSyllable("re", "ke")   // Specific syllables provided
        .WithGenerator(x => x
            .WithVowels("ae")
            .WithLeadingConsonants("strlmn"));

var g = new NameGenerator()
        .UsingSyllables(s)
        .UsingSyllableCount(3);
```
This name generator above will produce names like:
```
Komare
Rosake
Kotare
```
### Tip❕
 * If you are manually adding syllables, it's ok to only provide a few
 * A ```SyllableSet``` will automatically generate the rest of the syllables until it fills itself to capacity
