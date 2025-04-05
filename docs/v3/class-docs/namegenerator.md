# NameGenerator

Generates names by constructing syllables and joining them together.

*Implements: Archigen.IGenerator&lt;System.String&gt;, [IRandomizable](irandomizable.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| NameGenerator() | Initializes a new instance of the [NameGenerator](namegenerator.md) class with no symbols, no transformer, and no filter. |
| NameGenerator(System.String firstSymbols, System.String middleSymbols) | Initializes a new [NameGenerator](namegenerator.md) with specified symbol pools for the first and middle symbol positions of a syllable. Each character in the provided strings is considered a separate symbol. The generated syllables will be used for all positions of a name. |
| NameGenerator(System.String firstSymbols, System.String middleSymbols, System.String lastSymbols) | Initializes a new [NameGenerator](namegenerator.md) with specified symbol pools for the first, middle, and last symbol positions of a syllable. Each character in the provided strings is considered a separate symbol. The generated syllables will be used for all positions of a name. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Next()| System.String | Generates a name and returns it as a string. |
| SetFilter([INameFilter](inamefilter.md) filter)| [NameGenerator](namegenerator.md) | Sets the name filter to use when generating names. Returns this instance of [NameGenerator](namegenerator.md) for chaining. |
| SetSize(System.Int32 size)| [NameGenerator](namegenerator.md) | Sets both the minimum and maximum number of syllables to use per name. Returns this instance of [NameGenerator](namegenerator.md) for chaining. |
| SetSize(System.Int32 minSize, System.Int32 maxSize)| [NameGenerator](namegenerator.md) | Sets the minimum and maximum number of syllables to use per name. Returns this instance of [NameGenerator](namegenerator.md) for chaining. |
| SetSyllables([SyllablePosition](syllableposition.md) position, [ISyllableGenerator](isyllablegenerator.md) generator)| [NameGenerator](namegenerator.md) | Sets the [SyllableGenerator](syllablegenerator.md) for the specified position. Returns this instance of [NameGenerator](namegenerator.md) for chaining. |
| SetTransform([INameTransformer](inametransformer.md) transformer)| [NameGenerator](namegenerator.md) | Sets the name transformer to use when generating names. Returns this instance of [NameGenerator](namegenerator.md) for chaining. |

## Fluent Methods
All fluent methods below return an instance of [NameGenerator](namegenerator.md).

| Method | Description |
|--------|-------------|
| Any(*lambda => [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md)*)| Sets the [SyllableGenerator](syllablegenerator.md) for all syllable positions. |
| Any([ISyllableGenerator](isyllablegenerator.md) syllables)| Sets the [SyllableGenerator](syllablegenerator.md) for all syllable positions. |
| End(*lambda => [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md)*)| Configures the ending [SyllableGenerator](syllablegenerator.md) of a [NameGenerator](namegenerator.md). |
| End([ISyllableGenerator](isyllablegenerator.md) syllables)| Sets the ending [SyllableGenerator](syllablegenerator.md) of a [NameGenerator](namegenerator.md). |
| Filter(System.String[] regexPatterns)| Sets the filter for a [NameGenerator](namegenerator.md). |
| Filter(*lambda => [NameFilter](namefilter.md)*)| Sets the filter for a [NameGenerator](namegenerator.md). |
| Inner(*lambda => [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md)*)| Configures the inner [SyllableGenerator](syllablegenerator.md) of a [NameGenerator](namegenerator.md). |
| Inner([ISyllableGenerator](isyllablegenerator.md) syllables)| Sets the inner [SyllableGenerator](syllablegenerator.md) of a [NameGenerator](namegenerator.md). |
| Start(*lambda => [SyllableGeneratorFluentWrapper](syllablegeneratorfluentwrapper.md)*)| Configures the starting [SyllableGenerator](syllablegenerator.md) of a [NameGenerator](namegenerator.md). |
| Start([ISyllableGenerator](isyllablegenerator.md) syllables)| Sets the starting [SyllableGenerator](syllablegenerator.md) of a [NameGenerator](namegenerator.md). |
| Transform([Transform](transform.md) transform)| Sets the transform for a [NameGenerator](namegenerator.md). |
| Transform([TransformSet](transformset.md) transformSet)| Sets the transform for a [NameGenerator](namegenerator.md). |
| Transform(*lambda => [Transform](transform.md)*)| Sets the transform for a [NameGenerator](namegenerator.md). |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| MaximumRetries | System.Int32 | If this generator has a filter, this is the maximum attempts that will be made to satisfy the filter before an InvalidOperationException is thrown. The default maximum retry count is 1000. |
| MaximumSize | System.Int32 | The maximum number of syllables in generated names. The default maximum size is 3 syllables. |
| MinimumSize | System.Int32 | The minimum number of syllables in generated names. The default minimum size is 2 syllables. |
| NameFilter | [INameFilter](inamefilter.md) | The filter used to control generated names. Can be null if no filter is being used. |
| NameTransformer | [INameTransformer](inametransformer.md) | The transformer used to modify generated names. Can be null if no transform is being used. |
| Random | System.Random | The instance of *System.Random* used to simulate randomness. |
| SyllableGenerators | System.Collections.Generic.Dictionary &lt;[SyllablePosition](syllableposition.md),[ISyllableGenerator](isyllablegenerator.md)&gt; | The [SyllableGenerator](syllablegenerator.md) used by this [NameGenerator](namegenerator.md). |
