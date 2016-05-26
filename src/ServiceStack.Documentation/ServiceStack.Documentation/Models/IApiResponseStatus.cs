﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Models
{
    public interface IApiResponseStatus : IApiMetadata
    {
        string[] Verbs { get; set; }
        StatusCode[] StatusCodes { get; set; }
        string[] ContentTypes { get; set; }
        string RelativePath { get; set; }

        ApiResourceType ReturnType { get; set; } // Could this be IApiResourceType
    }
}