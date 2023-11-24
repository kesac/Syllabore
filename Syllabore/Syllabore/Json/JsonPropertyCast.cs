using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syllabore.Json
{
    /// <summary>
    /// A special kind of <see cref="JsonConverter{Transform}"/> 
    /// that's only role is to cast the value of a property to a different <see cref="Type"/>.
    /// </summary>
    public class JsonPropertyCast<T> : JsonConverter<T>
    {
        private Type TargetType;

        /// <summary>
        /// Instantiates a new <see cref="JsonPropertyCast{T}"/> that will
        /// cast properties to the specified <see cref="Type"/>.
        /// </summary>
        public JsonPropertyCast(Type targetType)
        {
            this.TargetType = targetType;
        }
        
        /// <summary>
        /// Override. Reads the value of a property and casts it to the specified <see cref="Type"/>.
        /// </summary>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (T)JsonSerializer.Deserialize(ref reader, this.TargetType, options);
        }

        /// <summary>
        /// Override. Writes the value of a property and casts it to the specified <see cref="Type"/>.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, this.TargetType, options);
        }
    }
}
