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
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> array)
        {
            return array == null || array.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> array)
        {
            return array == null || !array.Any();
        }

        /// <summary>
        /// Produces the set union of two sequences by using the default equality comparer.
        /// Verifies validity of arrays by checking for null before running union
        /// </summary>
        /// <typeparam name="T">Type of array</typeparam>
        /// <param name="array">Array whose elemets form first set for the union</param>
        /// <param name="getValues">Function to get array whose elements form second set for the union</param>
        /// <returns>Array that contains the distinct elements from both input arrays</returns>
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