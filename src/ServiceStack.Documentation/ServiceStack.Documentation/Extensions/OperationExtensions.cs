// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Host;
    using Logging;
    using Models;
    using Utilities;
    using Web;

    public static class OperationExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(OperationExtensions));

        public static bool IsForVerb(this RouteAttribute routeAttribute, string verb)
        {
            if (routeAttribute == null)
                return false;

            if (string.IsNullOrEmpty(routeAttribute.Verbs))
                return true;

            return routeAttribute.Verbs.IndexOf(verb, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsForVerb(this AuthenticateAttribute authAttribute, string verb)
        {
            if (authAttribute == null)
                return false;

            if (authAttribute.ApplyTo == ApplyTo.All)
                return true;

            var applyTo = EnumUtilities.SafeParse<ApplyTo>(verb);

            if (!applyTo.IsSuccess)
                log.Info($"Unable to parse verb {verb} to ApplyTo enum");

            return applyTo.IsSuccess && authAttribute.ApplyTo.HasFlag(applyTo.Value);
        }

        public static bool AuthenticationAppliesForVerb(this Operation operation, string verb)
        {
            var authenticateAttr = operation.GetRequestAttr<AuthenticateAttribute>();

            // NOTE AuthenticateAttribute is AllowMultiple = false so be 0:1
            return authenticateAttr.IsNullOrEmpty() || authenticateAttr.FirstOrDefault().IsForVerb(verb);
        }

        public static Permissions GetRoles(this Operation operation, string verb)
        {
            var any = operation.GetRoles<RequiresAnyRoleAttribute>(verb, attribute => attribute.RequiredRoles);
            var all = operation.GetRoles<RequiredRoleAttribute>(verb, attribute => attribute.RequiredRoles);

            return Permissions.Create(any, all);
        }

        public static Permissions GetPermissions(this Operation operation, string verb)
        {
            var any = operation.GetRoles<RequiresAnyPermissionAttribute>(verb, attribute => attribute.RequiredPermissions);
            var all = operation.GetRoles<RequiredPermissionAttribute>(verb, attribute => attribute.RequiredPermissions);

            return Permissions.Create(any, all);
        }

        private static IEnumerable<T> GetRequestAttr<T>(this Operation operation) where T : IHasRequestFilter
            => operation.RequestFilterAttributes?.OfType<T>();

        private static List<string> GetRoles<T>(this Operation operation, string verb,
            Func<T, IEnumerable<string>> func) where T : AuthenticateAttribute
            => operation.GetRequestAttr<T>()?
                        .Where(a => a.IsForVerb(verb))
                        .SelectMany(func)
                        .ToList();
    }
}
