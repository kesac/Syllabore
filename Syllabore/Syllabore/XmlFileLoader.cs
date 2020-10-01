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

            var provider = new SyllableProvider();
            var validator = new NameValidator();

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

        private void DefineSyllableProvider(SyllableProvider provider, XmlNode componentsRoot)
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
                        provider.WithVowels(valuesAsArray);
                    }
                    else if (type.Equals("VowelSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.WithVowelSequences(valuesAsArray);
                    }
                    else if (type.Equals("LeadingConsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.WithLeadingConsonants(valuesAsArray);
                    }
                    else if (type.Equals("LeadingConsonantSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.WithLeadingConsonantSequences(valuesAsArray);
                    }
                    else if (type.Equals("TrailingConsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.WithTrailingConsonants(valuesAsArray);
                    }
                    else if (type.Equals("TrailingConsonantSequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.WithTrailingConsonantSequences(valuesAsArray);
                    }
                    else
                    {
                        throw new InvalidOperationException("Attempted to add an invalid component type.");
                    }

                }
            }
        }

        private void DefineConstraints(NameValidator validator, XmlNode constraintsRoot)
        {
            foreach (XmlNode node in constraintsRoot.ChildNodes)
            {
                if (node.Name.Equals("Invalid", StringComparison.OrdinalIgnoreCase))
                {
                    var when = node.Attributes["if"].Value;

                    if (when.Equals("NameMatchesRegex", StringComparison.OrdinalIgnoreCase))
                    {
                        var regex = node.Attributes["regex"].Value;
                        validator.DoNotAllowPattern(regex);
                    }
                    else if (when.Equals("NameEndsWith", StringComparison.OrdinalIgnoreCase))
                    {
                        var values = node.Attributes["values"].Value;
                        var regex = string.Format("({0})$", values.Trim().Replace(" ", "|"));
                        validator.DoNotAllowPattern(regex);
                    }
                    else
                    {
                        throw new InvalidOperationException("Attempted to add an invalid component type.");
                    }

                }
            }
        }

        private void DefineProbability(SyllableProvider provider, XmlNode probabilityNode)
        {
            foreach (XmlNode node in probabilityNode)
            {
                if (node.Name.Equals("set", StringComparison.OrdinalIgnoreCase))
                {
                    var type = node.Attributes["type"].Value;
                    var value = node.Attributes["value"].Value;

                    if (type.Equals("LeadingVowelProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.Probability.OfStartingSyllableLeadingVowels(double.Parse(value));
                        if(provider.Probability.StartingSyllableLeadingVowel == 1)
                        {
                            provider.DisallowLeadingConsonants();
                            provider.DisallowLeadingConsonantSequences();
                        }
                    }
                    else if (type.Equals("LeadingConsonantSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.Probability.OfLeadingConsonantSequences(double.Parse(value));
                        if (provider.Probability.LeadingConsonantSequence == 0)
                        {
                            provider.DisallowLeadingConsonantSequences();
                        }
                    }
                    else if (type.Equals("VowelSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        // provider.VowelSequenceProbability = double.Parse(value);
                        provider.Probability.OfVowelSequences(double.Parse(value));
                        if (provider.Probability.VowelSequence == 0)
                        {
                            //provider.UseVowelSequences = false;
                            provider.DisallowVowelSequences();
                        }
                    }
                    else if (type.Equals("TrailingConsonantProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        //provider.TrailingConsonantProbability = double.Parse(value);
                        provider.Probability.OfTrailingConsonants(double.Parse(value));
                        if(provider.Probability.TrailingConsonant == 0)
                        {
                            //provider.UseTrailingConsonants = false;
                            provider.DisallowTrailingConsonants();
                        }
                    }
                    else if (type.Equals("TrailingConsonantSequenceProbability", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.Probability.OfTrailingConsonantSequence(double.Parse(value));
                        if(provider.Probability.TrailingConsonantSequence == 0)
                        {
                            provider.DisallowTrailingConsonantSequences();
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
