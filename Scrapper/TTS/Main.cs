using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Transactions;
using TTS.Interfaces;
using TTS.Models;

namespace TTS
{
    public class Main
    {
        IParserService _parserService;
        ILogger<Main> _logger = null;
        IModelOption _options = null;
        ConfigurationParserService _configParserService;
        TextFunctionsService _textFunctionService;
        public Main(IParserService parserService, ILogger<Main> logger, ConfigurationParserService configParserService, TextFunctionsService textFunctionService)
        {
            _parserService = parserService;
            _configParserService = configParserService;
            _textFunctionService = textFunctionService;
        }
        public async Task StartAsync(string[] args = null)
        {
            string url = "https://novelbin.net/n/wizards-begin-liver-experience-with-knight-breathing-nov-376981065/ccchapter-125-ninth-order-black-snake-gaseous-black-scales-1-update-5000-words";
            _options = _configParserService.ConfigureOptions(url);
            var result = await _parserService.ParseAsync(url);
            result = _textFunctionService.ClearParserInfo(result, _options);
            await Console.Out.WriteLineAsync("-".PadRight(Console.BufferWidth - 1, '-'));
            await Console.Out.WriteLineAsync(result.BookName);
            await Console.Out.WriteLineAsync(result.ChapterTitle);
            await Console.Out.WriteLineAsync(result.ChapterUrl);
            await Console.Out.WriteLineAsync(result.CreatedAt.ToString("yyyy/MM/dd"));
            await Console.Out.WriteLineAsync("-".PadRight(Console.BufferWidth - 1, '-'));

            await Console.Out.WriteLineAsync(result.Content);
            await Console.Out.WriteLineAsync(result.NextChapterUrl);
            //_logger.LogInformation(result.Content);
            Debug.Print("yolo");
        }
    }



    public class TextFunctionsService
    {
        internal readonly string doubleSpace = "  ";
        internal readonly string space = " ";

        public string TrimText(string text)
        {
            text = text.Trim();
            var items = new string[] { "\'", "\t", "\n", ":", "(", ")" };
            foreach (string item in items)
                text = text.Replace(item, space);
            text = text.Replace(doubleSpace, space);
            text = text.Replace(space,"_");
            return text;
        }
        public string ShortenText(string text, string seprataor = " ")
        {
            string shortText = string.Empty;
            var items = text.Split(seprataor);

            foreach (string item in items)
                shortText = $"{shortText}{item[0].ToString().ToUpper()}";
            return shortText;
        }
        public string ClearBookName(string text, IEnumerable<string> itemsToClear = null)
        {
            if (itemsToClear != null && itemsToClear.Any())
                text = LoopClear(text, itemsToClear);
            
            return TrimText(text);            
        }        
        public string ClearChapterTitle(string text, IEnumerable<string> itemsToClear = null)
        {
            if (itemsToClear != null && itemsToClear.Any())
                text = LoopClear(text, itemsToClear);

            string chapterNumber = string.Empty;
            var matches = Regex.Match(text, @"chapter[\s|-][0-9]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (matches.Success)
                chapterNumber = TrimText(matches.Value);
            text = text.Replace(matches.Value, chapterNumber);
            return TrimText(text);
        }
        public string ClearContent(string text, IEnumerable<string> itemsToClear = null)
        {
            if (itemsToClear != null && itemsToClear.Any())
                text = LoopClear(text, itemsToClear);

            string chapterNumber = string.Empty;
            foreach (string htmlCaracterKey in HtlmCaracters.Keys)
            {
                text = text.Replace(htmlCaracterKey, HtlmCaracters[htmlCaracterKey]);                
            }
            return text;
        }
        
        
        public ParserInfo? ClearParserInfo(ParserInfo parserInfo, IModelOption options)
        {
            parserInfo.BookName = ClearBookName(parserInfo.BookName, options.ClearBookName);
            parserInfo.ChapterTitle = ClearChapterTitle(parserInfo.ChapterTitle, options.ClearChapterTitle);
            parserInfo.Content = ClearContent(parserInfo.Content, options.ClearContent);
            return parserInfo;
        }
        
        private string LoopClear(string text, IEnumerable<string> items)
        {
            foreach (string item in items)
            {
                var matches = Regex.Match(text, item, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (matches.Success)
                    text = text.Replace(matches.Value, "");
            }
            return text;
        }
        private Dictionary<string, string> HtlmCaracters = new Dictionary<string, string>()
        {
            {"&#10;",@"\n"},
            {"&#13;",@"\r\t"},
            {"&#32;",@"\s"},
            {"&#33;","!"},
            {"&#34;","\""},
            {"&#35;","#"},
            {"&#36;",@"\$"},
            {"&#37;","%"},
            {"&#38;","&"},
            {"&#39;","'"},
            {"&#40;","("},
            {"&#41;",")"},
            {"&#42;","*"},
            {"&#43;","+"},
            {"&#44;",","},
            {"&#45;",@"\-"},
            {"&#46;",@"\."},
            {"&#47;","/"},
            {"&#58;",":"},
            {"&#59;",";"},
            {"&#60;","<"},
            {"&#61;","="},
            {"&#62;",">"},
            {"&#63;",@"\?"},
            {"&#64;","@"},
            {"&#91;",@"\["},
            {"&#95;","_"},
            {"&#123;",@"\{"},
            {"&#124;",@"\|"},
            {"&#125;",@"\}"},
            {"&#126;","~"},
            {"&nbsp;"," "},
            {"&#[a-z]+;", ""},
            {"&#[a-z0-9]+;", ""},
            {"&[a-z]+;", ""},
            {"&[a-z0-9]+;", ""},
            {@"\[http.+\]", ""},
            {@"^\d+\n",""},
            {"&#x201C;","" },
            {"&#x201D;","" },
            {@"(=){2,}",""},
            {"Translator:.+",""},
            {@"\]",""},
            {@"\[",""},
            {"&#x2019;","'" },
            {@"^_.+$|.+lightnovelpub\.com.+(experience\b|platform\b|website\b)|.+lightnovelpub\.com\.?",""},
            //{@"(Visit)([a-z\s\.]+)|([A-Z][a-z]+)([a-z\-\s,]+)(lightnovelpub.com)",""},
            //{@"^[a-zA-Z][a-z\s_]+\.com[a-z\s\.]+|^_[\w\s]+[\.\w]$|^[a-zA-Z]+['a-zA-Z\s\0-9]+\]",""},
        };
    }
}
