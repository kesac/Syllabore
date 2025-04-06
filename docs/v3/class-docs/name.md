# Name

Represents a sequence of syllables that make up a name.

## Constructors

| Constructor | Description |
|-------------|-------------|
| Name() | Creates an empty name with no syllables. |
| Name(System.String[] syllable) | Creates a new name with the desired syllables. |
| Name([Name](name.md) copy) | Instantiates a new name that is a copy of the specified [Name](name.md). (This constructor is useful for a [INameTransformer](inametransformer.md).) |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Append(System.String syllable)| [Name](name.md) | Adds a new syllable to this name. Returns this instance of [Name](name.md) for chaining. |
| Equals(System.Object obj)| System.Boolean | Returns true if this [Name](name.md) is equal to the specified [Name](name.md). A [Name](name.md) is equal to another [Name](name.md) only if their string values are also equal. |
| GetHashCode()| System.Int32 | Returns a hash code for this [Name](name.md). |
| ToString()| System.String | Sequences the syllables of this [Name](name.md) into a single string, capitalizes it, and returns it. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Syllables | System.Collections.Generic.List &lt;System.String&gt; | The ordered syllables that make up this name. |
