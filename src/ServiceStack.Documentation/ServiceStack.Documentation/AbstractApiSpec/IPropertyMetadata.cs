namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System;
    using System.Linq.Expressions;

    // Is there any value in this interface? Should I just return the concrete implementation?
    public interface IPropertyMetadata : IProperty, IFluentInterface
    {
        IPropertyMetadata With<TValue>(Expression<Func<IProperty, TValue>> prop, TValue value);
    }
}