using System;
using System.Reflection;
using System.Text;

namespace CSharpTypePrinter
{
    /// <summary>
    /// Contains an extension method for the System.Type to print it as a valid C# code
    /// </summary>
    public static class TypePrinter
    {
        /// <summary>
        /// Prints System.Type object as a valid C# literal
        /// </summary>
        /// <param name="type">The type to print</param>
        /// <param name="stripNamespace">self explanatory</param>
        /// <param name="printType">function may configure the final result given the input type and the output string</param>
        /// <param name="printGenericTypeArgs">if set ti true will output open-generic type arguments otherwise outputs them as empty strings to be used in the typeof construct</param>
        /// <returns>The output C# literal for the type</returns>
        public static string ToCSharpCode(this Type type,
            bool stripNamespace = false, Func<Type, string, string> printType = null, bool printGenericTypeArgs = false)
        {
            if (type.IsGenericParameter)
                return !printGenericTypeArgs ? string.Empty
                    : (printType?.Invoke(type, type.Name) ?? type.Name);

            Type arrayType = null;
            if (type.IsArray)
            {
                // store the original type for the later and process its element type further here
                arrayType = type;
                type = type.GetElementType();
            }

            // the default handling of the built-in types
            string buildInTypeString = null;
            if (type == typeof(void))
                buildInTypeString = "void";
            if (type == typeof(object))
                buildInTypeString = "object";
            if (type == typeof(bool))
                buildInTypeString = "bool";
            if (type == typeof(int))
                buildInTypeString = "int";
            if (type == typeof(short))
                buildInTypeString = "short";
            if (type == typeof(byte))
                buildInTypeString = "byte";
            if (type == typeof(double))
                buildInTypeString = "double";
            if (type == typeof(float))
                buildInTypeString = "float";
            if (type == typeof(char))
                buildInTypeString = "char";
            if (type == typeof(string))
                buildInTypeString = "string";

            if (buildInTypeString != null)
                return printType?.Invoke(arrayType ?? type, buildInTypeString) ?? buildInTypeString;

            var parentCount = 0;
            for (var ti = type.GetTypeInfo(); ti.IsNested; ti = ti.DeclaringType.GetTypeInfo())
                ++parentCount;

            Type[] parentTypes = null;
            if (parentCount > 0) 
            {
                parentTypes = new Type[parentCount];
                var pt = type.DeclaringType;
                for (var i = 0; i < parentTypes.Length; i++, pt = pt.DeclaringType)
                    parentTypes[i] = pt;
            }

            var typeInfo = type.GetTypeInfo();
            Type[] typeArgs = null;
            var isTypeClosedGeneric = false;
            if (type.IsGenericType)
            {
                isTypeClosedGeneric = !typeInfo.IsGenericTypeDefinition;
                typeArgs = isTypeClosedGeneric ? typeInfo.GenericTypeArguments : typeInfo.GenericTypeParameters;
            }

            var typeArgsConsumedByParentsCount = 0;
            var s = new StringBuilder();
            if (!stripNamespace) 
                s.Append(type.Namespace).Append('.');

            if (parentTypes != null) 
            {
                for (var p = parentTypes.Length - 1; p >= 0; --p)
                {
                    var parentType = parentTypes[p];
                    if (!parentType.IsGenericType)
                    {
                        s.Append(parentType.Name).Append('.');
                    }
                    else
                    {
                        var parentTypeInfo = parentType.GetTypeInfo();
                        Type[] parentTypeArgs = null;
                        if (parentTypeInfo.IsGenericTypeDefinition)
                        {
                            parentTypeArgs = parentTypeInfo.GenericTypeParameters;

                            // replace the open parent args with the closed child args,
                            // and close the parent
                            if (isTypeClosedGeneric)
                                for (var t = 0; t < parentTypeArgs.Length; ++t) 
                                    parentTypeArgs[t] = typeArgs[t];

                            var parentTypeArgCount = parentTypeArgs.Length;
                            if (typeArgsConsumedByParentsCount > 0)
                            {
                                int ownArgCount = parentTypeArgCount - typeArgsConsumedByParentsCount;
                                if (ownArgCount == 0)
                                    parentTypeArgs = null;
                                else
                                {
                                    var ownArgs = new Type[ownArgCount];
                                    for (var a = 0; a < ownArgs.Length; ++a)
                                        ownArgs[a] = parentTypeArgs[a + typeArgsConsumedByParentsCount];
                                    parentTypeArgs = ownArgs;
                                }
                            }
                            typeArgsConsumedByParentsCount = parentTypeArgCount;
                        }
                        else 
                        {
                            parentTypeArgs = parentTypeInfo.GenericTypeArguments;
                        }

                        var parentTickIndex = parentType.Name.IndexOf('`');
                        s.Append(parentType.Name.Substring(0, parentTickIndex));

                        // The owned parentTypeArgs maybe empty because all args are defined in the parent's parents
                        if (parentTypeArgs?.Length > 0)
                        {
                            s.Append('<');
                            for (var t = 0; t < parentTypeArgs.Length; ++t)
                                (t == 0 ? s : s.Append(", "))
                                    .Append(parentTypeArgs[t].ToCSharpCode(stripNamespace, printType, printGenericTypeArgs));
                            s.Append('>');
                        }
                        s.Append('.');
                    }
                }
            }

            if (typeArgs != null && typeArgsConsumedByParentsCount < typeArgs.Length)
            {
                var tickIndex = type.Name.IndexOf('`');
                s.Append(type.Name.Substring(0, tickIndex)).Append('<');
                for (var i = 0; i < typeArgs.Length - typeArgsConsumedByParentsCount; ++i) 
                    (i == 0 ? s : s.Append(", "))
                        .Append(typeArgs[i + typeArgsConsumedByParentsCount]
                            .ToCSharpCode(stripNamespace, printType, printGenericTypeArgs));
                s.Append('>');
            }
            else
            {
                s.Append(type.Name);
            }

            if (arrayType != null)
                s.Append("[]");

            return printType?.Invoke(arrayType ?? type, s.ToString()) ?? s.ToString();
        }
    }
}
