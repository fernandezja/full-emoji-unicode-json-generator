using Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnicodeEmojiParserToJson
{
    public class UnicodeEmojiValidator
    {
        public ValidationResults ValidationResults { get; private set; }

        public bool IsValid(string emojiDataJson) {

            var jsonSchema = JsonSchema.FromFile(@"Resources/emoji-json-scheme.json");

            var dataJson = JsonDocument.Parse(emojiDataJson).RootElement;

            ValidationResults = jsonSchema.Validate(dataJson);

            return ValidationResults.IsValid;

        }
            

    }
}
