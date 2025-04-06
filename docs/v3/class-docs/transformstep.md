# TransformStep

Represents one action or step in a [Transform](transform.md).

*Implements: [IPotentialAction](ipotentialaction.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| TransformStep() | Instantiates a new [TransformStep](transformstep.md) with no type or arguments. |
| TransformStep(System.Action&lt;[Name](name.md)&gt; unserializableAction) | Instantiates a new [TransformStep](transformstep.md) with type *Syllabore.TransformStepType.Lambda* and the specified *System.Action* to execute. Note that this type of [TransformSet](transformset.md) is not serializable. |
| TransformStep([TransformStepType](transformsteptype.md) type, System.String[] args) | Instantiates a new [TransformStep](transformstep.md) with the specified type and arguments. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Modify([Name](name.md) name)| System.Void | Applies this transform step to the specified [Name](name.md). This method is destructive. |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Arguments | System.Collections.Generic.List &lt;System.String&gt; | The arguments that are passed to the action. |
| Chance | System.Double | The probability this [TransformStep](transformstep.md) will make changes when *M:Syllabore.TransformStep.Modify* is called. |
| Type | [TransformStepType](transformsteptype.md) | The type of action this [TransformSet](transformset.md) represents. |
