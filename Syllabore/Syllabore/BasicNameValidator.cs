using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // This is meant to be a standlone name validator for quick use
    public class BasicNameValidator : IValidator
    {

        private static readonly string[] AwkwardEndings =
        {
            "J", "P", "Q", "V", "W", "Z"
        };

        public bool IsValidName(string name)
        {
            bool isValid = true;

            if (name != null) {
                foreach (var ending in AwkwardEndings)
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
