// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Settings
{
    using System;
    using Models;

    public class ApiSpecConfig
    {
        public string Description { get; set; }
        public Uri LicenseUrl { get; set; }

        public ApiContact Contact { get; set; }
    }
}