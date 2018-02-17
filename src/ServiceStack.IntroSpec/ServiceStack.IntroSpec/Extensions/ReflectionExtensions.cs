// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using ServiceStack.IntroSpec.Models;

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

        public static bool IsCollection(this Type type) => (type != typeof(string)) && type.GetInterface("IEnumerable") != null;

        /// <summary>
        /// Shamelessly 'borrowed' from this excellent answer ... and tweaked a bit
        /// http://stackoverflow.com/a/7072121/191877
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <remarks>If there are more than one <see cref="IEnumerable"/> interface, will only return the first one</remarks>
        /// <returns>The type of the first <see cref="IEnumerable"/> interface found</returns>
        public static Type GetEnumerableType(this Type type)
        {
            while (true)
            {
                type.ThrowIfNull(nameof(type));

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return type.GetGenericArguments()[0];

                var iface = (from i in type.GetInterfaces() where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>) select i).FirstOrDefault();

                if (iface == null)
                    throw new ArgumentException("Does not represent an enumerable type.", nameof(type));

                type = iface;
            }
        }

        /// <summary>
        /// Shamelessly 'borrowed' from this excellent answer
        /// http://stackoverflow.com/a/17481450/191877
        /// </summary>
        /// <param name="t">the type to output as string</param>
        /// <remarks>Most useful for outputting generics nicely</remarks>
        /// <returns>a nice representation of a type name</returns>
        public static string GetDocumentationTypeName(this Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            var sb = new StringBuilder();
            sb.Append(t.Name.Substring(0, t.Name.IndexOf('`')));
            sb.Append('<');
            var appendComma = false;
            foreach (var arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(GetDocumentationTypeName(arg));
                appendComma = true;
            }
            sb.Append('>');
            return sb.ToString();
        }

        /// <summary>
        /// Converts a CLR Native System.Type to an ApiClrType to work with other languages (typescript etc)
        /// </summary>
        /// <param name="type">The CLR Type</param>
        /// <returns>A x-plat type poco</returns>
        public static ApiClrType ToApiClrType(this Type type)
        {
            return new ApiClrType
            {
                AssemblyName = type.AssemblyQualifiedName,
                IsPrimitive = type.IsPrimitive,
                Name = type.Name,
                OriginalType = type
            };
        }
    }
}