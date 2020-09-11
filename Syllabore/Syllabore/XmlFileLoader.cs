using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Syllabore
{
    /// <summary>
    /// Creates <c>NameGenerators</c> using an XML definition file.
    /// </summary>
    public class XmlFileLoader
    {
        private string Path { get; set; }
        private Dictionary<string, NameGenerator> NameGenerators { get; set; }

        public XmlFileLoader(string path)
        {
            this.Path = path;
            this.NameGenerators = new Dictionary<string, NameGenerator>();
        }

        public XmlFileLoader Load()
        {
            if (!File.Exists(this.Path))
            {
                throw new ArgumentException("The specified file does not exist.");
            }

            var doc = new XmlDocument();
            doc.Load(this.Path);

            var root = doc.FirstChild;

            if (root.Name.ToLower().Equals("syllabore"))
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("define", StringComparison.OrdinalIgnoreCase))
                    {
                        this.DefineGenerator(node);
                    }
                }
            }

            return this;
        }

        public NameGenerator GetNameGenerator(string name)
        {
            return this.NameGenerators?[name] ?? throw new ArgumentException("The specified name generator does not exist.");
        }

        private void DefineGenerator(XmlNode generatorRoot)
        {
            var generatorName = generatorRoot.Attributes["name"].Value;

            var provider = new ConfigurableSyllableProvider();
            var validator = new ConfigurableNameValidator();

            foreach(XmlNode node in generatorRoot.ChildNodes)
            {
                if (node.Name.Equals("components", StringComparison.OrdinalIgnoreCase))
                {
                    this.DefineSyllableProvider(provider, node);
                }
                else if (node.Name.Equals("constraints", StringComparison.OrdinalIgnoreCase))
                {
                    this.DefineConstraints(validator, node);
                }
                else if (node.Name.Equals("probability", StringComparison.OrdinalIgnoreCase))
                {
                    this.DefineProbability(provider, node);
                }
                else
                {
                    throw new InvalidOperationException("Invalid generator definition tag encountered.");
                }
            }

            this.NameGenerators.Add(generatorName, new NameGenerator(provider, validator));
        }

        private void DefineSyllableProvider(ConfigurableSyllableProvider provider, XmlNode componentsRoot)
        {
            foreach (XmlNode node in componentsRoot.ChildNodes)
            {
                if (node.Name.Equals("Add", StringComparison.OrdinalIgnoreCase))
                {

                    var type = node.Attributes["type"].Value;
                    var values = node.Attributes["values"].Value;
                    var valuesAsArray = values.Split(' ');

                    if(type.Equals("Vowels", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddVowel(valuesAsArray);
                    }
                    else if (type.Equals("VowelSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddVowelSequence(valuesAsArray);
                    }
                    else if (type.Equals("LeadingConsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddLeadingConsonant(valuesAsArray);
                    }
                    else if (type.Equals("LeadingConsonantSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddLeadingConsonantSequence(valuesAsArray);
                    }
                    else if (type.Equals("TrailingConsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddTrailingConsonant(valuesAsArray);
                    }
                    else if (type.Equals("TrailingConsonantSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddTrailingConsonantSequence(valuesAsArray);
                    }
                    else
                    {
                        throw new InvalidOperationException("Attempted to add an invalid component type.");
                    }

                }
            }
        }

        private void DefineConstraints(ConfigurableNameValidator validator, XmlNode constraintsRoot)
        {
            foreach (XmlNode node in constraintsRoot.ChildNodes)
            {
                if (node.Name.Equals("Invalid", StringComparison.OrdinalIgnoreCase))
                {
                    var when = node.Attributes["if"].Value;

                    if (when.Equals("NameMatchesRegex", StringComparison.OrdinalIgnoreCase))
                    {
                        var regex = node.Attributes["regex"].Value;
                        validator.AddRegexConstraint(regex);
                    }
                    else if (when.Equals("NameEndsWith", StringComparison.OrdinalIgnoreCase))
                    {
                        var values = node.Attributes["values"].Value;
                        var regex = string.Format("({0})$", values.Trim().Replace(" ", "|"));
                        validator.AddRegexConstraint(regex);
                    }
                    else
                    {
                        throw new InvalidOperationException("Attempted to add an invalid component type.");
                    }

                }
            }
        }

        private void DefineProbability(ConfigurableSyllableProvider provider, XmlNode probabilityNode)
        {
            foreach (XmlNode node in probabilityNode)
            {
                if (node.Name.Equals("set", StringComparison.OrdinalIgnoreCase))
                {
                    var type = node.Attributes["type"].Value;
                    var value = node.Attributes["value"].Value;

                    if (type.Equals("LeadingVowelProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.LeadingVowelProbability = double.Parse(value);
                        if(provider.LeadingVowelProbability == 1)
                        {
                            provider.UseLeadingConsonants = false;
                            provider.UseLeadingConsonantSequences = false;
                        }
                    }
                    else if (type.Equals("LeadingConsonantSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.LeadingConsonantSequenceProbability = double.Parse(value);
                        if(provider.LeadingConsonantSequenceProbability == 0)
                        {
                            provider.UseLeadingConsonantSequences = false;
                        }
                    }
                    else if (type.Equals("VowelSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.VowelSequenceProbability = double.Parse(value);
                        if (provider.VowelSequenceProbability == 0)
                        {
                            provider.UseVowelSequences = false;
                        }
                    }
                    else if (type.Equals("TrailingConsonantProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.TrailingConsonantProbability = double.Parse(value);
                        if(provider.TrailingConsonantProbability == 0)
                        {
                            provider.UseTrailingConsonants = false;
                        }
                    }
                    else if (type.Equals("TrailingConsonantSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.TrailingConsonantSequenceProbability = double.Parse(value);
                        if(provider.TrailingConsonantSequenceProbability == 0)
                        {
                            provider.UseTrailingConsonantSequences = false;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid probability type encountered.");
                    }

                }
                else
                {
                    throw new InvalidOperationException("Invalid probability tag encountered.");
                }
            }
        }

    }
}
