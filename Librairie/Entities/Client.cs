using System;
using System.Collections.Generic;

namespace Librairie.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
        public string NomUtilisateur { get; set; }
        public Dictionary<Guid, int> ListeLivreAchete { get; set; }

        public Client()
        {
            ListeLivreAchete = new Dictionary<Guid, int>();
        }
    }
}
