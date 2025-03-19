using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    /// <summary>
    /// Names that sound like they could be a boss in a video game
    /// </summary>
    public class BossNames : Example
    {
        private IGenerator<string> _names;

        private RandomSelector<string> _adjectives;
        private RandomSelector<string> _places;
        private RandomSelector<string> _titles;
        private RandomSelector<string> _colors;
        private List<string> _formats;

        public BossNames()
        {
            // A simple name for the boss
            _names = new NameGenerator()
                .All(x => x
                    .First("stlr")
                    .Middle("aeio"))
                .SetSize(2, 3);

            // Descriptors for the boss
            _adjectives = new RandomSelector<string>("burned", "ruined", "ascended", "petrified");
            _places = new RandomSelector<string>("abyss", "void", "city", "sky");
            _titles = new RandomSelector<string>("sage", "king", "knight", "priest");
            _colors = new RandomSelector<string>("black", "ashen", "gold", "silver");

            _formats = new List<string>
            {
                "{adjective} {title} {name}",
                "{name} of The {adjective} {place}",
                "{title} {name}, The One Who {adjective}",
                "The {adjective} {title}, {name} of The {color} {place}"
            };

        }

        /// <summary>
        /// Returns a generator with names like:
        /// "The Burned Knight, Latula of the Ashen City",
        /// "Sirita of The Petrified Abyss",
        /// "Ascended King Ralo"
        /// </summary>
        public IGenerator<string> GetGenerator()
        {
            var pool = new GeneratorPool<string>();

            foreach(var format in _formats)
            {
                pool.Add(GetFormatter(format));
            }

            return pool;
        }

        private IGenerator<string> GetFormatter(string format)
        {
            return new NameFormatter(format)
                .Define("name", _names)
                .Define("adjective", _adjectives)
                .Define("place", _places)
                .Define("title", _titles)
                .Define("color", _colors);
        }
    }
}
