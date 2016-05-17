// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get an ienumerable representing full inheritance hierarchy for Type
        /// </summary>
        /// <param name="type">Type to get inheritance hierarchy for</param>
        /// <returns>IEnumerable of inherited types</returns>
        public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }
    }
}