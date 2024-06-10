using Librairie.Entities;
using Librairie.Services.Interfaces;
using System;

namespace Librairie.Services
{
    public class ClientService : IServiceClient
    {
        private readonly IServiceBD _serviceBD;

        public ClientService(IServiceBD serviceBD)
        {
            _serviceBD = serviceBD;
        }

        public Client CreerClient(string nomClient)
        {
            if (string.IsNullOrWhiteSpace(nomClient))
                throw new ArgumentException("Le nom d'utilisateur doit �tre renseign�.");

            if (_serviceBD.ObtenirClient(nomClient) != null)
                throw new InvalidOperationException("Le nom d'utilisateur est d�j� utilis�.");

            var client = new Client { Id = Guid.NewGuid(), NomUtilisateur = nomClient };
            _serviceBD.AjouterClient(client);  //Ajouter le client
            return client;
        }

        public void RenommerClient(Guid clientId, string nouveauNomClient)
        {
            if (string.IsNullOrWhiteSpace(nouveauNomClient))
                throw new ArgumentException("Le nom d'utilisateur doit �tre renseign�.");

            var client = _serviceBD.ObtenirClient(clientId);
            if (client == null)
                throw new InvalidOperationException("Le client n'existe pas.");

            if (client.NomUtilisateur == nouveauNomClient)
                throw new InvalidOperationException("Le nom d'utilisateur n'a pas chang�.");

            if (_serviceBD.ObtenirClient(nouveauNomClient) != null)
                throw new InvalidOperationException("Le nom d'utilisateur est d�j� utilis�.");

            client.NomUtilisateur = nouveauNomClient;  //Modifier le nom de l'utilisateur
            _serviceBD.ModifierClient(client);
        }
    }
}