// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    using System;

    /// <summary>
    /// Technical contact details for this api
    /// </summary>
    public class ApiContact
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public string Email { get; set; }
    }
}