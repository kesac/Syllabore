using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// A special kind of syllable generator that constructs a finite
    /// set of syllables and only returns syllables from that set.
    /// A <see cref="SyllableSet"/> can be used as the syllable generator for
    /// a <see cref="NameGenerator"/>.
    /// </para>
    /// <para>
    /// Names constructed from a <see cref="SyllableSet"/> can give the appearance
    /// of cohesion as if they originated from a similar geographic region,
    /// culture, historical period, etc.
    /// </para>
    /// </summary>
    public class SyllableSet : ISyllableGenerator
    {
        /// <summary>
        /// The syllable set size for starting syllables.
        /// </summary>
        public int StartingSyllableMax { get; set; }

        /// <summary>
        /// The syllable set size for syllables occurring
        /// between the starting and ending syllable.
        /// </summary>
        public int MiddleSyllableMax { get; set; }

        /// <summary>
        /// The syllable set size for ending syllables.
        /// </summary>
        public int EndingSyllableMax { get; set; }

        private ISyllableGenerator _generator { get; set; }

        /// <summary>
        /// The finite set of syllables to be used in the starting position of a name.
        /// </summary>
        public HashSet<string> StartingSyllables { get; set; }

        /// <summary>
        /// The finite set of syllables to be used between the starting and ending
        /// positions of a name.
        /// </summary>
        public HashSet<string> MiddleSyllables { get; set; }

        /// <summary>
        /// The finite set of syllables to be used in the ending position of a name.
        /// </summary>
        public HashSet<string> EndingSyllables { get; set; }

        /// <summary>
        /// Instantiates a new syllable set with a default size of
        /// 8 starting syllables, 8 middle syllables, and 8 ending syllables.
        /// The <see cref="DefaultSyllableGenerator"/> is used to construct the
        /// syllables.
        /// </summary>
        public SyllableSet() : this(8, 8, 8) 
        {
            // Purposely empty
        }

        /// <summary>
        /// Instantiates a new syllable set with the specified sizes.
        /// A <see cref="DefaultSyllableGenerator"/> is used to construct the
        /// syllables unless replaced with a call to <see cref="WithGenerator(ISyllableGenerator)"/>.
        /// </summary>
        public SyllableSet(int startingSyllableCount, int middleSyllableCount, int endingSyllableCount)
        {
            this.StartingSyllables = new HashSet<string>();
            this.MiddleSyllables = new HashSet<string>();
            this.EndingSyllables = new HashSet<string>();

            this.StartingSyllableMax = startingSyllableCount;
            this.MiddleSyllableMax = middleSyllableCount;
            this.EndingSyllableMax = endingSyllableCount;

            this.WithGenerator(new DefaultSyllableGenerator());
        }

        /// <summary>
        /// Uses the specified <see cref="SyllableGenerator"/> to create this
        /// syllable set's finite pool of syllables.
        /// </summary>
        public SyllableSet WithGenerator(Func<SyllableGenerator, SyllableGenerator> config)
        {
            this.WithGenerator(config(new SyllableGenerator()));
            return this;
        }

        /// <summary>
        /// Uses the specified <see cref="ISyllableGenerator"/> to create this
        /// syllable set's finite pool of syllables.
        /// </summary>
        public SyllableSet WithGenerator(ISyllableGenerator provider)
        {
            _generator = provider;
            return this;
        }

        /// <summary>
        /// Adds a syllable to this <see cref="SyllableSet"/>'s pool of starting syllables.
        /// </summary>
        /// <param name="syllables">One or more starting syllables to add this syllable set</param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if adding the specified starting syllables exceeds the maximum size of this
        ///     syllable set.
        /// </exception>
        public SyllableSet WithStartingSyllable(params string[] syllables)
        {
            foreach (var syllable in syllables)
            {
                this.StartingSyllables.Add(syllable);

                if (this.StartingSyllables.Count > this.StartingSyllableMax)
                {
                    throw new InvalidOperationException("Exceeded limit for number of starting syllables");
                }

            }

            return this;
        }

        /// <summary>
        /// Adds a syllable to this <see cref="SyllableSet"/>'s pool of "middle" (neither
        /// starting nor ending) syllables.
        /// </summary>
        /// <param name="syllables">One or more middle syllables to add this syllable set</param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if adding the specified middle syllables exceeds the maximum size of this
        ///     syllable set.
        /// </exception>
        public SyllableSet WithMiddleSyllable(params string[] syllables)
        {
            foreach (var syllable in syllables)
            {
                this.MiddleSyllables.Add(syllable);

                if (this.MiddleSyllables.Count > this.MiddleSyllableMax)
                {
                    throw new InvalidOperationException("Exceeded limit for number of middle syllables");
                }
            }

            return this;
        }

        /// <summary>
        /// Adds a syllable to this <see cref="SyllableSet"/>'s pool of ending syllables.
        /// </summary>
        /// <param name="syllables">One or more ending syllables to add this syllable set</param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if adding the specified ending syllables exceeds the maximum size of this
        ///     syllable set.
        /// </exception>
        public SyllableSet WithEndingSyllable(params string[] syllables)
        {
            foreach (var syllable in syllables)
            {
                this.EndingSyllables.Add(syllable);

                if (this.EndingSyllables.Count > this.EndingSyllableMax)
                {
                    throw new InvalidOperationException("Exceeded limit for number of ending syllables");
                }
            }

            return this;
        }

        /// <summary>
        /// Returns a random syllable suitable for use in the starting position
        /// of a name.
        /// </summary>
        public string NextStartingSyllable()
        {
            if (this.StartingSyllables.Count < this.StartingSyllableMax)
            {
                for (int i = this.StartingSyllables.Count; i < this.StartingSyllableMax; i++)
                {
                    this.StartingSyllables.Add(_generator.NextStartingSyllable());
                }
            }

            return this.StartingSyllables.RandomItem<string>();
        }

        /// <summary>
        /// Returns a random syllable suitable for use between the starting and ending
        /// positions of a name.
        /// </summary>
        public string NextSyllable()
        {
            if (this.MiddleSyllables.Count < this.MiddleSyllableMax)
            {
                for (int i = this.MiddleSyllables.Count; i < this.MiddleSyllableMax; i++)
                {
                    this.MiddleSyllables.Add(_generator.NextSyllable());
                }
            }

            return this.MiddleSyllables.RandomItem<string>();
        }

        /// <summary>
        /// Returns a random syllable suitable for use in the ending position of a name.
        /// </summary>
        public string NextEndingSyllable()
        {
            if (this.EndingSyllables.Count < this.EndingSyllableMax)
            {
                for (int i = this.EndingSyllables.Count; i < this.EndingSyllableMax; i++)
                {
                    this.EndingSyllables.Add(_generator.NextEndingSyllable());
                }
            }

            return this.EndingSyllables.RandomItem<string>();
        }

    }
}
