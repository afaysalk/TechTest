using System;

namespace Librairie.Services.Interfaces
{
    public interface IServiceLivre
    {
        decimal AcheterLivre(Guid IdClient, Guid IdLivre, decimal montant);

        decimal RembourserLivre(Guid IdClient, Guid idLivre);
    }
}
