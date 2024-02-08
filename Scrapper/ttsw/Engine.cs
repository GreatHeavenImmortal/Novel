using System;
using System.Collections.Generic;
using System.Text;

namespace tts_wuxia
{
    static class Engine
    {
        public static void Run(string argument,
            Func<string, (string, int, int, int)> ParsingCommand,
            Func<string, Func<string, Chapitre>> SelectParsingDocumentMethod)
        {
            string url = string.Empty;
            int debut = 0;
            int fin = 0;
            int taille = 0;
            string titre = string.Empty;
            StringBuilder content = new StringBuilder();
            Chapitre result = null;
            int i = 0;
            int lastEnregistrement = 0;
            try        
            {
                (url, debut, fin, taille) = ParsingCommand(argument);
                lastEnregistrement = debut;
                Console.WriteLine("----------");
                Console.WriteLine("Url de départ: " + url);
                Console.WriteLine("Début du chapitre: " + debut);
                Console.WriteLine("Nombre de chapitre à télécharger: " + fin);
                Console.WriteLine("Nombre de chapitre par fichier: " + taille);
                Console.WriteLine("----------");
                Func<string, Chapitre> ParsingDocumentMethod = SelectParsingDocumentMethod(argument);
                List<string> visitedUrl = new List<string>();
                for (i = 0; i <= fin; i++)
                {

                    Console.WriteLine($"Download chapitre {i + 1}");

                    result = ParsingDocumentMethod(url);
                    if (visitedUrl.Contains(result.Url) == false)
                    {
                        visitedUrl.Add(result.Url);
                        url = result.PageSuivante;
                    }
                    else
                    {
                        url = null;
                    }                                            
                    content.AppendLine(result.Content);
                    content.AppendLine("");

                    if (((i + 1) % taille == 0) || (i  == fin) || string.IsNullOrEmpty(url) )
                    {
                        titre = result.TitreLivre;
                    string documentASauvegarder = FonctionTexte.Nettoyage(content.ToString());
                    int numeroChapitre = debut + i;
                        Enregistreur.Sauvegarder(documentASauvegarder ,$"{result.TitreLivre}_{result.TitreChapitre}".Replace("--","-").Replace("__","_"),result.TitreLivre);
                        //Enregistreur.Sauvegarde(documentASauvegarder, titre, $"{titre}_ch_{lastEnregistrement}-{debut + i}.txt");
                        Enregistreur.Log(result);
                        Enregistreur.Bdd(result);
                        lastEnregistrement = debut + i + 1;
                        
                        if (((i + 1) % taille == 0))
                            content.Clear();
                    }

                    Console.Write($" Done {i + 1}/{fin} {result.TitreChapitre}");
                    if (url is null)
                        break;
                }
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Console.Clear();
            }
            catch(Exception ex)
            {
                titre = result?.TitreLivre;
                Console.WriteLine(ex.Message.PadRight((int)(Console.BufferWidth *0.5), '_'));
                Console.WriteLine("_".PadRight(Console.BufferWidth - 1));

                titre = result.TitreLivre;
                string documentASauvegarder = FonctionTexte.Nettoyage(content.ToString());
                int numeroChapitre = debut + i;
                Enregistreur.Sauvegarder(documentASauvegarder, $"{result.TitreLivre}_{result.TitreChapitre}".Replace("--", "-").Replace("__", "_"), result.TitreLivre);
                Enregistreur.Log(result);
                Enregistreur.Bdd(result);
                lastEnregistrement = debut + i + 1;

                Console.WriteLine("Cliquer sur touche pour quitter");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }
}