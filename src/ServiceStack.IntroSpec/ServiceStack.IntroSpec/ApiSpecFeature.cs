// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    using System;
    using Settings;

    [Obsolete("Use IntroSpecFeature rather than ApiSpecFeature. Maintained for backwards compat.")]
    public class ApiSpecFeature : IntroSpecFeature
    {
        [Obsolete("Use parameterless ctor and set public properties")]
        public ApiSpecFeature(ApiConfigDelegate config)
        {
            var cfg = config(new ApiSpecConfig());
            cfg.ThrowIfNull(nameof(cfg));

            cfg.PopulateProperties(this);
        }

        public ApiSpecFeature()
        {
            
        }
    }
}
