using System;
using System.Text;

namespace tts_wuxia
{
     static class Menu
    {
        public static string CreateMenu(string parametre)
        {
            Func<string> SelectionMenu =  parametre == "1" ? Menu.Telechargement : Menu.Listing;
            return SelectionMenu();
        }

        private static string Telechargement()
        {
            StringBuilder message = new StringBuilder();
            string command = string.Empty;

            message.AppendLine("_____WUXIA > MENU TELECHARGER_____");
            message.AppendLine("Entrez l'adresse url de roman que vous voulez télécharger: ");
            message.AppendLine("\t Exemple: https://www.site.com/mon-roman, [debut], [nombre de chapitre à télécharger], [taille du recueil]");
            Console.Clear();
            Console.WriteLine(message.ToString());
            command = Console.ReadLine();
            return command; 
        }

        private static string Listing()
        {
            StringBuilder sb =  new StringBuilder();
            int id = 1;
            AllowedSites.GetSites.ForEach(x => 
            {
                sb.AppendLine($"{id,-4}{x,3}");
                id++;
            });
            Console.WriteLine("Menu Listing");
            return sb.ToString();
        }
    }   
}