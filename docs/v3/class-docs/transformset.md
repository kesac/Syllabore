# TransformSet

A [TransformSet](transformset.md) takes a source name, applies one or more [Transform](transform.md), then creates a new name. By default, all [Transform](transform.md) of the same set are applied to the source name and in the order they were added. To randomize what transforms are applied, call *M:Syllabore.TransformSet.RandomlySelect* when configuring a [TransformSet](transformset.md).

*Implements: [IPotentialAction](ipotentialaction.md), [INameTransformer](inametransformer.md), [IRandomizable](irandomizable.md)*

## Constructors

| Constructor | Description |
|-------------|-------------|
| TransformSet() | Instantiates a new [TransformSet](transformset.md). By default, all future [Transform](transform.md)s that are added to this set will be used in the order they were added unless there is a call to *M:Syllabore.TransformSet.RandomlySelect*. |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| Add([Transform](transform.md) transform)| [TransformSet](transformset.md) | Adds a new [Transform](transform.md) to this [TransformSet](transformset.md). |
| Apply([Name](name.md) sourceName)| [Name](name.md) | Returns a new [Name](name.md) that is the result of one or more [Transform](transform.md)s applied to the specified source [Name](name.md). This method leaves the source [Name](name.md) untouched. This method can result in no changes if *Syllabore.TransformSet.Chance* is less than 1.0. |
| Join([TransformSet](transformset.md) set)| [TransformSet](transformset.md) | Combines this [TransformSet](transformset.md) with the specified [TransformSet](transformset.md). A new [TransformSet](transformset.md) that is the combination of the two is returned. |
| RandomlySelect(System.Int32 limit)| [TransformSet](transformset.md) | Sets this [TransformSet](transformset.md) to randomly select transforms to apply to the source name. The parameter specifies the maximum number of unique transforms that will be applied. |

## Fluent Methods
All fluent methods below return an instance of [TransformSet](transformset.md).

| Method | Description |
|--------|-------------|
| Add(*lambda => [Transform](transform.md)*)| Adds a new [Transform](transform.md) to this [TransformSet](transformset.md). |
| Chance(System.Double chance)|  |

## Properties

| Property | Type | Description |
|----------|------|-------------|
| Chance | System.Double | The probability this transform set will make changes to a name. This value must be between 0 and 1 inclusive. Note that each [Transform](transform.md) in the set can also have its own chance value which is rolled separately. |
| Random | System.Random | Used to simulate randomness when *Syllabore.TransformSet.UseRandomSelection* is true. |
| RandomSelectionCount | System.Int32 | When *Syllabore.TransformSet.UseRandomSelection* is true, this property is used to determine how many random [Transform](transform.md) are selected and applied. |
| Transforms | System.Collections.Generic.List &lt;[Transform](transform.md)&gt; | The [Transform](transform.md) that make up this [TransformSet](transformset.md). |
| UseRandomSelection | System.Boolean | When true, [Transform](transform.md) are not applied in the order they were added. Instead, a random number of [Transform](transform.md) are selected and applied. Property *Syllabore.TransformSet.RandomSelectionCount* is used to determine how many random selections are made. |
