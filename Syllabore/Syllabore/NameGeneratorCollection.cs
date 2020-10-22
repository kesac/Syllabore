using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // A convenience class for managing a collection
    // of NameGenerators.
    public class NameGeneratorCollection
    {
        private Dictionary<string, NameGenerator> _nameGenerators;

        public NameGeneratorCollection()
        {
            this._nameGenerators = new Dictionary<string, NameGenerator>();
        }

        public NameGeneratorCollection Add(string id, NameGenerator g)
        {
            if (this._nameGenerators.ContainsKey(id))
            {
                throw new InvalidOperationException(string.Format("A name generator already exists with ID '{0}'", id));
            }

            this._nameGenerators[id] = g;
            return this;
        }

        public NameGeneratorCollection Define(string id, Func<NameGenerator, NameGenerator> config)
        {
            if (this._nameGenerators.ContainsKey(id))
            {
                throw new InvalidOperationException(string.Format("A name generator already exists with ID '{0}'", id));
            }

            this._nameGenerators[id] = config(new NameGenerator());
            return this;
        }

        public NameGenerator Get(string id)
        {
            if (!this._nameGenerators.ContainsKey(id))
            {
                throw new InvalidOperationException(string.Format("A name generator with ID '{0}' does not exist", id));
            }

            return this._nameGenerators[id];
        } 

    }
}
