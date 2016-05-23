// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System;
    using System.Linq;
    using DataAnnotations;

    public static class TypeExtensions
    {
        public static bool HasXmlSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Xml, RequestAttributes.Xml, restrictedTo);

        public static bool HasJsvSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Jsv, RequestAttributes.Jsv, restrictedTo);

        public static bool HasJsonSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Json, RequestAttributes.Json, restrictedTo);

        public static bool HasSoap11Support(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Soap, RequestAttributes.Soap11, restrictedTo);

        public static bool HasSoap12Support(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Soap, RequestAttributes.Soap12, restrictedTo);

        public static bool HasCsvSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Csv, RequestAttributes.Csv, restrictedTo);

        public static bool HasHtmlSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupport(Feature.Html, RequestAttributes.Html, restrictedTo);

        // TODO Is there a nicer way to handle this without taking dependency on MsgPack nuget package?
        public static bool HasMsgPackSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupportExternal(Feature.MsgPack, RequestAttributes.MsgPack, Constants.Features.MsgPackFeature, restrictedTo);

        public static bool HasProtoBufSupport(this Type type, RestrictAttribute restrictedTo = null)
            => type.HasSupportExternal(Feature.ProtoBuf, RequestAttributes.ProtoBuf, Constants.Features.ProtoBufFeature, restrictedTo);

        private static bool HasSupportExternal(this Type type, Feature feature, RequestAttributes request, string featureName, RestrictAttribute restrictedTo)
        {
            var appHost = HostContext.AppHost;
            if (appHost == null) return false;

            return appHost.Plugins.Any(p => p.GetType().Name == featureName) && type.HasSupport(feature, request, restrictedTo);
        }

        private static bool HasSupport(this Type type, Feature feature, RequestAttributes request,
            RestrictAttribute restrictedTo)
        {
            var hasExcludeFlag = type.AllAttributes<ExcludeAttribute>().Any(t => t.Feature.HasFlag(feature));
            var hasAccessTo = type.HasAccessTo(request, restrictedTo);

            return !hasExcludeFlag && hasAccessTo;
        }

        private static bool HasAccessTo(this Type type, RequestAttributes request, RestrictAttribute restrictedTo)
        {
            var restrictAttribute = restrictedTo ?? type.FirstAttribute<RestrictAttribute>();

            bool? hasAccessTo = restrictAttribute?.HasAccessTo(request);
            return hasAccessTo ?? true;
        }
    }
}
