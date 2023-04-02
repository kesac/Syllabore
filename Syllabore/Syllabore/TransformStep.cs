using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public enum TransformStepType
    {
        InsertSyllable,
        AppendSyllable,
        ReplaceSyllable,
        RemoveSyllable,
        Lambda, // Note: this one cannot be serialized
        ReplaceAllSubstring
    }

    /// <summary>
    /// Represents one action or step in a mutation chain.
    /// </summary>
    public class TransformStep
    {
        public TransformStepType Type { get; set; }
        public List<string> Arguments { get; set; }
        private Action<Name> UnserializableAction { get; set; }

        public TransformStep()
        {
            this.Arguments = new List<string>();
        }

        public TransformStep(TransformStepType type, params string[] args)
        {
            this.Type = type;
            this.Arguments = new List<string>(args);
        }

        public TransformStep(Action<Name> unserializableAction)
        {
            this.Type = TransformStepType.Lambda;
            this.UnserializableAction = unserializableAction;
        }

        /// <summary>
        /// Applies this transform step to the specified <see cref="Name"/>.
        /// This method is destructive.
        /// </summary>
        public void Apply(Name name)
        {
            if(this.Type == TransformStepType.InsertSyllable)
            {
                int index = int.Parse(this.Arguments[0]);
                string insertion = this.Arguments[1];

                if (index < 0)
                {
                    index = index + name.Syllables.Count;
                }

                name.Syllables.Insert(index, insertion);
            }
            else if (this.Type == TransformStepType.AppendSyllable)
            {
                string append = this.Arguments[0];

                name.Syllables.Add(append);
            }
            else if (this.Type == TransformStepType.ReplaceSyllable)
            {
                int index = int.Parse(this.Arguments[0]);
                string replacement = this.Arguments[1];

                if(index < 0)
                {
                    index = index + name.Syllables.Count;
                }

                name.Syllables[index] = replacement;
            }
            else if (this.Type == TransformStepType.RemoveSyllable)
            {
                int index = int.Parse(this.Arguments[0]);

                if (index < 0)
                {
                    index = index + name.Syllables.Count;
                }

                name.Syllables.RemoveAt(index);
            }
            else if(this.Type == TransformStepType.ReplaceAllSubstring)
            {
                string substring = this.Arguments[0].ToLower();
                string replacement = this.Arguments[1].ToLower();

                for(int i = 0; i < name.Syllables.Count; i++)
                {
                    var syllable = name.Syllables[i].ToLower();
                    if (syllable.Contains(substring))
                    {
                        name.Syllables[i] = syllable.Replace(substring, replacement);
                    }
                }

            }
            else if(this.Type == TransformStepType.Lambda)
            {
                UnserializableAction(name);
            }
            

        }

    }
}
