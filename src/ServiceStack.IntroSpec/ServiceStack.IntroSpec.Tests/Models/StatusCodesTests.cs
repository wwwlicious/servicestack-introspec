// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Models
{
    using System.Net;
    using FluentAssertions;
    using IntroSpec.Models;
    using Xunit;

    public class StatusCodesTests
    {
        [Fact]
        public void ImplicitHttpStatusCodeCast_SetsStatus()
        {
            var statusCode = (StatusCode)HttpStatusCode.Forbidden;
            statusCode.Code.Should().Be(403);
        }

        [Fact]
        public void ImplicitHttpStatusCodeCast_SetsName()
        {
            var statusCode = (StatusCode)HttpStatusCode.Forbidden;
            statusCode.Name.Should().Be("Forbidden");
        }

        [Fact]
        public void ImplicitHttpStatusCodeCast_SetsNameFormatted()
        {
            var statusCode = (StatusCode)HttpStatusCode.GatewayTimeout;
            statusCode.Name.Should().Be("Gateway Timeout");
        }

        [Fact]
        public void ImplicitHttpStatusCodeCast_DoesNotSetDescription()
        {
            var statusCode = (StatusCode)HttpStatusCode.Forbidden;
            statusCode.Description.Should().BeNull();
        }

        [Fact]
        public void ImplicitIntCast_SetsStatus()
        {
            var statusCode = (StatusCode)403;
            statusCode.Code.Should().Be(403);
        }

        [Fact]
        public void ImplicitIntCast_SetsName()
        {
            var statusCode = (StatusCode)403;
            statusCode.Name.Should().Be("Forbidden");
        }

        [Fact]
        public void ImplicitIntCast_SetsNameFormatted()
        {
            var statusCode = (StatusCode)504;
            statusCode.Name.Should().Be("Gateway Timeout");
        }

        [Fact]
        public void ImplicitIntCast_DoesNotSetDescription()
        {
            var statusCode = (StatusCode)504;
            statusCode.Description.Should().BeNull();
        }

        [Fact]
        public void Equals_True_SameStatusCode()
        {
            var code = new StatusCode { Code = 201, Description = "One", Name = "Strong" };
            var code2 = new StatusCode { Code = 201, Description = "Two", Name = "Weak" };

            code.Equals(code2).Should().BeTrue();
        }

        [Fact]
        public void Equals_False_DifferentStatusCode()
        {
            var code = new StatusCode { Code = 204 };
            var code2 = new StatusCode { Code = 201 };

            code.Equals(code2).Should().BeFalse();
        }

        [Fact]
        public void WithDescription_SetsDescription()
        {
            const string desc = "test description";
            var code = new StatusCode().WithDescription(desc);
            code.Description.Should().Be(desc);
        }

        [Fact]
        public void WithName_SetsName()
        {
            const string name = "john";
            var code = new StatusCode().WithName(name);
            code.Name.Should().Be(name);
        }

        [Fact]
        public void WithCode_SetsCode()
        {
            const int code = 200;
            var statusCode = new StatusCode().WithCode(code);
            statusCode.Code.Should().Be(code);
        }
    }
}
