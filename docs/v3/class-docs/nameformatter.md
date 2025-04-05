# NameFormatter

A convenience class used for modeling names that have multiple parts and need multiple generators to create them.

*Implements: Archigen.IGenerator&lt;System.String&gt;*

## Constructors

| Constructor | Description |
|-------------|-------------|
| NameFormatter(System.String format) | Instantiates a new [NameFormatter](nameformatter.md) with the specified format. Substrings that need to be replaced with a generated name should be surrounded with curly brackets. For example, the format "John {middle-name} Smith" tells a [NameFormatter](nameformatter.md) that the name between the first and last needs to be generated. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Define(System.String propertyName, Archigen.IGenerator &lt;System.String&gt; nameGenerator, [NameFormat](nameformat.md) stringCase, System.Boolean useLeadingSpace)| [NameFormatter](nameformatter.md) | Specifies a generator for the specified property. |
| Next()| System.String | Returns a new generated name based on the previously specified format. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| BoundNameGenerators | System.Collections.Generic.Dictionary &lt;System.String,Archigen.IGenerator &lt;System.String&gt;&gt; | The [NameGenerator](namegenerator.md) used by this [NameFormatter](nameformatter.md). |
| Format | System.String | The desired format for names. Surround substrings that need to be replaced with a generated name with curly brackets. For example, the format "John {middle-name} Smith" tells a [NameFormatter](nameformatter.md) that the name between the first and last needs to be generated. |
| Options | System.Collections.Generic.Dictionary &lt;System.String,[NameFormatterGeneratorOptions](nameformattergeneratoroptions.md)&gt; | Provides hints on whether a name should be upper case, lower case, capitalized, etc. |
