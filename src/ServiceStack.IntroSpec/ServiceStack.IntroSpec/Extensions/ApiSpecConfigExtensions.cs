// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace ServiceStack.IntroSpec.Extensions
{
    using System;
    using ServiceStack.Configuration;
    using ServiceStack.IntroSpec.Settings;

    public static class ApiSpecConfigExtensions
    {
        private const string Config_Key_Prefix = "ServiceStack.Plugins.IntroSpec.";
        public const string Config_Key_ContactName = Config_Key_Prefix + "Contact.Name";
        public const string Config_Key_ContactEmail = Config_Key_Prefix + "Contact.Email";
        public const string Config_Key_ContactUrl = Config_Key_Prefix + "Contact.Url";
        public const string Config_Key_Description = Config_Key_Prefix + "Description";
        public const string Config_Key_LicenseUrl = Config_Key_Prefix + "LicenseUrl";

        /// <summary>
        /// Loads the plugin configuration from appsettings
        /// </summary>
        /// <param name="config">the config</param>
        /// <param name="appSettings">optionally pass in an appsettings provider</param>
        public static ApiSpecConfig FromAppSettings(this ApiSpecConfig config, IAppSettings appSettings = null)
        {
            var settings = appSettings ?? new AppSettings();

            config.Contact.Name = settings.GetIfExists<string>(Config_Key_ContactName);
            config.Contact.Email = settings.GetIfExists<string>(Config_Key_ContactEmail);
            config.Contact.Url = settings.GetIfExists<Uri>(Config_Key_ContactUrl);
            config.Description = settings.GetIfExists<string>(Config_Key_Description);
            config.LicenseUrl = settings.GetIfExists<Uri>(Config_Key_LicenseUrl);

            return config;
        }

        private static T GetIfExists<T>(this IAppSettings appSettings, string key)
        {
            return appSettings.Exists(key) ? appSettings.Get<T>(key) : default(T);
        }
    }
}