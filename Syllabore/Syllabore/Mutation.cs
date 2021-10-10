using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// Used by <see cref="NameGenerator"/> through <see cref="NameMutator"/> to capture
    /// mutations that produce variations on names. Mutations can also have
    /// an optional condition that must be fulfilled for the mutation to occur.
    /// </summary>
    public class Mutation
    {
        public List<MutationStep> Steps { get; set; }

        /// <summary>
        /// A positive integer that influences the probability of this mutation being used over others.
        /// Given two mutations X and Y with a weight of 3 and 1 respectively, mutation X will be applied 75% of the time.
        /// All mutations default to a weight of 1.
        /// </summary>
        public int Weight { get; set; }
        public int? ConditionalIndex { get; set; }
        public string ConditionalRegex { get; set; }

        public Mutation()
        {
            this.Steps = new List<MutationStep>();
            this.Weight = 1;
            this.ConditionalIndex = null;
            this.ConditionalRegex = null;
        }
        public void Apply(Name name)
        {
            foreach (var step in this.Steps)
            {
                step.Apply(name);
            }
        }

        public Mutation When(string regex)
        {
            return this.When(int.MinValue, regex);
        }

        /// <summary>
        /// Adds a condition to this mutation. The condition is a regex pattern applied
        /// to a syllable at the specified index. It must be satisfied for the entire mutation
        /// to run. Note that while multiple calls to <c>When()</c> are possible, only the last
        /// call will have an effect.
        /// </summary>
        /// <param name="index">The index of the syllable that the condition operates 
        /// on. A negative index can be provided to traverse from the end of the name
        /// instead. (For example, an index -1 will be interpreted as the last syllable
        /// of a name.</param>
        /// <param name="regex">The pattern that must be satisfied.</param>
        /// <returns></returns>
        public Mutation When(int index, string regex)
        {
            if (index == int.MaxValue)
            {
                this.ConditionalIndex = null;
            }
            else
            {
                this.ConditionalIndex = index;
            }

            this.ConditionalRegex = regex;

            return this;
        }

        /// <summary>
        /// Applies a weight to this mutation that influences the probability of being used over others.
        /// Given two mutations X and Y with a weight of 3 and 1 respectively, mutation X will be applied 75% of the time.
        /// All mutations default to a weight of 1.
        /// </summary>
        public Mutation WithWeight(int weight)
        {
            this.Weight = weight;
            return this;
        }

        /// <summary>
        /// Adds a mutation step that replaces a syllable at the specified index with
        /// a desired string.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        /// <param name="replacement">The string to substitute in.</param>
        /// <returns></returns>
        public Mutation ReplaceSyllable(int index, string replacement)
        {
            this.Steps.Add(new MutationStep(MutationStepType.ReplaceSyllable, index.ToString(), replacement));
            return this;
        }

        public Mutation ReplaceAll(string substring, string replacement)
        {
            this.Steps.Add(new MutationStep(MutationStepType.ReplaceAllSubstring, substring, replacement));
            return this;
        }

        /// <summary>
        /// Adds a mutation step that inserts a new syllable at the specified index. The
        /// syllable at that index and the others after it will be pushed one index to the right.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        /// <param name="replacement">The string to insert.</param>
        /// <returns></returns>
        public Mutation InsertSyllable(int index, string syllable)
        {
            this.Steps.Add(new MutationStep(MutationStepType.InsertSyllable, index.ToString(), syllable));
            return this;
        }

        /// <summary>
        /// Adds a mutation step that appends a new syllable to the end of a name.
        /// </summary>
        public Mutation AppendSyllable(string syllable)
        {
            this.Steps.Add(new MutationStep(MutationStepType.AppendSyllable, syllable));
            return this;
        }

        /// <summary>
        /// Adds a mutation step that removes the syllable at the specified index.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        public Mutation RemoveSyllable(int index)
        {
            this.Steps.Add(new MutationStep(MutationStepType.RemoveSyllable, index.ToString()));
            return this;
        }

        /// <summary>
        /// Executes the specified action on a name. Note that this mutation step cannot
        /// be serialized.
        /// </summary>
        public Mutation ExecuteUnserializableAction(Action<Name> unserializableAction)
        {
            this.Steps.Add(new MutationStep(unserializableAction));
            return this;
        }
        
    }
}
