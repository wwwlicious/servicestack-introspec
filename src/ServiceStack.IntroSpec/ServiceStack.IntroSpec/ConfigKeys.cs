// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    public static class ConfigKeys
    {
        private const string KeyPrefix = "servicestack.plugins.introspec.";
        public const string ContactName = KeyPrefix + "contact.name";
        public const string ContactEmail = KeyPrefix + "contact.email";
        public const string ContactUrl = KeyPrefix + "contact.url";
        public const string Description = KeyPrefix + "description";
        public const string LicenseUrl = KeyPrefix + "licenseurl";
    }
}