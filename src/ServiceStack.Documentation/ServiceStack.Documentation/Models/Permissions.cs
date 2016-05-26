// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Models
{
    using System.Collections.Generic;
    using Extensions;

    public class Permissions
    {
        public IList<string> AnyOf { get; set; }
        public IList<string> AllOf { get; set; }

        public static Permissions Create(IList<string> anyOf, IList<string> allOf)
        {
            var any = anyOf.IsNullOrEmpty() ? null : anyOf;
            var all = allOf.IsNullOrEmpty() ? null : allOf;

            if (any == null && all == null)
                return null;

            return new Permissions { AllOf = all, AnyOf = any };
        }
    }
}