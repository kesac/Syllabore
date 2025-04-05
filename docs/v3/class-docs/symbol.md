# Symbol

An indivisible unit of a writing system. In Syllabore, [Symbol](symbol.md) are used to represents vowels, consonants, sequences, or clusters.

*Implements: Archigen.IWeighted*

## Constructors

| Constructor | Description |
|-------------|-------------|
| Symbol(System.String value) | Instantiates a new [Symbol](symbol.md) with the specified value. |
| Symbol(System.String value, System.Int32 weight) | Instantiates a new [Symbol](symbol.md) with the specified value and weight. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Copy()| [Symbol](symbol.md) | Creates a deep copy of this [Symbol](symbol.md). |
| ToString()| System.String | Returns a string representation of this [Symbol](symbol.md). |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Value | System.String | A character or set of characters representing this [Symbol](symbol.md). |
| Weight | System.Int32 | A weight value that affects how frequently it should be selected compared to other weighted elements. The default weight of a [Symbol](symbol.md) is 1. |
