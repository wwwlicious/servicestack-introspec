namespace ServiceStack.Documentation.Settings
{
    using System;
    using Models;

    public class ApiSpecConfig
    {
        public string Description { get; set; }
        public Uri LicenseUrl { get; set; }

        public ApiContact Contact { get; set; }
    }
}