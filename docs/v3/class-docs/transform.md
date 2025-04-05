# Transform

A [Transform](transform.md) is a mechanism for changing a source name into a new, modified name. Transforming names is useful for adding some determinism in name generation or for creating iterations on an established name. [Transform](transform.md) can have an optional condition that must be fulfilled for a transformation to occur.

*Implements: [IPotentialAction](ipotentialaction.md), Archigen.IWeighted, [INameTransformer](inametransformer.md), [IRandomizable](irandomizable.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| Transform() | Instantiates a new [Transform](transform.md). By default, a [Transform](transform.md) has no optional condition and a weight of 1. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| AddStep([TransformStep](transformstep.md) step)| [Transform](transform.md) | Adds a new step to this transform. |
| Apply([Name](name.md) name)| [Name](name.md) | Applies this [Transform](transform.md) on the specified [Name](name.md) and returns a new [Name](name.md) as a result. The transform may result in no changes if a condition was added and is not met, or if the ** property is between 0 and 1 exclusive (less than 100%). This method leaves the source [Name](name.md) unchanged. |
| Modify([Name](name.md) name)| System.Void | Applies this [Transform](transform.md) on the specified [Name](name.md) in a destructive manner. For a non-destructive alternative, use *M:Syllabore.Transform.Apply* instead. The transform may result in no changes if a condition was added and is not met, or if the ** property is between 0 and 1 exclusive (less than 100%). |

## Fluent Methods
All fluent methods below return an instance of [Transform](transform.md).

| Method | Description |
|--------|-------------|
| Append(System.String syllable)| Adds a transform step that appends a new syllable to the end of a name. |
| ExecuteUnserializableAction(System.Action &lt;[Name](name.md)&gt; unserializableAction)| Executes the specified action on a name. Note that this transform step cannot be serialized. |
| Insert(System.Int32 index, System.String syllable)| Adds a transform step that inserts a new syllable at the specified index. The syllable at that index and the others after it will be pushed one index to the right. |
| Remove(System.Int32 index)| Adds a step that removes the syllable at the specified index. |
| Replace(System.Int32 index, System.String replacement)| Adds a step that replaces a syllable at the specified index with a desired string. The index can be a negative integer to traverse from the end of the name instead. For example, an index -1 will be interpreted as the last syllable of a name. |
| ReplaceSubstring(System.String substring, System.String replacement)| Adds a step that replaces all instances of the specified substring in each syllable with a desired string. Note that the substring must be completely contained in a syllable to be replaced. |
| StepChance(System.Double chance)| Sets the probability of the last added transform step. To set the probability of the transform itself, use *!:TriggerChance*. |
| TransformChance(System.Double chance)| Sets the probability that this transform and all of its steps runs. |
| Weight(System.Int32 weight)|  |
| When(System.Int32 index, System.String regex)| Adds a condition to this [Transform](transform.md). The condition is a regular expression applied to a syllable at the specified . It must be satisfied for the [Transform](transform.md) to be applied successfully. The specified determines the location of the syllable that the condition operates on. A negative can be provided to traverse from the end of the name instead. (For example, an index -1 will be interpreted as the last syllable of a name.) |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Chance | System.Double | The probability this [Transform](transform.md) will attempt to change a name when *M:Syllabore.Transform.Apply* is called. The value must be a double between 0 and 1 inclusive. Note that each [TransformStep](transformstep.md) in this [Transform](transform.md) can also have its own chance value. |
| ConditionalIndex | System.Nullable &lt;System.Int32&gt; | The index of the syllable that the condition operates on. A negative index can be provided to traverse right-to-left from the end of the name instead. |
| ConditionalRegex | System.String | A regular expression that must be satisfied for the transform to be applied. |
| Random | System.Random | Used to simulate randomness. |
| Steps | System.Collections.Generic.List &lt;[TransformStep](transformstep.md)&gt; | The [TransformStep](transformstep.md) that this transform will execute. |
| Weight | System.Int32 | A positive integer that influences the probability of this transform being used over others. Given two transforms X and Y with a weight of 3 and 1 respectively, transform X will be applied 75% of the time. All transforms default to a weight of 1. |
