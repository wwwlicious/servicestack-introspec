// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.DTO
{
    public interface IFilterableSpecRequest
    {
        string[] DtoName { get; set; }
        string Category { get; set; }
        string[] Tags { get; set; }
    }
}