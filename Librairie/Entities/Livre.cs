using System;

namespace Librairie.Entities
{
    public class Livre
    {
        public Guid Id { get; set; }
        public int Quantite { get; set; }
        public decimal Valeur { get; set; }
    }
}
