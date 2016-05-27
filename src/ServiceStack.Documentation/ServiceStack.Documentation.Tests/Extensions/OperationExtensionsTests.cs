// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using System.Collections.Generic;
    using Documentation.Extensions;
    using FluentAssertions;
    using Host;
    using Web;
    using Xunit;

    public class OperationExtensionsTests
    {
        [Fact]
        public void IsForVerb_False_AttributeNull()
        {
            RouteAttribute attr = null;
            attr.IsForVerb("GET").Should().BeFalse();
        }

        [Fact]
        public void IsForVerb_True_AttributeHasNoVerbs()
        {
            var attr = new RouteAttribute("/here");
            attr.IsForVerb("GET").Should().BeTrue();
        }

        [Theory]
        [InlineData("get")]
        [InlineData("GET")]
        [InlineData("POST GET")]
        [InlineData("GET POST")]
        [InlineData("POST GET put")]
        public void IsForVerb_True_IfForVerb(string verbs)
        {
            var attr = new RouteAttribute("/here", verbs);
            attr.IsForVerb("GET").Should().BeTrue();
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT POST")]
        public void IsForVerb_False_IfNotForVerb(string verbs)
        {
            var attr = new RouteAttribute("/here", verbs);
            attr.IsForVerb("GET").Should().BeFalse();
        }

        [Fact]
        public void AuthenticationAppliesForVerb_True_IfNoAuthenticateAttribute()
        {
            var operation = new Operation();
            operation.AuthenticationAppliesForVerb("GET").Should().BeTrue();
        }

        [Fact]
        public void AuthenticationAppliesForVerb_True_IfAuthenticateAttributeApplyToAll()
        {
            var operation = new Operation
            {
                RequestFilterAttributes = new List<IHasRequestFilter> { new AuthenticateAttribute() }
            };
            operation.AuthenticationAppliesForVerb("GET").Should().BeTrue();
        }

        [Theory]
        [InlineData(ApplyTo.Get, "GET")]
        [InlineData(ApplyTo.Get, "get")]
        [InlineData(ApplyTo.Get | ApplyTo.Post, "GET")]
        [InlineData(ApplyTo.Get | ApplyTo.Post, "get")]
        public void AuthenticationAppliesForVerb_True_IfApplyToVerb(ApplyTo applyTo, string verb)
        {
            var operation = new Operation
            {
                RequestFilterAttributes = new List<IHasRequestFilter> { new AuthenticateAttribute(applyTo) }
            };
            operation.AuthenticationAppliesForVerb(verb).Should().BeTrue();
        }

        [Fact]
        public void AuthenticationAppliesForVerb_False_IfNotApplyToVerb()
        {
            var operation = new Operation
            {
                RequestFilterAttributes = new List<IHasRequestFilter> { new AuthenticateAttribute(ApplyTo.Get) }
            };
            operation.AuthenticationAppliesForVerb("POST").Should().BeFalse();
        }

        [Fact]
        public void AuthenticationAppliesForVerb_False_IfNotKnownVerb()
        {
            var operation = new Operation
            {
                RequestFilterAttributes = new List<IHasRequestFilter> { new AuthenticateAttribute(ApplyTo.Get) }
            };
            operation.AuthenticationAppliesForVerb("FOO").Should().BeFalse();
        }

        [Fact]
        public void IsForVerb_AuthAttr_True_IfAuthenticateAttributeApplyToAll()
            => new AuthenticateAttribute().IsForVerb("GET").Should().BeTrue();

        [Theory]
        [InlineData(ApplyTo.Get, "GET")]
        [InlineData(ApplyTo.Get, "get")]
        [InlineData(ApplyTo.Get | ApplyTo.Post, "GET")]
        [InlineData(ApplyTo.Get | ApplyTo.Post, "get")]
        public void IsForVerb_AuthAttr_True_IfApplyToVerb(ApplyTo applyTo, string verb)
            => new AuthenticateAttribute(applyTo).IsForVerb(verb).Should().BeTrue();

        [Fact]
        public void IsForVerb_AuthAttr_False_IfNotApplyToVerb()
            => new AuthenticateAttribute(ApplyTo.Get).IsForVerb("POST").Should().BeFalse();

        [Fact]
        public void IsForVerb_AuthAttr_False_IfNotKnownVerb()
            => new AuthenticateAttribute(ApplyTo.Get).IsForVerb("FOO").Should().BeFalse();

        [Fact]
        public void GetRoles_Null_IfNoAttributes() => new Operation().GetRoles("verb").Should().BeNull();

        [Fact]
        public void GetRoles_Null_IfNoAttributesForVerb()
        {
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiresAnyRoleAttribute(ApplyTo.Delete),
                        new RequiredRoleAttribute(ApplyTo.Get)
                    }
            };

            operation.GetRoles("POST").Should().BeNull();
        }

        [Fact]
        public void GetRoles_Any_IfAnyForVerb()
        {
            var roles = new[] { "Admin", "SU" };
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiresAnyRoleAttribute(ApplyTo.Delete, roles)
                    }
            };

            var permissions = operation.GetRoles("Delete");
            permissions.AnyOf.Should().BeEquivalentTo(roles);
            permissions.AllOf.Should().BeNull();
        }

        [Fact]
        public void GetRoles_All_IfAllForVerb()
        {
            var roles = new[] { "Admin", "SU" };
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiredRoleAttribute(ApplyTo.Delete, roles)
                    }
            };

            var permissions = operation.GetRoles("Delete");
            permissions.AllOf.Should().BeEquivalentTo(roles);
            permissions.AnyOf.Should().BeNull();
        }

        [Fact]
        public void GetRoles_AllAndAny_IfPresentForVerb()
        {
            var roles = new[] { "Admin", "SU" };
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiredRoleAttribute(ApplyTo.Delete, roles),
                        new RequiresAnyRoleAttribute(ApplyTo.Delete, roles)
                    }
            };

            var permissions = operation.GetRoles("Delete");
            permissions.AllOf.Should().BeEquivalentTo(roles);
            permissions.AnyOf.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public void GetPermissions_Null_IfNoAttributes() => new Operation().GetPermissions("verb").Should().BeNull();

        [Fact]
        public void GetPermissions_Null_IfNoAttributesForVerb()
        {
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiresAnyPermissionAttribute(ApplyTo.Delete),
                        new RequiredPermissionAttribute(ApplyTo.Get)
                    }
            };

            operation.GetPermissions("POST").Should().BeNull();
        }

        [Fact]
        public void GetPermissions_Any_IfAnyForVerb()
        {
            var roles = new[] { "Admin", "SU" };
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiresAnyPermissionAttribute(ApplyTo.Delete, roles)
                    }
            };

            var permissions = operation.GetPermissions("Delete");
            permissions.AnyOf.Should().BeEquivalentTo(roles);
            permissions.AllOf.Should().BeNull();
        }

        [Fact]
        public void GetPermissions_All_IfAllForVerb()
        {
            var roles = new[] { "Admin", "SU" };
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiredPermissionAttribute(ApplyTo.Delete, roles)
                    }
            };

            var permissions = operation.GetPermissions("Delete");
            permissions.AllOf.Should().BeEquivalentTo(roles);
            permissions.AnyOf.Should().BeNull();
        }

        [Fact]
        public void GetPermissions_AllAndAny_IfPresentForVerb()
        {
            var roles = new[] { "Admin", "SU" };
            var operation = new Operation
            {
                RequestFilterAttributes =
                    new List<IHasRequestFilter>
                    {
                        new RequiredPermissionAttribute(ApplyTo.Delete, roles),
                        new RequiresAnyPermissionAttribute(ApplyTo.Delete, roles)
                    }
            };

            var permissions = operation.GetPermissions("Delete");
            permissions.AllOf.Should().BeEquivalentTo(roles);
            permissions.AnyOf.Should().BeEquivalentTo(roles);
        }
    }
}