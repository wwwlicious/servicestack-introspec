// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    using System;

    public interface IApiSpecSettings
    {
        string ContactEmail { get; set; }
        string ContactName { get; set; }
        Uri ContactUrl { get; set; }
        string Description { get; set; }
        Uri LicenseUrl { get; set; }
    }
}