// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.AbstractApiSpec
{
    using System;
    using System.Collections.Generic;
    using Models;

    public static class DefaultDocumentationSettings
    {
        public static string Notes { get; }
        public static List<string> Verbs { get; }
        public static List<StatusCode> StatusCodes { get; }
        public static string Category { get; }
        public static List<string> Tags { get; }

        public static Func<Type, string> GetTitle;
        public static Func<Type, string> GetDescription;
        public static Func<Type, string> GetNotes;
    }
}