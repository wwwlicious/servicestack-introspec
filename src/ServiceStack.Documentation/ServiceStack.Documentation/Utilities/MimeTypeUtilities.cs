// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    public static class MimeTypeUtilities
    {
        private static readonly Dictionary<string, string> mimeDictionary;
        static MimeTypeUtilities()
        {
            // There's no nice way to get mime type (application/json) from type (json).
            // MimeTypes class has all known types as constants so build dictionary of these

            mimeDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var fieldInfos = typeof(MimeTypes).GetFields(BindingFlags.Public | BindingFlags.Static);

            var strType = typeof(string);
            foreach (var fi in fieldInfos.Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == strType))
                mimeDictionary.Add(fi.Name, fi.GetValue(null).ToString());
        }

        public static string GetMimeType(string type) => mimeDictionary.SafeGet(type, () => MimeTypes.GetMimeType(type));
    }
}
