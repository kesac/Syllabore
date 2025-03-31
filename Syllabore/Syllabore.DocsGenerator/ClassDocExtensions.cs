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
                        className = $"[{className}]({type.ToFileSystemSafeName()}.md)";
                    }
                }
                else if(!type.IsSyllaboreType() && type.Namespace != null)
                {
                    className = $"{type.Namespace}.{className}";
                }

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
    }
}
