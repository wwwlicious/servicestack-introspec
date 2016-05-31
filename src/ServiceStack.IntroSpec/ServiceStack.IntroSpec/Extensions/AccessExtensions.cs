// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Extensions
{
    using System;
    using System.Linq;
    using DataAnnotations;
    using Infrastructure;

    internal static class AccessExtensions
    {
        internal static bool CanAccess(this RestrictAttribute restrictAttribute, Result<RequestAttributes> request)
            => restrictAttribute == null || (request.IsSuccess && restrictAttribute.HasAccessTo(request.Value));

        internal static bool HasAccessToFeature(this Type requestType, Result<Feature> feature)
            => feature.IsSuccess &&
               !requestType.AllAttributes<ExcludeAttribute>().Any(t => t.Feature.HasFlag(feature.Value));
    }
}