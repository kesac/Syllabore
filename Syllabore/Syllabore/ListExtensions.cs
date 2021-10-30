using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    public static class ListExtensions
    {
        private static Random Random = new Random();

        public static void ReplaceWith<T>(this List<T> graphemes, List<T> values)
        {
            graphemes.Clear();
            graphemes.AddRange(values);
        }

        public static int TotalWeight<T>(this List<T> weightedItems) where T : IWeighted
        {
            return weightedItems.Sum(x => x.Weight);
        }

        public static T RandomItem<T>(this IList<T> list)
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

        public static T RandomItem<T>(this ISet<T> set)
        {
            return set.ToList<T>().RandomItem<T>();
        }

        public static T RandomWeightedItem<T>(this List<T> weightedItems) where T : IWeighted
        {
            int totalWeight = weightedItems.TotalWeight();
            int randomSelection = Random.Next(totalWeight);

            int runningTotal = 0;

            for(int i = 0; i < weightedItems.Count; i++)
            {
                runningTotal += weightedItems[i].Weight;

                if(randomSelection < runningTotal)
                {
                    return weightedItems[i];
                }
            }

            throw new InvalidOperationException("A random choice could not be made on the specified list of weighted items. Check if there were any non-positive weights.");

        }

    }
}
