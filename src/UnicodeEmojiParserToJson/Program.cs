using AngleSharp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnicodeEmojiParserToJson
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("UnicodeEmojiParserToJson!");

            var services = new ServiceCollection();

            services.AddHttpClient();

            services.AddScoped<UnicodeEmojiParser>();

            var serviceProvider = services.BuildServiceProvider();


            await ParserRun(serviceProvider);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task ParserRun(ServiceProvider serviceProvider)
        {
            var parser = serviceProvider.GetService<UnicodeEmojiParser>();

            await parser.ParserRunFromAddress();

            var jsonData = parser.ToJson();

            parser.ToJsonWriteToFile("emojis.json");
        }
    }


}
