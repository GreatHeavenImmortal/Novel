using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS.Interfaces;

namespace TTS.Models
{
    public class ParserInfo : IParserInfo
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string ChapterUrl { get; set; }
        public string BookName { get; set; }
        public string ChapterTitle { get; set; }
        public string Content { get; set; }
        public string NextChapterUrl { get; set; }
    }
}
