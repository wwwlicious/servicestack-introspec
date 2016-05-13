
namespace ServiceStack.Documentation.Tests.AbstractApiSpec
{
    using Documentation.AbstractApiSpec;
    using Documentation.Settings;
    using FluentAssertions;
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
