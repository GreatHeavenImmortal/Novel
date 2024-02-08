using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TTS.Interfaces;

namespace TTS.Models
{
    public class ParserService : IParserService
    {
        internal IModelOption _options;
        internal HttpClient _httpClient;
        internal ConfigurationParserService _configParserService;
        public ParserService(IHttpClientFactory httpClientFactory, ConfigurationParserService configParserService)
        {
            _httpClient = httpClientFactory.CreateClient("defaultClient");
            _configParserService = configParserService;
        }

        public Task<string> GetBookNameAsync(HtmlNode document)
        {
            string bookName = string.Empty;
            if (_options.IsBookNameInUrl)
                bookName = document.QuerySelector(_options.BookName)?.Attributes[_options.BookNameUrlAttribute]?.Value ?? string.Empty;
            else
                bookName = document.QuerySelector(_options.BookName).InnerText;

            return Task.FromResult(bookName);
        }
        public Task<string> GetChapterTitleAsync(HtmlNode document)
        {
            string result = document.QuerySelector(_options.ChapterTitle)?.InnerText ?? string.Empty;
            return Task.FromResult(result);
        }
        public Task<string> GetContentAsync(HtmlNode document)
        {
            var result = new StringBuilder();
            var contents = document.QuerySelectorAll(_options.Content)?.ToList() ?? new List<HtmlNode>();           
            if(contents.Any())
                contents.ForEach(content => result.Append(content.InnerText));
            return Task.FromResult(result.ToString());
        }
        public Task<string> GetNextChapterUrlAsync(HtmlNode document)
        {
            string result = document.QuerySelector(_options.NextChapterUrl)?.Attributes["href"].Value ?? string.Empty;
            return Task.FromResult(result);
        }
        public async Task<ParserInfo> ParseAsync(string url)
        {
            _options = _configParserService.ConfigureOptions(url);
            if(_options is null)
            {
                await Console.Out.WriteLineAsync("no parser for website url");
                return null;
            }
            var result = new ParserInfo();
            
            string html = await _httpClient.GetStringAsync(url);

            if (string.IsNullOrEmpty(html))
                throw new Exception($"No data at url: {url}");

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            result.CreatedAt = DateTimeOffset.Now;
            result.ChapterUrl = url;
            result.BookName = await GetBookNameAsync(document.DocumentNode);
            result.ChapterTitle = await GetChapterTitleAsync(document.DocumentNode);
            result.Content = await GetContentAsync(document.DocumentNode);
            result.NextChapterUrl = await GetNextChapterUrlAsync(document.DocumentNode);

            return result;
        }
    }
}
