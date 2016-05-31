// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Enrichers.Interfaces
{
    using System.Reflection;
    using Models;

    /// <summary>
    /// Methods for populating a resource property
    /// </summary>
    public interface IPropertyEnricher : IEnrich
    {
        string GetTitle(MemberInfo mi);
        string GetDescription(MemberInfo mi);
        string GetNotes(MemberInfo mi);
        bool? GetAllowMultiple(MemberInfo mi);
        string[] GetExternalLinks(MemberInfo mi);
        PropertyConstraint GetConstraints(MemberInfo mi);
        bool? GetIsRequired(MemberInfo mi);
        string GetParamType(MemberInfo mi);
    }
}