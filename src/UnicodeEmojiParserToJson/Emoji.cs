using System.Text.Json.Serialization;

namespace UnicodeEmojiParserToJson;

public class Emoji
{
    [JsonPropertyName("chars")]
    [JsonConverter(typeof(EmojiCharsConverter))]
    public string Chars { get; set; } = string.Empty;

    [JsonPropertyName("group")]
    public string Group { get; set; } = string.Empty;

    [JsonPropertyName("subgroup")]
    public string Subgroup { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("shortname")]
    public string Shortname { get; set; } = string.Empty;
}
