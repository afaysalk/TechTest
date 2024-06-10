using Librairie.Entities;
using Librairie.Services.Interfaces;
using System;

namespace Librairie.Services
{
    public class LivreService : IServiceLivre
    {
        private readonly IServiceBD _serviceBD;

        public LivreService(IServiceBD serviceBD)
        {
            _serviceBD = serviceBD;
        }

        public decimal AcheterLivre(Guid IdClient, Guid IdLivre, decimal montant)
        {
            if (montant <= 0)
                throw new ArgumentException("Le montant doit être supérieur à 0.");

            var client = _serviceBD.ObtenirClient(IdClient);
            if (client == null)
                throw new InvalidOperationException("Le client n'existe pas.");

            var livre = _serviceBD.ObtenirLivre(IdLivre);
            if (livre == null)
                throw new InvalidOperationException("Le livre n'existe pas.");

            if (livre.Quantite <= 0)
                throw new InvalidOperationException("Le livre n'est plus disponible.");

            if (montant < livre.Valeur)
                throw new InvalidOperationException("Le montant est insuffisant.");

            livre.Quantite -= 1;
            _serviceBD.ModifierLivre(livre);

            if (!client.ListeLivreAchete.ContainsKey(IdLivre))
                client.ListeLivreAchete[IdLivre] = 0;
            client.ListeLivreAchete[IdLivre] += 1;   //Ajouter un exemplaire vendu du livre au client

            _serviceBD.ModifierClient(client);

            return montant - livre.Valeur;   //	Retourner le montant restant à la suite de l'achat

        }

        public decimal RembourserLivre(Guid IdClient, Guid IdLivre)
        {
            var client = _serviceBD.ObtenirClient(IdClient);
            if (client == null)
                throw new InvalidOperationException("Le client n'existe pas.");

            if (!client.ListeLivreAchete.ContainsKey(IdLivre) || client.ListeLivreAchete[IdLivre] <= 0)
                throw new InvalidOperationException("Le client n'a pas acheté ce livre.");

            var livre = _serviceBD.ObtenirLivre(IdLivre);
            if (livre == null)
                throw new InvalidOperationException("Le livre n'existe pas.");

            livre.Quantite += 1;
            _serviceBD.ModifierLivre(livre);

            client.ListeLivreAchete[IdLivre] -= 1; 
            if (client.ListeLivreAchete[IdLivre] == 0)
                client.ListeLivreAchete.Remove(IdLivre); //Supprimer un exemplaire vendu du livre au client

            _serviceBD.ModifierClient(client);

            return livre.Valeur; //Retourner la valeur du livre

        }
    }
}