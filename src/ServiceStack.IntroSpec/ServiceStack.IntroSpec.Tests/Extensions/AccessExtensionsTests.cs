// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Extensions
{
    using DataAnnotations;
    using FluentAssertions;
    using Infrastructure;
    using IntroSpec.Extensions;
    using Xunit;

    public class AccessExtensionsTests
    {
        [Fact]
        public void CanAccess_True_IfRestrictAttributeNull()
        {
            RestrictAttribute attr = null;
            attr.CanAccess(Result<RequestAttributes>.Success(RequestAttributes.Csv)).Should().BeTrue();
        }

        [Fact]
        public void CanAccess_True_IfRestrictAttributeHasAccessToRequestAttribute()
        {
            var attr = new RestrictAttribute(RequestAttributes.Csv);
            attr.CanAccess(Result<RequestAttributes>.Success(RequestAttributes.Csv)).Should().BeTrue();
        }

        [Fact]
        public void CanAccess_False_IfRestrictAttributeDoesNotHaveAccessToRequestAttribute()
        {
            var attr = new RestrictAttribute(RequestAttributes.Html);
            attr.CanAccess(Result<RequestAttributes>.Success(RequestAttributes.Csv)).Should().BeFalse();
        }

        [Fact]
        public void HasAccessToFeature_True_IfFeatureNotParsedSuccessfully()
            => typeof(int).HasAccessToFeature(Result<Feature>.Fail()).Should().BeTrue();

        [Fact]
        public void HasAccessToFeature_True_IfTypeHasNoExcludeAttribute()
            => typeof(NoExclude).HasAccessToFeature(Result<Feature>.Success(Feature.Html)).Should().BeTrue();

        [Fact]
        public void HasAccessToFeature_True_IfTypeHasExcludeAttributeWithDifferentFeature()
            => typeof(HasExclude).HasAccessToFeature(Result<Feature>.Success(Feature.Html)).Should().BeTrue();

        [Fact]
        public void HasAccessToFeature_False_IfTypeHasExcludeAttributeWithFeature()
            => typeof(HasExclude).HasAccessToFeature(Result<Feature>.Success(Feature.Csv)).Should().BeFalse();
    }

    [Exclude(Feature.Csv)]
    public class HasExclude { }

    public class NoExclude { }
}
