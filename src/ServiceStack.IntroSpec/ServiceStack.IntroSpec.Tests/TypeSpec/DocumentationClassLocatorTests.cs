// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.TypeSpec
{
    using FluentAssertions;
    using IntroSpec.Settings;
    using IntroSpec.TypeSpec;
    using Xunit;

    public class DocumentationClassLocatorTests
    {
        [Fact]
        public void GetLookup_FindsDirectDescendant()
        {
            using (DocumenterSettings.With(assemblies: new[] { typeof (ClassToDocument).Assembly }))
            {
                var lookup = DocumentationClassLocator.GetLookup();
                var value = lookup[typeof (ClassToDocument)];
                value.Should().BeOfType<DirectDescendant>();
            }
        }

        [Fact]
        public void GetLookup_FindsDeeperDescendant()
        {
            using (DocumenterSettings.With(assemblies: new[] { typeof(ClassToDocument).Assembly }))
            {
                var lookup = DocumentationClassLocator.GetLookup();
                var value = lookup[typeof(AnotherClassToDocument)];
                value.Should().BeOfType<DeeperDescendant>();
            }
        }

        [Fact]
        public void GetLookup_DoesNotReturn_NonInheriter()
        {
            using (DocumenterSettings.With(assemblies: new[] { typeof(ClassToDocument).Assembly }))
            {
                var lookup = DocumentationClassLocator.GetLookup();
                lookup.ContainsKey(typeof (NotDocumented)).Should().BeFalse();
            }
        }
    }

    public class ClassToDocument { }
    public class AnotherClassToDocument { }
    public class NotDocumented { }

    public class DirectDescendant : TypeSpec<ClassToDocument> { }
    public class DeeperDescendant : RequestSpec<AnotherClassToDocument> { }
}
