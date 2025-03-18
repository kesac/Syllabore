using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Archigen;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// A <see cref="Transform"/> is a mechanism for changing a source name into 
    /// a new, modified name. Transforming names is useful for adding some 
    /// determinism in name generation or for creating iterations on an established name.
    /// </para>
    /// <para>
    /// <see cref="Transform">Transforms</see> can have an optional condition that 
    /// must be fulfilled for a transformation to occur.
    /// </para>
    /// </summary>
    public class Transform : IPotentialAction, IWeighted, INameTransformer, IRandomizable
    {
        /// <summary>
        /// Used to simulate randomness.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// The <see cref="TransformStep">steps</see> that this transform will execute.
        /// </summary>
        public List<TransformStep> Steps { get; set; }

        /// <summary>
        /// A positive integer that influences the probability of this transform being 
        /// used over others. Given two transforms X and Y with a weight of 3 and 1 
        /// respectively, transform X will be applied 75% of the time. All transforms 
        /// default to a weight of 1.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// The probability this <see cref="Transform"/> will attempt to change a 
        /// name when <see cref="Apply(Name)"/> is called. The value must be a double between 0 and 1 inclusive.
        /// <para>
        /// Note that each <see cref="TransformStep"/> in this <see cref="Transform"/> can also have its own chance value.
        /// </para>
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// The index of the syllable that the condition operates on. A negative index 
        /// can be provided to traverse right-to-left from the end of the name instead.
        /// </summary>
        public int? ConditionalIndex { get; set; }

        /// <summary>
        /// A regular expression that must be satisfied for the transform to be applied.
        /// </summary>
        public string ConditionalRegex { get; set; }

        /// <summary>
        /// <para>
        /// Instantiates a new <see cref="Transform"/>.
        /// </para>
        /// <para>
        /// By default, a <see cref="Transform"/> has no optional condition and a weight of 1.
        /// </para>
        /// </summary>
        public Transform()
        {
            this.Random = new Random();
            this.Steps = new List<TransformStep>();
            this.Weight = 1;
            this.Chance = 1;
            this.ConditionalIndex = null;
            this.ConditionalRegex = null;
        }


        /// <summary>
        /// <para>
        /// Applies this <see cref="Transform"/> on the specified <see cref="Name"/>
        /// and returns a new <see cref="Name"/> as a result. 
        /// </para>
        /// <para>
        /// The transform may result in no changes if a condition 
        /// was added and is not met, or if the <see href="Chance"/>
        /// property is between 0 and 1 exclusive (less than 100%).
        /// </para>
        /// <para>
        /// This method leaves the source <see cref="Name"/> unchanged.
        /// </para>
        /// </summary>
        public Name Apply(Name name)
        {
            var result = new Name(name);
            this.Modify(result);
            return result;
        }

        /// <summary>
        /// <para>
        /// Applies this <see cref="Transform"/> on the specified <see cref="Name"/> in
        /// a destructive manner. For a non-destructive alternative, use <see cref="Apply(Name)"/> instead.
        /// </para>
        /// <para>
        /// The transform may result in no changes if a condition 
        /// was added and is not met, or if the <see href="Chance"/>
        /// property is between 0 and 1 exclusive (less than 100%).
        /// </para>
        /// </summary>
        public void Modify(Name name)
        {
            if (this.Random.NextDouble() < this.Chance) 
            { 
                var conditionSatisfied = true;

                if (this.ConditionalRegex != null)
                {
                    if (this.ConditionalIndex.HasValue)
                    {
                        int index = this.ConditionalIndex.Value;

                        if (index < 0) // reverse index provided, so translate into a forward index (eg. -1 is the last syllable)
                        {
                            index = name.Syllables.Count + index;
                        }

                        if (!Regex.IsMatch(name.Syllables[index], this.ConditionalRegex))
                        {
                            conditionSatisfied = false;
                        }
                    }
                    else if (!Regex.IsMatch(name.ToString(), this.ConditionalRegex))
                    {
                        conditionSatisfied = false;
                    }
                }

                if (conditionSatisfied)
                {
                    foreach (var step in this.Steps)
                    {
                        if (this.Random.NextDouble() < step.Chance)
                        {
                            step.Modify(name);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Adds a new step to this transform.
        /// </summary>
        public Transform AddStep(TransformStep step)
        {
            this.Steps.Add(step);
            return this;
        }

    }
}
