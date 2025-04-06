using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Syllabore.DocsGenerator
{
    /// <summary>
    /// Extension methods used to generate Syllabore class docs.
    /// </summary>
    public static class ClassDocExtensions
    {
        /// <summary>
        /// Returns true if the specified type is type defined in the Syllabore library.
        /// </summary>
        public static bool IsSyllaboreType(this Type type)
        {
            return type.Namespace != null && type.Namespace.StartsWith("Syllabore");
        }

        /// <summary>
        /// Returns true if the specified type is a type that should be documented.
        /// </summary>
        public static bool IsDocumentable(this Type t)
        {
            return t.Namespace != null
                && t.Namespace.StartsWith("Syllabore")
                && t.IsPublic && (t.IsAbstract || t.IsClass || t.IsInterface || t.IsEnum)
                && !t.Name.StartsWith("<")
                && !t.Name.EndsWith("Extension")
                && !t.Name.EndsWith("Extensions");
        }

        /// <summary>
        /// Returns a string that replaces generic type placeholder ("`t")
        /// with the more readable "<T>".
        /// </summary>
        public static string ToReadableName(this Type type, bool useEscapeCharacters = true, bool tryToUseFullName = false)
        {
            var result = type.Name;

            if(tryToUseFullName && type.FullName != null)
            {
                result = type.FullName;
            }

            // "`1" becomes "<T>"
            if(useEscapeCharacters)
            {
                result = result.Replace("`1", "&lt;T&gt;");
            }
            else
            {
                result = result.Replace("`1", "<T>");
            }

            return result;

        }

        public static string ToReadableName(this MethodInfo methodInfo)
        {
            return methodInfo.Name.Replace("`1", "<T>");
        }

        /// <summary>
        /// Returns a string that replaces any instances of <c>`t</c>
        /// or <c>&lt;T&gt;</c>
        /// with <c>-less-than-t-greater-than</c>".
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string ToFileSystemSafeName(this Type type)
        {
            // Also gitbook friendly format
            return type.Name.ToLower()
                .Replace("`1", "-less-than-t-greater-than")
                .Replace("<", "-less-than-")
                .Replace(">", "-greater-than");
        }

        /// <summary>
        /// For very long type strings, this method will insert spaces just
        /// before the "<" character. This is helpful in markdown tables.
        /// </summary>
        public static string ToSpacedLongString(this string value, int maxLength = 32)
        {
            if(value.Length > maxLength)
            {
                return value.Replace("<", " <").Replace("&lt;", " &lt;");
            }

            return value;
        }

        /// <summary>
        /// Returns a reference to the specified type in markdown format.
        /// If the type is part of the Syllabore library, it will make the markdown
        /// a hyperlink. Otherwise it will resolve the type along with its namespace.
        /// This method also handles generic types.
        /// </summary>
        public static string ToMarkdownReference(this Type type, bool checkForGenericType = true)
        {
            if (type == null)
            {
                throw new ArgumentNullException("No type was specified.");
            }

            var result = new StringBuilder();

            if (type.IsGenericType)
            {
                var className = type.Name.Substring(0, type.Name.IndexOf('`'));

                if(type.IsSyllaboreType())
                {
                    if (type.IsDocumentable())
                    {
                        // Hyperlink to class documentation
                        className = $"[{className}]({type.ToFileSystemSafeName()}.md)";
                    }
                }
                else if(!type.IsSyllaboreType() && type.Namespace != null)
                {
                    // Show non-Syllabore class names along with the namespace they belong to
                    className = $"{type.Namespace}.{className}";
                }

                // In the format Class<Namespace.T1, Namespace2.T2>
                var genericTypeNames = string.Join(',', type.GetGenericArguments().Select(x => x.ToMarkdownReference()));
                result.Append($"{className}&lt;{genericTypeNames}&gt;");
            }
            else if(type.IsGenericTypeParameter)
            {
                result.Append(type.Name);
            }
            else if (type.IsSyllaboreType() && type.IsDocumentable())
            {
                result.Append($"[{type.ToReadableName()}]({type.ToFileSystemSafeName()}.md)");
            }
            else
            {
                result.Append(type.ToReadableName(true, true));
            }

            return result.ToString();

        } // ToMarkdownReference()

        public static string ToDocumentId(this Type t)
        {
            return GenerateDocumentId(t);
        }

        public static string ToDocumentId(this ConstructorInfo c)
        {
            return GenerateDocumentId(c);
        }

        public static string ToDocumentId(this MethodInfo m)
        {
            return GenerateDocumentId(m);
        }

        public static string ToDocumentId(this PropertyInfo p)
        {
            return GenerateDocumentId(p);
        }

        /// <summary>
        /// Returns an ID for a type, property, field, etc. that is suitable
        /// for doing lookups in the XML documentation of a library.
        /// </summary>
        private static string GenerateDocumentId(object info)
        {
            if (info is Type type)
            {
                return $"T:{type.FullName}";
            }
            else if (info is PropertyInfo property)
            {
                return $"P:{property.DeclaringType.FullName}.{property.Name}";
            }
            else if (info is ConstructorInfo constructor)
            {
                var parameters = constructor.GetParameters();

                if (parameters.Length == 0)
                {
                    return $"M:{constructor.DeclaringType.FullName}.#ctor";
                }
                else
                {
                    return $"M:{constructor.DeclaringType.FullName}.#ctor({string.Join(",", parameters.Select(p =>
                    {
                        if (p.ParameterType.IsGenericType)
                        {
                            var genericTypeName = $"{p.ParameterType.Namespace}.{p.ParameterType.Name.Substring(0, p.ParameterType.Name.IndexOf("`"))}";
                            var parameterTypeNames = string.Join(",", p.ParameterType.GetGenericArguments().Select(x => $"{x.Namespace}.{x.Name}"));
                            return genericTypeName + "{" + parameterTypeNames + "}";
                        }
                        else
                        {
                            return p.ParameterType.FullName;
                        }
                    }))})";
                }
            }
            else if (info is MethodInfo method)
            {
                var parameters = method.GetParameters();

                if (method.IsConstructor)
                {
                    if (parameters.Length == 0)
                    {
                        return $"M:{method.DeclaringType.FullName}.#ctor";
                    }
                    else
                    {
                        return $"M:{method.DeclaringType.FullName}.#ctor({string.Join(",", parameters.Select(p =>
                        {
                            if (p.ParameterType.IsGenericType)
                            {
                                var genericTypeName = $"{p.ParameterType.Namespace}.{p.ParameterType.Name.Substring(0, p.ParameterType.Name.IndexOf("`"))}";
                                var parameterTypeNames = string.Join(",", p.ParameterType.GetGenericArguments().Select(x => $"{x.Namespace}.{x.Name}"));
                                return genericTypeName + "{" + parameterTypeNames + "}";
                            }
                            else
                            {
                                return p.ParameterType.FullName;
                            }
                        }))})";
                    }
                }
                else
                {
                    if (parameters.Length == 0)
                    {
                        return $"M:{method.DeclaringType.FullName}.{method.Name}";
                    }
                    else
                    {
                        return $"M:{method.DeclaringType.FullName}.{method.Name}({string.Join(",", parameters.Select(p =>
                        {
                            if (p.ParameterType.IsGenericType)
                            {
                                var genericTypeName = $"{p.ParameterType.Namespace}.{p.ParameterType.Name.Substring(0, p.ParameterType.Name.IndexOf("`"))}";
                                var parameterTypeNames = string.Join(",", p.ParameterType.GetGenericArguments().Select(x => $"{x.Namespace}.{x.Name}"));
                                return genericTypeName + "{" + parameterTypeNames + "}";
                            }
                            else
                            {
                                return p.ParameterType.FullName;
                            }
                        }))})";
                    }
                }
            }
            else if (info is FieldInfo field)
            {
                return $"F:{field.DeclaringType.FullName}.{field.Name}";
            }
            else if (info is ParameterInfo parameter)
            {
                // For parameters, we need to get the method that defines the parameter
                var method2 = parameter.Member as MethodBase;
                if (method2 != null)
                {
                    // Create the method ID and append the parameter
                    string methodId;
                    if (method2.IsConstructor)
                    {
                        methodId = $"M:{method2.DeclaringType.FullName}.#ctor";
                    }
                    else
                    {
                        methodId = $"M:{method2.DeclaringType.FullName}.{method2.Name}";
                    }

                    return $"{methodId}({parameter.Position})";
                }
                else
                {
                    throw new ArgumentException("Could not determine the containing method for this parameter");
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported reflection object type: {info.GetType().Name}");
            }
        }

    }
}
