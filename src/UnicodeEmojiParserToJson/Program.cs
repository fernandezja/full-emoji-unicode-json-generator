using AngleSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnicodeEmojiParserToJson
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("UnicodeEmojiParserToJson!");
            
            await ParserRun();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task ParserRun()
        {
            var parser = new UnicodeEmojiParser();
            await parser.ParserRunFromAddress();

            var jsonData = parser.ToJson();

            parser.ToJsonWriteToFile("emojis.json");
        }
    }


}
