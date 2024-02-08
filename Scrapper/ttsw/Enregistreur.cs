using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace tts_wuxia
{    
    static class Enregistreur
    {
        //private static string Location = Environment.CurrentDirectory;
        private static string Location() => (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"/Users/{Environment.UserName}/novels" : Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));

        public static void Sauvegarde(string content, string titreDossier,string titre, string path = null)
        {
            if(path is null)
            {
                path = Path.Join(Location(),titreDossier);
            }
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            titre = titre.Trim().Trim(new char[] { '/', ':', '?', ';', '(', ')' });
            foreach (var c in new char[] { '/', ':', '?', ';', '(', ')' })
            {
                titre = titre.Replace(c, ' ').Replace("  ", " ");

            }
            using (StreamWriter sw = File.CreateText(Path.Join(path, titre)))
            {
                sw.Write(content);
            }
            Console.WriteLine($"FICHIER CREER: {titre}");
            Console.WriteLine($"LIEUX: {Path.Join(path,titre)}");
        }

        public static void Sauvegarder(string content, string titre, string nomDossier)
        {
            string path = Path.Join(Location(), nomDossier);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var chars = new char[] { '/', ':', '?', ';', '(', ')','.','#','\'','"','\\' };
            titre = titre.Trim().Trim(chars);
            foreach( var c in chars)
            {
                titre = titre.Replace(c, ' ').Replace("  ", " ");
            }

            using (StreamWriter sw = File.CreateText($"{Path.Join(path, titre)}.txt"))
            {
                sw.Write(content);
            }
            Console.WriteLine($"FICHIER CREER: {titre}");
            Console.WriteLine($"LIEUX: {Path.Join(path, titre)}");
        }

        public static void Log(Chapitre chapitre, string path = null)
        {
            bool isFirstWrite = false;
            if (path is null)
            {
                path = Location();
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string file = Path.Join(path, "Log.txt");
            if (!File.Exists(file))
            {
                isFirstWrite = true;
            }

            using (StreamWriter sw = File.AppendText(file))
            {
                (string Livre, string Chapitre) item = ("Livre", "Chapitre");

                Dictionary<string, string> data = new Dictionary<string, string>() { { item.Livre , string.Empty }, { item.Chapitre , string.Empty } };

                int limit = 20;
                const int placeholderLength = -40;


                foreach((string label, string titre)e  in new List<(string, string)>{ (item.Livre, chapitre.TitreLivre), (item.Chapitre , chapitre.TitreChapitre) })
                {
                    string t = RaccourcirNom(e.titre.Trim(), limit, placeholderLength);
                    data[e.label] = t;
                }


                string donnee = string.Empty;
                if(isFirstWrite)
                {
                    donnee = $"{"DATE",placeholderLength}{"TITRE_LIVRE",placeholderLength}{"TITRE_CHAPITRE",placeholderLength}{"SOURCE_LIVRE",placeholderLength}";
                    sw.WriteLine(donnee);
                }
             
                donnee = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),placeholderLength}{data[item.Livre],placeholderLength}{data[item.Chapitre],placeholderLength}{chapitre.Url,placeholderLength}{chapitre.TitreLivreCourt,40}";
                sw.WriteLine(donnee);
            }
        }

        public static void Bdd(Chapitre chapitre, string path = null)
        {
            bool isFirstWrite = false;
            if (path is null){ path = Location(); }
            if (!Directory.Exists(path)){ Directory.CreateDirectory(path); }

            string file = Path.Join(path, "Log.txt");
            if (!File.Exists(file))
            {
                isFirstWrite = true;
            }

            using (StreamWriter sw = File.AppendText(Path.Join(path, "bdd.csv")))
            {
                string donnee = string.Empty;
                if (isFirstWrite)
                {
                    donnee = $"{"DATE"},{"TITRE_LIVRE"},{"TITRE_CHAPITRE"},{"SOURCE_LIVRE"}";
                    sw.WriteLine(donnee);
                }
                donnee = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},{chapitre.TitreLivre},{chapitre.TitreChapitre},{chapitre.Url}";
                sw.WriteLine(donnee);
            }
        }

        private static string RaccourcirNom(string titre, int limit, int placeholder)
        {
            placeholder = placeholder < 0 ? placeholder * -1 : placeholder;

            if(titre.Length > placeholder )
            {
                string ellipse = "... ";
                string t = titre.Replace('-', ' ').Replace('_', ' ').Substring(0, limit);
                t = t.Trim();
                int index = t.LastIndexOf(' ');
                t = t.Substring(0, index - 1);
                t = $"{t} {ellipse}";
                return RaccourcirNom(t, limit, placeholder);
            }
            else
            {
                return titre;
            }                        
        }
    }
}