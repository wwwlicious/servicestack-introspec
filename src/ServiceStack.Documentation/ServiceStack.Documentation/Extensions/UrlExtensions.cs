// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class UrlExtensions
    {
        // Regex to get any 
        private static readonly Regex pathVariableRegex = new Regex("\\{([A-Za-z0-9-_]+)\\}");
        
        // TODO Handle wildcards
        public static List<string> GetPathParams(this string path)
        {
            if (path.IsNullOrEmpty())
                return Enumerable.Empty<string>().ToList();

            var matches = pathVariableRegex.Matches(path);

            var output = new List<string>();
            foreach (Match match in matches)
            {
                if (!match.Success)
                    continue;
                
                output.Add(match.Groups[1].Value);
            }

            return output.Count > 0 ? output : Enumerable.Empty<string>().ToList();
        }
    }
}