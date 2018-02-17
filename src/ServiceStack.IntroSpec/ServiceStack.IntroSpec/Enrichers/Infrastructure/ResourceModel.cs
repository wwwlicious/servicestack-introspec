// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Infrastructure
{
    using System;
    using ServiceStack.IntroSpec.Extensions;
    using ServiceStack.IntroSpec.Models;

    /// <summary>
    /// Represents details of resource being processed
    /// </summary>
    public class ResourceModel
    {
        /// <summary>
        /// Whether is request or response resource
        /// </summary>
        public bool IsRequest { get; }

        /// <summary>
        /// ClrType of current resource
        /// </summary>
        public ApiClrType ResourceType { get; }

        public ResourceModel(Type resourceType, bool isRequest)
        {
            IsRequest = isRequest;
            ResourceType = resourceType?.ToApiClrType();
        }
    }
}