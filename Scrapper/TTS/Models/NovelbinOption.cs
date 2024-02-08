using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Interfaces;

namespace TTS.Models;

public class NovelbinnetOption : IModelOption
{
    public static string SectionName = nameof(NovelbinnetOption);
    public string InstanceSectionName { get; } = nameof(NovelbinnetOption);
    public string BookName { get; set; }
    public bool IsBookNameInUrl { get; set; }
    public string BookNameUrlAttribute { get; set; }
    public string ChapterTitle { get; set; }
    public string Content { get; set; }
    public string NextChapterUrl { get; set; }
    public List<string> ClearBookName { get; set; }
    public List<string> ClearChapterTitle { get; set; }
    public List<string> ClearContent { get; set; }
}
