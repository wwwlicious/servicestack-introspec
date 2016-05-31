// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Tests.Utilities
{
    using System;
    using FluentAssertions;
    using IntroSpec.Utilities;
    using Xunit;

    public class EnumUtilitiesTests
    {
        [Fact]
        public void SafeParse_Fail_IfNotEnum()
            => EnumUtilities.SafeParse<FakeEnum>("wa").IsSuccess.Should().BeFalse();

        [Fact]
        public void SafeParse_Fail_IfValueNotFound()
            => EnumUtilities.SafeParse<TestEnum>("Three").IsSuccess.Should().BeFalse();

        [Theory]
        [InlineData("two")]
        [InlineData("Two")]
        public void SafeParse_Success_IfValueNotFound_CaseInsensitive(string value)
            => EnumUtilities.SafeParse<TestEnum>(value).IsSuccess.Should().BeTrue();

        [Fact]
        public void SafeParse_CorrectValue_IfValueNotFound_CaseInsensitive()
            => EnumUtilities.SafeParse<TestEnum>("two").Value.Should().Be(TestEnum.Two);
    }

    public enum TestEnum
    {
        One,
        Two
    }

    public struct FakeEnum : IConvertible
    {
        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
