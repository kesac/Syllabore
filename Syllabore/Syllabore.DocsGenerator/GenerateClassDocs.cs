using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Syllabore.DocsGenerator
{
    /// <summary>
    /// Generates markdown documentation for the classes in the Syllabore library.
    /// </summary>
    public class GenerateClassDocs
    {
        private static readonly string DefaultOutputDirectory = "output-docs";

        public static void Main(string[] args)
        {
            // Output directory setup
            if (!Directory.Exists(DefaultOutputDirectory))
            {
                Directory.CreateDirectory(DefaultOutputDirectory);
                Console.WriteLine("Created directory " + DefaultOutputDirectory);
            }

            var existingDocuments = Directory.GetFiles(DefaultOutputDirectory);
            if (existingDocuments.Count() > 0)
            {
                foreach (var file in existingDocuments)
                {
                    if (file.ToLower().EndsWith(".md"))
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
                .OrderBy(t => t.Name)
                .ToList();

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
            }

            Console.WriteLine("Documentation generation complete!");
        }

        private static string GenerateMarkdown(Type type, XDocument xmlDoc)
        {
            var markdown = new StringBuilder();

            AppendHeader(markdown, type, xmlDoc);

            if (type.IsClass || type.IsInterface || type.IsAbstract)
            {
                AppendConstructorsSection(markdown, type, xmlDoc);
                AppendMethodsSection(markdown, type, xmlDoc);
                AppendPropertiesSection(markdown, type, xmlDoc);
            }

            if (type.IsEnum)
            {
                AppendEnumValuesSection(markdown, type, xmlDoc);
            }

            return markdown.ToString();
        }


        private static void AppendHeader(StringBuilder markdown, Type type, XDocument xmlDoc)
        {
            // Name and header
            var name = type.ToReadableName();
            markdown.AppendLine($"# {name}");
            markdown.AppendLine();

            // Description
            string targetId = $"T:{type.FullName}";
            string description = ExtractXmlSummary(xmlDoc, targetId);

            if (type.IsEnum)
            {
                description = "*Enum*. " + description;
            }
            else if (type.IsInterface)
            {
                description = "*Interface*. " + description;
            }

            if (!string.IsNullOrEmpty(description))
            {
                markdown.AppendLine(description);
                markdown.AppendLine();
            }

            if (type.IsAbstract || type.IsInterface || type.IsClass)
            {
                // Get interfaces (before adding the class description)
                var interfacesUsed = type.GetInterfaces();
                if (interfacesUsed.Length > 0)
                {
                    // Create a list of interface links instead of just names
                    var interfaceLinks = interfacesUsed
                        .Select(x => x.ToMarkdownReference())
                        .ToList();

                    string interfacesList = string.Join(", ", interfaceLinks);
                    markdown.AppendLine($"*Implements: {interfacesList}*");
                    markdown.AppendLine();
                }
            }
        }

        private static void AppendConstructorsSection(StringBuilder markdown, Type type, XDocument xmlDoc)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(c => c.GetParameters().Length)
                .ToList();

            if (constructors.Count > 0)
            {
                markdown.AppendLine("## Constructors");
                markdown.AppendLine();
                markdown.AppendLine("| Constructor | Description |");
                markdown.AppendLine("|-------------|-------------|");

                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.ToMarkdownReference()} {p.Name}"));

                    // Fix: For parameterless constructors, don't include the parentheses in XML ID
                    string targetId;
                    if (parameters.Length == 0)
                    {
                        targetId = $"M:{type.FullName}.#ctor";
                    }
                    else
                    {
                        targetId = $"M:{type.FullName}.#ctor({string.Join(",", parameters.Select(p => p.ParameterType.ToMarkdownReference()))})";
                    }

                    var description = ExtractXmlSummary(xmlDoc, targetId);
                    var typeName = type.ToReadableName(false); // Leave brackets in since we're printing in a code block

                    markdown.AppendLine($"| {typeName}({paramList}) | {description} |");
                }

                markdown.AppendLine();
            }
        }

        private static void AppendMethodsSection(StringBuilder markdown, Type type, XDocument xmlDoc)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName) // Exclude property accessors
                .OrderBy(m => m.Name)
                .ToList();

            markdown.AppendLine("## Methods");
            markdown.AppendLine();

            if (methods.Count > 0)
            {
                markdown.AppendLine("| Method | Returns | Description |");
                markdown.AppendLine("|--------|---------|-------------|");

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    var returnType = method.ReturnParameter.ParameterType;
                    var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.ToMarkdownReference().ToSpacedLongString()} {p.Name}"));

                    var targetId = string.Empty;
                    if (parameters.Length == 0)
                    {
                        targetId = $"M:{type.FullName}.{method.Name}";
                    }
                    else
                    {
                        targetId = $"M:{type.FullName}.{method.Name}({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})";
                    }

                    var methodDescription = ExtractXmlSummary(xmlDoc, targetId);
                    var returnTypeValue = returnType.ToMarkdownReference();

                    var methodName = method.ToReadableName();
                    markdown.AppendLine($"| {methodName}({paramList})| {returnTypeValue} | {methodDescription} |");
                }

                markdown.AppendLine();
            }
            else
            {
                markdown.AppendLine("No public methods.");
            }
        }

        private static void AppendPropertiesSection(StringBuilder markdown, Type type, XDocument xmlDoc)
        {
            markdown.AppendLine("## Properties");
            markdown.AppendLine();

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.Name)
                .ToList();

            if (properties.Count > 0)
            {
                markdown.AppendLine("| Property | Type | Description |");
                markdown.AppendLine("|----------|------|-------------|");

                foreach (var property in properties)
                {
                    var targetId = $"P:{type.FullName}.{property.Name}";
                    var description = ExtractXmlSummary(xmlDoc, targetId);
                    var propertyType = property.PropertyType;

                    var typeResult = propertyType.ToMarkdownReference().ToSpacedLongString();

                    markdown.AppendLine($"| {property.Name} | {typeResult} | {description} |");
                }
            }
            else
            {
                markdown.AppendLine("No public properties.");
            }
        }

        private static void AppendEnumValuesSection(StringBuilder markdown, Type type, XDocument xmlDoc)
        {
            if (!type.IsEnum)
                return;

            markdown.AppendLine("## Values");
            markdown.AppendLine();
            markdown.AppendLine("| Name | Description |");
            markdown.AppendLine("|------|-------------|");

            // Get all enum values
            var enumNames = Enum.GetNames(type);

            for (int i = 0; i < enumNames.Length; i++)
            {
                string enumName = enumNames[i];

                // Build the XML documentation ID for this enum value
                string fieldXmlId = $"F:{type.FullName}.{enumName}";
                string description = ExtractXmlSummary(xmlDoc, fieldXmlId);

                if (description.Trim() == string.Empty)
                {
                    description = "*No description available.*";
                }

                // Add row to the markdown table
                markdown.AppendLine($"| {enumName} | {description} |");
            }

            markdown.AppendLine();
        }


        private static string ExtractXmlSummary(XDocument xmlDoc, string targetId)
        {
            string result = string.Empty;

            if (xmlDoc != null)
            {
                var memberElement = xmlDoc
                    .Descendants("member")
                    .FirstOrDefault(e => e.Attribute("name")?.Value == targetId);

                if (memberElement != null)
                {
                    var summaryElement = memberElement.Element("summary");

                    if (summaryElement != null)
                    {
                        var processedElement = new XElement(summaryElement);

                        // Process all <see cref="..."> tags
                        foreach (var seeElement in processedElement.Descendants("see").ToList())
                        {
                            string crefValue = seeElement.Attribute("cref")?.Value ?? string.Empty;
                            string typeName = ExtractNameFromCref(crefValue);

                            var typeMarkdown = $"*{typeName}*";
                            var type = Type.GetType(typeName + ",Syllabore"); // This will be null for non-Syllabore types                       

                            if (type != null)
                            {
                                typeMarkdown = type.ToMarkdownReference();
                            }

                            // Replace the <see> element with the markdown link
                            seeElement.ReplaceWith(new XText(typeMarkdown));
                            
                        }

                        result = processedElement.Value.Trim();
                        result = Regex.Replace(result, @"\s+", " ");
                    }
                }
            }

            return result;
        }

        private static string ExtractNameFromCref(string crefValue)
        {
            if (string.IsNullOrEmpty(crefValue))
                return string.Empty;

            // Remove the type prefix (T:, M:, P:, etc.)
            var colonLocation = crefValue.IndexOf(':');
            if (colonLocation >= 0 && colonLocation < crefValue.Length - 1)
            {
                crefValue = crefValue.Substring(colonLocation + 1);
            }

            // Remove any method parameters if present
            var startBracketLocation = crefValue.IndexOf('(');
            if (startBracketLocation > 0)
            {
                crefValue = crefValue.Substring(0, startBracketLocation);
            }

            // Handle generic types by removing the ` and any number that follows
            var tickLocation = crefValue.IndexOf('`');
            if (tickLocation > 0)
            {
                crefValue = crefValue.Substring(0, tickLocation);
            }

            // Special case for constructors
            if (crefValue == "#ctor")
            {
                return "Constructor";
            }

            return crefValue;
        }
        
    }
}
