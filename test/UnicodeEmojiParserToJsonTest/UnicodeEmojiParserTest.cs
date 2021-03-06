using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnicodeEmojiParserToJson;
using Xunit;

namespace UnicodeEmojiParserToJsonTest
{
    public class UnicodeEmojiParserTest
    {

        [Fact]
        public void ParserRunFromStaticContentSimple()
        {
            var parser = new UnicodeEmojiParser();

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
            var parser = new UnicodeEmojiParser();

            var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");

            var htmlContent = File.ReadAllText(htmlContentPath);

            parser.ParserRunFromStaticContent(htmlContent).ConfigureAwait(false);

            var jsonData = parser.ToJson();

            Assert.NotNull(jsonData);
        }


        //[Fact]
        //public async Task ToJsonDemo()
        //{
        //    var parser = new UnicodeEmojiParser();

        //    var jsonData = parser.ToJsonDemo();


        //    Assert.NotNull(jsonData);
        //}


        [Fact]
        public void ToJsonWriteToFile()
        {
            var parser = new UnicodeEmojiParser();

            var htmlContentPath = TestHelper.ToApplicationPath("resources/simple.html");
            var jsonPath = TestHelper.ToApplicationPath("resources/emoji.json");

            var htmlContent = File.ReadAllText(htmlContentPath);

            parser.ParserRunFromStaticContent(htmlContent).ConfigureAwait(false);

            parser.ToJsonWriteToFile(jsonPath);

            Assert.Equal(6, parser.RowsCount);
            Assert.Equal(3, parser.Emojis.Count);
        }


        //[Fact]
        //public async Task ParserRunFromStaticContentFull()
        //{
        //    var parser = new UnicodeEmojiParser();

        //    var htmlContentPath = TestHelper.ToApplicationPath("resources/unicode-org-emoji-charts-full-emoji-list-13.0.html");

        //    var htmlContent = await File.ReadAllTextAsync(htmlContentPath);

        //    await parser.ParserRunFromStaticContent(htmlContent);

        //    Assert.True(parser.RowsCount > 0);
        //}


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
