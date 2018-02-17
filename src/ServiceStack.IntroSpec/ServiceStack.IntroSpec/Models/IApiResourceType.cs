// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    public interface IApiResourceType : IApiSpec
    {
        string TypeName { get; set; }
        ApiPropertyDocumentation[] Properties { get; set; }
        bool? AllowMultiple { get; set; }
        bool? IsCollection { get; set; }
    }
}