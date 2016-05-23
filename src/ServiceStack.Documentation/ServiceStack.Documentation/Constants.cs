// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation
{
    internal static class Constants
    {
        internal const string SpecUri = "/spec";

        internal const string XmlEnricherKey = "xml";
        internal const string ReflectionEnricherKey = "reflection";
        internal const string ClassEnricherKey = "abstractclass";
        internal const string FallbackEnricherKey = "fallback";

        internal static class Features
        {
            internal const string ProtoBufFeature = "ProtoBufFormat";
            internal const string MsgPackFeature = "MsgPackFormat";
        }
    }
}