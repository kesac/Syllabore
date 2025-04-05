# SyllableSet

A special kind of syllable generator that constructs a finite set of syllables and only returns syllables from that set. Names constructed from a [SyllableSet](syllableset.md) can give the appearance of cohesion as if they originated from a similar geographic region, culture, historical period, etc.

*Implements: [ISyllableGenerator](isyllablegenerator.md), Archigen.IGenerator&lt;System.String&gt;, [IRandomizable](irandomizable.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| SyllableSet() | Initializes an empty set. |
| SyllableSet(System.String[] syllables) | Initializes a new instance of the [SyllableSet](syllableset.md) class with the specified syllables. |
| SyllableSet([SyllableGenerator](syllablegenerator.md) syllableGenerator, System.Int32 maxSyllableCount, System.Boolean forceUnique) |  |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Add(System.String[] syllables)| [SyllableSet](syllableset.md) | Adds the specified syllables to the set. |
| Copy()| [ISyllableGenerator](isyllablegenerator.md) | Creates a deep copy of this [SyllableSet](syllableset.md). |
| Next()| System.String | Generates a new syllable from the set. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| ForceUnique | System.Boolean |  |
| MaximumGeneratedSyllables | System.Int32 | The maximum number of syllables for the *Syllabore.SyllableSet.SyllableGenerator* to generate. This value has no effect if there is no *Syllabore.SyllableSet.SyllableGenerator*. |
| PossibleSyllables | System.Collections.Generic.List &lt;System.String&gt; |  |
| Random | System.Random | The instance used to simulate randomness. |
| SyllableGenerator | [SyllableGenerator](syllablegenerator.md) |  |
