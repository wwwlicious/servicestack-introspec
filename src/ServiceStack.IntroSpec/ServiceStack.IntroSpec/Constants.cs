// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec
{
    public static class Constants
    {
        public const string SpecUri = "/spec";
        public const string SpecSummaryUri = "/spec/summary";
        public const string PostmanSpecUri = "/spec/postman";

        public const string XmlEnricherKey = "xml";
        public const string ReflectionEnricherKey = "reflection";
        public const string ClassEnricherKey = "abstractclass";
        public const string FallbackEnricherKey = "fallback";

        public const string GlobalSettingsKey = "_all";

        public static class RouteSources
        {
            public const string Attribute = "Attribute";
            public const string AutoRoute = "AutoRoute";
        }
    }
}