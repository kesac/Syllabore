using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Convenience methods for manipulating or drawing elements
    /// from <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        private static Random Random = new Random();

        /// <summary>
        /// Clears the contents of the current list and replaces it
        /// with all elements of the new list.
        /// </summary>
        public static void ReplaceWith<T>(this IList<T> current, IList<T> newValues)
        {
            current.Clear();

            // AddRange doesn't exist for IList<T>
            foreach(var value in newValues)
            {
                current.Add(value);
            }

        }

        /// <summary>
        /// Given a list of <see cref="IWeighted"/>, this method returns the sum of
        /// all elements' <c>Weight</c> property.
        /// </summary>
        public static int TotalWeight<T>(this IList<T> weightedItems) where T : IWeighted
        {
            return weightedItems.Sum(x => x.Weight);
        }

        /// <summary>
        /// Returns a random item from an <see cref="IList{T}"/>.
        /// </summary>
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

        /// <summary>
        /// Returns a random item from an <see cref="ISet{T}"/>.
        /// </summary>
        public static T RandomItem<T>(this ISet<T> set)
        {
            return set.ToList<T>().RandomItem<T>();
        }

        /// <summary>
        /// Returns a weighted-random item from an <see cref="IList{T}"/>, 
        /// where <c>T</c> implements <see cref="IWeighted"/>.
        /// Elements with a higher <c>Weight</c> value will have a higher probability
        /// of being selected over elements with lower <c>Weight</c> values.
        /// </summary>
        public static T RandomWeightedItem<T>(this IList<T> weightedItems) where T : IWeighted
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
