// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.DTO
{
    using System.Runtime.Serialization;
    using DataAnnotations;

    [Route(Constants.SpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    [DataContract]
    public class SpecRequest : IReturn<SpecResponse>, IFilterableSpecRequest
    {
        [DataMember(Name = "DtoName")]
        public string[] DtoNames { get; set; }

        [DataMember(Name = "Category")]
        public string[] Categories { get; set; }

        [DataMember(Name = "Tag")]
        public string[] Tags { get; set; }
    }
}