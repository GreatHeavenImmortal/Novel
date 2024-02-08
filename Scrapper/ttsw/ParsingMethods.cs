using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.DevTools.V107.Runtime;
using OpenQA.Selenium.Remote;
using AngleSharp.Dom;
using System.Xml.Linq;

namespace tts_wuxia
{
    static class ParsingMethods
    {
        public static Dictionary<string, Func<string, Chapitre>> ParsingMethodsCollection = new Dictionary<string, Func<string, Chapitre>>();

        private static HtmlNode GetHtmlNode(string url)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(Requests.GetUrl(url));
            return html.DocumentNode;
        }
        public static string ShortTile(string titre)
        {
            string retour = string.Empty;
            titre = titre.Replace('-', ' ').Replace('_', ' ');
            if (titre.Length < 10) { return titre; }
            foreach (var s in titre.Split(' '))
            {
                if (Int32.TryParse(s, out int _) == false)
                {
                    System.Diagnostics.Debug.Print(s);
                    if (s.Length >=1)
                    {                        
                        retour = retour + s.ToUpper()[0];                        
                    }
                    System.Diagnostics.Debug.Print(retour);
                }
            }
            return retour;
        }


        public static Chapitre BoxnovelCom(string url)
        {

            var document = GetHtmlNode(url);

            StringBuilder content = new StringBuilder();
            string titreLivre = url.Split('/')[4].Replace(" ", "-").Replace("_","-");
            string titreLivreCourt = ShortTile(url.Split('/')[4]);
            string titreChapitre = url.Split('/')[5];
            titreChapitre = titreChapitre.Trim().Trim().Replace(' ', '-');
            string pageSuivante = string.Empty;

            content.AppendLine(titreChapitre.ToUpper());

            foreach (HtmlNode element in document.QuerySelectorAll(".reading-content p"))
            {
                if(element.InnerText.Length >= 4 )
                {
                    if (element.InnerText.Contains("Thank you for reading on myboxnovel.com"))
                        continue;
                    else
                        //content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(element.InnerHtml)));
                        content.AppendLine(FonctionTexte.Nettoyage(element.InnerText));



                }
            }
            //Debug.Print(content.ToString());
            pageSuivante = document.QuerySelector(".next_page")?.Attributes["href"].Value ?? string.Empty;
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre Scribblehub(string url)
        {

            var document = GetHtmlNode(url);

            StringBuilder content = new StringBuilder();
            string titreLivre = string.Empty;
            string titreLivreCourt = string.Empty;
            string titreChapitre = string.Empty;
            string pageSuivante = string.Empty;



            titreLivre = FonctionTexte.TitreCreation(url, 2, 4);
            titreLivre = titreLivre.Trim().Replace('-', ' ').Trim().Replace(' ', '_');
            titreLivreCourt = ShortTile(titreLivre);

            titreChapitre = document.QuerySelector("#chapter-title")?.InnerText
                            ?? document.QuerySelector(".chapter-title")?.InnerText
                            ?? "chapitre";
            titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_');

            content.AppendLine(titreChapitre.ToUpper());
            content.AppendLine("");

            foreach (HtmlNode element in document.QuerySelectorAll("#chp_raw p"))
            {
                if (element.InnerText.Contains("End Of Chapter") == false)
                {
                    content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(element.InnerHtml)));
                }
            }

            pageSuivante = document.QuerySelector(".btn-next")?.Attributes["href"].Value;
            pageSuivante = pageSuivante == "#" ? null : pageSuivante;
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre WuxiaworldCo(string url)
        {
            var document = GetHtmlNode(url);

            StringBuilder content = new StringBuilder();
            string titreLivre = url.Split('/')[3].Replace("-", "_");
            string titreLivreCourt = ShortTile(url.Split('/')[3]);
            string titreChapitre = string.Empty;
            string pageSuivante = string.Empty;
            int lastSlash = url.LastIndexOf('/') + 1;

            titreChapitre = document.QuerySelector("h1")?.InnerText ?? "chapitre";
            titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_');

            content.AppendLine($"CHAPTER {titreChapitre.ToUpper()}:");
            content.AppendLine("");

            string[] texte = document.QuerySelector("section .chapter-entity").InnerHtml.Split("<br>");
            if (texte.Length > 0)
            {
                for (var i = 0; i < texte.Length; i++)
                {
                    if (texte[i].ToLower().Contains("chapter")
                        || texte[i].ToLower().Contains("Please go to")
                        || string.IsNullOrEmpty(texte[i]))
                    {
                        continue;
                    }
                    else
                    {
                        content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(texte[i])));
                    }
                }
            }
            if (document.QuerySelector(".next") != null)
            {
                int tailleLien = document.QuerySelector(".next").Attributes["href"].Value.Split('/').Length - 1;
                pageSuivante = url.Substring(0, lastSlash) + document.QuerySelector(".next")?.Attributes["href"].Value.Split('/')[tailleLien];
            }
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre WuxiaworldCom(string url)
        {
            var document = GetHtmlNode(url);

            StringBuilder content = new StringBuilder();
            string titreLivre = url.Split('/')[4];;
            titreLivre = titreLivre.Trim().Replace('-', ' ').Trim().Replace(' ', '_').Replace('_','-');
            string titreLivreCourt = ShortTile(url.Split('/')[4]);

            string titreChapitre = string.Empty;
            string pageSuivante = string.Empty;
            int lastSlash = url.LastIndexOf('/') + 1;

            titreChapitre = url.Split('/')[5];
            titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_').Replace('_','-');
            content.AppendLine($"{titreChapitre.ToUpper()}:");
            content.AppendLine("");



            foreach (var el in document.QuerySelectorAll("#chapter-content p"))
            {
                content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(el.InnerHtml)));
            }

            if (document.QuerySelector("div.top-bar-area li.next a.btn-link") != null)
            {
                var link = document.QuerySelector("div.top-bar-area li.next a.btn-link").Attributes["href"].Value;
                int tailleLien = link.Split('/').Length - 1;
                pageSuivante = url.Substring(0, lastSlash) + link.Split('/')[tailleLien];

                Console.WriteLine(pageSuivante);
            }
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre BoxnovelOrg(string url)
        {
            var document = GetHtmlNode(url);

            StringBuilder content = new StringBuilder();


            string titreTemp = url.Split('/')[3];
            int indexOfChapter = titreTemp.ToLower().IndexOf("chapter") == -1 ? titreTemp.Length : titreTemp.ToLower().IndexOf("chapter");
            string titreLivre = titreTemp.Substring(0, indexOfChapter - 1).Replace("-", "_");
            string titreLivreCourt = ShortTile(titreTemp.Substring(0, indexOfChapter - 1));
            string titreChapitre = titreTemp.Substring(indexOfChapter);
            titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_');
            string pageSuivante = string.Empty;
            int lastSlash = url.LastIndexOf('/') + 1;

            content.AppendLine($"{titreChapitre.ToUpper()}:");
            content.AppendLine("");


            foreach (var el in document.QuerySelectorAll("#chr-content .cha-words p"))
            {
                content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(el.InnerHtml)));
            }

            pageSuivante = document.QuerySelector("#next_chap")?.Attributes["href"]?.Value ?? null;

            Console.WriteLine(pageSuivante);
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre ReadWn(string url)
        {

            try
            {
                var document = GetHtmlNode(url);

                StringBuilder content = new StringBuilder();


                string titreLivre = url.Split('/')[url.Split('/').Length - 1].Split('_')[0].Replace('-', ' ').Trim().Replace(' ', '_');
                titreLivre = titreLivre.Replace('\n', ' ').Replace('\t', ' ').Trim(' ').Replace(' ', '_');
                string titreLivreCourt = ShortTile(titreLivre);
                string titreChapitre = document.QuerySelector(".titles h2")?.InnerText ?? string.Empty;
                titreChapitre = titreChapitre.Replace('\n', ' ').Replace('\t', ' ').Trim(' ').Replace(' ', '_');
                string pageSuivante = string.Empty;
                String nextPage = string.Empty;
                string baseUrl = string.Empty;

                content.AppendLine("");
                content.AppendLine(titreChapitre);

                
                string textContent = document.QuerySelector(".chapter-content").InnerText.Replace("/", "").Replace("[", "").Replace("]", "");
                content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(textContent)));
                //content.AppendLine(document.QuerySelector(".chapter-content").InnerText.Replace("/", "").Replace("[", "").Replace("]", ""));

                nextPage = document.QuerySelector(".nextchap")?.Attributes["href"]?.Value ?? null;
                baseUrl = url.Substring(0, (url.ToLower().IndexOf("/novel")));

                pageSuivante = baseUrl + nextPage;                
                return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static Chapitre NovelHallCom(string url)
        {
            try
            {
                if (!url.ToLower().Contains("https"))
                {
                    url = $"https://{url}";
                }
                var document = GetHtmlNode(url);

                StringBuilder content = new StringBuilder();

                string name = url.Split('/')[url.Split('/').Length - 2];
                string titreLivre = name.Substring(0, name.LastIndexOf("-")).Replace("-", " ").Trim().Replace(' ', '_');
                titreLivre = titreLivre.Trim().Replace('-', ' ').Trim().Replace(' ', '_').Replace("_","-");
                string titreLivreCourt = ShortTile(titreLivre);
                string titreChapitre = document.QuerySelector(".content .breadcrumb li:last-child")?.InnerText ?? string.Empty;
                titreChapitre = titreChapitre.Replace('\n', ' ').Replace('\t', ' ').Trim(' ').Replace(' ', '_').Replace("_","-");
                titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_').Replace("_","-");
                string pageSuivante = string.Empty;
                string nextPage = string.Empty;

                content.AppendLine(titreChapitre);
                content.AppendLine("");
                string data = FonctionTexte.Nettoyage(Uri.EscapeDataString(document.QuerySelector("#htmlContent").InnerHtml));

                var t = data.Replace("<br>", "\n").Split('\n');

                List<string> listExclusion = new List<string>()
                {
                    "Kindly read it on webnovel to support me"
                    ,"Join the discord for fun stuff and suggestions"
                    ,"[[[[[[[[[[Chapter"
                    ,"Editors and Proof Readers:"
                    ,"[[[[[[[[[[[[[End"
                    ,"Don't forget to vote and leave a review"
                    ,"Please tell me what you think of my novel so far."
                };

                bool aExclure = false;
                foreach (string s in t)
                {
                    string ss = s.Replace('\\', ' ').Trim().Trim('\n', ' ').Trim().Trim('\"', ' ');

                    aExclure = false;
                    foreach (string e in listExclusion)
                    {
                        if (s.Contains(e))
                        {
                            aExclure = true;
                            break;
                        }
                    }

                    if (!aExclure)
                    {
                        content.AppendLine(ss);
                    }
                }

                var listLink3 = document.QuerySelectorAll("a");
                string next = listLink3.ToList().Find(x => x.InnerText.ToLower().Contains("next"))?.Attributes["href"].Value;
                if (!string.IsNullOrEmpty(next))
                {
                    string baseUrl = "https://" + url.Split('/')[2] + next;
                    nextPage = baseUrl;
                }

                pageSuivante = nextPage;                
                return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static Chapitre NovelFullCom(string url)
        {

            try
            {
                if (!url.ToLower().Contains("https"))
                {
                    url = $"https://{url}";
                }
                var document = GetHtmlNode(url);

                StringBuilder content = new StringBuilder();

                string name = url.Split('/')[url.Split('/').Length - 2];
                string titreLivre = name.Replace("-", " ").Trim().Replace(' ', '_').Replace('_','-');
                titreLivre = titreLivre.Trim().Replace('-', ' ').Trim().Replace(' ', '_').Replace('_','-');
                string titreLivreCourt = ShortTile(titreLivre);
                string titreChapitre = document.QuerySelector("h2 span")?.InnerText ?? string.Empty;
                titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_').Replace('_','-');
                string pageSuivante = string.Empty;
                string nextPage = string.Empty;

                content.AppendLine("");
                content.AppendLine(titreChapitre);

                if (!document.QuerySelectorAll("#chapter-content p").Any())
                {
                    throw new Exception("No content found");
                }

                var listHtmlNode = document.QuerySelector("#chapter-content");
                int compteur = 0;
                bool contentIsNull = true;
                foreach (HtmlNode n in listHtmlNode.ChildNodes)
                {
                    if (compteur > 5 && n.Name == "hr") { break; }
                    if (n.Name == "p")
                    {                        
                        if (!string.IsNullOrEmpty(n.InnerHtml))
                        {
                            content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(n.InnerText)));
                            contentIsNull = false;
                        }
                        compteur++;
                    }
                }

                if (contentIsNull)
                {
                    List<HtmlNode> list = document.QuerySelectorAll("#chapter-content p").ToList();
                    int nbText = default(int);
                    foreach (HtmlNode node in list)
                    {
                        string text = FonctionTexte.Nettoyage(Uri.EscapeDataString(node?.InnerHtml))  ?? string.Empty;
                        if (!string.IsNullOrEmpty(text))
                        {
                            nbText = text.Length > 35 ? 35 : text.Length - 1;
                            if (text.Substring(0, nbText).Contains("Idiom: &#") || text.Substring(0, nbText).ToLower().Contains("http") || text.Substring(0, nbText).ToLower().Contains("if you find any errors"))
                            {
                                continue;
                            }
                            else
                            {
                                content.AppendLine(text);
                            }
                        }
                    }
                }

                string next = document.QuerySelector("#next_chap")?.Attributes["href"]?.Value ?? string.Empty;
                if (!string.IsNullOrEmpty(next))
                {
                    nextPage = "https://" + url.Split('/')[2] + next;
                }

                pageSuivante = nextPage;
                Console.WriteLine(pageSuivante);
                return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static Chapitre ComrademaoCom(string url)
        {

            try
            {
                var document = GetHtmlNode(url);

                StringBuilder content = new StringBuilder();

                string name = url.Split('/')[url.Split('/').Length - 3];
                string titreLivre = name.Replace("-", " ").Trim().Replace(' ', '-').Replace('_','-');
                string titreLivreCourt = ShortTile(titreLivre);
                
                var n =  url.Split('/')[url.Split('/').Length - 2].Split('-');
                string titreChapitre = String.Join("-",n.Skip(n.Length-2).Take(2).ToList());
                string pageSuivante = string.Empty;
                string nextPage = string.Empty;

                content.AppendLine("");
                content.AppendLine(titreChapitre);


                    List<HtmlNode> list = document.QuerySelectorAll("#content div > p").ToList();
                    int nbText = default(int);
                    foreach (HtmlNode node in list)
                    {
                    
                        string text = FonctionTexte.Nettoyage(Uri.EscapeDataString(node?.InnerHtml)) ?? string.Empty;
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (text.Substring(0, nbText).Contains("Idiom: &#") || text.Substring(0, nbText).ToLower().Contains("http") || text.Substring(0, nbText).ToLower().Contains("if you find any errors"))
                            {
                                continue;
                            }
                            else
                            {
                                content.AppendLine(text);
                            }
                        }
                    }

                int i = 0;
                var next = document.QuerySelectorAll(".pagination-list li a").ToList();
                if(next.Any())
                {
                    pageSuivante = next.ElementAt(2).Attributes["href"]?.Value.ToString() ?? string.Empty;
                }
                else
                {   
                    var zz = new List<string>();
                    next = document.QuerySelectorAll("a").ToList();
                    if(next.Any())
                    {
                        pageSuivante = next.Where(x => x.Attributes["rel"]?.Value?.ToString() == "next")
                            .Select(x => x.Attributes["href"]?.Value?.ToString()).First();
                    }                       
                }  
                Console.WriteLine(pageSuivante);
                return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        //Galaxytranslations97Com
        public static Chapitre Galaxytranslations97Com(string url)
        {

            var document = GetHtmlNode(url);

            StringBuilder content = new StringBuilder();
            int index = url.Split('/')[6].Split('-').Count();
            int pos = 0;
            var arrUrl = url.Split('/')[6].Split('-');
            string titreLivre = string.Empty;
            foreach (string s in arrUrl)
            {
                if (pos < index)
                    titreLivre = $"{titreLivre} {s}";
            }
            titreLivre = titreLivre.Trim();
            string titreLivreCourt = ShortTile(titreLivre);
            string titreChapitre = url.Split('/')[6].Split('-')[index - 1].Trim();
            titreChapitre = titreChapitre.Trim().Replace('-', ' ').Trim().Replace(' ', '_');
            string pageSuivante = string.Empty;



            foreach (HtmlNode element in document.QuerySelectorAll(".entry-content > p"))
            {
                content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(element?.InnerHtml)));
            }

            pageSuivante = document.QuerySelector(".wp-next-post-navi-next > a")?.Attributes["href"].Value;
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre RoyalroadCom(string url)
        {

            var document = GetHtmlNode(url);
            StringBuilder content = new StringBuilder();
            string titreLivre = url.Split('/')[5].Trim();
            string titreLivreCourt = ShortTile(titreLivre);
            string titreChapitre = document.QuerySelector("h1")?.InnerText.Trim();
            var pageSuivanteTemp = document.QuerySelectorAll(".nav-buttons a");
            string pageSuivante = pageSuivanteTemp?.Count() >= 0
            ? pageSuivanteTemp.ToList()[pageSuivanteTemp.Count() - 1]?.Attributes["href"]?.Value
            : string.Empty;
            pageSuivante = pageSuivante?.Substring(1, pageSuivante.Length - 1);
            var arrUrl = url.Split('/').Take(3).Append(pageSuivante);

            pageSuivante = string.Join('/', arrUrl);


            //"https://www.royalroad.com/fiction/38676/earths-greatest-magus/chapter/603937/institution"
            foreach (HtmlNode element in document.QuerySelectorAll(".chapter-content > p"))
            {
                content.AppendLine(FonctionTexte.Nettoyage(Uri.EscapeDataString(element?.InnerHtml)));
            }

            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre NovelhiCom(string url)
        {
            var document = GetHtmlNode(url);
            string titreLivre = url.Split('/')[4]?.Trim()?.ToLower() ?? string.Empty;
            string titreLivreCourt = ShortTile(titreLivre);
            string titreChapitre =  document.QuerySelector(".book_title h1")?.InnerText?.Trim() ?? string .Empty;

            IEnumerable<HtmlNode> nodesContent = document.QuerySelectorAll("#showReading sent");
            if(nodesContent.Any() == false)
                throw new ArgumentException("Pas de publication pour la page {0}",url);


            StringBuilder content = new StringBuilder();
            var t = document.QuerySelectorAll("sent");
            var t2 = document.QuerySelector("#showReading").InnerHtml;
            System.Diagnostics.Debug.Print(t2);
            nodesContent.Select(item => FonctionTexte.Nettoyage(Uri.EscapeDataString(item?.InnerHtml)) ?? "--")
            .ToList()
            .ForEach( item =>  content.AppendLine(item));
            t2 = document.QuerySelector("#showReading").InnerHtml;
            
            string[] splitedUrl = url.Split('/');
            string baseUrl  = string.Join("/",splitedUrl.Take(splitedUrl.Length-1));
            string endUrl = string.Join(" ",splitedUrl.Skip(splitedUrl.Length-1));
            int urlChapter = 0;                    
            if(Int32.TryParse(endUrl, out urlChapter) == false)
                throw new ArgumentException("Numero de chaptire incorret {0}",endUrl);
            
            string pageSuivante = $"{baseUrl}/{urlChapter+1}";


            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre Novelfullchaptercom(string url)
        {
            var document = GetHtmlNode(url);
            string titreLivre = url.Split('/')[4].Replace('-','_');
            string titreLivreCourt = ShortTile(titreLivre);
            string titreChapitre = document.QuerySelector("h4")?.InnerText;
            
            StringBuilder content = new StringBuilder();
            if (titreChapitre is null)
            {
                titreChapitre = url.Split('/')[5].Replace("-", "_");
                if(titreChapitre.Split('_').Length > 2)
                    titreChapitre = $"{titreChapitre.Split("_")[0]}_{titreChapitre.Split("_")[1].PadLeft(4, '0')}_{titreChapitre.Split('_')[2]}";
                else
                    titreChapitre = $"{titreChapitre.Split("_")[0]}_{titreChapitre.Split("_")[1].PadLeft(4, '0')}";
                content.AppendLine(titreChapitre);
            }
            else
            {
                titreChapitre = titreChapitre.Replace(':', ' ').Trim(' ').Replace("  "," ").Replace(' ','_');
                if (titreChapitre.Split('_').Length > 2)
                    titreChapitre = $"{titreChapitre.Split("_")[0]}_{titreChapitre.Split("_")[1].PadLeft(4, '0')}_{string.Join("_",titreChapitre.Split('_').Skip(2).Take(1000))}";
                else
                    titreChapitre = $"{titreChapitre.Split("_")[0]}_{titreChapitre.Split("_")[1].PadLeft(4, '0')}";
            }
            
            
            string pageSuivante = document.QuerySelector("#next_chap").Attributes["href"].Value;

            string texte = FonctionTexte.Nettoyage(Uri.EscapeDataString(document.QuerySelector("#chr-content").InnerHtml));
            content.AppendLine(texte);
            

            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre Novelbinnet(string url)
        {
            var document = GetHtmlNode(url);
            string titreLivre = url.Split('/')[4]?.Trim().Replace("_", "-").Replace(" ", "-") ?? "no-book-title" ;
            string titreLivreCourt = ShortTile(titreLivre);
            string titreChapitre = document.QuerySelector("h2")?.InnerText?.Trim().Replace("_","-").Replace(" ","-") ?? "no-chapter-title";

            titreLivre = FonctionTexte.Nettoyage(titreLivre);
            titreChapitre = FonctionTexte.Nettoyage(titreChapitre);
            titreChapitre = titreChapitre.Replace("__", "_").Replace("--", "-");
            titreLivre = titreLivre.Replace("__", "_").Replace("--", "-");
            titreLivre = string.Join('-', titreLivre.Split('-').Take(titreLivre.Split('-').Length - 1));
            StringBuilder content = new StringBuilder();
            content.AppendLine(titreChapitre);
            content.AppendLine("");
            content.AppendLine();

            var ps = document.QuerySelectorAll(".chapter,.container p");

            ps.ToList().ForEach(x =>
            {
                if (x.HasAttributes == false)
                {
                    //x.GetAttributes().Select(x => x.Name).ToList().ForEach(x => Debug.Print(x));
                    var texteNettoyage = FonctionTexte.Nettoyage(Uri.EscapeDataString(x.InnerHtml)); 
                    if (!texteNettoyage.ToLower().Contains(@"nov//el/bin./net'".ToLower()) && 
                        !texteNettoyage.ToLower().Contains("The source of this content".ToLower())) 
                        content.AppendLine(texteNettoyage);
                }
            });
     
            string pageSuivante = document.QuerySelector("#next_chap")?.Attributes["href"]?.Value ?? string.Empty;

            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre LightNovelPub(string url)
        {
            //try
            //{
            var document = GetHtmlNode(url);
            StringBuilder content = new StringBuilder();


            string titreLivre = document.QuerySelector("a.booktitle").Attributes["title"].Value.Replace(" ", "-").Replace("_", "-").Replace(':', ' ').Trim(' ');
            titreLivre = titreLivre.Trim(new char[] { '-', '_', '!', '?', '.', ' ' });
            titreLivre = FonctionTexte.Nettoyage(titreLivre);
            string titreLivreCourt = ShortTile(titreLivre);

            string titreChapitre = document.QuerySelector(".titles h2")?.InnerText ?? string.Empty;
            if (string.IsNullOrEmpty(titreChapitre))
                titreChapitre = document.QuerySelector("span.chapter-title").InnerText ?? string.Empty;
            if (string.IsNullOrEmpty(titreChapitre))
                titreChapitre = url.Split("/")[url.Split("/").Length - 1];
            titreChapitre = titreChapitre.Trim().Replace('_', '-').Replace(' ', '-').Replace(':', '-').Replace("--", "-").Trim(new char[] { '-', '_', '!', '?', '.', ':', ' ' });
            titreChapitre = FonctionTexte.Nettoyage(titreChapitre);

            string pageSuivante = string.Empty;
            String nextPage = string.Empty;
            string baseUrl = string.Empty;

            content.AppendLine("");
            var chapterInContentPage = document.QuerySelectorAll("p")?.ToList()[0]?.InnerText.ToLower() ?? string.Empty;
            if (chapterInContentPage.Contains("chapter") == false)
                content.AppendLine(titreChapitre.Replace('-', ' '));


            //var listLigneTexte = from item in document.QuerySelectorAll("p") where item.HasAttributes == false  &&  item.HasChildNodes == false select item;
            var listLigneTexte = document.QuerySelectorAll("p")
                .Where(x => x.HasAttributes == false)
                .Where(x => x.InnerText != "You can find the rest of this content on the lightno‍velpub.c‎om platform");

            
            foreach(HtmlNode item in listLigneTexte)
            {
                string s = item.InnerHtml.Replace("/", "").Replace("[","");                
                //s = FonctionTexte.Nettoyage(Uri.EscapeDataString(s));
                content.AppendLine(s);
            }
            Debug.Print(content.ToString());
            string pattern = @"([A-Za-z]\.)+[A-Za-z]";
            var reg = new Regex(pattern, RegexOptions.IgnoreCase);
            string contentPage = content.ToString();
            var matchCollection = reg.Matches(contentPage);

            if (matchCollection.Any())
            {
                foreach (var item in matchCollection)
                {
                    string replacement = item.ToString().Replace(".", "");
                    contentPage = Regex.Replace(contentPage, item.ToString(), replacement, RegexOptions.IgnoreCase);
                }
            }
            //contentPage = Regex.Replace(contentPage, "k.i.n.g", "king", RegexOptions.IgnoreCase);
            //contentPage = Regex.Replace(contentPage, "k.e.d", "ked", RegexOptions.IgnoreCase);

            //matchCollection = reg.Matches(contentPage);
            //if (matchCollection.Any())
            //{
            //    foreach (var item in matchCollection)
            //    {
            //        Console.WriteLine(item.ToString());
            //    }
            //}

            
            nextPage = document.QuerySelector(".nextchap")?.Attributes["href"]?.Value ?? null;
            baseUrl = url.Substring(0, (url.ToLower().IndexOf("/novel")));
            pageSuivante = baseUrl + nextPage;
            if (pageSuivante.ToLower().Contains("javascript"))
                pageSuivante = null;
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, contentPage, pageSuivante, url);
            //}
            //catch (System.Exception)
            //{
            //    return new Chapitre(null, null, null, null, null,null) ;
            //}
        }

        public static Chapitre Ranobes(string url)
        {
            var document = GetHtmlNode(url);
            StringBuilder content = new StringBuilder();


            string titreLivre = document.QuerySelector(".category.grey.ellipses a")?.InnerText?.Replace(" ", "-")?.Replace("_", "-")?.Replace(':', ' ')?.Trim(' ') ?? string.Empty;
            titreLivre = titreLivre.Trim(new char[] { '-', '_', '!', '?', '.', ' ' });
            titreLivre = FonctionTexte.Nettoyage(titreLivre);
            string titreLivreCourt = ShortTile(titreLivre);

            string titreChapitre = document.QuerySelector("h1.h4.title")?.InnerText.Split(':')[1]?.Trim().Replace(' ','_') ?? string.Empty;
            titreChapitre = titreChapitre.Trim(new char[] { '-', '_', '!', '?', '.', ':', ' ' });
            titreChapitre = FonctionTexte.Nettoyage(titreChapitre);

            content.AppendLine(titreLivre);
            content.AppendLine(titreChapitre);
            content.AppendLine("");
            var chapterInContentPage = document.QuerySelectorAll("#arrticle.text > p")?.ToList().Skip(1).ToList().FirstOrDefault()?.InnerHtml.ToLower() ?? string.Empty;
            content.AppendLine("");

            var listLigneTexte = document.QuerySelectorAll("#arrticle.text > p");
            
            foreach(HtmlNode item in listLigneTexte)
            {
                string s = FonctionTexte.Nettoyage(Uri.EscapeDataString(item.InnerHtml));
                content.AppendLine(s);
            }

            string pageSuivante = document.QuerySelector(".btn.btn-icon.right.dark-btn")?.Attributes["href"]?.Value ?? null;
            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        public static Chapitre Wtr(string url)
        {
            var document = GetHtmlNode(url);
            StringBuilder content = new StringBuilder();

            string titreLivre = document.QuerySelector("title")?.InnerText.Split("-")[0] ?? string.Empty;
            titreLivre = titreLivre.Trim(new char[] { '-', '_', '!', '?', '.', ' ' });
            titreLivre = FonctionTexte.Nettoyage(titreLivre);
            string titreLivreCourt = ShortTile(titreLivre);

            string titreChapitre = document.QuerySelector(".chapter-title")?.InnerText ?? string.Empty;
            titreChapitre = titreChapitre.Trim(new char[] { '-', '_', '!', '?', '.', ':', ' ' });
            titreChapitre = FonctionTexte.Nettoyage(titreChapitre);

            content.AppendLine(titreLivre);
            content.AppendLine(titreChapitre);
            content.AppendLine("");

            var listLigneTexte = document.QuerySelector("#__NEXT_DATA__").InnerHtml;

            string s = FonctionTexte.Nettoyage(Uri.EscapeDataString(listLigneTexte));
            content.AppendLine(s);

            //listLigneTexte.ToList().ForEach(x => content.AppendLine(x.InnerText));


            string pageSuivante = string.Empty; //document.QuerySelector(".btn.btn-icon.right.dark-btn")?.Attributes["href"]?.Value ?? null;
            var btNext = document.QuerySelectorAll(".chapter-navigator a");
            if(btNext != null)
            {
                foreach(var item in btNext)
                {
                    if(item.InnerText?.ToLower() == "next")
                    {
                        pageSuivante = item.Attributes["href"].Value ?? string.Empty;
                    }
                }
            }

            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }


        public static Chapitre NovelMedium(string url)
        {
            var document = GetHtmlNode(url);
            StringBuilder content = new StringBuilder();


            var breadcrumb = document.QuerySelectorAll(".breadcrumb li").Select(x => x.InnerText).ToList();

            string titreLivre = breadcrumb.Any() ?  breadcrumb[breadcrumb.Count - 1] : string.Empty;
            titreLivre = titreLivre.Trim(new char[] { '-', '_', '!', '?', '.', ' ' });
            titreLivre = FonctionTexte.Nettoyage(titreLivre);
            string titreLivreCourt = ShortTile(titreLivre);


            string titreChapitre = document.QuerySelector(".heading h2")?.InnerText.Replace("(", "").Replace(")", "") ?? string.Empty;
            titreChapitre = titreChapitre.Trim(new char[] { '-', '_', '!', '?', '.', ':', ' ' });
            titreChapitre = FonctionTexte.Nettoyage(titreChapitre);

            content.AppendLine(titreLivre);
            content.AppendLine(titreChapitre);
            content.AppendLine("");


            var textes = document.QuerySelectorAll(".content p").Select(x => x.InnerText);

            foreach(string texte in textes)
            {
                content.AppendLine(FonctionTexte.Nettoyage(texte));
            }

            var links = document.QuerySelectorAll(".actions a").Select(x => x.Attributes["href"]?.Value).ToList();
            string pageSuivante = links.Any() ? links[links.Count() -1] : string.Empty;

            return new Chapitre(titreLivre, titreLivreCourt, titreChapitre, content.ToString(), pageSuivante, url);
        }

        //public static Chapitre LightNovelPub(string url)
        //{
        //    new DriverManager().SetUpDriver(new EdgeConfig());
        //    IWebDriver driver = new EdgeDriver();
        //    driver.Navigate().GoToUrl(url);
        //    Console.WriteLine(driver.Title);
        //    driver.Quit();

        //    return new Chapitre(null, null, null, null, null, null);
        //}





    }
}