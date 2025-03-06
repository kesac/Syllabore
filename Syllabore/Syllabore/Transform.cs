using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
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
    public class Transform : IWeighted, INameTransformer
    {
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
            this.Steps = new List<TransformStep>();
            this.Weight = 1;
            this.ConditionalIndex = null;
            this.ConditionalRegex = null;
        }

        /// <summary>
        /// <para>
        /// Applies this <see cref="Transform"/> on the specified <see cref="Name"/> in
        /// a destructive manner.
        /// </para>
        /// <para>
        /// For a non-destructive alternative, use <see cref="Apply(Name)"/> instead.
        /// </para>
        /// </summary>
        public void Modify(Name name)
        {
            foreach (var step in this.Steps)
            {
                step.Modify(name);
            }
        }

        /// <summary>
        /// <para>
        /// Applies this <see cref="Transform"/> on the specified <see cref="Name"/>
        /// and returns a new <see cref="Name"/> as a result.
        /// </para>
        /// <para>
        /// This method leaves the source <see cref="Name"/> unchanged.
        /// </para>
        /// </summary>
        public Name Apply(Name name)
        {
            var result = new Name(name);
            foreach (var step in this.Steps)
            {
                step.Modify(result);
            }
            return result;
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
        public Transform When(int index, string regex)
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
        /// Adds a step that replaces a syllable at the specified index with
        /// a desired string.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        /// <param name="replacement">The string to substitute in.</param>
        /// <returns></returns>
        public Transform ReplaceSyllable(int index, string replacement)
        {
            this.Steps.Add(new TransformStep(TransformStepType.ReplaceSyllable, index.ToString(), replacement));
            return this;
        }

        /// <summary>
        /// Adds a step that replaces all instances of the specified substring in each syllable with
        /// a desired string. Note that the substring must be completely contained in a syllable to be replaced.
        /// </summary>
        public Transform ReplaceAll(string substring, string replacement)
        {
            this.Steps.Add(new TransformStep(TransformStepType.ReplaceAllSubstring, substring, replacement));
            return this;
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
        public Transform InsertSyllable(int index, string syllable)
        {
            this.Steps.Add(new TransformStep(TransformStepType.InsertSyllable, index.ToString(), syllable));
            return this;
        }

        /// <summary>
        /// Adds a transform step that appends a new syllable to the end of a name.
        /// </summary>
        public Transform AppendSyllable(string syllable)
        {
            this.Steps.Add(new TransformStep(TransformStepType.AppendSyllable, syllable));
            return this;
        }

        /// <summary>
        /// Adds a step that removes the syllable at the specified index.
        /// </summary>
        /// <param name="index">The index can be a negative integer to traverse from the
        /// end of the name instead. (For example, an index -1 will be interpreted as the
        /// last syllable of a name.</param>
        public Transform RemoveSyllable(int index)
        {
            this.Steps.Add(new TransformStep(TransformStepType.RemoveSyllable, index.ToString()));
            return this;
        }

        /// <summary>
        /// Executes the specified action on a name. Note that this transform step cannot
        /// be serialized.
        /// </summary>
        public Transform ExecuteUnserializableAction(Action<Name> unserializableAction)
        {
            this.Steps.Add(new TransformStep(unserializableAction));
            return this;
        }


    }
}
