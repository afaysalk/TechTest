using Librairie.Entities;
using System;

namespace Librairie.Services.Interfaces
{
    public interface IServiceClient
    {
        Client CreerClient(string nomClient);
        void RenommerClient(Guid clientId, string nouveauNomClient);
    }
}
