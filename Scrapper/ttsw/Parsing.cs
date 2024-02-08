using System;

namespace tts_wuxia
{
    static class Parsing
    {
        public static (string Url, int Debut, int Fin, int Taille) ParseCommandArgument(string argument)
        {
            (string url, int debut, int fin, int taille) retour;
            string url = string.Empty;
            int debut = 0;
            int fin = 0;
            int taille = 0;
            int intValueConverted;

            string site = AllowedSites.GetSites.Find(x => x.ToLower() == argument.Split('/')[2].ToLower());
            if(string.IsNullOrEmpty(site))
            {
                site = AllowedSites.GetSites.Find(x => argument.ToLower().Contains(x));
            }

            if (string.IsNullOrEmpty(site))
            {
                throw new NotInAllowedSiteException($"{argument} : Error - not in allowed site");
            }

            string[] arguments = argument.Split(',');
            for (var i = 0; i < arguments.Length; i++)
            {
                if (i == 0)
                    url = arguments[0];
                else if (i == 1)
                    debut = Int32.TryParse(arguments[1], out intValueConverted) ? intValueConverted : 0;
                else if (i == 2)
                    fin = Int32.TryParse(arguments[2], out intValueConverted) ? intValueConverted : 0;
                else if (i == 3)
                    taille = Int32.TryParse(arguments[3], out intValueConverted) ? intValueConverted : 0;
            }
            retour.url = url;
            retour.debut = debut != 0 ? debut : 1;
            retour.fin = fin != 0 ? fin : 10;
            retour.taille = taille != 0 ? taille : 10;

            return retour;
        }
        
        public static Func<string, Chapitre> SelectParser(string argument)
        {
            string site = AllowedSites.GetSites.Find(x => x.ToLower() == argument.Split('/')[2].ToLower());

            if (string.IsNullOrEmpty(site))
            {
                site = AllowedSites.GetSites.Find(x => argument.ToLower().Contains(x.ToLower()));
            }

            if (string.IsNullOrEmpty(site))
            {
                throw new NotInAllowedSiteException($"{argument} : Error - not in allowed site");
            }
            return ParsingMethods.ParsingMethodsCollection[site];
        }
    }
}