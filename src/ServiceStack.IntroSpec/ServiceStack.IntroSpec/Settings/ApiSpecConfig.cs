// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Settings
{
    using System;
    using Models;
    using TypeSpec;

    public class ApiSpecConfig : IFluentInterface
    {
        public string Description { get; set; }
        public Uri LicenseUrl { get; set; }

        private ApiContact contact;
        public ApiContact Contact
        {
            get { return contact ?? (contact = new ApiContact()); }
            set { contact = value; }
        }

        public ApiSpecConfig WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public ApiSpecConfig WithLicenseUrl(Uri licenseUrl)
        {
            LicenseUrl = licenseUrl;
            return this;
        }

        public ApiSpecConfig WithContact(ApiContact apiContact)
        {
            Contact = apiContact;
            return this;
        }

        public ApiSpecConfig WithContactName(string name)
        {
            Contact.Name = name;
            return this;
        }

        public ApiSpecConfig WithContactUrl(Uri url)
        {
            Contact.Url = url;
            return this;
        }

        public ApiSpecConfig WithContactEmail(string email)
        {
            Contact.Email = email;
            return this;
        }
    }
}