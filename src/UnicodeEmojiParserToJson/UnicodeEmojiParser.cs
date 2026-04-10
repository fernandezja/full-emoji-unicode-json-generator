using AngleSharp;
using AngleSharp.Dom;
using SerilogTimings;
using System.Text.Json;

namespace UnicodeEmojiParserToJson;

public class UnicodeEmojiParser
{
    private const string UNICODE_ORG_EMOJI_FULL_LIST = "https://unicode.org/emoji/charts/full-emoji-list.html";

    public int RowsCount { get; set; }
    public List<Emoji> Emojis { get; set; } = [];

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Serilog.ILogger _logger;

    public UnicodeEmojiParser(IHttpClientFactory httpClientFactory, Serilog.ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task ParserRunFromAddress() =>
        await ParserRunFromAddressGetWithHttpClient(UNICODE_ORG_EMOJI_FULL_LIST);

    public async Task ParserRunFromAddressGetWithAngleSharpBrowsingContextOpen(string address)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var browserContext = BrowsingContext.New(config);

        Console.WriteLine("Get Html... ");

        IDocument document;
        using (Operation.Time($"Get HTML from {address}", address))
        {
            document = await browserContext.OpenAsync(new Url(address));
        }

        ParseDocument(document);
    }

    public async Task ParserRunFromAddressGetWithHttpClient(string address)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var browserContext = BrowsingContext.New(config);

        Console.WriteLine("Get Html... ");

        string htmlContent;
        using (Operation.Time($"Get HTML from {address}", address))
        {
            htmlContent = await GetContentFromUrl(url: address, timeOut: Timeout.InfiniteTimeSpan);
        }

        IDocument document;
        using (Operation.Time("Document parse with AngleSharp", address))
        {
            document = await browserContext.OpenAsync(req => req.Content(htmlContent));
        }

        ParseDocument(document);
    }

    public async Task ParserRunFromStaticContent(string htmlContent)
    {
        var config = Configuration.Default.WithDefaultLoader();
        var browserContext = BrowsingContext.New(config);

        Console.WriteLine("Get Html... ");

        var document = await browserContext.OpenAsync(req => req.Content(htmlContent));

        ParseDocument(document);
    }

    private void ParseDocument(IDocument document) =>
        ParseDocumentV15(document);

    private void ParseDocumentV15(IDocument document)
    {
        Console.WriteLine($"document.Body.TextContent.Length = {document.Body!.TextContent.Length}");

        var rows = document.QuerySelectorAll("table tbody tr");
        RowsCount = rows.Length;

        Console.WriteLine($"rows.count = {RowsCount}");

        var group = string.Empty;
        var subGroup = string.Empty;
        Emojis = [];

        foreach (var r in rows)
        {
            var className = r.FirstElementChild?.ClassName;
            var textFirstElement = r.FirstElementChild?.TextContent;

            switch (className)
            {
                case "bighead":
                    group = r.TextContent.Trim();
                    break;
                case "mediumhead":
                    subGroup = r.TextContent.Trim();
                    break;
                case "rchars":
                    if (textFirstElement != "№")
                    {
                        var chars = r.QuerySelector("td.chars")?.TextContent.Trim() ?? string.Empty;
                        var number = r.QuerySelector("td.rchars")?.TextContent.Trim() ?? string.Empty;
                        var code = r.QuerySelector("td.code")?.TextContent.Trim() ?? string.Empty;
                        var shortname = r.QuerySelector("td.name")?.TextContent.Trim() ?? string.Empty;

                        Emojis.Add(new Emoji
                        {
                            Group = group,
                            Subgroup = subGroup,
                            Chars = chars,
                            Code = code,
                            Number = number,
                            Shortname = shortname
                        });
                    }
                    break;
            }
        }
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        return JsonSerializer.Serialize(Emojis, options);
    }

    public void ToJsonWriteToFile(string filePath) =>
        File.WriteAllText(filePath, ToJson());

    public async Task<string> GetContentFromUrl(string url, TimeSpan timeOut)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = timeOut;

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        _logger.Error("[GetContentFromUrl] {StatusCode} {RequestMessage}", response.StatusCode, response.RequestMessage);
        return string.Empty;
    }

    public Task<string> GetContentFromUrl(string url) =>
        GetContentFromUrl(url, TimeSpan.FromSeconds(100));
}
