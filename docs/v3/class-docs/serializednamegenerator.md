# SerializedNameGenerator

Contains a [NameGenerator](namegenerator.md) plus the *System.Type* information of its properties. This class is used by [NameGeneratorSerializer](namegeneratorserializer.md) when serializing or deserializing a [NameGenerator](namegenerator.md).

## Constructors

| Constructor | Description |
|-------------|-------------|
| SerializedNameGenerator() |  |

## Methods

No public methods.
## Properties

| Property | Type | Description |
|----------|------|-------------|
| Types | [NameGeneratorTypeInformation](namegeneratortypeinformation.md) | The type information to serialize. |
| Value | [NameGenerator](namegenerator.md) | The [NameGenerator](namegenerator.md) to serialize. |
