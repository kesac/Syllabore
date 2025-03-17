using System;
using System.Collections.Generic;
using System.Linq;
using Archigen;

namespace Syllabore
{
    /// <summary>
    /// Convenience methods for manipulating or drawing elements
    /// from <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Returns a weighted-random item from an <see cref="IList{T}"/>, 
        /// where <c>T</c> implements <see cref="IWeighted"/>.
        /// Elements with a higher <c>Weight</c> value will have a higher probability
        /// of being selected over elements with lower <c>Weight</c> values.
        /// <para>
        /// Random selection is done using the specified instance of <see cref="System.Random"/>.
        /// </para>
        /// </summary>
        public static T RandomWeightedItem<T>(this IList<T> weightedItems, Random random) where T : IWeighted
        {
            int totalWeight = weightedItems.Sum(x => x.Weight);
            int randomSelection = random.Next(totalWeight);

            int runningTotal = 0;

            for(int i = 0; i < weightedItems.Count; i++)
            {
                runningTotal += weightedItems[i].Weight;

                if(randomSelection < runningTotal)
                {
                    return weightedItems[i];
                }
            }

            throw new InvalidOperationException("A random choice could not be made. Check if the list was empty or if there were any non-positive weights.");

        }

    }
}
