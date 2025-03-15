using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// A <see cref="TransformSet"/> takes a source name, 
    /// applies one or more <see cref="Transform">Transforms</see>, 
    /// then creates a new name. By default, all 
    /// <see cref="Transform">Transforms</see> of the same set are 
    /// applied to the source name and in the order they were added.
    /// </para>
    /// <para>
    /// To randomize what transforms are applied, call 
    /// <see cref="RandomlySelect(int)"/> when configuring a 
    /// <see cref="TransformSet"/>.
    /// </para>
    /// </summary>
    [Serializable]
    public class TransformSet : IPotentialAction, INameTransformer, IRandomizable
    {
        /// <summary>
        /// Used to simulate randomness when <see cref="UseRandomSelection"/> is true.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// The probability this transform set will make changes to a name. 
        /// This value must be between 0 and 1 inclusive.
        /// <para>
        /// Note that each <see cref="Transform">Transform</see> in the set can also have its own chance value
        /// which is rolled separately.
        /// </para>
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// The <see cref="Transform">Transforms</see> that make up this
        /// <see cref="TransformSet"/>.
        /// </summary>
        public List<Transform> Transforms { get; set; }

        /// <summary>
        /// When true, <see cref="Transform">Transforms</see> are not
        /// applied in the order they were added. Instead, a random
        /// number of <see cref="Transform">Transforms</see> are selected
        /// and applied. Property <see cref="RandomSelectionCount"/> is
        /// used to determine how many random selections are made.
        /// </summary>
        public bool UseRandomSelection { get; set; }

        /// <summary>
        /// When <see cref="UseRandomSelection"/> is true, this property
        /// is used to determine how many random <see cref="Transform">Transforms</see>
        /// are selected and applied.
        /// </summary>
        public int RandomSelectionCount { get; set; }


        /// <summary>
        /// Instantiates a new <see cref="TransformSet"/>.
        /// By default, all future <see cref="Transform"/>s that are added
        /// to this set will be used in the order they were added
        /// unless there is a call to <see cref="RandomlySelect(int)"/>.
        /// </summary>
        public TransformSet()
        {
            this.Random = new Random();
            this.Chance = 1.0;
            this.Transforms = new List<Transform>();
            this.UseRandomSelection = false;
            this.RandomSelectionCount = 0;
        }

        /// <summary>
        /// Adds a new <see cref="Transform"/> to this <see cref="TransformSet"/>.
        /// </summary>
        public TransformSet Add(Transform transform)
        {
            this.Transforms.Add(transform);
            return this;
        }

        /// <summary>
        /// Returns a new <see cref="Name"/> that
        /// is the result of one or more <see cref="Transform"/>s
        /// applied to the specified source <see cref="Name"/>.
        /// This method leaves the source <see cref="Name"/> untouched.
        /// </summary>
        public Name Apply(Name sourceName)
        {
            if(this.UseRandomSelection)
            {
                return this.ApplyRandomTransforms(sourceName);
            }
            else
            {
                return this.ApplyAllTransforms(sourceName);
            }
        }

        /// <summary>
        /// Combines this <see cref="TransformSet"/> with the specified <see cref="TransformSet"/>.
        /// A new <see cref="TransformSet"/> that is the combination of the two is returned.
        /// </summary>
        public TransformSet Join(TransformSet set)
        {
            TransformSet result = new TransformSet() { RandomSelectionCount = this.RandomSelectionCount };

            result.Transforms.AddRange(this.Transforms);
            result.Transforms.AddRange(set.Transforms);
            return result;
        }

        /// <summary>
        /// <para>
        /// Sets this <see cref="TransformSet"/> to randomly select transforms to apply to the source name.
        /// </para>
        /// <para>
        /// The <paramref name="limit"/> parameter specifies the maximum number of unique transforms that will be applied.
        /// </para>
        /// </summary>
        public TransformSet RandomlySelect(int limit)
        {
            this.UseRandomSelection = true;
            this.RandomSelectionCount = limit;
            return this;
        }

        private Name ApplyRandomTransforms(Name sourceName)
        {
            var result = new Name(sourceName);

            var unusedTransforms = new List<Transform>();
            unusedTransforms.AddRange(this.Transforms);

            for (int i = 0; i < this.RandomSelectionCount && unusedTransforms.Count > 0; i++)
            {
                var transform = unusedTransforms.RandomWeightedItem<Transform>(this.Random);
                unusedTransforms.Remove(transform);

                if (this.CanTransformBeApplied(transform, sourceName))
                {
                    transform.Modify(result);
                }
            }

            return result;
        }

        private Name ApplyAllTransforms(Name sourceName)
        {
            var result = new Name(sourceName);

            foreach (var transform in this.Transforms)
            {
                if (this.CanTransformBeApplied(transform, sourceName))
                {
                    transform.Modify(result);
                }
            }

            return result;
        }

        private bool CanTransformBeApplied(Transform transform, Name sourceName)
        {
            var canApplyTransform = false;

            if (this.Random.NextDouble() <= this.Chance)
            {
                if (transform.ConditionalRegex != null)
                {
                    if (transform.ConditionalIndex.HasValue)
                    {
                        int index = transform.ConditionalIndex.Value;

                        if (index < 0) // reverse index provided, so translate into a forward index (eg. -1 is the last syllable)
                        {
                            index = sourceName.Syllables.Count + index;
                        }

                        if (Regex.IsMatch(sourceName.Syllables[index], transform.ConditionalRegex))
                        {
                            // The target syllable passes the condition
                            canApplyTransform = true;
                        }
                    }
                    else if (Regex.IsMatch(sourceName.ToString(), transform.ConditionalRegex))
                    {
                        // Entire name passes the condition
                        canApplyTransform = true;
                    }
                }
                else
                {
                    // No conditions, automatic pass
                    canApplyTransform = true;
                }
            }

            return canApplyTransform;

        }


    }

}
