using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// Used by <see cref="SyllableProvider"/> to support context-aware
    /// method chaining.
    /// </summary>
    public enum Context
    {
        None,
        LeadingConsonant,
        LeadingConsonantSequence,
        Vowel,
        VowelSequence,
        TrailingConsonant,
        TrailingConsonantSequence,
        LeadingVowelInStartingSyllable,
        LeadingVowelSequenceInStartingSyllable
    }

    /// <summary>
    /// Generates syllables based on a set of configurable vowels and consonants. A instance
    /// of this class should have its consonant and vowel pools defined before being added to
    /// a <see cref="NameGenerator"/>.
    /// </summary>
    [Serializable]
    public class SyllableProvider : ISyllableProvider
    {
        public const double DefaultChanceLeadingConsonantExists = 0.95;
        public const double DefaultChanceLeadingConsonantBecomesSequence = 0.25;
        public const double DefaultChanceVowelExists = 1.0;
        public const double DefaultChanceVowelBecomesSequence = 0.25;
        public const double DefaultChanceTrailingConsonantExists = 0.10;
        public const double DefaultChanceTrailingConsonantBecomesSequence = 0.25;
        public const double DefaultChanceLeadingVowelInStartingSyllableExists = 0.10;
        public const double DefaultChanceLeadingVowelInStartingSyllableBecomesSequence = 0.25;

        private Random Random { get; set; }
        private Context Context { get; set; } // For contextual command .Sequences()
        private List<Grapheme> LastChanges { get; set; } // For contextual command .Weight()
        

        public SyllableProviderProbability Probability { get; set; }

        public List<Grapheme> LeadingConsonants { get; set; }
        public List<Grapheme> LeadingConsonantSequences { get; set; }
        public List<Grapheme> Vowels { get; set; }
        public List<Grapheme> VowelSequences { get; set; }
        public List<Grapheme> TrailingConsonants { get; set; }
        public List<Grapheme> TrailingConsonantSequences { get; set; }

        public bool AllowEmptyStringGeneration { get; set; }

        public bool LeadingConsonantsAllowed 
        { 
            get
            {
                return Probability.ChanceLeadingConsonantExists.HasValue && Probability.ChanceLeadingConsonantExists > 0;
            }
        }

        public bool LeadingConsonantSequencesAllowed
        {
            get
            {
                return Probability.ChanceLeadingConsonantBecomesSequence.HasValue && Probability.ChanceLeadingConsonantBecomesSequence > 0;
            }
        }

        public bool VowelsAllowed
        {
            get
            {
                return Probability.ChanceVowelExists.HasValue && Probability.ChanceVowelExists > 0;
            }
        }

        public bool VowelSequencesAllowed
        {
            get
            {
                return Probability.ChanceVowelBecomesSequence.HasValue && Probability.ChanceVowelBecomesSequence > 0;
            }
        }

        public bool TrailingConsonantsAllowed
        {
            get
            {
                return Probability.ChanceTrailingConsonantExists.HasValue && Probability.ChanceTrailingConsonantExists > 0;
            }
        }

        public bool TrailingConsonantSequencesAllowed
        {
            get
            {
                return Probability.ChanceTrailingConsonantBecomesSequence.HasValue && Probability.ChanceTrailingConsonantBecomesSequence > 0;
            }
        }

        public bool LeadingVowelForStartingSyllableAllowed
        {
            get
            {
                return Probability.StartingSyllable.ChanceLeadingVowelExists.HasValue && Probability.StartingSyllable.ChanceLeadingVowelExists > 0;
            }
        }

        public bool LeadingVowelSequenceForStartingSyllableAllowed
        {
            get
            {
                return Probability.StartingSyllable.ChanceLeadingVowelBecomesSequence.HasValue && Probability.StartingSyllable.ChanceLeadingVowelBecomesSequence > 0;
            }
        }


        public SyllableProvider()
        {
            this.Random = new Random();
            this.Probability = new SyllableProviderProbability();
            this.Context = Context.None;
            this.LastChanges = new List<Grapheme>();
            this.AllowEmptyStringGeneration = false;

            this.LeadingConsonants = new List<Grapheme>();
            this.LeadingConsonantSequences = new List<Grapheme>();
            this.Vowels = new List<Grapheme>();
            this.VowelSequences = new List<Grapheme>();
            this.TrailingConsonants = new List<Grapheme>();
            this.TrailingConsonantSequences = new List<Grapheme>();
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can occur before or after a vowel.
        /// </summary>
        public SyllableProvider WithConsonants(params string[] consonants)
        {
            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.LeadingConsonants.AddRange(changes);
            this.TrailingConsonants.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceLeadingConsonantExists == null)
            {
                this.Probability.ChanceLeadingConsonantExists = DefaultChanceLeadingConsonantExists;
            }

            if (this.Probability.ChanceTrailingConsonantExists == null)
            {
                this.Probability.ChanceTrailingConsonantExists = DefaultChanceTrailingConsonantExists;
            }

            this.Context = Context.None;

            return this;
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can occur before a vowel.
        /// </summary>
        public SyllableProvider WithLeadingConsonants(params string[] consonants)
        {

            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.LeadingConsonants.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceLeadingConsonantExists == null)
            {
                this.Probability.ChanceLeadingConsonantExists = DefaultChanceLeadingConsonantExists;
            }

            this.Context = Context.LeadingConsonant;

            return this;
        }


        /// <summary>
        /// Adds the specified consonant sequences as sequences that can occur before a vowel.
        /// </summary>
        public SyllableProvider WithLeadingConsonantSequences(params string[] consonantSequences)
        {
            var changes = consonantSequences.Select(x => new Grapheme(x)).ToList();

            this.LeadingConsonantSequences.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if(this.Probability.ChanceLeadingConsonantBecomesSequence == null)
            {
                this.Probability.ChanceLeadingConsonantBecomesSequence = DefaultChanceLeadingConsonantBecomesSequence;
            }

            this.Context = Context.LeadingConsonantSequence;

            return this;
        }

        /// <summary>
        /// Adds the specified vowels as vowels that can be used to form the 'center' of syllables.
        /// </summary>
        public SyllableProvider WithVowels(params string[] vowels)
        {
            var changes = new List<Grapheme>();

            foreach (var v in vowels)
            {
                changes.AddRange(v.Atomize().Select(x => new Grapheme(x)));
            }

            this.Vowels.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceVowelExists == null)
            {
                this.Probability.ChanceVowelExists = DefaultChanceVowelExists;
            }

            this.Context = Context.Vowel;

            return this;
        }

        /// <summary>
        /// Adds the specified vowel sequences as sequences that can be used to form the 'center' of syllables.
        /// </summary>
        public SyllableProvider WithVowelSequences(params string[] vowelSequences)
        {
            var changes = vowelSequences.Select(x => new Grapheme(x)).ToList();

            this.VowelSequences.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if(this.Probability.ChanceVowelBecomesSequence == null)
            {
                this.Probability.ChanceVowelBecomesSequence = DefaultChanceVowelBecomesSequence;
            }

            this.Context = Context.VowelSequence;

            return this;
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can appear after a vowel.
        /// </summary>
        public SyllableProvider WithTrailingConsonants(params string[] consonants)
        {
            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.TrailingConsonants.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceTrailingConsonantExists == null)
            {
                this.Probability.ChanceTrailingConsonantExists = DefaultChanceTrailingConsonantExists;
            }

            this.Context = Context.TrailingConsonant;

            return this;
        }


        /// <summary>
        /// Adds the specified consonant sequences as sequences that can appear after a vowel.
        /// </summary>
        public SyllableProvider WithTrailingConsonantSequences(params string[] consonantSequences)
        {
            var changes = consonantSequences.Select(x => new Grapheme(x)).ToList();
            this.TrailingConsonantSequences.AddRange(changes);
            this.LastChanges.ReplaceWith(changes);

            if(this.Probability.ChanceTrailingConsonantBecomesSequence == null)
            {
                this.Probability.ChanceTrailingConsonantBecomesSequence = DefaultChanceTrailingConsonantBecomesSequence;
            }

            this.Context = Context.TrailingConsonantSequence;

            return this;
        }

        /// <summary>
        /// Defines the specified sequences for leading consonants, trailing consonants,
        /// or vowels depending on the context.
        /// </summary>
        public SyllableProvider Sequences(params string[] sequences)
        {
            switch (this.Context)
            {
                case Context.LeadingConsonant:
                    this.WithLeadingConsonantSequences(sequences);
                    break;
                case Context.Vowel:
                    this.WithVowelSequences(sequences);
                    break;
                case Context.TrailingConsonant:
                    this.WithTrailingConsonantSequences(sequences);
                    break;
                default:
                    this.Context = Context.None;
                    break;
            }

            return this;
        }

        
        public SyllableProvider Weight(int weight)
        {

            foreach(var g in this.LastChanges)
            {
                g.Weight = weight;
            }

            return this;
        }
        

        /// <summary>
        /// Used to manage the probability of vowels/consonants turning into sequences, of leading
        /// consonants starting a syllable, of trailing consonants ending a syllable, etc.
        /// </summary>
        public SyllableProvider WithProbability(Func<SyllableProviderProbability, SyllableProviderProbability> configure)
        {
            this.Probability = configure(this.Probability);
            return this;
        }

        /// <summary>
        /// Used to manage the probability of vowels/consonants turning into sequences, of leading
        /// consonants starting a syllable, of trailing consonants ending a syllable, etc.
        /// </summary>
        public SyllableProvider WithProbability(
            double LeadingConsonants = -1,
            double LeadingConsonantSequences = -1,
            double Vowels = -1,
            double VowelSequences = -1,
            double TrailingConsonants = -1,
            double TrailingConsonantSequences = -1
        )
        {
            if(LeadingConsonants != -1)
            {
                this.Probability.ChanceLeadingConsonantExists = LeadingConsonants;
            }

            if (LeadingConsonantSequences != -1)
            {
                this.Probability.ChanceLeadingConsonantBecomesSequence = LeadingConsonantSequences;
            }

            if (Vowels != -1)
            {
                this.Probability.ChanceVowelExists = Vowels;
            }

            if (VowelSequences != -1)
            {
                this.Probability.ChanceVowelBecomesSequence = VowelSequences;
            }

            if (TrailingConsonants != -1)
            {
                this.Probability.ChanceTrailingConsonantExists = TrailingConsonants;
            }

            if (TrailingConsonantSequences != -1)
            {
                this.Probability.ChanceTrailingConsonantBecomesSequence = TrailingConsonantSequences;
            }

            return this;
        }

        /// <summary>
        /// Specifying a value of <c>true</c> will permit generation of empty strings
        /// as syllables. This is a scenario if there are no vowels/consonants to choose from or if the probability
        /// table does not guarantee that syllable output is never a zero-length string. By default, this is <c>false</c>
        /// and <see cref="SyllableProvider"/> throws an Exception whenever an empty string is generated.
        /// </summary>
        public SyllableProvider AllowEmptyStrings(bool allow)
        {
            this.AllowEmptyStringGeneration = allow;
            return this;
        }

        private string NextLeadingConsonant()
        {
            if(this.LeadingConsonants.Count > 0)
            {
                //return this.LeadingConsonants[this.Random.Next(this.LeadingConsonants.Count)].Value;
                return this.LeadingConsonants.RandomWeightedItem().Value;
            }
            else
            {
                throw new InvalidOperationException("No leading consonants defined.");
            }
        }

        private string NextLeadingConsonantSequence()
        {
            if (this.LeadingConsonantSequences.Count > 0)
            {
                //return this.LeadingConsonantSequences[this.Random.Next(this.LeadingConsonantSequences.Count)].Value;
                return this.LeadingConsonantSequences.RandomWeightedItem().Value;
            }
            else
            {
                throw new InvalidOperationException("No leading consonant sequences defined.");
            }
        }

        private string NextVowel()
        {
            if (this.Vowels.Count > 0)
            {
                //return this.Vowels[this.Random.Next(this.Vowels.Count)].Value;
                return this.Vowels.RandomWeightedItem().Value;
            }
            else
            {
                throw new InvalidOperationException("A vowel was required, but no vowels defined.");
            }
        }

        private string NextVowelSequence()
        {
            if (this.VowelSequences.Count > 0)
            {
                // return this.VowelSequences[this.Random.Next(this.VowelSequences.Count)].Value;
                return this.VowelSequences.RandomWeightedItem().Value;
            }
            else
            {
                throw new InvalidOperationException("A vowel sequence was required, but no vowel sequences defined.");
            }
        }

        private string NextTrailingConsonant()
        {
            if (this.TrailingConsonants.Count > 0)
            {
                //return this.TrailingConsonants[this.Random.Next(this.TrailingConsonants.Count)].Value;
                return this.TrailingConsonants.RandomWeightedItem().Value;
            }
            else
            {
                throw new InvalidOperationException("No trailing consonants defined.");
            }
        }

        private string NextTrailingConsonantSequence()
        {
            if (this.TrailingConsonantSequences.Count > 0)
            {
                // return this.TrailingConsonantSequences[this.Random.Next(this.TrailingConsonantSequences.Count)].Value;
                return this.TrailingConsonantSequences.RandomWeightedItem().Value;
            }
            else
            {
                throw new InvalidOperationException("No trailing consonant sequences defined.");
            }

        }


        public virtual string NextStartingSyllable()
        {
            return GenerateSyllable(true);
        }

        public virtual string NextSyllable()
        {
            return GenerateSyllable(false);
        }


        public virtual string NextEndingSyllable()
        {
            return GenerateSyllable(false);
        }

        private string GenerateSyllable(bool isStartingSyllable)
        {
            var output = new StringBuilder();

            if (isStartingSyllable && this.LeadingVowelForStartingSyllableAllowed && this.Random.NextDouble() < this.Probability.StartingSyllable.ChanceLeadingVowelExists)
            {

                //if (this.VowelSequencesAllowed && this.Random.NextDouble() < this.ChanceVowelBeginsStartingSyllableAndIsSequence)
                if (this.VowelSequencesAllowed && this.Random.NextDouble() < this.Probability.StartingSyllable.ChanceLeadingVowelBecomesSequence)
                {
                    output.Append(this.NextVowelSequence());
                }
                else
                {
                    output.Append(this.NextVowel());
                }
            }
            else
            {

                if (this.LeadingConsonantsAllowed && this.Random.NextDouble() < this.Probability.ChanceLeadingConsonantExists)
                {

                    if (this.LeadingConsonantSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceLeadingConsonantBecomesSequence)
                    {
                        output.Append(this.NextLeadingConsonantSequence());
                    }
                    else
                    {
                        output.Append(this.NextLeadingConsonant());
                    }
                }

                if(this.VowelsAllowed && this.Random.NextDouble() < this.Probability.ChanceVowelExists)
                {
                    if (this.VowelSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceVowelBecomesSequence)
                    {
                        output.Append(this.NextVowelSequence());
                    }
                    else
                    {
                        output.Append(this.NextVowel());
                    }
                }

            }

            if (this.TrailingConsonantsAllowed && this.Random.NextDouble() < this.Probability.ChanceTrailingConsonantExists)
            {

                if (this.TrailingConsonantSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceTrailingConsonantBecomesSequence)
                {
                    output.Append(this.NextTrailingConsonantSequence());
                }
                else
                {
                    output.Append(this.NextTrailingConsonant());
                }
            }

            if(!AllowEmptyStringGeneration && output.Length == 0)
            {
                throw new InvalidOperationException("SyllableProvider could not generate anything but an empty string.");
            }

            return output.ToString();
        }


    }
}
