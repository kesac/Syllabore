using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    // This type of provider constructs a finite set of syllables
    // and only returns syllables from that set. This gives
    // the appearance of cohesion across all output and is useful
    // for simulating names that might come from a specific region.
    public class SyllableSet : ISyllableProvider
    {

        public int StartingSyllableMax { get; set; }
        public int NormalSyllableMax { get; set; }
        public int EndingSyllableMax { get; set; }

        public HashSet<string> StartingSyllables { get; set; }
        public HashSet<string> NormalSyllables { get; set; }
        public HashSet<string> EndingSyllables { get; set; }

        public SyllableSet() : this(8, 8, 8) { }

        public SyllableSet(int startingMax, int normalMax, int endingMax)
        {
            this.StartingSyllables = new HashSet<string>();
            this.NormalSyllables = new HashSet<string>();
            this.EndingSyllables = new HashSet<string>();

            this.StartingSyllableMax = startingMax;
            this.NormalSyllableMax = normalMax;
            this.EndingSyllableMax = endingMax;
        }

        public SyllableSet InitializeWith(Func<SyllableProvider, SyllableProvider> config)
        {
            this.InitializeWith(config(new SyllableProvider()));
            return this;
        }

        public SyllableSet InitializeWith(ISyllableProvider provider)
        {

            this.StartingSyllables.Clear();
            for (int i = 0; i < this.StartingSyllableMax; i++)
            {
                this.StartingSyllables.Add(provider.NextStartingSyllable());
            }

            this.NormalSyllables.Clear();
            for (int i = 0; i < this.NormalSyllableMax; i++)
            {
                this.NormalSyllables.Add(provider.NextSyllable());
            }

            this.EndingSyllables.Clear();
            for (int i = 0; i < this.EndingSyllableMax; i++)
            {
                this.EndingSyllables.Add(provider.NextEndingSyllable());
            }

            return this;
        }


        public string NextStartingSyllable() => this.StartingSyllables.RandomItem<string>();

        public string NextSyllable() => this.NormalSyllables.RandomItem<string>();

        public string NextEndingSyllable() => this.EndingSyllables.RandomItem<string>();

    }
}
