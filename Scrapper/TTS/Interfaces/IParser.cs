using HtmlAgilityPack;
using TTS.Models;

namespace TTS.Interfaces
{
    public interface IParserService
    {
        Task<string> GetBookNameAsync(HtmlNode document);
        Task<string> GetChapterTitleAsync(HtmlNode document);
        Task<string> GetContentAsync(HtmlNode document);
        Task<string> GetNextChapterUrlAsync(HtmlNode document);
        Task<ParserInfo> ParseAsync(string url);
    }
}