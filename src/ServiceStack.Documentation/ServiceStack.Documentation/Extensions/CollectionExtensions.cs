// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> array)
        {
            return array == null || !array.Any();
        }

        public static T[] SafeUnion<T>(this T[] array, Func<T[]> getValues)
        {
            if (array.IsNullOrEmpty())
                return getValues();

            IEnumerable<T> enumerable = getValues();
            if (enumerable.IsNullOrEmpty())
                return array;

            return array.Union(enumerable).ToArray();
        }
    }
}