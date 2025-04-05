# NameFilter

Validates names produced by a name generator against a set of configurable constraints.

*Implements: [INameFilter](inamefilter.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| NameFilter() | Instantiates a new [NameFilter](namefilter.md) with no constraints. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Add(System.String[] patterns)| [NameFilter](namefilter.md) | Adds new patterns to this filter. |
| Add([FilterConstraint](filterconstraint.md) constraint)| [NameFilter](namefilter.md) | Adds a new constraint to this filter. |
| IsValid([Name](name.md) name)| System.Boolean | Returns true if the specified name does not match any of this filter's contraints, else returns false. |
| IsValid(System.String name)| System.Boolean | Returns true if the specified name does not match any of this filter's contraints, else returns false. |

## Fluent Methods
All fluent methods below return an instance of [NameFilter](namefilter.md).

| Method | Description |
|--------|-------------|
| DoNotAllowEnding(System.String[] suffixes)| Makes a name invalid if it ends with any of the specified substrings. |
| DoNotAllowRegex(System.String[] regex)| Makes a name invalid if it matches any of the specified regular expressions. |
| DoNotAllowStart(System.String[] prefixes)| Makes a name invalid if it starts with any of the specified substrings. |
| DoNotAllowSubstring(System.String[] substring)| Makes a name invalid if it contains any of the specified substrings. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Constraints | System.Collections.Generic.List &lt;[FilterConstraint](filterconstraint.md)&gt; | The list of constraints that names must pass to be considered valid. |
