using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace UnicodeEmojiParserToJson
{
    public class Emoji
    {
        [JsonPropertyName("chars")]
        [JsonConverter(typeof(EmojiCharsConverter))]
        public string Chars { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("subgroup")]
        public string Subgroup { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("shortname")]
        public string Shortname { get; set; }
    }
}
