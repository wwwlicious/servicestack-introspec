// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

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

        /// <summary>
        /// Gets the FieldInfo.FieldType or PropertyInfo.PropertyType from this MemberInfo object.
        /// </summary>
        /// <param name="memberInfo">MemberInfo object to get type for</param>
        /// <returns>Type of underlying Property/Field</returns>
        /// <remarks>This only works for MemberInfo/FieldInfo</remarks>
        public static Type GetFieldPropertyType(this MemberInfo memberInfo)
        {
            if (!(memberInfo is PropertyInfo) && !(memberInfo is FieldInfo))
                throw new ArgumentException("Method only supports PropertyInfo or FieldInfo", nameof(memberInfo));

            return (memberInfo as PropertyInfo)?.PropertyType ?? (memberInfo as FieldInfo).FieldType;
        }
            
    }
}