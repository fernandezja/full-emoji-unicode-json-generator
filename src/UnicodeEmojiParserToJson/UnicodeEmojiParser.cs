using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SerilogTimings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace UnicodeEmojiParserToJson
{
    public class UnicodeEmojiParser
    {
        private const string UNICODE_ORG_EMOJI_FULL_LIST = "https://unicode.org/emoji/charts/full-emoji-list.html";

        public int RowsCount { get; set; }
        public List<Emoji> Emojis { get; set; }

        private IHttpClientFactory _httpClientFactory;
        private Serilog.ILogger _logger;


        public UnicodeEmojiParser(IHttpClientFactory httpClientFactory,
                                  Serilog.ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;                               

        }


        public async Task ParserRunFromAddress() {
            //await ParserRunFromAddressGetWithAngleSharpBrowsingContextOpen(UNICODE_ORG_EMOJI_FULL_LIST);
            await ParserRunFromAddressGetWithHttpClient(UNICODE_ORG_EMOJI_FULL_LIST);
        }


        public async Task ParserRunFromAddressGetWithAngleSharpBrowsingContextOpen(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();

            Console.WriteLine($"Get Html... ");

            IBrowsingContext browserContext = BrowsingContext.New(config);

            var url = new Url(address);

            IDocument document;

            using (Operation.Time($"Get HTML from {address}", address))
            {
                document = await browserContext.OpenAsync(url: url);
            }
           

            ParseDocument(document);

        }

        public async Task ParserRunFromAddressGetWithHttpClient(string address)
        {
            var config = Configuration.Default.WithDefaultLoader();

            Console.WriteLine($"Get Html... ");

            IBrowsingContext browserContext = BrowsingContext.New(config);

            IDocument document;
            string htmlContent;

            using (Operation.Time($"Get HTML from {address}", address))
            {
                htmlContent = await GetContentFromUrl(url: address,
                                                      timeOut: Timeout.InfiniteTimeSpan);
            }

            using (Operation.Time($"Document parse with AngleSharp", address))
            {
                document = await browserContext.OpenAsync(req => req.Content(htmlContent));
            }


            ParseDocument(document);

        }



        public async Task ParserRunFromStaticContent(string htmlContent)
        {
            var config = Configuration.Default.WithDefaultLoader();

            Console.WriteLine($"Get Html... ");

            IBrowsingContext browserContext = BrowsingContext.New(config);

            IDocument document = await browserContext.OpenAsync(req => req.Content(htmlContent));


            ParseDocument(document);
        }

       
        private void ParseDocument(IDocument document) {
            //TODO: Verify version, search into document
            ParseDocumentV15(document);
        }


        private void ParseDocumentV15(IDocument document)
        {
            Console.WriteLine($"document.Body.TextContent.Length = {document.Body.TextContent.Length}");

            var tableRowsSelector = "table tbody tr";

            var rows = document.QuerySelectorAll(tableRowsSelector);

            RowsCount = rows.Count();

            Console.WriteLine($"rows.count = {rows.Count()}");

            var group = string.Empty;
            var subGroup = string.Empty;

            Emojis = new List<Emoji>();

            foreach (var r in rows)
            {
                var className = r.FirstElementChild.ClassName;
                var textFirstElement = r.FirstElementChild.TextContent;

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
                            var chars = r.QuerySelector("td.chars").TextContent.Trim();
                            var number = r.QuerySelector("td.rchars").TextContent.Trim();
                            var code = r.QuerySelector("td.code").TextContent.Trim();
                            var shortname = r.QuerySelector("td.name").TextContent.Trim();

                            var emoji = new Emoji()
                            {
                                Group = group,
                                Subgroup = subGroup,
                                Chars = chars,
                                Code = code,
                                Number = number,
                                Shortname = shortname
                            };

                            Emojis.Add(emoji);
                        }
                        break;
                }

            }

        }



        private void ParseDocumentV14(IDocument document)
        {
            Console.WriteLine($"document.Body.TextContent.Length = {document.Body.TextContent.Length}");

            var tableRowsSelector = ".main table tbody tr";

            var rows = document.QuerySelectorAll(tableRowsSelector);

            RowsCount = rows.Count();

            Console.WriteLine($"rows.count = {rows.Count()}");

            var group = string.Empty;
            var subGroup = string.Empty;

            Emojis = new List<Emoji>();

            foreach (var r in rows)
            {
                var className = r.FirstElementChild.ClassName;
                var textFirstElement = r.FirstElementChild.TextContent;

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
                            var chars = r.QuerySelector("td.chars").TextContent.Trim();
                            var number = r.QuerySelector("td.rchars").TextContent.Trim();
                            var code = r.QuerySelector("td.code").TextContent.Trim();
                            var shortname = r.QuerySelector("td.name").TextContent.Trim();

                            var emoji = new Emoji()
                            {
                                Group = group,
                                Subgroup = subGroup,
                                Chars = chars,
                                Code = code,
                                Number = number,
                                Shortname = shortname
                            };

                            Emojis.Add(emoji);
                        }
                        break;
                }

            }

        }


        public string ToJson() {

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(Emojis, settings: settings);
        }


        //public string ToJsonDemo()
        //{


        //    var emojiDemo = new { emoji = "😀" };

        //    var stringJson = JsonConvert.SerializeObject(emojiDemo);


        //    return stringJson;

        //}



        public void ToJsonWriteToFile(string filePath)
        {
            File.WriteAllText(filePath, ToJson());
        }

        public async Task<string> GetContentFromUrl(string url,
                                                    TimeSpan timeOut)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _httpClientFactory.CreateClient();
            client.Timeout = timeOut;

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                _logger.Error($"[GetContentFromUrl] {response.StatusCode} {response.RequestMessage}");
                return string.Empty;
            }
        }

        public async Task<string> GetContentFromUrl(string url)
        {
            return await GetContentFromUrl(url, new TimeSpan(0, 0, seconds: 100));
        }
    }

}
