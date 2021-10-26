using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    public static class ListExtensions
    {
        private static Random Random = new Random();

        public static void ReplaceWith(this List<Grapheme> graphemes, List<Grapheme> values)
        {
            graphemes.Clear();
            graphemes.AddRange(values);
        }

        public static int TotalWeight(this List<Grapheme> graphemes)
        {
            return graphemes.Sum(x => x.Weight);
        }

        public static T RandomChoice<T>(this List<T> list)
        {

            if(list.Count == 0)
            {
                return default(T);
            }
            else
            {
                int selection = Random.Next(list.Count);

                return list[selection];
            }

        }

        public static T RandomChoice<T>(this HashSet<T> set)
        {
            if (set.Count == 0)
            {
                return default(T);
            }
            else
            {
                return set.ToList().RandomChoice<T>();
            }

        }

        public static Grapheme RandomWeightedChoice(this List<Grapheme> graphemes)
        {
            int totalWeight = graphemes.TotalWeight();
            int randomSelection = Random.Next(totalWeight);

            int runningTotal = 0;

            for(int i = 0; i < graphemes.Count; i++)
            {
                runningTotal += graphemes[i].Weight;

                if(randomSelection < runningTotal)
                {
                    return graphemes[i];
                }
            }

            throw new InvalidOperationException("A random choice could not be made on a weighted list of graphemes. Check if there were any non-positive weights.");

        }

    }
}
