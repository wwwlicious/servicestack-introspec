// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Postman.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PostmanSpecCollection
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        // NOTE If order not specified in collection folders are ignored
        [DataMember(Name = "order")]
        public string[] Order { get; set; } = new string[0];

        [DataMember(Name = "folders")]
        public List<PostmanFolder> Folders { get; set; } = new List<PostmanFolder>();

        [DataMember(Name = "timestamp")]
        public long Timestamp { get; set; }

        // owner
        // remoteLink

        [DataMember(Name = "public")]
        public bool Public { get; set; }

        [DataMember(Name = "requests")]
        public PostmanSpecRequest[] Requests { get; set; }
    }
}