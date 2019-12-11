using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // This is meant to be a standlone name validator for quick use
    public class BasicNameValidator : INameValidator
    {

        private static readonly string[] InvalidEndings =
        {
            "J", "Q", "V", "X", "Z", "CH", "BL", "CL", "FL", "GL", "BR", "CR", "DR", "PR", "TR", "TH", "SC", "SP"
        };

        public bool IsValidName(string name)
        {
            bool isValid = true;

            if (name != null) {
                foreach (var ending in InvalidEndings)
                {
                    if (name.ToLower().EndsWith(ending))
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            else
            {
                isValid = false;
            }

            return isValid;

        }
    }
}
