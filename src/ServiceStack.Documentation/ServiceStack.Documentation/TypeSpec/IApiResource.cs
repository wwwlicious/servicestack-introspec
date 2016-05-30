// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.TypeSpec
{
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    public interface IApiResource
    {
        string Title { get; }
        string Description { get; }
        string Notes { get; }
    }

    public interface IApiRequest : IApiResource
    {
        Dictionary<string, List<string>> ContentTypes { get; }
        Dictionary<string, List<StatusCode>> StatusCodes { get; }
        List<string> Tags { get; }
        string Category { get; }
    }

    public interface IApiPropertyResource
    {
        IProperty GetPropertySpec(MemberInfo pi);
    }
}
