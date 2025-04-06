using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Fluent
{
    public static class TransformSetFluentExtensions
    {
        /// <summary>
        /// Sets the probability of this <see cref="TransformSet"/>. The value must be a double between 0 and 1 inclusive.
        /// </summary>
        public static TransformSet Chance(this TransformSet set, double chance)
        {
            set.Chance = chance;
            return set;
        }

        /// <summary>
        /// Adds a new <see cref="Transform"/> to this <see cref="TransformSet"/>.
        /// </summary>
        public static TransformSet Add(this TransformSet set, Func<Transform, Transform> config)
        {
            set.Add(config(new Transform()));
            return set;
        }
    }
}
