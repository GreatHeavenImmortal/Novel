namespace TTS.Interfaces
{
    public interface IParserInfo
    {
        string BookName { get; set; }
        string ChapterTitle { get; set; }
        string Content { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        string NextChapterUrl { get; set; }
    }
}