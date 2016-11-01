// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Host;
    using Interfaces;
    using Logging;
    using Models;
    using Settings;

    public class ActionEnricherManager : IActionEnricherManager
    {
        private readonly IActionEnricher actionEnricher;
        private readonly ISecurityEnricher securityEnricher;

        private static readonly ILog log = LogManager.GetLogger(typeof(ActionEnricherManager));

        public ActionEnricherManager(IActionEnricher actionEnricher, ISecurityEnricher securityEnricher)
        {
            this.actionEnricher = actionEnricher;
            this.securityEnricher = securityEnricher;
        }

        public ApiAction[] EnrichActions(ApiAction[] actions, Operation operation)
        {
            // Build a list of all possible Verbs
            var verbs = GetVerbs(operation);

            // For each verb create an ApiAction object and populate it's properties.
            Dictionary<string, ApiAction> actionLookup;
            List<ApiAction> actionWorkingSet = null;
            bool newList = false;

            if (actions.IsNullOrEmpty())
            {
                actionLookup = new Dictionary<string, ApiAction>();
                newList = true;
                actionWorkingSet = new List<ApiAction>(verbs.Length);
            }
            else
                actionLookup = actions.ToDictionary(k => k.Verb, v => v);

            // Iterate through each verb and update the appropriate action object
            foreach (var verb in verbs)
            {
                var action = actionLookup.SafeGet(verb, new ApiAction { Verb = verb });

                EnrichAction(action, operation);

                if (newList)
                    actionWorkingSet.Add(action);
            }

            return newList ? actionWorkingSet.ToArray() : actions;
        }

        private void EnrichAction(IApiAction action, Operation operation)
        {
            var verb = action.Verb;

            if (actionEnricher != null)
            {
                action.StatusCodes =
                    action.StatusCodes.GetBasedOnStrategy(() => actionEnricher.GetStatusCodes(operation, verb));

                action.ContentTypes =
                    action.ContentTypes.GetBasedOnStrategy(() => actionEnricher.GetContentTypes(operation, verb));

                action.RelativePaths =
                    action.RelativePaths.GetBasedOnStrategy(() => actionEnricher.GetRelativePaths(operation, verb));

                action.Notes = action.Notes.GetIfNullOrEmpty(() => actionEnricher.GetNotes(operation, verb));
            }

            if (securityEnricher != null)
                action.Security = securityEnricher.GetSecurity(operation, verb);
        }

        private string[] GetVerbs(Operation operation)
        {
            // TODO - Look at [Restrict] [Authenticate] to determine if some should be ignored
            const string anyVerb = "ANY";

            if (!operation.Actions.Contains(anyVerb)) return operation.Actions.ToArray();

            var routeVerbs = GetRouteVerbs(operation);
            log.Debug($"Got route attribute verbs {routeVerbs.Join(",")} for type {operation.RequestType}");

            var replacementVerbs = DocumenterSettings.ReplacementVerbs as string[] ??
                                   DocumenterSettings.ReplacementVerbs.ToArray();

            // Available verbs are: ReplacementVerbs + RouteAttribute verbs + Action (from svc methods)
            string[] verbs = routeVerbs
                .Union(replacementVerbs)
                .Union(operation.Actions.Where(s => s != anyVerb))
                .Distinct()
                .ToArray();

            log.Debug($"Got verbs {routeVerbs.Join(",")} for type {operation.RequestType}");
            return verbs;
        }

        private static List<string> GetRouteVerbs(Operation operation)
        {
            // For the request type, get a list of all route attributes (with verb set to discount Fallback route)
            var routeAttributes =
                operation.RequestType.GetCustomAttributes<RouteAttribute>().Where(r => !string.IsNullOrEmpty(r.Verbs));

            var routeVerbs = routeAttributes.Select(r => r.Verbs).ToList();
            return routeVerbs;
        }
    }
}
