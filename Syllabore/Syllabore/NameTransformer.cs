using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{

    /// <summary>
    /// Takes source names and applies one or more transforms
    /// to a name to create a new name.
    /// </summary>
    [Serializable]
    public class NameTransformer : ITransformer
    {


        private Random Random;
        public List<Transform> Transforms { get; set; }
        public int SelectionLimit { get; set; }

        /// <summary>
        /// A number from 0 to 1 inclusive that represents the probablity
        /// that a NameGenerator mutator will apply a transform from this
        /// transformer during name generation (during a call to Next() or
        /// NextName()).
        /// A value of 0 means a mutation can never occur and a value of 1
        /// means a mutation will always occur.
        /// </summary>
        public double? TransformChance { get; set; }

        public NameTransformer()
        {
            this.Random = new Random();
            this.Transforms = new List<Transform>();
            this.SelectionLimit = 1;
        }

        private Transform GetWeightedSelection()
        {
            int totalWeight = this.Transforms.Sum(x => x.Weight);
            int selection = this.Random.Next(totalWeight);

            int runningTotal = 0;
            for (int j = 0; j < totalWeight; j++)
            {
                runningTotal += this.Transforms[j].Weight;
                if (selection < runningTotal)
                {
                    return this.Transforms[j];
                }
            }

            throw new InvalidOperationException("GetWeightedSelection() failed generated a selection that is in range of total weights");
        }


        public Name Transform(Name sourceName)
        {
            var result = new Name(sourceName);

            for (int i = 0; this.Transforms.Count > 0 && i < this.SelectionLimit; i++)
            {
                var transform = this.GetWeightedSelection();
                var canApplyTransform = transform.ConditionalRegex == null;

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

                if (canApplyTransform)
                {
                    transform.Apply(result);
                }

                
            }
            
            return result;
        }

        public NameTransformer WithTransform(Func<Transform, Transform> config)
        {
            this.Transforms.Add(config(new Transform()));
            return this;
        }

        /// <summary>
        /// Applies a weight to the last added mutation that influences the probability of being used over others.
        /// Given two mutations X and Y with a weight of 3 and 1 respectively, mutation X will be applied 75% of the time.
        /// All mutations default to a weight of 1.
        /// </summary>
        public NameTransformer Weight(int weight)
        {
            this.Transforms[this.Transforms.Count - 1].Weight = weight;
            return this;
        }

        public NameTransformer Join(NameTransformer m)
        {
            NameTransformer newMutator = new NameTransformer() { SelectionLimit = this.SelectionLimit };

            newMutator.Transforms.AddRange(this.Transforms);
            newMutator.Transforms.AddRange(m.Transforms);
            return newMutator;
        }

        public NameTransformer Select(int limit)
        {
            this.SelectionLimit = limit;
            return this;
        }

        public NameTransformer Chance(double probability)
        {

            if (probability < 0 || probability > 1)
            {
                throw new ArgumentException("The mutation probability must be a number between 0 and 1 inclusive.");
            }

            this.TransformChance = probability;

            return this;
        }


    }

}
