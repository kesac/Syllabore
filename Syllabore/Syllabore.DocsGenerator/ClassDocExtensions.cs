using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.DocsGenerator
{
    public static class ClassDocExtensions
    {
        /// <summary>
        /// Returns a string that replaces generic type placeholder ("`t")
        /// with the more readable "<T>".
        /// </summary>
        public static string ToReadableName(this Type type)
        {
            return type.Name.Replace("`1", "<T>");
        }

        public static string ToReadableName(this MethodInfo methodInfo)
        {
            return methodInfo.Name.Replace("`1", "<T>");
        }

        /// <summary>
        /// Returns a string that replaces any instances of <c>`t</c>
        /// or <c>&lt;T&gt;</c>
        /// with <c>[T]</c>".
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static string ToFileSystemSafeName(this Type type)
        {
            return type.Name.Replace("`1", "[T]").Replace("<", "[").Replace(">", "]");
        }

        /// <summary>
        /// Converts the specified string into a file system safe name
        /// then returns a markdown link to a file with that name.
        /// </summary>
        public static string ToMarkdownLink(this Type type)
        {
            var namespaceName = type.Namespace ?? null;
            var typeName = type.ToReadableName();

            if (namespaceName != null)
            {
                if (namespaceName.StartsWith("Syllabore"))
                {
                    return $"[{type.ToReadableName()}]({type.ToFileSystemSafeName()}.md)";
                }
                else
                {
                    return type.Namespace + "." + typeName;
                }
            }
            else
            {
                return typeName;
            }
        }
    }
}
