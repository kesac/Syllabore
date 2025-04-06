using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

namespace Syllabore.DocsGenerator
{
    /// <summary>
    /// Experimental. Generates markdown documentation for the classes in the Syllabore library.
    /// </summary>
    public class GenerateClassDocs
    {
        private static readonly string DefaultOutputDirectory = "../../../../../docs/v3/class-docs/";

        public static void Main(string[] args)
        {
            // Output directory setup
            if (!Directory.Exists(DefaultOutputDirectory))
            {
                throw new DirectoryNotFoundException($"Class docs directory not found: {DefaultOutputDirectory}");
            }

            var existingDocuments = Directory.GetFiles(DefaultOutputDirectory);
            if (existingDocuments.Count() > 0)
            {
                foreach (var file in existingDocuments)
                {
                    if (!file.StartsWith("readme") && file.ToLower().EndsWith(".md"))
                    {
                        File.Delete(file);
                    }
                }
                Console.WriteLine("Cleaned up inside directory " + DefaultOutputDirectory);
            } 
            
            // Get all classes that we want to document
            var assembly = typeof(Syllabore.NameGenerator).Assembly;
            var classes = assembly.GetTypes()
                .Where(t => t.IsDocumentable())
                .ToList(); // So we can get the size later

            // Find where the XML docs live
            XDocument xmlDoc = null;
            var xmlDocsFile = Path.ChangeExtension(assembly.Location, "xml");

            if (File.Exists(xmlDocsFile))
            {
                xmlDoc = XDocument.Load(xmlDocsFile);
            }
            else
            {
                throw new FileNotFoundException($"XML documentation file not found at: {xmlDocsFile}");
            }

            Console.WriteLine($"Found {classes.Count} classes, interfaces, or enums in the Syllabore namespace");

            // For each class, generate markdown and output to disk
            foreach (var type in classes)
            {
                var markdownContent = GenerateMarkdown(type, xmlDoc);

                // Remove generic type symbols with ones that are friendlier to file systems
                var fileName = $"{type.ToFileSystemSafeName()}.md";
                var filePath = Path.Combine(DefaultOutputDirectory, fileName);
                File.WriteAllText(filePath, markdownContent);

                // Output for the gitbook summary.md
                // Console.WriteLine($"* [{type.ToReadableName()}](class-docs/{type.ToFileSystemSafeName().ToLower()}.md)");
            }

            Console.WriteLine("Documentation generation complete!");
        }

        private static string GenerateMarkdown(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();

            result.Append(GenerateHeader(type, xmlDoc));

            if (type.IsClass || type.IsInterface || type.IsAbstract)
            {
                result.Append(GenerateConstructorsSection(type, xmlDoc));
                result.Append(GenerateMethodsSection(type, xmlDoc));
                result.Append(GenerateFluentMethodsSection(type, xmlDoc));
                result.Append(GeneratePropertiesSection(type, xmlDoc));
            }

            if (type.IsEnum)
            {
                result.Append(GenerateEnumValuesSection(type, xmlDoc));
            }

            return result.ToString();
        }

        private static string GenerateHeader(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();
            result.AppendLine($"# {type.ToReadableName()}");
            result.AppendLine();

            var description = new StringBuilder(ExtractDescription(xmlDoc, type.ToDocumentId()));

            if (type.IsEnum)           { description.Insert(0, "*Enum*. "); }
            else if (type.IsInterface) { description.Insert(0, "*Interface*. "); }

            description.AppendLine();
            description.AppendLine();

            // Display a list of implemented interfaces
            if (type.IsAbstract || type.IsInterface || type.IsClass)
            {
                var interfacesUsed = type.GetInterfaces();
                if (interfacesUsed.Length > 0)
                {
                    var interfaceLinks = interfacesUsed.Select(x => x.ToMarkdownReference());
                    var interfacesList = string.Join(", ", interfaceLinks);
                    description.AppendLine($"*Implements: {interfacesList}*");
                    description.AppendLine();
                }
            }

            return result.Append(description).ToString();
        }

        private static string GenerateConstructorsSection(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Length)
                .ToList(); // So we can get the number of constructors

            if (constructors.Count > 0)
            {
                result.AppendLine("## Constructors");
                result.AppendLine();
                result.AppendLine("| Constructor | Description |");
                result.AppendLine("|-------------|-------------|");

                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    var parameterList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.ToMarkdownReference()} {p.Name}"));
                    var description = ExtractDescription(xmlDoc, constructor.ToDocumentId());
                    var typeName = type.ToReadableName(false); // Leave brackets in since we're printing in a code block

                    result.AppendLine($"| {typeName}({parameterList}) | {description} |");
                }

                result.AppendLine();
            }

            return result.ToString();
        }

        private static string GenerateFluentMethodsSection(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();
            // Find all extension methods in Syllabore.Fluent namespace that apply to this type
            var assembly = typeof(Syllabore.NameGenerator).Assembly;

            var extensionMethods = assembly.GetTypes()
                .Where(t => t.Namespace == "Syllabore.Fluent" && t.Name.EndsWith("Extensions"))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false) 
                    && m.GetParameters().Length > 0 
                    && m.GetParameters()[0].ParameterType == type)
                .OrderBy(m => m.Name)
                .ToList(); // To get total number of methods

            if (extensionMethods.Count > 0)
            {
                result.AppendLine("## Fluent Methods");
                result.AppendLine($"All fluent methods below return an instance of {type.ToMarkdownReference()}.");
                result.AppendLine();
                result.AppendLine("| Method | Description |");
                result.AppendLine("|--------|-------------|");

                foreach (var method in extensionMethods)
                {
                    var methodParameters = string.Join(", ", method.GetParameters().Skip(1).Select(p => { // Skip the 'this' parameter

                        var methodParameterResult = $"{p.ParameterType.ToMarkdownReference().ToSpacedLongString()} {p.Name}";

                        if (p.ParameterType.Name.StartsWith("Func"))
                        {
                            methodParameterResult = "*lambda*";
                            var lambdaType = p.ParameterType.GetGenericArguments().FirstOrDefault();

                            if (lambdaType != null)
                            {
                                methodParameterResult = $"*lambda => {lambdaType.ToMarkdownReference()}*";
                            }
                        }

                        return methodParameterResult;
                    }));

                    var methodDescription = ExtractDescription(xmlDoc, method.ToDocumentId());
                    var methodName = method.ToReadableName();

                    result.AppendLine($"| {methodName}({methodParameters})| {methodDescription} |");
                }

                result.AppendLine();
            }

            return result.ToString();
        }

        private static string GenerateMethodsSection(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName) // Exclude property accessors
                .OrderBy(m => m.Name)
                .ToList();

            result.AppendLine("## Methods");
            result.AppendLine();

            if (methods.Count > 0)
            {
                result.AppendLine("| Method | Returns | Description |");
                result.AppendLine("|--------|---------|-------------|");

                foreach (var method in methods)
                {
                    var paramList = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.ToMarkdownReference().ToSpacedLongString()} {p.Name}"));
                    var returnTypeValue = method.ReturnParameter.ParameterType.ToMarkdownReference();
                    var methodDescription = ExtractDescription(xmlDoc, method.ToDocumentId());
                    var methodName = method.ToReadableName();

                    result.AppendLine($"| {methodName}({paramList})| {returnTypeValue} | {methodDescription} |");
                }

                result.AppendLine();
            }
            else
            {
                result.AppendLine("No public methods.");
            }

            return result.ToString();
        }

        private static string GeneratePropertiesSection(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();
            result.AppendLine("## Properties");
            result.AppendLine();

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.Name)
                .ToList();

            if (properties.Count > 0)
            {
                result.AppendLine("| Property | Type | Description |");
                result.AppendLine("|----------|------|-------------|");

                foreach (var property in properties)
                {
                    var description = ExtractDescription(xmlDoc, property.ToDocumentId());
                    var propertyType = property.PropertyType;
                    var typeResult = propertyType.ToMarkdownReference().ToSpacedLongString();

                    result.AppendLine($"| {property.Name} | {typeResult} | {description} |");
                }
            }
            else
            {
                result.AppendLine("No public properties.");
            }

            return result.ToString();
        }

        private static string GenerateEnumValuesSection(Type type, XDocument xmlDoc)
        {
            var result = new StringBuilder();

            if (type.IsEnum)
            {
                result.AppendLine("## Values");
                result.AppendLine();
                result.AppendLine("| Name | Description |");
                result.AppendLine("|------|-------------|");

                // Get all enum values
                var enumNames = Enum.GetNames(type);

                for (int i = 0; i < enumNames.Length; i++)
                {
                    var enumName = enumNames[i];
                    var documentId = $"F:{type.FullName}.{enumName}"; // TODO: Use extension method ToDocumentId() instead of this
                    var description = ExtractDescription(xmlDoc, documentId);

                    if (description.Trim() == string.Empty)
                    {
                        description = "*No description available.*";
                    }

                    result.AppendLine($"| {enumName} | {description} |");
                }

                result.AppendLine();
            }

            return result.ToString();
        }


        private static string ExtractDescription(XDocument xmlDoc, string targetId)
        {
            string result = string.Empty;

            if (xmlDoc == null)
            {
                throw new ArgumentNullException("Cannot extract description. A null XML file was specified.");
            }

            var memberElement = xmlDoc
                .Descendants("member")
                .FirstOrDefault(e => e.Attribute("name")?.Value == targetId);

            if(memberElement == null)
            {
                Console.WriteLine($"No XML documentation found for target ID: {targetId}");
            }
            else
            {
                var summaryElement = memberElement.Element("summary");

                if (summaryElement != null)
                {
                    var processedElement = new XElement(summaryElement);

                    // Turn '<see cref="">' tags into markdown links, if possible
                    foreach (var seeElement in processedElement.Descendants("see").ToList())
                    {
                        string crefValue = seeElement.Attribute("cref")?.Value ?? string.Empty;
                        string typeName = ExtractTypeNameFromCrefString(crefValue);

                        var markdown = $"*{typeName}*";
                        var type = Type.GetType(typeName + ",Syllabore"); // This will be null for non-Syllabore types                       

                        if (type != null)
                        {
                            // If it's a Syllabore class then we can make it into a markdown link
                            markdown = type.ToMarkdownReference();
                        }

                        // Replace the <see> element with the markdown link
                        seeElement.ReplaceWith(new XText(markdown));

                    }

                    // Whitespace clean-up
                    result = Regex.Replace(processedElement.Value.Trim(), @"\s+", " ");
                }
            }
            

            return result;
        }

        /// <summary>
        /// Extracts the type name from a cref string read from a .dll's XML docs file.
        /// They're usually in one of these formats:
        /// - A simple class: "T:Syllabore.NameGenerator"
        /// - A simple property: "P:Syllabore.NameGenerator.Random"
        /// - An empty constructor: "M:Syllabore.NameGenerator.#ctor"
        /// - A method with parameters: "M:Syllabore.NameGenerator.GenerateName(System.Int32)"
        /// - A class with generic type: "T:Syllabore.GeneratorPool`1" (Note the `1 at the end)
        /// </summary>
        private static string ExtractTypeNameFromCrefString(string cref)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(cref))
            {
                // Remove the type prefix (T:, M:, P:, etc.)
                var colonLocation = cref.IndexOf(':');
                if (colonLocation >= 0 && colonLocation < cref.Length - 1)
                {
                    result = cref.Substring(colonLocation + 1);
                }

                // Remove any method parameters if present
                var startBracketLocation = cref.IndexOf('(');
                if (startBracketLocation > 0)
                {
                    result = cref.Substring(0, startBracketLocation);
                }

                // Handle generic types by removing the ` and any number that follows
                var tickLocation = cref.IndexOf('`');
                if (tickLocation > 0)
                {
                    result = cref.Substring(0, tickLocation);
                }

                // Detect constructors
                if (cref == "#ctor")
                {
                    result = "Constructor";
                }
            }

            return result;
        }

        
        
    }
}
