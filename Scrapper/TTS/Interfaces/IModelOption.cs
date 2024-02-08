namespace TTS.Interfaces
{
    public interface IModelOption
    {
        static string SectionName { get; }
        string InstanceSectionName { get; }
        string BookName { get; set; }
        bool IsBookNameInUrl { get; set; }
        string BookNameUrlAttribute { get; set; }
        string ChapterTitle { get; set; }
        string Content { get; set; }
        string NextChapterUrl { get; set; }
        List<string> ClearBookName { get; set; }
        List<string> ClearChapterTitle { get; set; }
        List<string> ClearContent { get; set; }
    }
}