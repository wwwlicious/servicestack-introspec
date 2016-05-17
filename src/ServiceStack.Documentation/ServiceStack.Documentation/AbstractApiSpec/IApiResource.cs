// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    public interface IApiResource
    {
        string Title { get; }
        string Description { get; }
        string Notes { get; }

        IProperty GetPropertySpec(PropertyInfo pi);
    }

    public interface IApiRequest : IApiResource
    {
        List<string> Verbs { get; }
        List<StatusCode> StatusCodes { get; }

        string Category { get; }
        List<string> Tags { get; }
    }
}
