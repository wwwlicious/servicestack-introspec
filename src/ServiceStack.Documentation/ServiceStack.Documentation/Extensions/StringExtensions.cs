// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System.Text;

    public static class StringExtensions
    {
        const char space = ' ';

        /// <summary>
        /// Takes a camel/pascal-case string and adds space before any Capital letter (except first)
        /// </summary>
        /// <param name="text"></param>
        /// <returns>String split on capitals</returns>
        /// <remarks>Based on accepted answer for http://stackoverflow.com/questions/272633/add-spaces-before-capital-letters</remarks>
        public static string ToSpaced(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            
            for (var x = 1; x < text.Length; x++)
            {
                if (char.IsUpper(text[x]) && (IsNewUpper(text, x) || IsEndOfAcronym(text, x)))
                    newText.Append(space);
                newText.Append(text[x]);
            }
            return newText.ToString();
        }

        private static bool IsEndOfAcronym(string text, int index)
        {
            // Preceding char is upper AND next character isn't beyond bounds AND next char isn't upper
            return char.IsUpper(text[index - 1]) && index < text.Length - 1 && !char.IsUpper(text[index + 1]);
        }

        private static bool IsNewUpper(string text, int index)
        {
            // Preceding char is not a space OR uppercase
            var preceding = text[index - 1];
            return !(preceding == space || char.IsUpper(preceding));
        }
    }
}
