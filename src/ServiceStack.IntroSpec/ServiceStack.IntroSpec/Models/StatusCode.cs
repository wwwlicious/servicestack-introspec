// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    using System;
    using System.Net;
    using Extensions;

    public class StatusCode : IEquatable<StatusCode>
    {
        public int Code { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public StatusCode WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public StatusCode WithCode(int code)
        {
            Code = code;
            return this;
        }

        public StatusCode WithName(string name)
        {
            Name = name;
            return this;
        }

        public static explicit operator StatusCode(int statusCode)
        {
            var httpStatusCode = (HttpStatusCode) statusCode;
            return (StatusCode) httpStatusCode;
        }

        public static explicit operator StatusCode(HttpStatusCode httpStatusCode)
        {
            return new StatusCode { Code = (int) httpStatusCode, Name = httpStatusCode.ToString().ToSpaced() };
        }

        public bool Equals(StatusCode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StatusCode) obj);
        }

        public override int GetHashCode()
        {
            return Code;
        }
    }
}