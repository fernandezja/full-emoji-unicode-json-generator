using Moq;
using UnicodeEmojiParserToJson;
using Xunit;

namespace UnicodeEmojiParserToJsonTest;

public class UnicodeEmojiParserTest
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<Serilog.ILogger> _loggerMock;

    public UnicodeEmojiParserTest()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<Serilog.ILogger>();
    }

    [Fact]
    public async Task ParserRunFromStaticContentSimple()
    {
        var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object,
                                             _loggerMock.Object);

        var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");
        var htmlContent = File.ReadAllText(htmlContentPath);

        await parser.ParserRunFromStaticContent(htmlContent);

        Assert.True(parser.RowsCount > 0);
        Assert.Equal(6, parser.RowsCount);
        Assert.Equal(3, parser.Emojis.Count);
        Assert.Equal("😀", parser.Emojis[0].Chars);
        Assert.Equal("😃", parser.Emojis[1].Chars);
        Assert.Equal("😄", parser.Emojis[2].Chars);
    }

    [Fact]
    public async Task ToJson()
    {
        var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object,
                                             _loggerMock.Object);

        var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");
        var htmlContent = File.ReadAllText(htmlContentPath);

        await parser.ParserRunFromStaticContent(htmlContent);

        var jsonData = parser.ToJson();

        Assert.NotNull(jsonData);
    }

    [Fact]
    public async Task ToJsonWriteToFile()
    {
        var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object,
                                             _loggerMock.Object);

        var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");
        var jsonPath = TestHelper.ToApplicationPath("resources/emoji.json");
        var htmlContent = File.ReadAllText(htmlContentPath);

        await parser.ParserRunFromStaticContent(htmlContent);

        parser.ToJsonWriteToFile(jsonPath);

        Assert.Equal(6, parser.RowsCount);
        Assert.Equal(3, parser.Emojis.Count);
    }

    [Theory]
    [InlineData("unicode-org-emoji-charts-full-emoji-list-v14-0.html", 2109, 1853)]
    [InlineData("unicode-org-emoji-charts-full-emoji-list-v15-1.html", 2162, 1902)]
    public async Task ParserRunFromStaticContentFull(string resourceHtmlPageName,
                                                     int expectedRowsCount,
                                                     int expectedEmojiCount)
    {
        var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object,
                                             _loggerMock.Object);

        var htmlContentPath = TestHelper.ToApplicationPath($"resources/{resourceHtmlPageName}");
        var htmlContent = await File.ReadAllTextAsync(htmlContentPath);

        await parser.ParserRunFromStaticContent(htmlContent);

        Assert.Equal(expectedRowsCount, parser.RowsCount);
        Assert.Equal(expectedEmojiCount, parser.Emojis.Count);
    }
}
