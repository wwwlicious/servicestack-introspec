// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Enrichers.Interfaces
{
    using System.Reflection;
    using Models;

    /// <summary>
    /// Methods for populating a documentatino resource parameter
    /// </summary>
    public interface IPropertyEnricher : IEnrich
    {
        string GetTitle(PropertyInfo pi);
        string GetDescription(PropertyInfo pi);
        string GetNotes(PropertyInfo pi);
        bool? GetAllowMultiple(PropertyInfo pi);
        string[] GetExternalLinks(PropertyInfo pi);
        PropertyConstraint GetConstraints(PropertyInfo pi);
        bool? GetIsRequired(PropertyInfo pi);
        string GetParamType(PropertyInfo pi);
    }
}