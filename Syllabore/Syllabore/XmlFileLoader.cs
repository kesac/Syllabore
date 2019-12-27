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

        public void Load()
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
                if (node.Name.Equals("add", StringComparison.OrdinalIgnoreCase))
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
                    else if (type.Equals("StartingConsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddStartingConsonant(valuesAsArray);
                    }
                    else if (type.Equals("StartingConsonantSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddStartingConsonantSequence(valuesAsArray);
                    }
                    else if (type.Equals("EndingConsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddEndingConsonant(valuesAsArray);
                    }
                    else if (type.Equals("EndingConsonantSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddEndingConsonantSequence(valuesAsArray);
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
                if (node.Name.Equals("Constrain", StringComparison.OrdinalIgnoreCase))
                {
                    var when = node.Attributes["when"].Value;

                    if (when.Equals("NameMatchesRegex", StringComparison.OrdinalIgnoreCase))
                    {
                        var regex = node.Attributes["regex"].Value;
                        validator.AddConstraintAsRegex(regex);
                    }
                    else if (when.Equals("NameEndsWith", StringComparison.OrdinalIgnoreCase))
                    {
                        var values = node.Attributes["values"].Value;
                        var regex = string.Format("({0})$", values.Trim().Replace(" ", "|"));
                        validator.AddConstraintAsRegex(regex);
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

                    if (type.Equals("StartingVowelProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.StartingVowelProbability = double.Parse(value);
                    }
                    else if (type.Equals("StartingConsonantSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.StartingConsonantSequenceProbability = double.Parse(value);
                    }
                    else if (type.Equals("VowelSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.VowelSequenceProbability = double.Parse(value);
                    }
                    else if (type.Equals("EndingConsonantProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.EndingConsonantProbability = double.Parse(value);
                    }
                    else if (type.Equals("EndingConsonantSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.EndingConsonantSequenceProbability = double.Parse(value);
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
