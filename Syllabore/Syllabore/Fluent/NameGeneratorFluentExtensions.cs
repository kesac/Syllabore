using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Fluent
{
    /// <summary>
    /// Provides additional fluent extension methods for configuring a <see cref="NameGenerator"/>.
    /// </summary>
    public static class NameGeneratorFluentExtensions
    {
        /// <summary>
        /// Configures the leading <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Lead(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Leading, configuration);
        }

        /// <summary>
        /// Configures the inner <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Inner(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Inner, configuration);
        }

        /// <summary>
        /// Configures the trailing <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Trail(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Trailing, configuration);
        }

        /// <summary>
        /// Configures the leading <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Any(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Any, configuration);
        }

        /// <summary>
        /// Sets the filter for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Transform(this NameGenerator names, 
            Func<Transform, Transform> configuration)
        {
            names.Transform(1.0, configuration(new Transform()));
            return names;
        }

        /// <summary>
        /// Sets the filter for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Transform(this NameGenerator names, double chance,
            Func<Transform, Transform> configuration)
        {
            names.Transform(chance, configuration(new Transform()));
            return names;
        }

        /// <summary>
        /// Sets the filter for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Filter(this NameGenerator names, params string[] regexPatterns)
        {
            names.NameFilter = new NameFilter();

            foreach (var pattern in regexPatterns)
            {
                names.NameFilter.DoNotAllowRegex(pattern);
            }

            return names;
        }

        /// <summary>
        /// Sets the filter for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Filter(this NameGenerator names,
            Func<NameFilter, NameFilter> configuration)
        {
            names.NameFilter = configuration(new NameFilter());
            return names;
        }

        private static NameGenerator Define(this NameGenerator names, SyllablePosition syllablePosition, 
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            var wrapper = configuration(new SyllableGeneratorFluentWrapper(names, syllablePosition, new SyllableGenerator()));
            names.Set(syllablePosition, wrapper.Result);
            return names;
        }
    }
}
