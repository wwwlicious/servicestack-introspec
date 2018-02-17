// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Settings
{
    using System;
    using FluentAssertions;
    using IntroSpec.Settings;
    using Xunit;

    public class ApiSpecConfigTests
    {
        [Fact]
        public void Ctor_SetsDefaultContact()
            => new ApiSpecConfig().Contact.Should().NotBeNull();

        [Fact]
        public void WithBasicDetails_SetsDetails()
        {
            const string description = "Description of it";
            var uri = new Uri("http://acme.test");

            var spec = new ApiSpecConfig().WithLicenseUrl(uri).WithDescription(description);
            spec.Description.Should().Be(description);
            spec.LicenseUrl.Should().Be(uri);
        }

        [Fact]
        public void WithContactDetails_SetsContactDetails()
        {
            const string name = "Test Name";
            var uri = new Uri("http://acme.test");
            const string email = "test@example.com";

            var spec = new ApiSpecConfig().WithContactName(name).WithContactUrl(uri).WithContactEmail(email);

            var contact = spec.Contact;
            contact.Name.Should().Be(name);
            contact.Url.Should().Be(uri.OriginalString);
            contact.Email.Should().Be(email);
        }
    }
}
