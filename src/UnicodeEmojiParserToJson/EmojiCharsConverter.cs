using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnicodeEmojiParserToJson;

public class EmojiCharsConverter : JsonConverter<string>
{
    public override string Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        reader.GetString() ?? string.Empty;

    public override void Write(
        Utf8JsonWriter writer,
        string value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}