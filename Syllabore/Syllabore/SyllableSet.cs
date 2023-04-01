using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// A special kind of syllable provider that constructs a finite
    /// set of syllables and only returns syllables from that set.
    /// A <see cref="SyllableSet"/> can be used as the syllable provider for
    /// a <see cref="NameGenerator"/>.
    /// </para>
    /// <para>
    /// Names constructed from a syllable set can give the appearance
    /// of cohesion as if they originate from a similar geographic region,
    /// culture, historical period, etc.
    /// </para>
    /// </summary>
    public class SyllableSet : ISyllableProvider
    {
        public int StartingSyllableMax { get; set; }
        public int MiddleSyllableMax { get; set; }
        public int EndingSyllableMax { get; set; }

        private ISyllableProvider Provider { get; set; }

        public HashSet<string> StartingSyllables { get; set; }
        public HashSet<string> MiddleSyllables { get; set; }
        public HashSet<string> EndingSyllables { get; set; }

        /// <summary>
        /// Instantiates a new syllable set with a default size of
        /// 8 starting syllables, 8 middle syllables, and 8 ending syllables.
        /// The <see cref="DefaultSyllableProvider"/> is used to construct the
        /// syllables.
        /// </summary>
        public SyllableSet() : this(8, 8, 8) 
        {
            // Purposely empty
        }

        /// <summary>
        /// Instantiates a new syllable set with the specified sizes.
        /// A <see cref="DefaultSyllableProvider"/> is used to construct the
        /// syllables unless replaced with a call to <see cref="WithProvider(ISyllableProvider)"/>.
        /// </summary>
        public SyllableSet(int startingSyllableCount, int middleSyllableCount, int endingSyllableCount)
        {
            this.StartingSyllables = new HashSet<string>();
            this.MiddleSyllables = new HashSet<string>();
            this.EndingSyllables = new HashSet<string>();

            this.StartingSyllableMax = startingSyllableCount;
            this.MiddleSyllableMax = middleSyllableCount;
            this.EndingSyllableMax = endingSyllableCount;

            this.WithProvider(new DefaultSyllableProvider());
        }

        /// <summary>
        /// Uses the specified <see cref="SyllableProvider"/> to create this
        /// syllable set's finite pool of syllables.
        /// </summary>
        public SyllableSet WithProvider(Func<SyllableProvider, SyllableProvider> config)
        {
            this.WithProvider(config(new SyllableProvider()));
            return this;
        }

        /// <summary>
        /// Uses the specified <see cref="ISyllableProvider"/> to create this
        /// syllable set's finite pool of syllables.
        /// </summary>
        public SyllableSet WithProvider(ISyllableProvider provider)
        {
            this.Provider = provider;
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

        // Inherited
        public string NextStartingSyllable()
        {
            if (this.StartingSyllables.Count < this.StartingSyllableMax)
            {
                for (int i = this.StartingSyllables.Count; i < this.StartingSyllableMax; i++)
                {
                    this.StartingSyllables.Add(this.Provider.NextStartingSyllable());
                }
            }

            return this.StartingSyllables.RandomItem<string>();
        }

        // Inherited
        public string NextSyllable()
        {
            if (this.MiddleSyllables.Count < this.MiddleSyllableMax)
            {
                for (int i = this.MiddleSyllables.Count; i < this.MiddleSyllableMax; i++)
                {
                    this.MiddleSyllables.Add(this.Provider.NextSyllable());
                }
            }

            return this.MiddleSyllables.RandomItem<string>();
        }

        // Inherited
        public string NextEndingSyllable()
        {
            if (this.EndingSyllables.Count < this.EndingSyllableMax)
            {
                for (int i = this.EndingSyllables.Count; i < this.EndingSyllableMax; i++)
                {
                    this.EndingSyllables.Add(this.Provider.NextEndingSyllable());
                }
            }

            return this.EndingSyllables.RandomItem<string>();
        }

    }
}
