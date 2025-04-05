# NameGeneratorSerializer

A convenience class for reading and writing a [NameGenerator](namegenerator.md) to disk and back.

## Constructors

| Constructor | Description |
|-------------|-------------|
| NameGeneratorSerializer() | Initializes a new [NameGeneratorSerializer](namegeneratorserializer.md) with basic encoder settings. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Deserialize(System.String filepath)| [NameGenerator](namegenerator.md) | Reads a json file and turns it into a [NameGenerator](namegenerator.md). This method expects the json file to have been written by the *M:Syllabore.Json.NameGeneratorSerializer.Serialize* method of this class. |
| Serialize([NameGenerator](namegenerator.md) generator, System.String filepath)| System.Void | Writes the specified [NameGenerator](namegenerator.md) to a json file. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| EncoderSettings | System.Text.Encodings.Web.TextEncoderSettings | Allows characters and unicode ranges. |
| WriteIndented | System.Boolean | If true, json output will be indented and easier to read. |
