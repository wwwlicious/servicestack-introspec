namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentValidation.Internal;

    public class PropertyMetadata : IPropertyMetadata
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsRequired { get; set; }

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