// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Settings
{
    using System;
    using Models;
    using TypeSpec;

    [Obsolete("Use public properties of ApiSpecFeature")]
    public class ApiSpecConfig : IFluentInterface
    {
        [Obsolete("Use Description property of ApiSpecFeature")]
        public string Description { get; set; }

        [Obsolete("Use ContactUrl property of ApiSpecFeature")]
        public Uri LicenseUrl { get; set; }

        private ApiContact contact;
        [Obsolete("Use ContactEmail/ContactName/ContactUrl properties of ApiSpecFeature")]
        public ApiContact Contact
        {
            get { return contact ?? (contact = new ApiContact()); }
            set { contact = value; }
        }

        [Obsolete("Use Description property of ApiSpecFeature")]
        public ApiSpecConfig WithDescription(string description)
        {
            Description = description;
            return this;
        }

        [Obsolete("Use LicenseUrl property of ApiSpecFeature")]
        public ApiSpecConfig WithLicenseUrl(Uri licenseUrl)
        {
            LicenseUrl = licenseUrl;
            return this;
        }

        [Obsolete("Use ContactEmail/ContactName/ContactUrl properties of ApiSpecFeature")]
        public ApiSpecConfig WithContact(ApiContact apiContact)
        {
            Contact = apiContact;
            return this;
        }

        [Obsolete("Use ContactName property of ApiSpecFeature")]
        public ApiSpecConfig WithContactName(string name)
        {
            Contact.Name = name;
            return this;
        }

        [Obsolete("Use ContactUrl property of ApiSpecFeature")]
        public ApiSpecConfig WithContactUrl(Uri url)
        {
            Contact.Url = url.OriginalString;
            return this;
        }

        [Obsolete("Use ContactEmail property of ApiSpecFeature")]
        public ApiSpecConfig WithContactEmail(string email)
        {
            Contact.Email = email;
            return this;
        }

        internal void PopulateProperties(IApiSpecSettings feature)
        {
            if (!string.IsNullOrWhiteSpace(Description)) feature.Description = Description;

            if (!string.IsNullOrWhiteSpace(contact?.Name)) feature.ContactName = contact.Name;

            if (!string.IsNullOrWhiteSpace(contact?.Email)) feature.ContactEmail = contact.Email;

            if (contact?.Url != null) feature.ContactUrl = new Uri(contact.Url);

            if (LicenseUrl != null) feature.LicenseUrl = LicenseUrl;
        }
    }
}