using Archigen;
using System;
using System.Collections.Generic;

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
        private SyllableGenerator _syllableGenerator;
        private int _maxSyllableCount;
        private List<string> _possibleSyllables;
        private bool _forceUnique; // Should only be set via constructor

        /// <summary>
        /// The instance used to simulate randomness.
        /// </summary>
        public Random Random { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SyllableSet"/> class with the specified syllables.
        /// </summary>
        /// <param name="syllables">The syllables to include in this set.</param>
        public SyllableSet(params string[] syllables)
        {
            this.Random = new Random();
            _possibleSyllables = new List<string>(syllables);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyllableSet"/> class with the specified syllable generator.
        /// </summary>
        public SyllableSet(SyllableGenerator syllableGenerator, int maxSyllableCount, bool forceUnique = false)
        {
            this.Random = new Random();
            _possibleSyllables = new List<string>();
            _forceUnique = forceUnique;

            _syllableGenerator = syllableGenerator;
            _maxSyllableCount = maxSyllableCount;
        }

        /// <summary>
        /// Adds the specified syllables to the set.
        /// </summary>
        /// <param name="syllables">The syllables to add to the set.</param>
        public SyllableSet Add(params string[] syllables)
        {
            foreach(var syllable in syllables)
            {
                if(!_forceUnique || !_possibleSyllables.Contains(syllable))
                {
                    _possibleSyllables.Add(syllable);
                }
            }

            return this;
        }

        /// <summary>
        /// Generates a new syllable from the set.
        /// </summary>
        public string Next()
        {
            if (_possibleSyllables.Count < _maxSyllableCount && _syllableGenerator != null)
            {
                int attempts = 0;
                int maxAttempts = _maxSyllableCount * 2;

                while(_possibleSyllables.Count < _maxSyllableCount)
                {
                    var result = _syllableGenerator.Next();

                    if(!_forceUnique || !_possibleSyllables.Contains(result))
                    {
                        _possibleSyllables.Add(result);
                    }

                    attempts++;

                    if(attempts >= maxAttempts)
                    {
                        throw new InvalidOperationException("Could not generate enough unique syllables.");
                    }

                }
            }

            if(_possibleSyllables.Count == 0)
            {
                throw new InvalidOperationException("No syllables have been added to this set.");
            }

            return _possibleSyllables[this.Random.Next(_possibleSyllables.Count)];
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="SyllableSet"/>.
        /// </summary>
        public ISyllableGenerator Copy()
        {
            var copy = new SyllableSet();
            copy._possibleSyllables.AddRange(_possibleSyllables);
            
            if(this._syllableGenerator != null)
            {
                copy._syllableGenerator = this._syllableGenerator?.Copy() as SyllableGenerator;
                copy._maxSyllableCount = this._maxSyllableCount;
            }

            return copy;
        }
    }
}
