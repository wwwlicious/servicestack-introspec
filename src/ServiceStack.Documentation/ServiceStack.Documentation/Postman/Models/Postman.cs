// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Postman.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using DataAnnotations;

    // https://github.com/postmanlabs/schemas/blob/develop/json/collection/v2.0.0/index.json
    [Exclude(Feature.Soap | Feature.ServiceDiscovery)]
    public class Postman
    {
    }

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

    [DataContract]
    public class PostmanSpecRequest
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "headers")]
        public string Headers { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        // preRequestScript

        [DataMember(Name = "pathVariables")]
        public Dictionary<string, string> PathVariables { get; set; }

        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "data")]
        public List<PostmanSpecData> Data { get; set; }

        [DataMember(Name = "dataMode")]
        public string DataMode { get; set; } = "params"; // raw?

        [DataMember(Name = "version")]
        public int Version { get; set; } = 2;

        // tests
        // currentHelper

        [DataMember(Name = "time")]
        public long Time { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        // descriptionFormat

        [DataMember(Name = "collectionId")]
        public string CollectionId { get; set; }

        [DataMember(Name = "responses")]
        public string[] Responses { get; set; } = new string[0];

        [DataMember(Name = "folder")]
        public string FolderId { get; set; }

        // tests

        // rawModeData

        // helperAttributes
    }

    [DataContract]
    public class PostmanSpecData
    {
        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
        
        /* Parameter, BodyType(data?), PathVariable */
    }

    [DataContract]
    public class PostmanFolder
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "order")]
        public List<string> RequestIds { get; set; }
    }
}
