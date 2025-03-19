using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Fluent
{
    public static class TransformFluentExtensions
    {
        public static Transform Weight(this Transform transform, int weight)
        {
            transform.Weight = weight;
            return transform;
        }

        /// <summary>
        /// Sets the probability of the last added transform step. To set the probability of the 
        /// transform itself, use <see cref="TriggerChance(double)"/>.
        /// </summary>
        public static Transform StepChance(this Transform transform, double chance)
        {
            transform.Steps[transform.Steps.Count - 1].Chance = chance;
            return transform;
        }

        /// <summary>
        /// Sets the probability that this transform and all of its steps runs.
        /// </summary>
        public static Transform TransformChance(this Transform transform, double chance)
        {
            transform.Chance = chance;
            return transform;
        }

        /// <summary>
        /// <para>
        /// Adds a condition to this <see cref="Transform"/>. The condition is a regular expression applied
        /// to a syllable at the specified <paramref name="index"/>. It must be satisfied for the <see cref="Transform"/>
        /// to be applied successfully.
        /// </para>
        /// <para>The specified <paramref name="index"/> determines the location of the syllable 
        /// that the condition operates on. A negative <paramref name="index"/> can be provided to traverse from the end of the name
        /// instead. (For example, an index -1 will be interpreted as the last syllable of a name.)
        /// </para>
        /// </summary>
        public static Transform When(this Transform transform, int index, string regex)
        {
            if (index == int.MaxValue)
            {
                transform.ConditionalIndex = null;
            }
            else
            {
                transform.ConditionalIndex = index;
            }

            transform.ConditionalRegex = regex;

            return transform;
        }

        /// <summary>
        /// Adds a step that replaces a syllable at the specified index with
        /// a desired string.
        /// <para>
        /// The index can be a negative integer to traverse from the
        /// end of the name instead. For example, an index -1 will be interpreted as the
        /// last syllable of a name.
        /// </para>
        /// </summary>
        public static Transform Replace(this Transform transform, int index, string replacement)
        {
            transform.AddStep(new TransformStep(TransformStepType.ReplaceSyllable, index.ToString(), replacement));
            return transform;
        }

        /// <summary>
        /// Adds a step that replaces all instances of the specified substring in each syllable with
        /// a desired string. Note that the substring must be completely contained in a syllable to be replaced.
        /// </summary>
        public static Transform ReplaceSubstring(this Transform transform, string substring, string replacement)
        {
            transform.AddStep(new TransformStep(TransformStepType.ReplaceAllSubstring, substring, replacement));
            return transform;
        }

        /// <summary>
        /// Adds a transform step that inserts a new syllable at the specified index. The
        /// syllable at that index and the others after it will be pushed one index to the right.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        /// <param name="syllable">The string to insert.</param>
        /// <returns></returns>
        public static Transform Insert(this Transform transform, int index, string syllable)
        {
            transform.AddStep(new TransformStep(TransformStepType.InsertSyllable, index.ToString(), syllable));
            return transform;
        }

        /// <summary>
        /// Adds a transform step that appends a new syllable to the end of a name.
        /// </summary>
        public static Transform Append(this Transform transform, string syllable)
        {
            transform.AddStep(new TransformStep(TransformStepType.AppendSyllable, syllable));
            return transform;
        }

        /// <summary>
        /// Adds a step that removes the syllable at the specified index.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        public static Transform Remove(this Transform transform, int index)
        {
            transform.AddStep(new TransformStep(TransformStepType.RemoveSyllable, index.ToString()));
            return transform;
        }

        /// <summary>
        /// Executes the specified action on a name. Note that this transform step cannot
        /// be serialized.
        /// </summary>
        public static Transform ExecuteUnserializableAction(this Transform transform, Action<Name> unserializableAction)
        {
            transform.AddStep(new TransformStep(unserializableAction));
            return transform;
        }
    }
}
