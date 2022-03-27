using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnicodeEmojiParserToJson;
using Xunit;

namespace UnicodeEmojiParserToJsonTest
{
    public class UnicodeEmojiValidatorTests
    {

        [Fact]
        public async Task IsValid_True_When_DataJsonIsValid()
        {
            var validator = new UnicodeEmojiValidator();

            var dataJsonPath = TestHelper.ToApplicationPath("resources/emoji.json");

            var dataJson = await File.ReadAllTextAsync(dataJsonPath);

            var result = validator.IsValid(dataJson);

            Assert.True(result);
        }

        [Fact]
        public async Task IsValid_False_When_DataJsonIsEmtpy()
        {
            var validator = new UnicodeEmojiValidator();

            var result = validator.IsValid("{}");

            Assert.False(result);
        }


        [Fact]
        public async Task IsValid_False_When_DataJsonIsNotValid()
        {
            var validator = new UnicodeEmojiValidator();

            var dataJsonPath = TestHelper.ToApplicationPath("resources/emoji-invalid.json");

            var dataJson = await File.ReadAllTextAsync(dataJsonPath);

            var result = validator.IsValid(dataJson);

            Assert.False(result);
        }
    }
}
