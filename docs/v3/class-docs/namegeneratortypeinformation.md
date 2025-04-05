# NameGeneratorTypeInformation

Contains *System.Type* information for a [NameGenerator](namegenerator.md)'s properties. This class is used by [NameGeneratorSerializer](namegeneratorserializer.md).

## Constructors

| Constructor | Description |
|-------------|-------------|
| NameGeneratorTypeInformation() | Creates an empty [NameGeneratorTypeInformation](namegeneratortypeinformation.md). |

## Methods

No public methods.
## Properties

| Property | Type | Description |
|----------|------|-------------|
| NameFilterType | System.String | The concrete type name of *Syllabore.NameGenerator.NameFilter*. |
| NameTransformerTypeName | System.String | The concrete type name of *Syllabore.NameGenerator.NameTransformer*. |
| SyllableGeneratorTypeNames | System.Collections.Generic.Dictionary &lt;[SyllablePosition](syllableposition.md),System.String&gt; | The concrete type names of the generators in *Syllabore.NameGenerator.SyllableGenerators*. |
