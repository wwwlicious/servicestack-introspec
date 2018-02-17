namespace ServiceStack.IntroSpec.Models
{
    using System;
    using System.Runtime.Serialization;

    public class ApiClrType
    {
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public bool IsPrimitive { get; set; }
        [IgnoreDataMember]
        public Type OriginalType { get; set; }
    }
}