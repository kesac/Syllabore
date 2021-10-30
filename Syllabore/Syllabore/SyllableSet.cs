using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // This type of provider constructs a finite set of syllables
    // and only returns syllables from that set. This gives
    // the appearance of cohesion across all output and is useful
    // for simulating names that might come from a specific region.
    public class SyllableSet : SyllableProvider
    {

        public int StartingSyllableMax { get; set; }
        public int NormalSyllableMax { get; set; }
        public int EndingSyllableMax { get; set; }

        public HashSet<string> StartingSyllables { get; set; }
        public HashSet<string> NormalSyllables { get; set; }
        public HashSet<string> EndingSyllables { get; set; }

        public bool IsInitialized { get; set; }

        public SyllableSet(int startingMax, int normalMax, int endingMax)
        {
            this.StartingSyllables = new HashSet<string>();
            this.NormalSyllables = new HashSet<string>();
            this.EndingSyllables = new HashSet<string>();

            this.StartingSyllableMax = startingMax;
            this.NormalSyllableMax = normalMax;
            this.EndingSyllableMax = endingMax;

        }

        public SyllableSet Initialize()
        {
            this.IsInitialized = true;

            this.StartingSyllables.Clear();
            for(int i = 0; i < this.StartingSyllableMax; i++)
            {
                this.StartingSyllables.Add(base.NextStartingSyllable());
            }

            this.NormalSyllables.Clear();
            for (int i = 0; i < this.NormalSyllableMax; i++)
            {
                this.NormalSyllables.Add(base.NextSyllable());
            }

            this.EndingSyllables.Clear();
            for (int i = 0; i < this.EndingSyllableMax; i++)
            {
                this.EndingSyllables.Add(base.NextEndingSyllable());
            }

            return this;
        }

        public override string NextStartingSyllable()
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
            }

            return this.StartingSyllables.RandomItem<string>();
        }

        public override string NextSyllable()
        {

            if (!this.IsInitialized)
            {
                this.Initialize();
            }

            return this.NormalSyllables.RandomItem<string>();
        }

        public override string NextEndingSyllable()
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
            }

            return this.EndingSyllables.RandomItem<string>();
        }


    }
}
