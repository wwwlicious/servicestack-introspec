// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Postman.DTO
{
    using DataAnnotations;
    using Documentation.DTO;
    using Models;

    [Route(Constants.PostmanSpecUri)]
    [Exclude(Feature.Metadata | Feature.ServiceDiscovery)]
    public class PostmanRequest : IReturn<PostmanSpecCollection>, IFilterableSpecRequest
    {
        public string[] DtoName { get; set; }
        public string[] Categories { get; set; }
        public string[] Tags { get; set; }
    }
}