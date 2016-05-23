// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Models
{
    using System;

    // TODO Rename this api spec?
    /// <summary>
    /// General top level model with API (Service) wide vars
    /// </summary>
    public class ApiDocumentation
    {
        public string Title { get; set; }
        public string ApiVersion { get; set; }
        public string ApiBaseUrl { get; set; }

        public string Description { get; set; }
        /*public string Category { get; set; }
        public string[] Tags { get; set; }*/
        public string TermsOfService { get; set; }
        public string Licence { get; set; }
        public string LicenceUrl { get; set; }
        public ApiContact Contact { get; set; }

        public ApiResourceDocumentation[] Resources { get; set; }

        // Security
    }

    /// <summary>
    /// Contact details for this api
    /// </summary>
    public class ApiContact
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// A single API resource (DTO)
    /// </summary>
    //[DebuggerDisplay("{Title}")]
    public class ApiResourceDocumentation : IApiResourceType, IApiResponseStatus
    {
        // From IApiResourceType
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public ApiPropertyDocumention[] Properties { get; set; }

        // From IApiResponseStatus
        public string[] Verbs { get; set; }
        public StatusCode[] StatusCodes { get; set; }
        public string RelativePath { get; set; } // Depends on [Route]. .ToRelativeUri(). Can only do PreDefinedRoutes if that feature is in
        public ApiResourceType ReturnType { get; set; } // ReturnType w/params

        // From ICategorised
        public string Category { get; set; }
        public string[] Tags { get; set; }

        // IDictionary<string, string> of extra 'stuff' that doesn't fit in any specific place
    }

    public class ApiResourceType : IApiResourceType
    {
        // These will need key'd by something (Type?) for finding + enriching
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public ApiPropertyDocumention[] Properties { get; set; }
    }

    /// <summary>
    /// Each request property within a DTO
    /// </summary>
    public class ApiPropertyDocumention : IApiSpec
    {
        // Set when instantiated
        public string Id { get; set; }
        public Type ClrType { get; set; }

        // From IApiSpec
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
    
        public string ParamType { get; set; } // used to denote if the param is request querystring or body restricted
        public bool? IsRequired { get; set; }
        public string Contraints { get; set; } // used to specify accepted values (min/max) or iterate enum types. Could have separate for enum etc
        public string[] ExternalLinks { get; set; }
        public bool? AllowMultiple { get; set; }

        public IApiResourceType EmbeddedResource { get; set; }

        // IDictionary<string,string> for extra stuff?
        // ExcludeInSchema??
    }

    // Should this be split further down? IHasTitle, IHasDescription etc?
    public interface IApiResourceType : IApiSpec
    {
        ApiPropertyDocumention[] Properties { get; set; }
    }

    // NOTE Better name required
    public interface IApiResponseStatus : IApiMetadata
    {
        string[] Verbs { get; set; }
        StatusCode[] StatusCodes { get; set; }
        string RelativePath { get; set; }

        ApiResourceType ReturnType { get; set; } // Could this be IApiResourceType
    }

    public interface IApiSpec
    {
        string Title { get; set; }
        string Description { get; set; }
        string Notes { get; set; }
    }

    public interface IApiMetadata
    {
        string Category { get; set; }
        string[] Tags { get; set; }
    }
}
