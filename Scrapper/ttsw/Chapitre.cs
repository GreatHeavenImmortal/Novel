using System;

namespace tts_wuxia
{
    public interface IChapitre
    {
        public string TitreLivre { get; set; }
        public string TitreLivreCourt { get; set; }
        public string TitreChapitre { get; set; }
        public string Content { get; set; }
        public string PageSuivante { get; set; }
        public string DateEnregistrement { get; set; }
        public string Url { get; set; }

        public string ToString();
  
    }

    public class Chapitre : IChapitre
    {
        public string TitreLivre { get; set; }
        public string TitreLivreCourt { get; set; }
        public string TitreChapitre { get; set; }
        public string Content { get; set; }
        public string PageSuivante { get; set; }
        public string DateEnregistrement {get; set;} =  DateTime.Now.ToString("dd/MM/yyyy");
        public string Url {get; set;}

        public Chapitre(string titreLivre, string titreLivreCourt, string titreChapitre, string content, string PageSuivant, string url)
        {
            TitreLivre = titreLivre;
            TitreLivreCourt = titreLivreCourt;
            TitreChapitre = titreChapitre;
            Content = content;
            PageSuivante = PageSuivant;
            Url = url;
        }

        public override string ToString()
        {
            return string.Format("{0,-11}{1,-50}{2,-21}{3,-50}{4}", DateEnregistrement, TitreLivre, TitreLivreCourt, TitreChapitre,  Url);
            
        }
    }
}
