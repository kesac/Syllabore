using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Used by <see cref="SyllableGenerator"/> to support context-aware
    /// method chaining.
    /// </summary>
    public enum Context
    {
        /// <summary>
        /// Default generator context.
        /// </summary>
        None,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are leading consonants.
        /// </summary>
        LeadingConsonant,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are leading consonant sequences.
        /// </summary>
        LeadingConsonantSequence,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are vowels.
        /// </summary>
        Vowel,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are vowel sequences.
        /// </summary>
        VowelSequence,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are trailing consonants.
        /// </summary>
        TrailingConsonant,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are trailing consonant sequences.
        /// </summary>
        TrailingConsonantSequence,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are final consonants.
        /// </summary>
        LeadingVowelInStartingSyllable,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are final consonant sequences.
        /// </summary>
        LeadingVowelSequenceInStartingSyllable,

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are final consonants.
        /// </summary>
        FinalConsonant, 

        /// <summary>
        /// The last added <see cref="Grapheme">grapheme(s)</see> are final consonant sequences.
        /// </summary>
        FinalConsonantSequence
    }

    /// <summary>
    /// Descriptors for the position of a syllable within a name.
    /// </summary>
    public enum SyllablePosition
    {
        /// <summary>
        /// An indeterminate position.
        /// </summary>
        Unknown,

        /// <summary>
        /// The first syllable of a name.
        /// </summary>
        Starting,

        /// <summary>
        /// Any position that is not the 
        /// first or last syllable of a name.
        /// </summary>
        Middle,

        /// <summary>
        /// The last syllable of a name.
        /// </summary>
        Ending
    }

    /// <summary>
    /// Generates syllables based on a set of configurable vowels and consonants. A instance
    /// of this class should have its consonant and vowel pools defined before being added to
    /// a <see cref="NameGenerator"/>.
    /// </summary>
    [Serializable]
    public class SyllableGenerator : ISyllableGenerator, IRandomizable
    {
        // These probabilities are applied if custom values are not specified
        private const double DefaultChanceLeadingConsonantExists = 0.95;           // Only applies if consonants are supplied
        private const double DefaultChanceLeadingConsonantBecomesSequence = 0.25;  // Only if consonant sequences are supplied
        private const double DefaultChanceVowelExists = 1.0;                       // Only applies if vowels are supplied
        private const double DefaultChanceVowelBecomesSequence = 0.25;             // Only if vowel sequences are supplied
        private const double DefaultChanceTrailingConsonantExists = 0.10;          // Only applies if consonants are supplied
        private const double DefaultChanceTrailingConsonantBecomesSequence = 0.25; // Only if consonant sequences are supplied
        private const double DefaultChanceFinalConsonantExists = 0.50;             // Only applies if final consonants are supplied
        private const double DefaultChanceFinalConsonantBecomesSequence = 0.25;    // Only if consonant final sequences are supplied


        private Context _context { get; set; } // For contextual command .Sequences()
        private List<Grapheme> _lastChanges { get; set; } // For contextual command .Weight()

        /// <summary>
        /// Used to simulate randomness during syllable generation.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// Leading consonants are consonants that appear before 
        /// the vowel within a syllable.
        /// </summary>
        public List<Grapheme> LeadingConsonants { get; set; }

        /// <summary>
        /// Leading consonants sequences are sequences that appear 
        /// before the vowel within a syllable. Consonant sequences are made
        /// up of more than one <see cref="Grapheme"/>, but are treated
        /// like a single consonant during syllable generation.
        /// </summary>
        public List<Grapheme> LeadingConsonantSequences { get; set; }

        /// <summary>
        /// The vowels that can appear within a syllable.
        /// </summary>
        public List<Grapheme> Vowels { get; set; }

        /// <summary>
        /// The vowel sequences that can appear within a syllable.
        /// Sequences are made up of more than one 
        /// <see cref="Grapheme"/>, but treated as a single vowel.
        /// </summary>
        public List<Grapheme> VowelSequences { get; set; }

        /// <summary>
        /// Trailing consonants are consonants that appear after
        /// the vowel within a syllable.
        /// </summary>
        public List<Grapheme> TrailingConsonants { get; set; }

        /// <summary>
        /// Trailing consonant sequences are sequences that appear
        /// after the vowel within a syllable. Consonant sequences are made
        /// up of more than one <see cref="Grapheme"/>, but are treated
        /// like a single consonant during syllable generation.
        /// </summary>
        public List<Grapheme> TrailingConsonantSequences { get; set; }

        /// <summary>
        /// Final consonants are identical to trailing consonants, except
        /// they only appear in the final syllable of a name.
        /// </summary>
        public List<Grapheme> FinalConsonants { get; set; }

        /// <summary>
        /// Final consonant sequences are identical to trailing consonant sequences, 
        /// except they only appear in the final syllable of a name.
        /// </summary>
        public List<Grapheme> FinalConsonantSequences { get; set; }

        /// <summary>
        /// The probability settings for this <see cref="SyllableGenerator"/>.
        /// </summary>
        public GeneratorProbability Probability { get; set; }

        /// <summary>
        /// When true, <see cref="SyllableGenerator"/> will not throw an exception
        /// when it generates an empty string. By default, this is false.
        /// </summary>
        public bool AllowEmptyStringGeneration { get; set; }

        #region Convenience Properties

        /// <summary>
        /// Returns true if the probability of a leading consonant being generated is greater than zero.
        /// </summary>
        public bool LeadingConsonantsAllowed 
        { 
            get
            {
                return Probability.ChanceLeadingConsonantExists.HasValue && Probability.ChanceLeadingConsonantExists > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a leading consonant sequence being generated is greater than zero.
        /// </summary>
        public bool LeadingConsonantSequencesAllowed
        {
            get
            {
                return Probability.ChanceLeadingConsonantIsSequence.HasValue && Probability.ChanceLeadingConsonantIsSequence > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a vowel being generated is greater than zero.
        /// </summary>
        public bool VowelsAllowed
        {
            get
            {
                return Probability.ChanceVowelExists.HasValue && Probability.ChanceVowelExists > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a vowel sequence being generated is greater than zero.
        /// </summary>
        public bool VowelSequencesAllowed
        {
            get
            {
                return Probability.ChanceVowelIsSequence.HasValue && Probability.ChanceVowelIsSequence > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a trailing consonant being generated is greater than zero.
        /// </summary>
        public bool TrailingConsonantsAllowed
        {
            get
            {
                return Probability.ChanceTrailingConsonantExists.HasValue && Probability.ChanceTrailingConsonantExists > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a trailing consonant sequence being generated is greater than zero.
        /// </summary>
        public bool TrailingConsonantSequencesAllowed
        {
            get
            {
                return Probability.ChanceTrailingConsonantIsSequence.HasValue && Probability.ChanceTrailingConsonantIsSequence > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a final consonant being generated is greater than zero.
        /// </summary>
        public bool FinalConsonantsAllowed
        {
            get
            {
                return Probability.ChanceFinalConsonantExists.HasValue && Probability.ChanceFinalConsonantExists > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a final consonant sequence being generated is greater than zero.
        /// </summary>
        public bool FinalConsonantSequencesAllowed
        {
            get
            {
                return Probability.ChanceFinalConsonantIsSequence.HasValue && Probability.ChanceFinalConsonantIsSequence > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a leading vowel being generated within the starting syllable is greater than zero.
        /// </summary>
        public bool LeadingVowelForStartingSyllableAllowed
        {
            get
            {
                return Probability.ChanceStartingSyllableLeadingVowelExists.HasValue && Probability.ChanceStartingSyllableLeadingVowelExists > 0;
            }
        }

        /// <summary>
        /// Returns true if the probability of a leading vowel sequence being generated within the starting syllable is greater than zero.
        /// </summary>
        public bool LeadingVowelSequenceForStartingSyllableAllowed
        {
            get
            {
                return Probability.ChanceStartingSyllableLeadingVowelIsSequence.HasValue && Probability.ChanceStartingSyllableLeadingVowelIsSequence > 0;
            }
        }

        #endregion

        /// <summary>
        /// Instantiates a new <see cref="SyllableGenerator"/> with
        /// an empty pool of vowels and consonants.
        /// </summary>
        public SyllableGenerator()
        {
            this.Random = new Random();
            this.Probability = new GeneratorProbability();
            _context = Context.None;
            _lastChanges = new List<Grapheme>();
            this.AllowEmptyStringGeneration = false;

            this.LeadingConsonants = new List<Grapheme>();
            this.LeadingConsonantSequences = new List<Grapheme>();
            this.Vowels = new List<Grapheme>();
            this.VowelSequences = new List<Grapheme>();
            this.TrailingConsonants = new List<Grapheme>();
            this.TrailingConsonantSequences = new List<Grapheme>();
            this.FinalConsonants = new List<Grapheme>();
            this.FinalConsonantSequences = new List<Grapheme>();
        }

        /// <summary>
        /// Instantiates a new <see cref="SyllableGenerator"/> with
        /// an the specified vowels and consonants. Note that consonants
        /// added this way are considered both leading and trailing consonants.
        /// </summary>
        public SyllableGenerator(string vowels, string consonants) : this() 
        {
            this.WithVowels(vowels);
            this.WithConsonants(consonants);
        }


        /// <summary>
        /// Adds the specified consonants into the pool of leading and trailing
        /// consonants.  Within a syllable, leading consonants are consonants 
        /// that appear before a vowel and trailing consonants are consonants 
        /// that appear after a vowel.
        /// </summary>
        public SyllableGenerator WithConsonants(params string[] consonants)
        {
            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.LeadingConsonants.AddRange(changes);
            this.TrailingConsonants.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceLeadingConsonantExists == null)
            {
                this.Probability.ChanceLeadingConsonantExists = DefaultChanceLeadingConsonantExists;
            }

            if (this.Probability.ChanceTrailingConsonantExists == null)
            {
                this.Probability.ChanceTrailingConsonantExists = DefaultChanceTrailingConsonantExists;
            }

            _context = Context.None;

            return this;
        }

        /// <summary>
        /// Adds the specified consonants to the pool of leading consonants.
        /// Within a syllable, leading consonants are consonants that appear before a vowel.
        /// </summary>
        public SyllableGenerator WithLeadingConsonants(params string[] consonants)
        {

            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.LeadingConsonants.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceLeadingConsonantExists == null)
            {
                this.Probability.ChanceLeadingConsonantExists = DefaultChanceLeadingConsonantExists;
            }

            _context = Context.LeadingConsonant;

            return this;
        }


        /// <summary>
        /// Adds the specified consonant sequences into the pool of leading consonant sequences.
        /// </summary>
        public SyllableGenerator WithLeadingConsonantSequences(params string[] consonantSequences)
        {
            var changes = consonantSequences.Select(x => new Grapheme(x)).ToList();

            this.LeadingConsonantSequences.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if(this.Probability.ChanceLeadingConsonantIsSequence == null)
            {
                this.Probability.ChanceLeadingConsonantIsSequence = DefaultChanceLeadingConsonantBecomesSequence;
            }

            _context = Context.LeadingConsonantSequence;

            return this;
        }

        /// <summary>
        /// Adds the specified vowels to the pool of possible vowels this
        /// <see cref="SyllableGenerator"/> can generate.
        /// </summary>
        public SyllableGenerator WithVowels(params string[] vowels)
        {
            var changes = new List<Grapheme>();

            foreach (var v in vowels)
            {
                changes.AddRange(v.Atomize().Select(x => new Grapheme(x)));
            }

            this.Vowels.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceVowelExists == null)
            {
                this.Probability.ChanceVowelExists = DefaultChanceVowelExists;
            }

            _context = Context.Vowel;

            return this;
        }

        /// <summary>
        /// Adds the specified vowel sequences to the pool of possible vowel sequences this
        /// <see cref="SyllableGenerator"/> can generate.
        /// </summary>
        public SyllableGenerator WithVowelSequences(params string[] vowelSequences)
        {
            var changes = vowelSequences.Select(x => new Grapheme(x)).ToList();

            this.VowelSequences.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if(this.Probability.ChanceVowelIsSequence == null)
            {
                this.Probability.ChanceVowelIsSequence = DefaultChanceVowelBecomesSequence;
            }

            _context = Context.VowelSequence;

            return this;
        }

        /// <summary>
        /// Adds the specified consonants to the pool of trailing consonants.
        /// Within a syllable, trailing consonants are consonants that appear 
        /// after a vowel.
        /// </summary>
        public SyllableGenerator WithTrailingConsonants(params string[] consonants)
        {
            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.TrailingConsonants.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceTrailingConsonantExists == null)
            {
                this.Probability.ChanceTrailingConsonantExists = DefaultChanceTrailingConsonantExists;
            }

            _context = Context.TrailingConsonant;

            return this;
        }


        /// <summary>
        /// Adds the specified consonant sequences as sequences that can appear after a vowel.
        /// </summary>
        public SyllableGenerator WithTrailingConsonantSequences(params string[] consonantSequences)
        {
            var changes = consonantSequences.Select(x => new Grapheme(x)).ToList();
            this.TrailingConsonantSequences.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if(this.Probability.ChanceTrailingConsonantIsSequence == null)
            {
                this.Probability.ChanceTrailingConsonantIsSequence = DefaultChanceTrailingConsonantBecomesSequence;
            }

            _context = Context.TrailingConsonantSequence;

            return this;
        }

        /// <summary>
        /// Adds the specified consonants to the pool of consonants that must only appear 
        /// as the trailing consonant of an ending syllable.
        /// </summary>
        public SyllableGenerator WithFinalConsonants(params string[] consonants)
        {
            var changes = new List<Grapheme>();

            foreach (var c in consonants)
            {
                changes.AddRange(c.Atomize().Select(x => new Grapheme(x)));
            }

            this.FinalConsonants.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceFinalConsonantExists == null)
            {
                this.Probability.ChanceFinalConsonantExists = DefaultChanceFinalConsonantExists;
            }

            _context = Context.FinalConsonant;

            return this;
        }

        /// <summary>
        /// Adds the specified consonant sequences to the pool of sequences that must only appear 
        /// as the trailing consonant sequence of an ending syllable.
        /// </summary>
        public SyllableGenerator WithFinalConsonantSequences(params string[] consonantSequences)
        {
            var changes = consonantSequences.Select(x => new Grapheme(x)).ToList();
            this.FinalConsonantSequences.AddRange(changes);
            _lastChanges.ReplaceWith(changes);

            if (this.Probability.ChanceFinalConsonantIsSequence == null)
            {
                this.Probability.ChanceFinalConsonantIsSequence = DefaultChanceFinalConsonantBecomesSequence;
            }

            _context = Context.FinalConsonantSequence;

            return this;
        }

        /// <summary>
        /// <em>Contextual on the last call.</em> Adds the specified sequences as
        /// leading consonants, trailing consonants, or vowels depending on the context.
        /// </summary>
        public SyllableGenerator Sequences(params string[] sequences)
        {
            switch (_context)
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
                case Context.FinalConsonant:
                    this.WithFinalConsonantSequences(sequences);
                    break;
                default:
                    _context = Context.None;
                    break;
            }

            return this;
        }

        /// <summary>
        /// <em>Contextual on the last call.</em> Sets the weight of the last 
        /// added <see cref="Grapheme">graphemes</see>.
        /// </summary>
        public SyllableGenerator Weight(int weight)
        {

            foreach(var g in _lastChanges)
            {
                g.Weight = weight;
            }

            return this;
        }

        /// <summary>
        /// Sets the probability of vowels/consonants turning into sequences, of leading
        /// consonants starting a syllable, of trailing consonants ending a syllable, etc.
        /// </summary>
        public SyllableGenerator WithProbability(Func<GeneratorProbabilityBuilder, GeneratorProbabilityBuilder> configure)
        {
            var p = new GeneratorProbabilityBuilder(this.Probability);
            this.Probability = configure(p).ToProbability();
            return this;
        }

        /// <summary>
        /// Specifying a value of <c>true</c> will permit generation of empty strings
        /// as syllables. This is a scenario if there are no vowels/consonants to choose from or if the probability
        /// table does not guarantee that syllable output is never a zero-length string. By default, this is <c>false</c>
        /// and <see cref="SyllableGenerator"/> throws an Exception whenever an empty string is generated.
        /// </summary>
        public SyllableGenerator AllowEmptyStrings(bool allow)
        {
            this.AllowEmptyStringGeneration = allow;
            return this;
        }

        private string NextLeadingConsonant()
        {
            if(this.LeadingConsonants.Count > 0)
            {
                //return this.LeadingConsonants[this.Random.Next(this.LeadingConsonants.Count)].Value;
                return this.LeadingConsonants.RandomWeightedItem(this.Random).Value;
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
                return this.LeadingConsonantSequences.RandomWeightedItem(this.Random).Value;
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
                return this.Vowels.RandomWeightedItem(this.Random).Value;
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
                return this.VowelSequences.RandomWeightedItem(this.Random).Value;
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
                return this.TrailingConsonants.RandomWeightedItem(this.Random).Value;
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
                return this.TrailingConsonantSequences.RandomWeightedItem(this.Random).Value;
            }
            else
            {
                throw new InvalidOperationException("No trailing consonant sequences defined.");
            }

        }

        private string NextFinalConsonant()
        {
            if (this.FinalConsonants.Count > 0)
            {
                return this.FinalConsonants.RandomWeightedItem(this.Random).Value;
            }
            else
            {
                throw new InvalidOperationException("No final consonants defined.");
            }
        }

        private string NextFinalConsonantSequence()
        {
            if (this.FinalConsonantSequences.Count > 0)
            {
                return this.FinalConsonantSequences.RandomWeightedItem(this.Random).Value;
            }
            else
            {
                throw new InvalidOperationException("No final consonant sequences defined.");
            }
        }

        /// <summary>
        /// Returns a random syllable suitable for starting a name.
        /// </summary>
        public virtual string NextStartingSyllable()
        {
            return GenerateSyllable(SyllablePosition.Starting);
        }

        /// <summary>
        /// Returns a random syllable suitable for any position.
        /// </summary>
        /// <returns></returns>
        public virtual string NextSyllable()
        {
            return GenerateSyllable(SyllablePosition.Middle);
        }

        /// <summary>
        /// Returns a random suitable suitable for ending a name.
        /// </summary>
        /// <returns></returns>
        public virtual string NextEndingSyllable()
        {
            return GenerateSyllable(SyllablePosition.Ending);
        }

        private string GenerateSyllable(SyllablePosition position)
        {
            var output = new StringBuilder();

            if (position == SyllablePosition.Starting 
                && this.LeadingVowelForStartingSyllableAllowed 
                && this.Random.NextDouble() < this.Probability.ChanceStartingSyllableLeadingVowelExists)
            {

                if (this.VowelSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceStartingSyllableLeadingVowelIsSequence)
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
                // Determine if there is a leading consonant in this syllable
                if (this.LeadingConsonantsAllowed && this.Random.NextDouble() < this.Probability.ChanceLeadingConsonantExists)
                {

                    // If there is a leading consonant, determine if it is a sequence
                    if (this.LeadingConsonantSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceLeadingConsonantIsSequence)
                    {
                        output.Append(this.NextLeadingConsonantSequence());
                    }
                    else
                    {
                        output.Append(this.NextLeadingConsonant());
                    }
                }

                // Determine if there is a vowel in this syllable (by default, this probability is 100%)
                if(this.VowelsAllowed && this.Random.NextDouble() < this.Probability.ChanceVowelExists)
                {

                    // Then check if the vowel is a vowel sequence
                    if (this.VowelSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceVowelIsSequence)
                    {
                        output.Append(this.NextVowelSequence());
                    }
                    else
                    {
                        output.Append(this.NextVowel());
                    }
                }

            }

            // If we're generating the final syllable, check if we need to use a 'final' consonant
            // (as opposed to a 'trailing' consonant)
            if(position == SyllablePosition.Ending
                && this.FinalConsonantsAllowed
                && this.Random.NextDouble() < this.Probability.ChanceFinalConsonantExists)
            {
                // We need to append a final consonant,
                // but we need to determine if the consonant is a sequence first
                if (this.FinalConsonantSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceFinalConsonantIsSequence)
                {
                    output.Append(this.NextFinalConsonantSequence());
                }
                else
                {
                    output.Append(this.NextFinalConsonant());
                }
            }
            // Otherwise, roll for a trailing consonant
            else if (this.TrailingConsonantsAllowed && this.Random.NextDouble() < this.Probability.ChanceTrailingConsonantExists)
            {
                // If we need to append a trailing consonant, check if
                // we need a sequence instead
                if (this.TrailingConsonantSequencesAllowed && this.Random.NextDouble() < this.Probability.ChanceTrailingConsonantIsSequence)
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
