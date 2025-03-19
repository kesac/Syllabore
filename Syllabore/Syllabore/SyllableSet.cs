using Archigen;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Syllabore
{

    /// <summary>
    /// <para>
    /// A special kind of syllable generator that constructs a finite
    /// set of syllables and only returns syllables from that set.
    /// </para>
    /// <para>
    /// Names constructed from a <see cref="SyllableSet"/> can give the appearance
    /// of cohesion as if they originated from a similar geographic region,
    /// culture, historical period, etc.
    /// </para>
    /// </summary>
    public class SyllableSet : ISyllableGenerator, IRandomizable
    {
        public SyllableGenerator SyllableGenerator { get; set; }
        public List<string> PossibleSyllables { get; set; }

        /// <summary>
        /// The maximum number of syllables for the <see cref="SyllableSet.SyllableGenerator"/>
        /// to generate. This value has no effect if there is no <see cref="SyllableGenerator"/>.
        /// </summary>
        public int MaximumGeneratedSyllables { get; set; }
        public bool ForceUnique { get; set; }

        /// <summary>
        /// The instance used to simulate randomness.
        /// </summary>
        [JsonIgnore]
        public Random Random { get; set; }
        
        /// <summary>
        /// Initializes an empty set.
        /// </summary>
        public SyllableSet()
        {
            this.Random = new Random();
            this.PossibleSyllables = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyllableSet"/> class with the specified syllables.
        /// </summary>
        /// <param name="syllables">The syllables to include in this set.</param>
        public SyllableSet(params string[] syllables) : this()
        {
            this.PossibleSyllables.AddRange(syllables);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyllableSet"/> class with the specified syllable generator.
        /// </summary>
        public SyllableSet(SyllableGenerator syllableGenerator, int maxSyllableCount, bool forceUnique = false) : this()
        {
            this.ForceUnique = forceUnique;

            this.SyllableGenerator = syllableGenerator;
            this.MaximumGeneratedSyllables = maxSyllableCount;
        }

        /// <summary>
        /// Adds the specified syllables to the set.
        /// </summary>
        /// <param name="syllables">The syllables to add to the set.</param>
        public SyllableSet Add(params string[] syllables)
        {
            foreach(var syllable in syllables)
            {
                if(!this.ForceUnique || !this.PossibleSyllables.Contains(syllable))
                {
                    this.PossibleSyllables.Add(syllable);
                }
            }

            return this;
        }

        /// <summary>
        /// Generates a new syllable from the set.
        /// </summary>
        public string Next()
        {
            if (this.PossibleSyllables.Count < this.MaximumGeneratedSyllables && this.SyllableGenerator != null)
            {
                int attempts = 0;
                int maxAttempts = this.MaximumGeneratedSyllables * 2;

                while(this.PossibleSyllables.Count < this.MaximumGeneratedSyllables)
                {
                    var result = this.SyllableGenerator.Next();

                    if(!ForceUnique || !PossibleSyllables.Contains(result))
                    {
                        this.PossibleSyllables.Add(result);
                    }

                    attempts++;

                    if(attempts >= maxAttempts)
                    {
                        throw new InvalidOperationException("Could not generate enough unique syllables.");
                    }

                }
            }

            if(this.PossibleSyllables.Count == 0)
            {
                throw new InvalidOperationException("No syllables have been added to this set.");
            }

            return this.PossibleSyllables[this.Random.Next(this.PossibleSyllables.Count)];
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="SyllableSet"/>.
        /// </summary>
        public ISyllableGenerator Copy()
        {
            var copy = new SyllableSet();
            copy.PossibleSyllables.AddRange(this.PossibleSyllables);
            
            if(this.SyllableGenerator != null)
            {
                copy.SyllableGenerator = this.SyllableGenerator?.Copy() as SyllableGenerator;
                copy.MaximumGeneratedSyllables = this.MaximumGeneratedSyllables;
            }

            return copy;
        }
    }
}
