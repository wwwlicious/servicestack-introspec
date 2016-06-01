// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.DTO
{
    using DataAnnotations;

    [Route(Constants.SpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class SpecRequest : IReturn<SpecResponse>, IFilterableSpecRequest
    {
        public string[] DtoName { get; set; }
        public string[] Category { get; set; }
        public string[] Tag { get; set; }
    }
}