using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace tts_wuxia
{
    static class FonctionTexte
    {
        private static Dictionary<string, string> htlmCaracters = new Dictionary<string, string>()
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
            {@"&#(8220|8221)","\""},
            {@"&#8217;","'" },
            {"&nbsp;"," "},
            {"&#[a-z0-9]+;", ""},
            {@"\[http.+\]", ""},
            {@"^\d+\n",""},
            {@"(=){2,}",""},
            {"Translator:.+",""},
            {@"\]",""},
            {@"\[",""},
            {"&#x2019;","'" },
            {@"^_.+$|.+lightnovelpub\.com.+(experience\b|platform\b|website\b)|.+lightnovelpub\.com\.?",""},
            {@"&#[a-zA-Z0-9]+;",""},
            {@"%20"," " },
            {@"%22","\"" },
            {@"%27","'" },
            {@"%2C","," },
            {@"%[0-9A-Z]{2,3}",""}
            //{@"(Visit)([a-z\s\.]+)|([A-Z][a-z]+)([a-z\-\s,]+)(lightnovelpub.com)",""},
            //{@"^[a-zA-Z][a-z\s_]+\.com[a-z\s\.]+|^_[\w\s]+[\.\w]$|^[a-zA-Z]+['a-zA-Z\s\0-9]+\]",""},
        };
        public static string Nettoyage(string input)        
        {       
            input = input.Trim();
            foreach (var key in htlmCaracters.Keys)
            {
                input = Regex.Replace(input, key, htlmCaracters[key]);
            }
            return input;
        }
        public static string TitreCreation(string url, int indexSitename, int indexTitreChapitre, string sperator = null)
        {
            string sitename = url.Split('/')[indexSitename];            
            string titreChapitre = url.Split('/')[indexTitreChapitre];
            string retour = string.Empty;

            string site = sitename.Split('.').Length == 3 ? sitename.Split('.')[1] : sitename.Split('.')[0]; 

            foreach(var c in titreChapitre.Split('-'))
            {
                if(Int32.TryParse(c, out int result) == false )
                {
                    if(c.Contains(sitename) == false)
                    {
                        retour = retour + c + "_";                        
                    }
                }
            }
            return retour.Trim().Trim('_').ToUpper();
        }

        public static string NumberToString(this int obj)
        {
            Queue<char> q = new ();
            q.Enqueue('0');
            q.Enqueue('0');
            q.Enqueue('0');
            q.Enqueue('0');
            foreach(char c in obj.ToString())
            {
                q.Dequeue();
                q.Enqueue(c);                
            }
            return String.Join(null,q.ToArray());
        }
    }
}
