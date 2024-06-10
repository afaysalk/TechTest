using System;
using System.Collections.Generic;

namespace Questions
{
    public class Question           //Injection Collaborateur via constructeur
    {
        private readonly Collaborateur _collaborateur;

        public Question(Collaborateur collaborateur)
        {
            _collaborateur = collaborateur;
        }

        public void Traiter(List<string> listeContenu)
        {
            var listeContenuValide = new List<string>();
            string message = null;
            bool estValide = true;

            foreach (var contenu in listeContenu)
            {
                if (estValide)
                {
                    estValide = Valider(contenu, out message);
                }

                if (estValide && !listeContenuValide.Contains(contenu))
                {
                    listeContenuValide.Add(contenu.Substring(0, 10));
                }
            }

            if (!estValide)
            {
                throw new Exception(message);
            }

            if (listeContenuValide.Count > 0)
            {
                listeContenuValide.ForEach(x => _collaborateur.AjouterContenuBD(x));
            }
        }

        private bool Valider(string contenu, out string message)  // check taille contenu et valeurs null/vide
        {
            message = null;
            if (string.IsNullOrEmpty(contenu))
            {
                message = "Le contenu ne peut être vide";
                return false;
            }

            if (contenu.Length > 10)
            {
                message = "Le contenu est trop long";
                return false;
            }

            return true;
        }
    }
}