using Json.Schema;
using System.Text.Json;

namespace UnicodeEmojiParserToJson;

public class UnicodeEmojiValidator
{
    private readonly EvaluationOptions _evaluationOptions = new();
    private static readonly Lazy<JsonSchema> _schema = new(() =>
        JsonSchema.FromFile(Path.GetFullPath(@"Resources/emoji-json-scheme.json")));

    public bool IsValid(string emojiDataJson)
    {
        var dataJson = JsonDocument.Parse(emojiDataJson).RootElement;
        var result = _schema.Value.Evaluate(dataJson, _evaluationOptions);
        return result.IsValid;
    }
}
