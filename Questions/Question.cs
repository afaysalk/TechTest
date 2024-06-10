using System;
using System.Collections.Generic;

namespace Questions
{
    // Améliorer le code de cette classe, ainsi que sa relation avec la classe Collaborateur.
    public class Question
    {
        public void Traiter(List<string> listeContenu)
        {
            List<string> listeContenuValide = null;
            string message = null;
            bool estValide = true;

            foreach (var contenu in listeContenu)
            {
                if (estValide)
                {
                    estValide = Valider(contenu, message);
                }              

                if (estValide && !listeContenuValide.Contains(contenu))
                {
                    listeContenuValide.Add(contenu.Substring(0,10));
                }
            }

            if (!estValide)
            {
                throw new Exception(message);
            }

            if(listeContenuValide.Count > 0)
            {
                listeContenuValide.ForEach(x => Collaborateur.AjouterContenuBD(x));
            }

        }

        private bool Valider(string contenu, string message)
        {
            bool estValide = true;
            if (contenu == "")
            {
                estValide = false;
                message = "Le contenu ne peut être vide";
            }

            if (estValide && contenu.Length > 10)
            {
                estValide = false;
                message = "Le contenu est trop long";
            }

            return estValide;
        }
    }
}
