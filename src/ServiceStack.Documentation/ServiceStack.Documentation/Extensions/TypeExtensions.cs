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
        public static bool HasXmlSupport(this Type type)
            => type.HasSupport(Feature.Xml, RequestAttributes.Xml);

        public static bool HasJsvSupport(this Type type)
            => type.HasSupport(Feature.Jsv, RequestAttributes.Jsv);

        public static bool HasJsonSupport(this Type type)
            => type.HasSupport(Feature.Json, RequestAttributes.Json);

        public static bool HasSoap11Support(this Type type)
            => type.HasSupport(Feature.Soap, RequestAttributes.Soap11);

        public static bool HasSoap12Support(this Type type)
            => type.HasSupport(Feature.Soap, RequestAttributes.Soap12);

        public static bool HasMsgPackSupport(this Type type)
            => type.HasSupport(Feature.MsgPack, RequestAttributes.MsgPack);

        public static bool HasProtoBufSupport(this Type type)
            => type.HasSupport(Feature.ProtoBuf, RequestAttributes.ProtoBuf);
        
        public static bool HasSupport(this Type type, Feature feature, RequestAttributes request)
            => !type.AllAttributes<ExcludeAttribute>().Any(t => t.Feature.HasFlag(feature)) &&
               type.AllAttributes<RestrictAttribute>().All(t => t.HasAccessTo(request));
    }
}
