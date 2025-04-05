# TransformStepType

*Enum*. The type of action that a [TransformStep](transformstep.md) will apply to a [Name](name.md).

## Values

| Name | Description |
|------|-------------|
| Unknown | The step type is unknown. |
| InsertSyllable | Adds a syllable to a [Name](name.md), displacing other syllables as needed. |
| AppendSyllable | Adds a syllable to the end of a [Name](name.md). |
| ReplaceSyllable | Replaces a single syllable with a another syllable. |
| ReplaceAllSubstring | Replaces all instances of a substring with another substring. |
| RemoveSyllable | Deletes a syllable from a [Name](name.md), displacing other syllables as needed. |
| Lambda | An action that is not serializable and expressed in a lambda. |

