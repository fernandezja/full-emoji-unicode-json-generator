using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UnicodeEmojiParserToJson;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Console.WriteLine("UnicodeEmojiParserToJson!");

var services = new ServiceCollection();
services.AddHttpClient();
services.AddSingleton<ILogger>(logger);
services.AddScoped<UnicodeEmojiParser>();

var serviceProvider = services.BuildServiceProvider();

await ParserRun(serviceProvider);

static async Task ParserRun(ServiceProvider serviceProvider)
{
    var logger = serviceProvider.GetRequiredService<ILogger>();
    var parser = serviceProvider.GetRequiredService<UnicodeEmojiParser>();

    logger.Information("Step 1: Parser Init");

    await parser.ParserRunFromAddress();

    parser.ToJsonWriteToFile("emojis.json");
}
