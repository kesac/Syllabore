# SyllableGeneratorFluentWrapper

A convenience wrapper for [SyllableGenerator](syllablegenerator.md) that is used by NameGenerator's fluent extension methods. Not meant to be used outside of the fluent API.

## Constructors

| Constructor | Description |
|-------------|-------------|
| SyllableGeneratorFluentWrapper([NameGenerator](namegenerator.md) parent, [SyllablePosition](syllableposition.md) syllablePosition, [SyllableGenerator](syllablegenerator.md) syllables) | Used by [NameGenerator](namegenerator.md) to instantiate a new [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md). |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Chance(System.Double chance)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Sets the chance of generating a symbol for the last modified symbol position. |
| CopyEnd()| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Copies the SyllableGenerator from the ending position to the current syllable position. This method only works if the ending position is of type [SyllableGenerator](syllablegenerator.md). |
| CopyInner()| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Copies the SyllableGenerator from the inner position to the current syllable position. This method only works if the inner position is of type [SyllableGenerator](syllablegenerator.md). |
| CopyStart()| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Copies the SyllableGenerator from the starting position to the current syllable position. This method only works if the starting position is of type [SyllableGenerator](syllablegenerator.md). |
| First(System.String symbols)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the first position of the syllable. |
| First([SymbolGenerator](symbolgenerator.md) symbols)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the first position of the syllable. |
| First(System.Func &lt;[SymbolGenerator](symbolgenerator.md),[SymbolGenerator](symbolgenerator.md)&gt; configuration)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the first position of the syllable. |
| Last(System.String symbols)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the last position of the syllable. |
| Last([SymbolGenerator](symbolgenerator.md) symbols)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the last position of the syllable. |
| Last(System.Func &lt;[SymbolGenerator](symbolgenerator.md),[SymbolGenerator](symbolgenerator.md)&gt; configuration)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the last position of the syllable. |
| Middle(System.String symbols)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the middle position of the syllable. |
| Middle([SymbolGenerator](symbolgenerator.md) symbols)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the middle position of the syllable. |
| Middle(System.Func &lt;[SymbolGenerator](symbolgenerator.md),[SymbolGenerator](symbolgenerator.md)&gt; configuration)| [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md) | Adds symbols to the middle position of the syllable. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Result | [SyllableGenerator](syllablegenerator.md) | The resulting [SyllableGenerator](syllablegenerator.md) after applying the fluent configuration. |
