using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Syllabore
{
    /// <summary>
    /// Creates a <c>NameGenerator</c> using a pre-defined name generation XML definition file.
    /// </summary>
    public class XmlLoader
    {
        private Dictionary<string, NameGenerator> NameGenerators { get; set; }

        public XmlLoader(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("The specified file does not exist.");
            }

            this.NameGenerators = new Dictionary<string, NameGenerator>();

            var doc = new XmlDocument();
            doc.Load(path);

            var root = doc.FirstChild;

            if(root.Name.ToLower().Equals("syllabore"))
            {
                foreach(XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("define", StringComparison.OrdinalIgnoreCase))
                    {
                        this.DefineGenerator(node);
                    }
                }
            }

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
                    // TODO
                }
                else if (node.Name.Equals("probability", StringComparison.OrdinalIgnoreCase))
                {
                    // TODO
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

                    if(type.Equals("vowels", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddVowel(valuesAsArray);
                    }
                    else if (type.Equals("vowelsequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddVowelSequence(valuesAsArray);
                    }
                    else if (type.Equals("startingconsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddStartingConsonant(valuesAsArray);
                    }
                    else if (type.Equals("startingconsonantsequences", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddStartingConsonantSequence(valuesAsArray);
                    }
                    else if (type.Equals("endingconsonants", StringComparison.OrdinalIgnoreCase))
                    {
                        provider.AddEndingConsonant(valuesAsArray);
                    }
                    else if (type.Equals("endingconsonantsequences", StringComparison.OrdinalIgnoreCase))
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

    }
}
