using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Syllabore.Json
{
    /// <summary>
    /// <para>
    /// A simplified configuration for a <see cref="NameGenerator"/>. 
    /// </para>
    /// <para>
    /// Use <see cref="Load(string)"/> to instantiate an instance of this class.
    /// An instance of this class is meant to be supplied as a single argument to a <see cref="NameGenerator"/> constructor.
    /// </para>
    /// <para>
    /// Unlike the formal JSON format produced and parsed by <see cref="NameGeneratorSerializer"/>, this
    /// format is a simplified syntax meant to be written by hand.
    /// </para>
    /// </summary>
    public class NameGeneratorConfig
    {
        private static readonly string[] PositionKeys = { "any", "start", "inner", "end" };
        private static readonly Regex WeightSuffix = new Regex(@"^(.*)\*(\d+)$");
        private static readonly Regex StepPattern = new Regex(@"^(\w+)\((.*)\)$", RegexOptions.Singleline);
        private static readonly Dictionary<string, TransformStepType> StepVerbs = new Dictionary<string, TransformStepType>
        {
            { "append", TransformStepType.AppendSyllable },
            { "insert", TransformStepType.InsertSyllable },
            { "replace", TransformStepType.ReplaceSyllable },
            { "remove", TransformStepType.RemoveSyllable },
            { "replaceAll", TransformStepType.ReplaceAllSubstring },
        };

        /// <summary>
        /// The syllable generators resolved from the config, keyed by syllable position.
        /// </summary>
        public Dictionary<SyllablePosition, ISyllableGenerator> SyllableGenerators { get; set; }

        /// <summary>
        /// The name filter resolved from the config's "filters" section.
        /// Null if the config did not specify any filters.
        /// </summary>
        public NameFilter Filter { get; set; }

        /// <summary>
        /// The set of transforms resolved from the config's "transforms" section.
        /// Null if the config did not specify any transforms.
        /// </summary>
        public TransformSet Transform { get; set; }

        /// <summary>
        /// The minimum number of syllables per generated name.
        /// </summary>
        public int MinimumSize { get; set; }

        /// <summary>
        /// The maximum number of syllables per generated name.
        /// </summary>
        public int MaximumSize { get; set; }

        /// <summary>
        /// Instantiates an empty <see cref="NameGeneratorConfig"/> with no syllable generators,
        /// no filter, no transform, and the same default sizing as a new <see cref="NameGenerator"/>.
        /// </summary>
        public NameGeneratorConfig()
        {
            this.SyllableGenerators = new Dictionary<SyllablePosition, ISyllableGenerator>();
            this.MinimumSize = 2;
            this.MaximumSize = 3;
        }

        /// <summary>
        /// Reads the json file at the specified path and parses it into a new <see cref="NameGeneratorConfig"/>.
        /// </summary>
        public static NameGeneratorConfig Load(string path)
        {
            var json = File.ReadAllText(path);
            return Parse(json);
        }

        /// <summary>
        /// Parses the specified json text into a new <see cref="NameGeneratorConfig"/>.
        /// </summary>
        public static NameGeneratorConfig Parse(string json)
        {
            var config = new NameGeneratorConfig();

            using (var doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;

                ParseSyllablePositions(root, config);
                ParseSize(root, config);
                ParseFilters(root, config);
                ParseTransforms(root, config);
            }

            return config;
        }

        private static void ParseSyllablePositions(JsonElement root, NameGeneratorConfig config)
        {
            var raw = new Dictionary<string, JsonElement>();

            foreach (var key in PositionKeys)
            {
                if (root.TryGetProperty(key, out var element))
                {
                    raw[key] = element;
                }
            }

            if (raw.ContainsKey("any"))
            {
                var generator = BuildSyllableGenerator(ResolveTuple("any", raw));
                config.SyllableGenerators[SyllablePosition.Starting] = generator;
                config.SyllableGenerators[SyllablePosition.Inner] = generator;
                config.SyllableGenerators[SyllablePosition.Ending] = generator;
            }

            if (raw.ContainsKey("start"))
            {
                config.SyllableGenerators[SyllablePosition.Starting] = BuildSyllableGenerator(ResolveTuple("start", raw));
            }

            if (raw.ContainsKey("inner"))
            {
                config.SyllableGenerators[SyllablePosition.Inner] = BuildSyllableGenerator(ResolveTuple("inner", raw));
            }

            if (raw.ContainsKey("end"))
            {
                config.SyllableGenerators[SyllablePosition.Ending] = BuildSyllableGenerator(ResolveTuple("end", raw));
            }
        }

        /// <summary>
        /// Resolves the raw slot-array for the specified position key, following
        /// any "$reference" strings until a real array is found. Throws if a
        /// reference points to an undefined key or forms a cycle.
        /// </summary>
        private static List<JsonElement> ResolveTuple(string key, Dictionary<string, JsonElement> raw)
        {
            return ResolveTuple(key, raw, new HashSet<string>());
        }

        private static List<JsonElement> ResolveTuple(string key, Dictionary<string, JsonElement> raw, HashSet<string> visiting)
        {
            if (!visiting.Add(key))
            {
                throw new JsonException($"Circular reference detected involving position '{key}'.");
            }

            if (!raw.TryGetValue(key, out var element))
            {
                throw new JsonException($"Position '${key}' is referenced but not defined.");
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                var value = element.GetString();

                if (value != null && value.StartsWith("$"))
                {
                    return ResolveTuple(value.Substring(1), raw, visiting);
                }

                throw new JsonException($"Position '{key}' has an invalid string value. Expected a '$reference'.");
            }

            if (element.ValueKind == JsonValueKind.Array)
            {
                return element.EnumerateArray().ToList();
            }

            throw new JsonException($"Position '{key}' has an unsupported value type.");
        }

        private static SyllableGenerator BuildSyllableGenerator(List<JsonElement> slots)
        {
            var generator = new SyllableGenerator();
            var positions = new[] { SymbolPosition.First, SymbolPosition.Middle, SymbolPosition.Last };

            for (int i = 0; i < positions.Length && i < slots.Count; i++)
            {
                ApplySlot(generator, positions[i], slots[i]);
            }

            return generator;
        }

        private static void ApplySlot(SyllableGenerator generator, SymbolPosition position, JsonElement slot)
        {
            var tokens = new List<string>();
            double? chance = null;

            if (slot.ValueKind == JsonValueKind.String)
            {
                var value = slot.GetString();

                if (!string.IsNullOrEmpty(value))
                {
                    tokens.Add(value);
                }
            }
            else if (slot.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in slot.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.Number)
                    {
                        if (chance.HasValue)
                        {
                            throw new JsonException("A symbol slot can only specify one chance value.");
                        }

                        chance = element.GetDouble();
                    }
                    else if (element.ValueKind == JsonValueKind.String)
                    {
                        var value = element.GetString();

                        if (!string.IsNullOrEmpty(value))
                        {
                            tokens.Add(value);
                        }
                    }
                    else
                    {
                        throw new JsonException("A symbol slot may only contain strings and at most one chance value.");
                    }
                }
            }
            else
            {
                throw new JsonException("A symbol slot must be a string or an array.");
            }

            if (tokens.Count > 0)
            {
                var symbols = new SymbolGenerator();

                foreach (var token in tokens)
                {
                    ApplyToken(symbols, token);
                }

                generator.Add(position, symbols);
            }

            if (chance.HasValue)
            {
                generator.SetChance(position, chance.Value);
            }
        }


        /// <summary>
        /// Parses a single symbol token, which may be a run of plain letters
        /// (each exploded into its own symbol), an atomic "(cluster)", and
        /// may optionally carry a trailing "*N" weight.
        /// </summary>
        private static void ApplyToken(SymbolGenerator symbols, string token)
        {
            var body = token;
            int? weight = null;

            var match = WeightSuffix.Match(token);

            if (match.Success)
            {
                body = match.Groups[1].Value;
                weight = int.Parse(match.Groups[2].Value);
            }

            if (body.Length >= 2 && body.StartsWith("(") && body.EndsWith(")"))
            {
                symbols.Cluster(body.Substring(1, body.Length - 2));
            }
            else
            {
                symbols.Add(body);
            }

            if (weight.HasValue)
            {
                symbols.Weight(weight.Value);
            }
        }

        private static void ParseSize(JsonElement root, NameGeneratorConfig config)
        {
            if (!root.TryGetProperty("size", out var element))
            {
                return;
            }

            if (element.ValueKind == JsonValueKind.Number)
            {
                var size = element.GetInt32();
                config.MinimumSize = size;
                config.MaximumSize = size;
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                var values = element.EnumerateArray().Select(e => e.GetInt32()).ToList();

                if (values.Count == 0)
                {
                    throw new JsonException("The 'size' array must contain at least one value.");
                }

                config.MinimumSize = values[0];
                config.MaximumSize = values.Count > 1 ? values[1] : values[0];
            }
            else
            {
                throw new JsonException("The 'size' property must be a number or an array.");
            }
        }

        private static void ParseFilters(JsonElement root, NameGeneratorConfig config)
        {
            if (!root.TryGetProperty("filters", out var element) || element.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            var filter = new NameFilter();

            foreach (var item in element.EnumerateArray())
            {
                filter.Add(ParseFilterConstraint(item.GetString()));
            }

            config.Filter = filter;
        }

        /// <summary>
        /// Parses a single filter string. Supported forms:
        /// "/regex/" (MatchesPattern), "*text*" (Contains),
        /// "*text" (EndsWith), "text*" (StartsWith), or a bare
        /// literal (treated as Contains).
        /// </summary>
        private static FilterConstraint ParseFilterConstraint(string pattern)
        {
            if (pattern.Length >= 2 && pattern.StartsWith("/") && pattern.EndsWith("/"))
            {
                return new FilterConstraint(FilterCondition.MatchesPattern, pattern.Substring(1, pattern.Length - 2));
            }

            var leading = pattern.StartsWith("*");
            var trailing = pattern.EndsWith("*");

            if (leading && trailing && pattern.Length >= 2)
            {
                return new FilterConstraint(FilterCondition.Contains, pattern.Substring(1, pattern.Length - 2));
            }

            if (leading)
            {
                return new FilterConstraint(FilterCondition.EndsWith, pattern.Substring(1));
            }

            if (trailing)
            {
                return new FilterConstraint(FilterCondition.StartsWith, pattern.Substring(0, pattern.Length - 1));
            }

            return new FilterConstraint(FilterCondition.Contains, pattern);
        }


        private static void ParseTransforms(JsonElement root, NameGeneratorConfig config)
        {
            if (!root.TryGetProperty("transforms", out var element) || element.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            // Transforms are applied in "apply all eligible" mode: every transform
            // whose own condition and chance pass is applied, in the order they
            // were added. UseRandomSelection is intentionally left false.
            var set = new TransformSet();

            foreach (var item in element.EnumerateArray())
            {
                set.Add(ParseTransform(item));
            }

            config.Transform = set;
        }

        private static Transform ParseTransform(JsonElement item)
        {
            var transform = new Transform();

            if (item.ValueKind == JsonValueKind.String)
            {
                ApplyStep(transform, item.GetString());
                return transform;
            }

            if (item.ValueKind != JsonValueKind.Array)
            {
                throw new JsonException("A transform must be a string or an array.");
            }

            bool chanceSet = false;

            foreach (var element in item.EnumerateArray())
            {
                if (element.ValueKind == JsonValueKind.Number)
                {
                    if (chanceSet)
                    {
                        throw new JsonException("A transform can only specify one chance value.");
                    }

                    transform.Chance = element.GetDouble();
                    chanceSet = true;
                }
                else if (element.ValueKind == JsonValueKind.String)
                {
                    var text = element.GetString();

                    if (text.StartsWith("?"))
                    {
                        ApplyCondition(transform, text);
                    }
                    else
                    {
                        ApplyStep(transform, text);
                    }
                }
                else
                {
                    throw new JsonException("A transform array may only contain strings and at most one chance value.");
                }
            }

            return transform;
        }

        /// <summary>
        /// Parses a "?&lt;index&gt;~&lt;regex&gt;" condition string, eg. "?0~[aeiou]$".
        /// </summary>
        private static void ApplyCondition(Transform transform, string text)
        {
            var body = text.Substring(1);
            var separatorIndex = body.IndexOf('~');

            if (separatorIndex < 0)
            {
                throw new JsonException($"Invalid transform condition: '{text}'. Expected format '?<index>~<regex>'.");
            }

            var indexText = body.Substring(0, separatorIndex);
            var pattern = body.Substring(separatorIndex + 1);

            transform.ConditionalIndex = int.Parse(indexText);
            transform.ConditionalRegex = pattern;
        }

        /// <summary>
        /// Parses a "verb(args)" step string, eg. "replace(0,neo)" or "append(tar)".
        /// Arguments are split on unescaped commas; "\,", "\(", "\)", and "\\" are
        /// treated as escaped literals within an argument.
        /// </summary>
        private static void ApplyStep(Transform transform, string text)
        {
            var match = StepPattern.Match(text);

            if (!match.Success)
            {
                throw new JsonException($"Invalid transform step: '{text}'.");
            }

            var verb = match.Groups[1].Value;
            var args = SplitStepArguments(match.Groups[2].Value);

            if (!StepVerbs.TryGetValue(verb, out var stepType))
            {
                throw new JsonException($"Unknown transform step verb: '{verb}'.");
            }

            transform.AddStep(new TransformStep(stepType, args.ToArray()));
        }

        private static List<string> SplitStepArguments(string raw)
        {
            var args = new List<string>();
            var current = new StringBuilder();

            for (int i = 0; i < raw.Length; i++)
            {
                var c = raw[i];

                if (c == '\\' && i + 1 < raw.Length && "(),\\".IndexOf(raw[i + 1]) >= 0)
                {
                    current.Append(raw[i + 1]);
                    i++;
                }
                else if (c == ',')
                {
                    args.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            args.Add(current.ToString());

            return args;
        }
    }
}
