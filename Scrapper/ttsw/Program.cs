using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace tts_wuxia
{
    class Program
    {

    private static StringBuilder output = new StringBuilder();
        static void Main(string[] args)
        {
            AllowedSites.Add("boxnovel.com");
            AllowedSites.Add("boxnovel.org");
            AllowedSites.Add("novelfull.com");
            AllowedSites.Add("novelhall.com");
            AllowedSites.Add("www.lightnovelpub.com");
            AllowedSites.Add("www.readwn.com");
            AllowedSites.Add("www.scribblehub.com");
            AllowedSites.Add("www.wuxiaworld.co");
            AllowedSites.Add("www.wuxiaworld.com");
            AllowedSites.Add("galaxytranslations97.com");
            AllowedSites.Add("www.royalroad.com");
            AllowedSites.Add("novelhi.com");
            AllowedSites.Add("comrademao.com");
            AllowedSites.Add("novelfullchapter.com");
            AllowedSites.Add("novelbin.net");
            AllowedSites.Add("novelbin.com");
            AllowedSites.Add("novelusb.com");
            AllowedSites.Add("ranobes");
            AllowedSites.Add("wtr-lab.com");
            AllowedSites.Add("novelmax.net");
            AllowedSites.Add("novelmedium.com");
            //AllowedSites.Site.Add("www.xbiquge.cc");

            ParsingMethods.ParsingMethodsCollection.Add("boxnovel.com", ParsingMethods.BoxnovelCom);
            ParsingMethods.ParsingMethodsCollection.Add("boxnovel.org", ParsingMethods.BoxnovelOrg);
            ParsingMethods.ParsingMethodsCollection.Add("novelfull.com", ParsingMethods.NovelFullCom);
            ParsingMethods.ParsingMethodsCollection.Add("novelhall.com", ParsingMethods.NovelHallCom);
            ParsingMethods.ParsingMethodsCollection.Add("www.scribblehub.com", ParsingMethods.Scribblehub);
            ParsingMethods.ParsingMethodsCollection.Add("www.lightnovelpub.com", ParsingMethods.LightNovelPub);
            ParsingMethods.ParsingMethodsCollection.Add("www.readwn.com", ParsingMethods.ReadWn);
            ParsingMethods.ParsingMethodsCollection.Add("www.wuxiaworld.co", ParsingMethods.WuxiaworldCo);
            ParsingMethods.ParsingMethodsCollection.Add("www.wuxiaworld.com", ParsingMethods.WuxiaworldCom);
            ParsingMethods.ParsingMethodsCollection.Add("galaxytranslations97.com", ParsingMethods.Galaxytranslations97Com);
            ParsingMethods.ParsingMethodsCollection.Add("www.royalroad.com", ParsingMethods.RoyalroadCom);
            ParsingMethods.ParsingMethodsCollection.Add("novelhi.com", ParsingMethods.NovelhiCom);
            ParsingMethods.ParsingMethodsCollection.Add("comrademao.com", ParsingMethods.ComrademaoCom);
            ParsingMethods.ParsingMethodsCollection.Add("novelfullchapter.com", ParsingMethods.Novelfullchaptercom);
            ParsingMethods.ParsingMethodsCollection.Add("novelbin.net", ParsingMethods.Novelbinnet);
            ParsingMethods.ParsingMethodsCollection.Add("novelmax.net", ParsingMethods.Novelbinnet);
            ParsingMethods.ParsingMethodsCollection.Add("novelusb.com", ParsingMethods.Novelbinnet);
            ParsingMethods.ParsingMethodsCollection.Add("novelbin.com", ParsingMethods.Novelbinnet);
            ParsingMethods.ParsingMethodsCollection.Add("ranobes", ParsingMethods.Ranobes);
            ParsingMethods.ParsingMethodsCollection.Add("wtr-lab.com", ParsingMethods.Wtr);
            ParsingMethods.ParsingMethodsCollection.Add("novelmedium.com", ParsingMethods.NovelMedium);


            List<string> helpArgument = new List<string> { "-?", "-aide", "-help", "-a", "-h", "-?", };
            
            /*
            DirectoryInfo wd = new DirectoryInfo(Directory.GetCurrentDirectory());
            
            ProcessStartInfo say = new ProcessStartInfo();
            ProcessStartInfo lame = new ProcessStartInfo();


            say.WorkingDirectory = Directory.GetCurrentDirectory();
            say.Arguments = "-v Alex -f f.txt -o fichier.aiff";
            say.FileName = @"/usr/bin/say";
            say.UseShellExecute = false;
            say.RedirectStandardOutput = true;
            say.RedirectStandardError = true;

            lame.WorkingDirectory = Directory.GetCurrentDirectory();
            lame.Arguments = "-q0 fichier.aiff basename.mp3";
            lame.FileName = @"/usr/local/bin/lame";
            lame.UseShellExecute = false;
            lame.RedirectStandardOutput = true;
            lame.RedirectStandardError = true;


            CancellationTokenSource ctsSay = new CancellationTokenSource();
            CancellationTokenSource ctsLame = new CancellationTokenSource();
            Action<string, CancellationToken> waitter = (source, token) => {
                string msg ;
                while(token.IsCancellationRequested == false) 
                {
                    msg = $"{source}: ***";
                    Console.Write(msg);
                    Thread.Sleep(2000);
                    
                    Console.Clear();                    
                    msg = $"{source}:";
                    Console.Write(msg);
                    Thread.Sleep(2000);
                    Console.Clear();

                }
                Console.WriteLine("cancelation toker call");
                Thread.Sleep(5000);
            };

            Process psay = new Process();
            Process plame = new Process();            

            psay.StartInfo = say;
            plame.StartInfo = lame;

            psay.EnableRaisingEvents = true;
            plame.EnableRaisingEvents = true;

            psay.Exited += new EventHandler((object o, EventArgs ee) => { Console.WriteLine("say exited") ; ctsSay.Cancel(); });
            plame.Exited += new EventHandler((object o, EventArgs ee) => { Console.WriteLine("lame exited") ; ctsLame.Cancel();});
            
            psay.Disposed += new EventHandler((object o, EventArgs ee) => Console.WriteLine("say disposed"));            
            plame.Disposed += new EventHandler((object o, EventArgs ee) => Console.WriteLine("lame disposed"));            

            

            DataReceivedEventHandler myHander = (object sender, DataReceivedEventArgs e) => 
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                    lineCount++;
                    output.Append($"{sender.ToString(),-8}  \n[" + lineCount + "]: " + e.Data);
                    }
                };

            
            psay.ErrorDataReceived  += myHander;
            psay.OutputDataReceived += myHander;        
            plame.ErrorDataReceived  += myHander; 
            plame.OutputDataReceived += myHander;


            
            
            
            System.Console.WriteLine("*******say ****");           
            psay.Start();
            Task.Run(() => waitter("say", ctsSay.Token));
            //psay.BeginOutputReadLine();
            psay.BeginErrorReadLine();
            psay.WaitForExit();    
            System.Console.WriteLine(output.ToString()); 


            System.Console.WriteLine("*******lame****");                       
            plame.Start();
            Task.Run(() => waitter("Lame", ctsLame.Token), ctsLame.Token);            
            plame.BeginOutputReadLine();
            plame.BeginErrorReadLine();       
            plame.WaitForExit();
            
            

            Environment.Exit(0);
*/
        

            string retour = string.Empty;

            while (true)
            {
                if (args.Length > 0)
                {
                    bool helpNeeded = !string.IsNullOrEmpty(helpArgument.Find(x => x.Contains(args[0].Trim())));
                    if (helpNeeded == true)
                    {
                        Console.Clear();
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("_____HELP WUXIA TELECHARGEUR_____");
                        sb.AppendLine("Il existe deux moyens d'utiliser l'outil");
                        sb.AppendLine("1 - dotnet tts_wuxia.dll https://www.site.com/mon-roman [debut] [nombre de chapitre à télécharger] [taille du recueil]");
                        sb.AppendLine("2 - dotnet tts_wuxia.dll");
                        sb.AppendLine("");
                        sb.AppendLine("");
                        sb.AppendLine("Liste des sites à rip");
                        foreach (var el in AllowedSites.GetSites)
                        {
                            sb.AppendLine(el.ToString());
                        }
                        Console.WriteLine(sb.ToString());
                        Environment.Exit(0);
                    }
                    else
                    {
                        try
                        {
                            Engine.Run(String.Join(',', args), Parsing.ParseCommandArgument, Parsing.SelectParser);
                            Console.WriteLine("Fin du programme");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Fin du programme");
                        }
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("_____WUXIA TELECHARGEUR_____");
                    Console.WriteLine("1 - Télécharger un roman");
                    Console.WriteLine("2 - List de roman télécharger");
                    retour = Console.ReadLine();
                    if (retour == "1" || retour == "2")
                    {
                        string command = Menu.CreateMenu(retour);
                        if (retour == "2")
                        {
                            Console.Clear();
                            Console.WriteLine(command + "\n\n");
                            Console.WriteLine("Press any key to quit");
                            Console.ReadLine();
                        }
                        else
                        {
                            Engine.Run(command, Parsing.ParseCommandArgument, Parsing.SelectParser);
                        }
                    }
                    else if (retour.ToLower() == "quit" || retour.ToLower() == "exit")
                    {
                        Console.WriteLine("Fin du programme");
                        Environment.Exit(0);
                    }
                }
            }
        }
    }

    class NotInAllowedSiteException : Exception
    {
        public NotInAllowedSiteException(string message) : base(message) { }
        public NotInAllowedSiteException(string message, Exception inner) : base(message, inner) { }
        public NotInAllowedSiteException() { }
    }

    static class AllowedSites
    {
        private static List<string> _site = new List<string>();
        private static bool isSort = false;
        public static List<string> GetSites
        {
            get
            {
                if (isSort == false)
                {
                    _site.Sort();
                    isSort = true;
                }
                return _site;
            }
        }
        public static void Add(string nomSite)
        {
            if (!_site.Contains(nomSite))
                _site.Add(nomSite);
        }
    }

}   