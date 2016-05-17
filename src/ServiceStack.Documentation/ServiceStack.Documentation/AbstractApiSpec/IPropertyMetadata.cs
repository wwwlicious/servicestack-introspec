// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System;
    using System.Linq.Expressions;

    // NOTE Is there any value in this interface? Should I just return the concrete implementation?
    public interface IPropertyMetadata : IProperty, IFluentInterface
    {
        IPropertyMetadata With<TValue>(Expression<Func<IProperty, TValue>> prop, TValue value);
    }
}