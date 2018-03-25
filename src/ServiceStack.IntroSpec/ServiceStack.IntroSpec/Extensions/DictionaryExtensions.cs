// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DictionaryExtensions
    {
        /// <summary>
        /// Filter dictionary using provided delegate.
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <param name="dictionary">Dictionary to filter</param>
        /// <param name="filter">Predicate to use to filter dictionary</param>
        /// <returns>Filtered values, or empty list of dictionary null</returns>
        public static IEnumerable<TValue> FilterValues<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            Func<KeyValuePair<TKey, TValue>, bool> filter)
        {
            return dictionary?.Where(filter).Select(o => o.Value) ?? Enumerable.Empty<TValue>();
        }

        /// <summary>
        /// Attempt to get provided key from dictionary. Returns default for type if not found
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <param name="dictionary">Dictionary to get value from</param>
        /// <param name="key">Key of value to find</param>
        /// <returns>Value at key from dictionary, or default for type if not found</returns>
        public static TValue SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
                return default(TValue);

            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }

        /// <summary>
        /// Attempt to get provided key from dictionary. Returns fallback if not found
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <param name="dictionary">Dictionary to get value from</param>
        /// <param name="key">Key of value to find</param>
        /// <param name="fallback">Fallback value to return if key not present</param>
        /// <returns>Value at key from dictionary, or fallback if not found</returns>
        public static TValue SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue fallback)
        {
            if (dictionary == null)
                return fallback;

            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : fallback;
        }

        /// <summary>
        /// Attempt to get provided key from dictionary. Returns item from fallback delegate if not found
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <param name="dictionary">Dictionary to get value from</param>
        /// <param name="key">Key of value to find</param>
        /// <param name="fallbackProvider">Delegate used to create fallback value if key not present</param>
        /// <returns>Value at key from dictionary, or item provided by fallback delegate if not found</returns>
        public static TValue SafeGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> fallbackProvider)
        {
            if (dictionary == null)
                return fallbackProvider();

            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : fallbackProvider();
        }

        /// <summary>
        /// Attempt to get provided key from dictionary. If found call Func to get result from value. Returns fallback if key not found
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="dictionary">Dictionary to get value from</param>
        /// <param name="key">Key of value to find</param>
        /// <param name="getResult">Delegate used to create get result from value, if key found</param>
        /// <param name="fallback">Fallback value to return if key not present</param>
        /// <returns>Property of value at key from dictionary, or fallback if not found</returns>
        public static TResult SafeGetFromValue<TKey, TValue, TResult>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue, TResult> getResult, TResult fallback)
        {
            if (dictionary == null)
                return fallback;

            TValue value;
            return dictionary.TryGetValue(key, out value) ? getResult(value) : fallback;
        }

        /// <summary>
        /// Attempt to get provided key from dictionary. Returns item from dictionary if found. If not found calls delegate to get value, adds to dictionary and returns.
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <param name="dictionary">Dictionary to get value from</param>
        /// <param name="key">Key of value to find</param>
        /// <param name="fallbackProvider">Delegate used to create fallback value if key not present</param>
        /// <returns>Value at key from dictionary, or item provided by fallback delegate if not found</returns>
        public static TValue SafeGetOrInsert<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> fallbackProvider)
        {
            if (dictionary == null)
                return fallbackProvider();

            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;

            value = fallbackProvider();
            dictionary.Add(key, value);
            return value;
        }

        /// <summary>
        /// Adds specified values to list in dictionary at provided Key.
        /// </summary>
        /// <typeparam name="TKey">Type of dictionary key</typeparam>
        /// <typeparam name="TValue">Type of dictionary value</typeparam>
        /// <param name="dictionary">Dictionary to add values to</param>
        /// <param name="key">Key of value to set</param>
        /// <param name="values">List of values to add to dictionary at key</param>
        public static void UpdateList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dictionary, TKey key,
            params TValue[] values)
        {
            if (dictionary == null) return;

            List<TValue> valuesList;
            if (!dictionary.TryGetValue(key, out valuesList))
                valuesList = new List<TValue>();

            valuesList.AddRange(values);
            dictionary[key] = valuesList;
        }
    }
}