using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// A quick and dirty standlone name provider for quick use. Custom validators
    /// should use <c>ConfigurableNameValidator</c> instead.
    /// </summary>
    public class StandaloneNameValidator : ConfigurableNameValidator
    {
        public StandaloneNameValidator()
        {
            // Invalidate awkward looking endings
            this.AddConstraintAsRegex("[j|p|q|v|w|z]$");
        }
    }
}
