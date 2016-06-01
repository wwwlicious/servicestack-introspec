// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Extensions
{
    using System;

    /// <summary>
    /// Collection of extension methods used for calling Func to get value if value is empty/default
    /// </summary>
    public static class GetValueExtensions
    {
        public static string GetIfNullOrEmpty(this string value, Func<string> getValue)
            => string.IsNullOrEmpty(value) ? getValue() : value;

        public static T GetIfNull<T>(this T value, Func<T> getValue) 
            where T : class 
            => value ?? getValue();

        public static T[] GetIfNullOrEmpty<T>(this T[] array, Func<T[]> getValue)
            => array.IsNullOrEmpty() ? getValue() : array;

        public static T? GetIfNoValue<T>(this T? value, Func<T?> getValue) 
            where T : struct
            => !value.HasValue ? getValue() : value;
    }
}