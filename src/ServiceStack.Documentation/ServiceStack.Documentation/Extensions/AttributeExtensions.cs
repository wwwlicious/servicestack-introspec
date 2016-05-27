// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System;

    public static class AttributeExtensions
    {
        public static bool IsForVerb(this RouteAttribute routeAttribute, string verb)
        {
            if (routeAttribute == null)
                return false;

            if (string.IsNullOrEmpty(routeAttribute.Verbs))
                return true;

            return routeAttribute.Verbs.IndexOf(verb, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}