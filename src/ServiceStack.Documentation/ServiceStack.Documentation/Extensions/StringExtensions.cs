// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System.Text;

    public static class StringExtensions
    {
        private const char Space = ' ';

        /// <summary>
        /// Takes a camel/pascal-case string and adds space before any Capital letter (except first)
        /// </summary>
        /// <param name="text">String to operate on</param>
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
                    newText.Append(Space);
                newText.Append(text[x]);
            }
            return newText.ToString();
        }

        /// <summary>
        /// Removes occurrence of toTrim from start of string
        /// </summary>
        /// <param name="text">Text to trim</param>
        /// <param name="toTrim">String value to trim</param>
        /// <returns>Trimmed string</returns>
        public static string TrimStart(this string text, string toTrim)
        {
            if (string.IsNullOrEmpty(text) || !text.StartsWith(toTrim))
                return text;

            return text.Substring(toTrim.Length);
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
            return !(preceding == Space || char.IsUpper(preceding));
        }
    }
}
