// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// Documentation class for default object exposed in API (nested type, return object etc)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypeSpec<T> : IApiResource, IApiPropertyResource
        where T : class, new()
    {
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public string Notes { get; protected set; }

        private readonly Dictionary<MemberInfo, IProperty> parameterLookup;

        protected TypeSpec()
        {
            parameterLookup = new Dictionary<MemberInfo, IProperty>();
        }

        public IProperty GetPropertySpec(MemberInfo pi) => parameterLookup.SafeGet(pi, (IProperty)null);

        protected IPropertyMetadata For<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var parameter = PropertyMetadata.Create(expression);
            var memberInfo = parameter.MemberInfo;

            if (!(memberInfo is PropertyInfo) && !(memberInfo is FieldInfo))
                throw new ArgumentException("For() only supports PropertyInfo or FieldInfo", nameof(expression));

            parameterLookup.Add(memberInfo, parameter);
            return parameter;
        }
    }
}