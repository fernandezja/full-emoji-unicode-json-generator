using AngleSharp;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
            var logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();

            Console.WriteLine("UnicodeEmojiParserToJson!");

            var services = new ServiceCollection();

            services.AddHttpClient();

            services.AddSingleton<Serilog.ILogger>(logger);
            services.AddScoped<UnicodeEmojiParser>();

            var serviceProvider = services.BuildServiceProvider();


            await ParserRun(serviceProvider);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task ParserRun(ServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<Serilog.ILogger>();
            var parser = serviceProvider.GetService<UnicodeEmojiParser>();

            logger.Information("Step 1: Parser Init");

            await parser.ParserRunFromAddress();

            var jsonData = parser.ToJson();

            parser.ToJsonWriteToFile("emojis.json");
        }
    }


}
