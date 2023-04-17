using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{

    /// <summary>
    /// Takes source names and applies one or more transforms
    /// to create a new name.
    /// <para>
    /// By default, all transforms are applied to the source name
    /// in the order they were added to a <see cref="TransformSet"/>.
    /// </para>
    /// <para>
    /// Use <see cref="RandomlySelect(int)"/> to randomize what
    /// transforms are applied to a name.
    /// </para>
    /// </summary>
    [Serializable]
    public class TransformSet : INameTransformer
    {
        private Random Random;
        public List<Transform> Transforms { get; set; }

        public bool UseRandomSelection { get; set; }
        public int RandomSelectionCount { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="TransformSet"/>
        /// </summary>
        public TransformSet()
        {
            this.Random = new Random();
            this.Transforms = new List<Transform>();
            this.UseRandomSelection = false;
            this.RandomSelectionCount = 0;
        }

        /// <summary>
        /// Applies one or more transforms on the specified 
        /// <see cref="Name"/> and returns the result.
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

        private Name ApplyRandomTransforms(Name sourceName)
        {
            var result = new Name(sourceName);

            var unusedTransforms = new List<Transform>();
            unusedTransforms.AddRange(this.Transforms);

            for (int i = 0; i < this.RandomSelectionCount && unusedTransforms.Count > 0; i++)
            {
                var transform = unusedTransforms.RandomWeightedItem<Transform>();
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

            foreach(var transform in this.Transforms)
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
            var canApplyTransform = (transform.ConditionalRegex == null);

            if (transform.ConditionalRegex != null)
            {
                if (transform.ConditionalIndex.HasValue)
                {
                    int index = transform.ConditionalIndex.Value;

                    if (index < 0) // reverse index provided, so translate into forward index (eg. -1 is the last syllable)
                    {
                        index = sourceName.Syllables.Count + index;
                    }

                    if (Regex.IsMatch(sourceName.Syllables[index], transform.ConditionalRegex))
                    {
                        canApplyTransform = true;
                    }
                }
                else if (Regex.IsMatch(sourceName.ToString(), transform.ConditionalRegex))
                {
                    canApplyTransform = true;
                }
            }

            return canApplyTransform;

        }

        /// <summary>
        /// Adds a new transform to this <see cref="TransformSet"/>.
        /// </summary>
        public TransformSet WithTransform(Transform transform)
        {
            this.Transforms.Add(transform);
            return this;
        }

        /// <summary>
        /// Adds a new transform to this <see cref="TransformSet"/>.
        /// </summary>
        public TransformSet WithTransform(Func<Transform, Transform> config)
        {
            this.Transforms.Add(config(new Transform()));
            return this;
        }

        /// <summary>
        /// Applies a weight to the last added transform that influences the probability of being used over others.
        /// Given two transform X and Y with a weight of 3 and 1 respectively, transform X will be applied 75% of the time.
        /// All transforms have default weight of 1.
        /// </summary>
        public TransformSet Weight(int weight)
        {
            this.Transforms[this.Transforms.Count - 1].Weight = weight;
            return this;
        }

        /// <summary>
        /// Combines this instance with the specified <see cref="TransformSet"/> instance.
        /// The resulting <see cref="TransformSet"/> uses the transforms of the
        /// previous two.
        /// </summary>
        public TransformSet Join(TransformSet m)
        {
            TransformSet newMutator = new TransformSet() { RandomSelectionCount = this.RandomSelectionCount };

            newMutator.Transforms.AddRange(this.Transforms);
            newMutator.Transforms.AddRange(m.Transforms);
            return newMutator;
        }

        /// <summary>
        /// Sets the number of transforms to use on each call of <see cref="Apply(Name)"/>.
        /// </summary>
        public TransformSet RandomlySelect(int limit)
        {
            this.UseRandomSelection = true;
            this.RandomSelectionCount = limit;
            return this;
        }


    }

}
