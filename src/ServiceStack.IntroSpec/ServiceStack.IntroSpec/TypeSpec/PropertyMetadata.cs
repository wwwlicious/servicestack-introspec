// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.TypeSpec
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentValidation.Internal;
    using Models;

    /// <summary>
    /// Represents data about a property of a type exposed via API 
    /// </summary>
    public class PropertyMetadata : IPropertyMetadata
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsRequired { get; set; }
        public PropertyConstraint Constraint { get; set; }

        internal MemberInfo MemberInfo { get; private set; }

        public IPropertyMetadata With<TValue>(Expression<Func<IProperty, TValue>> prop, TValue value)
        {
            (prop.GetMember() as PropertyInfo)?.SetValue(this, value);
            return this;
        }

        public PropertyMetadata(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }

        public static PropertyMetadata Create<T, TProperty>(Expression<Func<T, TProperty>> expression)
            => new PropertyMetadata(expression.GetMember());
    }
}