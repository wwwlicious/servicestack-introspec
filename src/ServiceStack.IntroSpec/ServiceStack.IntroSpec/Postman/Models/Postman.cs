// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Postman.Models
{
    using DataAnnotations;

    // https://github.com/postmanlabs/schemas/blob/develop/json/collection/v2.0.0/index.json
    [Exclude(Feature.Soap | Feature.ServiceDiscovery)]
    public class Postman
    {
    }
}
