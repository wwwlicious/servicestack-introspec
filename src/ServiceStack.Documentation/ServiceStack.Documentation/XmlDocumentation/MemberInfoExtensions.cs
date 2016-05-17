namespace ServiceStack.Documentation.XmlDocumentation
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Logging;

    public static class MemberInfoExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MemberInfoExtensions));

        // from http://www.brad-smith.info/blog/archives/220
        public static string GetMemberElementName(this MemberInfo member)
        {
            member.ThrowIfNull(nameof(member));

            char prefixCode;
            var type = member as Type;

            string memberName;
            bool isGeneric = false;
            if (type == null)
            {
                // member belongs to a Type
                if (!member.DeclaringType.IsGeneric())
                    memberName = $"{member.DeclaringType.FullName}.{member.Name}";
                else
                {
                    isGeneric = true;
                    memberName = $"{member.DeclaringType.Namespace}.{member.DeclaringType.Name}.{member.Name}";
                }
            }
            else
            {
                // member is a Type
                if (!type.IsGeneric())
                    memberName = type.FullName;
                else
                {
                    memberName = $"{type.Namespace}.{type.Name}";
                    isGeneric = true;
                }
            }

            memberName = memberName.Replace('+', '.');

            if (isGeneric)
                log.Warn($"DTOs should be POCO objects without generics. Type: {memberName}");

            switch (member.MemberType)
            {
                case MemberTypes.Constructor:
                    // XML documentation uses slightly different constructor names
                    memberName = memberName.Replace(".ctor", "#ctor");
                    goto case MemberTypes.Method;
                case MemberTypes.Method:
                    prefixCode = 'M';

                    // parameters are listed according to their type, not their name
                    var parameters = ((MethodBase) member).GetParameters()
                        .Select(x => x.ParameterType.FullName
                        ).ToArray();

                    if (parameters.Length > 0)
                    {
                        if (isGeneric)
                        {
                            // Generics will want params like (`0,`1) rather than (System.String, System.Int32
                            var genericParams = member.DeclaringType.GenericTypeArguments.Select(g => g.FullName).ToArray();

                            // NOTE For multiple params of the same type does this catch them all??
                            var count = 0;
                            parameters =
                                parameters.Select(p => $"`{Math.Max(count++, Array.IndexOf(genericParams, p))}")
                                    .ToArray();
                        }
                        string paramTypesList = string.Join(",", parameters);
                        memberName += $"({paramTypesList})";
                    }
                    break;

                case MemberTypes.Event:
                    prefixCode = 'E';
                    break;

                case MemberTypes.Field:
                    prefixCode = 'F';
                    break;

                case MemberTypes.NestedType:
                    // XML documentation uses slightly different nested type names
                    memberName = memberName.Replace('+', '.');
                    goto case MemberTypes.TypeInfo;
                case MemberTypes.TypeInfo:
                    prefixCode = 'T';
                    break;

                case MemberTypes.Property:
                    prefixCode = 'P';
                    break;

                default:
                    throw new ArgumentException("Unknown member type", nameof(member));
            }

            // elements are of the form "M:Namespace.Class.Method"
            return $"{prefixCode}:{memberName}";
        }
    }
}