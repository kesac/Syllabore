using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Syllabore.Json
{
    /// <summary>
    /// A type of <see cref="JsonConverter{T}"/> used to 
    /// ensure properties with an interface type are serialized
    /// as the concrete class type instead.
    /// <para>
    /// This converter is used by <see cref="NameGeneratorSerializer"/>
    /// internally.
    /// </para>
    /// </summary>
    public class InterfaceConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // NameGeneratorSerializer does not use this when reading JSON
            throw new InvalidCastException(nameof(InterfaceConverter<T>) + " can only be used when writing JSON.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Calling GetType() on value will return the concrete type
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

}
