using Librairie.Entities;
using System;

namespace Librairie.Services.Interfaces
{
    public interface IServiceBD
    {       
        Client ObtenirClient(Guid ID);
        Client ObtenirClient(string nomClient);
        void AjouterClient(Client client);
        void ModifierClient(Client client);


        Livre ObtenirLivre(Guid idLivre);
        void ModifierLivre(Livre livre);

    }
}
