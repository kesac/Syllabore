﻿using System;
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
        /// Configures the starting <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Start(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Starting, configuration);
        }

        /// <summary>
        /// Sets the starting <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Start(this NameGenerator names, ISyllableGenerator syllables)
        {
            return names.SetSyllables(SyllablePosition.Starting, syllables);
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
        /// Sets the inner <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Inner(this NameGenerator names, ISyllableGenerator syllables)
        {
            return names.SetSyllables(SyllablePosition.Inner, syllables);
        }

        /// <summary>
        /// Configures the ending <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator End(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Ending, configuration);
        }

        /// <summary>
        /// Sets the ending <see cref="SyllableGenerator"/> of a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator End(this NameGenerator names, ISyllableGenerator syllables)
        {
            return names.SetSyllables(SyllablePosition.Ending, syllables);
        }

        /// <summary>
        /// Sets the <see cref="SyllableGenerator"/> for all syllable positions. 
        /// </summary>
        public static NameGenerator Any(this NameGenerator names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Any, configuration);
        }

        /// <summary>
        /// Sets the <see cref="SyllableGenerator"/> for all syllable positions. 
        /// </summary>
        public static NameGenerator Any(this NameGenerator names, ISyllableGenerator syllables)
        {
            return names.SetSyllables(SyllablePosition.Any, syllables);
        }

        /// <summary>
        /// Sets the transform for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Transform(this NameGenerator names, Transform transform)
        {
            names.SetTransform(transform);
            return names;
        }

        /// <summary>
        /// Sets the transform for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Transform(this NameGenerator names, TransformSet transformSet)
        {
            names.SetTransform(transformSet);
            return names;
        }

        /// <summary>
        /// Sets the transform for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Transform(this NameGenerator names, 
            Func<Transform, Transform> configuration)
        {
            names.SetTransform(configuration(new Transform()));
            return names;
        }

        /// <summary>
        /// Sets the filter for a <see cref="NameGenerator"/>.
        /// </summary>
        public static NameGenerator Filter(this NameGenerator names, params string[] regexPatterns)
        {
            var filter = new NameFilter();

            foreach (var pattern in regexPatterns)
            {
                filter.DoNotAllowRegex(pattern);
            }

            names.NameFilter = filter;

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
            names.SetSyllables(syllablePosition, wrapper.Result);
            return names;
        }
    }
}
