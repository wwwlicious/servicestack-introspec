namespace ServiceStack.Documentation.Tests.XmlDocumentation
{
    using System;
    using System.Reflection;
    using Documentation.XmlDocumentation;
    using FluentAssertions;
    using Xunit;

    public class MemberInfoExtensionsTests
    {
        private readonly Type nonGenericType = typeof(TestClass);
        private readonly Type embeddedType = typeof(TestClass.EmbeddedClass);
        private readonly Type genericType = typeof(TestGenericClass<string>);
        private readonly Type multiGenericType = typeof(TestMultiGenericClass<string, TestClass>);

        [Fact]
        public void GetMemberElementName_Throws_IfNull()
        {
            MemberInfo mi = null;
            Action act = () => mi.GetMemberElementName();
            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetMemberElementName_Type_NonGeneric()
        {
            nonGenericType.GetMemberElementName()
                .Should()
                .Be("T:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass");
        }

        [Fact]
        public void GetMemberElementName_Type_Embedded()
        {
            embeddedType.GetMemberElementName()
                .Should()
                .Be("T:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass");
        }

        [Fact]
        public void GetMemberElementName_Type_Generic()
        {
            genericType.GetMemberElementName()
                .Should()
                .Be("T:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1");
        }

        [Fact]
        public void GetMemberElementName_Type_MultiGeneric()
        {
            multiGenericType.GetMemberElementName()
                .Should()
                .Be("T:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2");
        }

        [Fact]
        public void GetMemberElementName_ParameterlessCtor_NonGeneric()
        {
            MemberInfo mi = nonGenericType.GetConstructor(Type.EmptyTypes);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.#ctor");
        }

        [Fact]
        public void GetMemberElementName_ParameterlessCtor_Embedded()
        {
            MemberInfo mi = embeddedType.GetConstructor(Type.EmptyTypes);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass.#ctor");
        }

        [Fact]
        public void GetMemberElementName_ParameterCtor_NonGeneric()
        {
            var types = new[] { typeof (string) };
            MemberInfo mi = nonGenericType.GetConstructor(types);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.#ctor(System.String)");
        }

        [Fact]
        public void GetMemberElementName_ParameterlessCtor_Generic()
        {
            MemberInfo mi = genericType.GetConstructor(Type.EmptyTypes);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.#ctor");
        }

        [Fact]
        public void GetMemberElementName_ParameterCtor_Generic()
        {
            var types = new[] { typeof (string) };
            MemberInfo mi = genericType.GetConstructor(types);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.#ctor(`0)");
        }

        [Fact]
        public void GetMemberElementName_ParameterlessCtor_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetConstructor(Type.EmptyTypes);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.#ctor");
        }

        [Fact]
        public void GetMemberElementName_FirstGenericParameterCtor_MultiGeneric()
        {
            var types = new[] { typeof(string) };
            MemberInfo mi = multiGenericType.GetConstructor(types);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.#ctor(`0)");
        }

        [Fact]
        public void GetMemberElementName_SecondGenericParameterCtor_MultiGeneric()
        {
            var types = new[] { typeof(TestClass) };
            MemberInfo mi = multiGenericType.GetConstructor(types);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.#ctor(`1)");
        }

        [Fact]
        public void GetMemberElementName_BothGenericParameterCtor_MultiGeneric()
        {
            var types = new[] { typeof(string), typeof(TestClass) };
            MemberInfo mi = multiGenericType.GetConstructor(types);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.#ctor(`0,`1)");
        }

        [Fact]
        public void GetMemberElementName_BothGenericParameterCtor_SameType_MultiGeneric()
        {
            var types = new[] { typeof(string), typeof(string) };
            MemberInfo mi = typeof(TestMultiGenericClass<string, string>).GetConstructor(types);
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.#ctor(`0,`1)");
        }

        [Fact]
        public void GetMemberElementName_Field_NonGeneric()
        {
            MemberInfo mi = nonGenericType.GetField("MyField");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("F:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.MyField");
        }

        [Fact]
        public void GetMemberElementName_Field_Embedded()
        {
            MemberInfo mi = embeddedType.GetField("MyField");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("F:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass.MyField");
        }

        [Fact]
        public void GetMemberElementName_Field_Generic()
        {
            MemberInfo mi = genericType.GetField("MyField");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("F:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.MyField");
        }

        [Fact]
        public void GetMemberElementName_Field_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetField("MyField");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("F:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.MyField");
        }

        [Fact]
        public void GetMemberElementName_Property_NonGeneric()
        {
            MemberInfo mi = nonGenericType.GetProperty("MyProperty");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("P:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.MyProperty");
        }

        [Fact]
        public void GetMemberElementName_Property_Embedded()
        {
            MemberInfo mi = embeddedType.GetProperty("MyProperty");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("P:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass.MyProperty");
        }

        [Fact]
        public void GetMemberElementName_Property_Generic()
        {
            MemberInfo mi = genericType.GetProperty("MyProperty");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("P:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.MyProperty");
        }

        [Fact]
        public void GetMemberElementName_Property_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetProperty("MyProperty");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("P:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.MyProperty");
        }

        [Fact]
        public void GetMemberElementName_Event_NonGeneric()
        {
            MemberInfo mi = nonGenericType.GetEvent("MyEvent");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("E:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.MyEvent");
        }

        [Fact]
        public void GetMemberElementName_Event_Embedded()
        {
            MemberInfo mi = embeddedType.GetEvent("MyEvent");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("E:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass.MyEvent");
        }

        [Fact]
        public void GetMemberElementName_Event_Generic()
        {
            MemberInfo mi = genericType.GetEvent("MyEvent");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("E:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.MyEvent");
        }

        [Fact]
        public void GetMemberElementName_Event_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetEvent("MyEvent");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("E:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.MyEvent");
        }

        [Fact]
        public void GetMemberElementName_ParameterlessMethod_NonGeneric()
        {
            MemberInfo mi = nonGenericType.GetMethod("ParameterlessAndVoidReturn");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.ParameterlessAndVoidReturn");
        }

        [Fact]
        public void GetMemberElementName_ParameterMethod_NonGeneric()
        {
            MemberInfo mi = nonGenericType.GetMethod("ParametersWithReturn");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.ParametersWithReturn(System.String,System.Int32)");
        }

        //
        [Fact]
        public void GetMemberElementName_ParameterlessMethod_Embedded()
        {
            MemberInfo mi = embeddedType.GetMethod("Parameterless");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass.Parameterless");
        }

        [Fact]
        public void GetMemberElementName_ParameterMethod_Embedded()
        {
            MemberInfo mi = embeddedType.GetMethod("Parameters");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestClass.EmbeddedClass.Parameters(System.String)");
        }
        //

        [Fact]
        public void GetMemberElementName_ParameterlessMethod_Generic()
        {
            MemberInfo mi = genericType.GetMethod("ReturnType");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.ReturnType");
        }

        [Fact]
        public void GetMemberElementName_ParameterMethod_Generic()
        {
            MemberInfo mi = genericType.GetMethod("ParameterOfT");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestGenericClass`1.ParameterOfT(`0)");
        }

        [Fact]
        public void GetMemberElementName_ParameterlessMethod_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetMethod("NoParam");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.NoParam");
        }

        
        [Fact]
        public void GetMemberElementName_FirstGenericParameterMethod_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetMethod("First");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.First(`0)");
        }

        [Fact]
        public void GetMemberElementName_SecondGenericParameterMethod_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetMethod("Second");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.Second(`1)");
        }

        [Fact]
        public void GetMemberElementName_BothGenericParameterMethod_MultiGeneric()
        {
            MemberInfo mi = multiGenericType.GetMethod("Both");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.Both(`0,`1)");
        }

        [Fact]
        public void GetMemberElementName_BothGenericParameterMethod_SameType_MultiGeneric()
        {
            MemberInfo mi = typeof(TestMultiGenericClass<string, string>).GetMethod("Both");
            var memberName = mi.GetMemberElementName();
            memberName.Should().Be("M:ServiceStack.Documentation.Tests.XmlDocumentation.TestMultiGenericClass`2.Both(`0,`1)");
        }
    }

    public class TestClass
    {
        public string MyProperty { get; set; }

        public int MyField;

        public event EventHandler MyEvent;

        public TestClass()
        {

        }

        public TestClass(string param1)
        {

        }

        public void ParameterlessAndVoidReturn()
        {
        }

        public int ParameterlessWithReturn()
        {
            throw new NotImplementedException();
        }

        public int ParametersWithReturn(string one, int two)
        {
            throw new NotImplementedException();
        }

        public class EmbeddedClass
        {
            public string MyProperty { get; set; }

            public int MyField;

            public event EventHandler MyEvent;

            public int Parameterless()
            {
                throw new NotImplementedException();
            }

            public int Parameters(string one)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class TestGenericClass<T>
    {
        public T MyProperty { get; set; }

        public T MyField;

        public event EventHandler MyEvent;

        public TestGenericClass()
        {

        }

        public TestGenericClass(T param)
        {

        }

        public T ReturnType()
        {
            throw new NotImplementedException();
        }

        public void ParameterOfT(T param)
        {
        }
    }

    public class TestMultiGenericClass<T, K>
    {
        public T MyProperty { get; set; }

        public K MyField;

        public event EventHandler MyEvent;

        public TestMultiGenericClass()
        {

        }

        public TestMultiGenericClass(T param1)
        {

        }

        public TestMultiGenericClass(K param1)
        {

        }

        public TestMultiGenericClass(T param1, K param2)
        {

        }

        public void NoParam()
        {
        }

        public void First(T param1) { }

        public void Second(K param1) { }

        public void Both(T param1, K param2) { }
    }
}
