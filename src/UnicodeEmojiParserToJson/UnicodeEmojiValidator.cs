using Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UnicodeEmojiParserToJson
{
    public class UnicodeEmojiValidator
    {
        private EvaluationOptions _evaluationOptions = new EvaluationOptions()
        {
            EvaluateAs = SpecVersion.Draft7
            //OutputFormat = OutputFormat.List,
            //ValidateAgainstMetaSchema = false
        };



        public bool IsValid(string emojiDataJson) {

            var jsonSchema = JsonSchema.FromFile(@"Resources/emoji-json-scheme.json");

            var dataJson = JsonDocument.Parse(emojiDataJson).RootElement;

            var result = jsonSchema.Evaluate(dataJson,
                                             _evaluationOptions);

            return result.IsValid;

        }
            

    }
}
