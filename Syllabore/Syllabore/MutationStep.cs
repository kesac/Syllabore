using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public enum MutationStepType
    {
        SyllableInsertion,
        SyllableAppend,
        SyllableReplacement,
        SyllableRemoval,
        Lambda // Note: this cannot be serialized
    }

    /// <summary>
    /// Represents one action or step in a mutation chain.
    /// </summary>
    public class MutationStep
    {
        public MutationStepType Type { get; set; }
        public List<string> Arguments { get; set; }
        private Action<Name> UnserializableAction { get; set; }

        public MutationStep()
        {
            this.Arguments = new List<string>();
        }

        public MutationStep(MutationStepType type, params string[] args)
        {
            this.Type = type;
            this.Arguments = new List<string>(args);
        }

        public MutationStep(Action<Name> unserializableAction)
        {
            this.Type = MutationStepType.Lambda;
            this.UnserializableAction = unserializableAction;
        }

        public void Apply(Name name)
        {
            if(this.Type == MutationStepType.SyllableInsertion)
            {
                int index = int.Parse(this.Arguments[0]);
                string insertion = this.Arguments[1];

                if (index < 0)
                {
                    index = index + name.Syllables.Count;
                }

                name.Syllables.Insert(index, insertion);
            }
            else if (this.Type == MutationStepType.SyllableAppend)
            {
                string append = this.Arguments[0];

                name.Syllables.Add(append);
            }
            else if (this.Type == MutationStepType.SyllableReplacement)
            {
                int index = int.Parse(this.Arguments[0]);
                string replacement = this.Arguments[1];

                if(index < 0)
                {
                    index = index + name.Syllables.Count;
                }

                name.Syllables[index] = replacement;
            }
            else if (this.Type == MutationStepType.SyllableRemoval)
            {
                int index = int.Parse(this.Arguments[0]);

                if (index < 0)
                {
                    index = index + name.Syllables.Count;
                }

                name.Syllables.RemoveAt(index);
            }
            else if(this.Type == MutationStepType.Lambda)
            {
                UnserializableAction(name);
            }
            

        }

    }
}
