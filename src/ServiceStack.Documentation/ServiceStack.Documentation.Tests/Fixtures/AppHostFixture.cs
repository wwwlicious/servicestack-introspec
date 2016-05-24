// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Fixtures
{
    using System;
    using Metadata;
    using Testing;

    public class AppHostFixture : IDisposable
    {
        public BasicAppHost AppHost { get; set; }

        public const string WebHostUrl = "http://127.0.0.1:8090";
        public const string ServiceName = "testService";

        public AppHostFixture()
        {
            var hostConfig = new HostConfig
            {
                WebHostUrl = WebHostUrl,
                DebugMode = true
            };

            AppHost = new BasicAppHost
            {
                TestMode = true,
                Config = hostConfig,
                ServiceName = ServiceName
            };

            AppHost.Init();
            AppHost.Config.WebHostUrl = WebHostUrl;
            var hal = "hal+json";

            HostContext.MetadataPagesConfig.AvailableFormatConfigs.Add(new MetadataConfig(hal, hal,
                hal, hal, hal));
        }

        public void Dispose() => AppHost?.Dispose();
    }
}