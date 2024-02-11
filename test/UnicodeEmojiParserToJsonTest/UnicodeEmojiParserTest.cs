using Moq;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnicodeEmojiParserToJson;
using Xunit;

namespace UnicodeEmojiParserToJsonTest
{
    public class UnicodeEmojiParserTest
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<Serilog.ILogger> _loggerMock;

        public UnicodeEmojiParserTest()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<Serilog.ILogger>();
        }


        [Fact]
        public void ParserRunFromStaticContentSimple()
        {
            var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object, 
                                                _loggerMock.Object);

            var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");

            var htmlContent = File.ReadAllText(htmlContentPath);

            parser.ParserRunFromStaticContent(htmlContent).ConfigureAwait(false);

            Assert.True(parser.RowsCount > 0);
            Assert.Equal(6, parser.RowsCount);
            Assert.Equal(3, parser.Emojis.Count);
            Assert.Equal("😀", parser.Emojis[0].Chars);
            Assert.Equal("😃", parser.Emojis[1].Chars);
            Assert.Equal("😄", parser.Emojis[2].Chars);
        }


        [Fact]
        public void ToJson()
        {
            var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object,
                                                _loggerMock.Object);

            var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");

            var htmlContent = File.ReadAllText(htmlContentPath);

            parser.ParserRunFromStaticContent(htmlContent).ConfigureAwait(false);

            var jsonData = parser.ToJson();

            Assert.NotNull(jsonData);
        }


        //[Fact]
        //public async Task ToJsonDemo()
        //{
        //    var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object);

        //    var jsonData = parser.ToJsonDemo();


        //    Assert.NotNull(jsonData);
        //}


        [Fact]
        public void ToJsonWriteToFile()
        {
            var parser = new UnicodeEmojiParser(_httpClientFactoryMock.Object,
                                                _loggerMock.Object);

            var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");
            var jsonPath = TestHelper.ToApplicationPath("resources/emoji.json");

            var htmlContent = File.ReadAllText(htmlContentPath);

            parser.ParserRunFromStaticContent(htmlContent).ConfigureAwait(false);

            parser.ToJsonWriteToFile(jsonPath);

            Assert.Equal(6, parser.RowsCount);
            Assert.Equal(3, parser.Emojis.Count);
        }


        [Theory]
        //[InlineData("unicode-org-emoji-charts-full-emoji-list-v13-0.html", 0, 0)]
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


            parser.ToJsonWriteToFile(@"D:\dev\fernandezja\full-emoji-unicode-json-generator\src\UnicodeEmojiParserToJson\bin\Debug\net8.0\emojis.json");
        }


        //[Fact]
        //public async Task ParserRunFromAddressLight()
        //{
        //    var parser = new UnicodeEmojiParser();

        //    await parser.ParserRunFromAddress();

        //    Assert.True(parser.RowsCount > 0);
        //}

        //[Fact]
        //public async Task ParserRunFromAddressFull()
        //{
        //    var parser = new UnicodeEmojiParser();

        //    await parser.ParserRunFromAddress();

        //    Assert.True(parser.RowsCount > 0);
        //    Assert.Equal(2064, parser.RowsCount);
        //    Assert.Equal(1809, parser.Emojis.Count);
        //    Assert.Equal("😀", parser.Emojis[0].Chars);
        //    Assert.Equal("😃", parser.Emojis[1].Chars);
        //    Assert.Equal("😄", parser.Emojis[2].Chars);
        //}



    }
}
