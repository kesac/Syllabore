using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Represents one action or step in a <see cref="Transform"/>.
    /// </summary>
    public class TransformStep : IPotentialAction
    {
        /// <summary>
        /// The type of action this <see cref="TransformSet"/> represents.
        /// </summary>
        public TransformStepType Type { get; set; }

        /// <summary>
        /// The probability this <see cref="TransformStep"/> will make changes
        /// when <see cref="Modify(Name)"/> is called.
        /// </summary>
        public double Chance { get; set; }

        /// <summary>
        /// The arguments that are passed to the action.
        /// </summary>
        public List<string> Arguments { get; set; }

        /// <summary>
        /// If this <see cref="TransformStep"/> is of type <see cref="TransformStepType.Lambda"/>,
        /// this property will contain the action to be applied.
        /// </summary>
        private Action<Name> _unserializableAction;

        /// <summary>
        /// Instantiates a new <see cref="TransformStep"/> with
        /// no type or arguments.
        /// </summary>
        public TransformStep() // Needs to exist for serialization
        {
            this.Arguments = new List<string>();
            this.Chance = 1;
        }

        /// <summary>
        /// Instantiates a new <see cref="TransformStep"/> with
        /// the specified type and arguments.
        /// </summary>
        public TransformStep(TransformStepType type, params string[] args) : this()
        {
            this.Type = type;
            this.Arguments.AddRange(args);
        }

        /// <summary>
        /// Instantiates a new <see cref="TransformStep"/> with
        /// type <see cref="TransformStepType.Lambda"/> and the
        /// specified <see cref="Action"/> to execute.
        /// Note that this type of <see cref="TransformSet"/> is 
        /// not serializable.
        /// </summary>
        /// <param name="unserializableAction"></param>
        public TransformStep(Action<Name> unserializableAction) : this()
        {
            this.Type = TransformStepType.Lambda;
            _unserializableAction = unserializableAction;
        }

        /// <summary>
        /// Applies this transform step to the specified <see cref="Name"/>.
        /// This method is destructive.
        /// </summary>
        public void Modify(Name name)
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
                _unserializableAction(name);
            }
            

        }

    }
}
