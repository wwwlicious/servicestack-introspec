// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.Documentation.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Documentation.Extensions;
    using FluentAssertions;
    using Xunit;

    public class DictionaryExtensionsTests
    {
        [Fact]
        public void FilterValues_CallsProvidedFilter()
        {
            var predicateCalled = false;
            Func<KeyValuePair<int, int>, bool> filter = kvp => predicateCalled = true;

            var dictionary = new Dictionary<int, int> { { 1, 1 } };
            dictionary.FilterValues(filter).ToList();
            predicateCalled.Should().BeTrue();
        }

        [Fact]
        public void FilterValues_ReturnsEmptyList_IfDictionaryNull()
        {
            Dictionary<string, string> dictionary = null;
            dictionary.FilterValues(kvp => true).Should().BeEmpty();
        }

        [Fact]
        public void FilterValues_ReturnsFilteredResults()
        {
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 },
                { "Test3", 3 }
            };

            var results = dictionary.FilterValues(kvp => kvp.Key == "Test3");
            results.Single().Should().Be(3);
        }

        [Fact]
        public void SafeGet_ReturnsFallback_IfDictionaryNull()
        {
            const string fallback = "fallback";
            Dictionary<string, string> dictionary = null;
            var result = dictionary.SafeGet("test", fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGet_ReturnsFallback_IfKeyNotFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGet("test", fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGet_ReturnsValue_IfKeyFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGet("Test1", fallback);

            result.Should().Be(1);
        }

        [Fact]
        public void SafeGet_Func_ReturnsFallback_IfDictionaryNull()
        {
            const string fallback = "fallback";
            Dictionary<string, string> dictionary = null;
            var result = dictionary.SafeGet("test", () => fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGet_Func_ReturnsFallback_IfKeyNotFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGet("test", () => fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGet_Func_ReturnsValue_IfKeyFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGet("Test1", () => fallback);

            result.Should().Be(1);
        }

        [Fact]
        public void SafeGetFromValue_ReturnsFallback_IfDictionaryNull()
        {
            const string fallback = "fallback";
            Dictionary<string, string> dictionary = null;
            var result = dictionary.SafeGetFromValue("test", s => s, fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGetFromValue_ReturnsFallback_IfKeyNotFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGetFromValue("test", s => s, fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGetFromValue_ReturnsResultOfFunc_IfKeyFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGetFromValue("Test1", s => s + 10, fallback);

            result.Should().Be(11);
        }
        
        [Fact]
        public void SafeGetOrInsert_ReturnsFallback_IfDictionaryNull()
        {
            const string fallback = "fallback";
            Dictionary<string, string> dictionary = null;
            var result = dictionary.SafeGetOrInsert("test", () => fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGetOrInsert_ReturnsFallback_IfKeyNotFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGetOrInsert("test", () => fallback);

            result.Should().Be(fallback);
        }

        [Fact]
        public void SafeGetOrInsert_AddsFallbackToDictionary_IfKeyNotFound()
        {
            const int fallback = 421;
            const string key = "test";

            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            dictionary.SafeGetOrInsert(key, () => fallback);
            dictionary[key].Should().Be(fallback);
        }

        [Fact]
        public void SafeGetOrInsert_ReturnsKey_IfFound()
        {
            const int fallback = 421;
            var dictionary = new Dictionary<string, int>
            {
                { "Test1", 1 },
                { "Test2", 2 }
            };
            var result = dictionary.SafeGetOrInsert("Test1", () => fallback);

            result.Should().Be(1);
        }
    }
}
