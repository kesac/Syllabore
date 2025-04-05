# SyllableGenerator

Generates syllables that can be sequenced into names.

*Implements: [ISyllableGenerator](isyllablegenerator.md), Archigen.IGenerator&lt;System.String&gt;, [IRandomizable](irandomizable.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| SyllableGenerator() | Instantiates a new [SyllableGenerator](syllablegenerator.md) with no symbol generators. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Add([SymbolPosition](symbolposition.md) position, System.String symbols)| [SyllableGenerator](syllablegenerator.md) | Adds symbols to the specified position. Each character in the string is considered a separate symbol. |
| Add([SymbolPosition](symbolposition.md) position, [SymbolGenerator](symbolgenerator.md) generator)| [SyllableGenerator](syllablegenerator.md) | Adds a [SymbolGenerator](symbolgenerator.md). The generator's symbols will only be used for the specified position. |
| Copy()| [ISyllableGenerator](isyllablegenerator.md) | Creates a deep copy of this [SyllableGenerator](syllablegenerator.md) excluding internal instances of *System.Random*. |
| Next()| System.String | Generates a new syllable and returns it as a string. |
| SetChance([SymbolPosition](symbolposition.md) position, System.Double chance)| [SyllableGenerator](syllablegenerator.md) | Sets the probability of generating a symbol for the specified position. The default value is 1.0 (100% probability) unless changed by calling this method. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| PositionChance | System.Collections.Generic.Dictionary &lt;[SymbolPosition](symbolposition.md),System.Double&gt; | The probability of generating a symbol for a given position. The default value is 1.0 (100%) for each position as long as there are symbols available. |
| Random | System.Random | The instance of *System.Random* used to simulate randomness. |
| SymbolGenerators | System.Collections.Generic.Dictionary &lt;[SymbolPosition](symbolposition.md),System.Collections.Generic.List &lt;[SymbolGenerator](symbolgenerator.md)&gt;&gt; | The symbol generators used to create new syllables. |
