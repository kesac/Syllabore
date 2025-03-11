﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Fluent
{
    /// <summary>
    /// Provides additional fluent extension methods for configuring a <see cref="NameGeneratorV3"/>.
    /// </summary>
    public static class NameGeneratorFluentExtensions
    {
        /// <summary>
        /// Configures the leading <see cref="SyllableGeneratorV3"/> of a <see cref="NameGeneratorV3"/>.
        /// </summary>
        public static NameGeneratorV3 Lead(this NameGeneratorV3 names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Leading, configuration);
        }

        /// <summary>
        /// Configures the inner <see cref="SyllableGeneratorV3"/> of a <see cref="NameGeneratorV3"/>.
        /// </summary>
        public static NameGeneratorV3 Inner(this NameGeneratorV3 names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Inner, configuration);
        }

        /// <summary>
        /// Configures the trailing <see cref="SyllableGeneratorV3"/> of a <see cref="NameGeneratorV3"/>.
        /// </summary>
        public static NameGeneratorV3 Trail(this NameGeneratorV3 names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Trailing, configuration);
        }

        /// <summary>
        /// Configures the leading <see cref="SyllableGeneratorV3"/> of a <see cref="NameGeneratorV3"/>.
        /// </summary>
        public static NameGeneratorV3 Any(this NameGeneratorV3 names,
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            return names.Define(SyllablePosition.Any, configuration);
        }

        private static NameGeneratorV3 Define(this NameGeneratorV3 names, SyllablePosition syllablePosition, 
            Func<SyllableGeneratorFluentWrapper, SyllableGeneratorFluentWrapper> configuration)
        {
            var wrapper = configuration(new SyllableGeneratorFluentWrapper(names, syllablePosition, new SyllableGeneratorV3()));
            names.Set(syllablePosition, wrapper.Result);
            return names;
        }
    }
}
