using System.Reflection;

namespace UnicodeEmojiParserToJsonTest;

public static class TestHelper
{
    public static string ToApplicationPath(string fileName)
    {
        var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var binSeparator = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";
        var binIndex = exePath.LastIndexOf(binSeparator, StringComparison.Ordinal);
        var appRoot = binIndex >= 0 ? exePath[..binIndex] : exePath;
        return Path.Combine(appRoot, fileName);
    }
}
