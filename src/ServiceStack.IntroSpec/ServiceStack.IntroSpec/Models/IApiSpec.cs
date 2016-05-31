// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    public interface IApiSpec
    {
        string Title { get; set; }
        string Description { get; set; }
        string Notes { get; set; }
    }
}