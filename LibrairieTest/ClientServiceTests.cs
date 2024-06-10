
using Librairie.Entities;
using Librairie.Services;
using Librairie.Services.Interfaces;
using Moq;
using System;
using Xunit;

namespace LibrairieTest
{
    public class ClientServiceTests
    {
        private readonly Mock<IServiceBD> _serviceBDMock;
        private readonly ClientService _clientService;

        public ClientServiceTests()
        {
            _serviceBDMock = new Mock<IServiceBD>();
            _clientService = new ClientService(_serviceBDMock.Object);
        }

        [Fact]
        public void CreerClient_ShouldThrowException_WhenNomClientIsEmpty()  //Valider que le nom d'utilisateur est renseigné
        {
            Assert.Throws<ArgumentException>(() => _clientService.CreerClient(string.Empty));
        }

        [Fact]
        public void CreerClient_ShouldThrowException_WhenNomClientIsAlreadyUsed() //Vérifier que le nom n'est pas déjà utilisé par un autre client
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<string>())).Returns(new Client());
            Assert.Throws<InvalidOperationException>(() => _clientService.CreerClient("existingUser"));
        }

        [Fact]
        public void CreerClient_ShouldAddClient_WhenNomClientIsValid()  //Valider que le nom d'utilisateur est valide
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<string>())).Returns((Client)null);
            _clientService.CreerClient("newUser");
            _serviceBDMock.Verify(s => s.AjouterClient(It.IsAny<Client>()), Times.Once);
        }

        [Fact]
        public void RenommerClient_ShouldThrowException_WhenNouveauNomClientIsEmpty() // //Valider que le nom d'utilisateur est renseigné
        {
            Assert.Throws<ArgumentException>(() => _clientService.RenommerClient(Guid.NewGuid(), string.Empty));
        }

        [Fact]
        public void RenommerClient_ShouldThrowException_WhenClientDoesNotExist() //Valider que le client existe
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns((Client)null);
            Assert.Throws<InvalidOperationException>(() => _clientService.RenommerClient(Guid.NewGuid(), "newName"));
        }

        [Fact]
        public void RenommerClient_ShouldThrowException_WhenNomClientHasNotChanged()  //Vérifier que le nom a changé
        {
            var client = new Client { NomUtilisateur = "sameName" };
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            Assert.Throws<InvalidOperationException>(() => _clientService.RenommerClient(Guid.NewGuid(), "sameName"));
        }

        [Fact]
        public void RenommerClient_ShouldThrowException_WhenNouveauNomClientIsAlreadyUsed()  //vérifier que le nom n'est pas déjà utilisé par un autre client
        {
            var client = new Client { NomUtilisateur = "oldName" };
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            _serviceBDMock.Setup(s => s.ObtenirClient("newName")).Returns(new Client());
            Assert.Throws<InvalidOperationException>(() => _clientService.RenommerClient(Guid.NewGuid(), "newName"));
        }

        [Fact]
        public void RenommerClient_ShouldModifyClient_WhenNouveauNomClientIsValid()
        {
            var client = new Client { NomUtilisateur = "oldName" };
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            _serviceBDMock.Setup(s => s.ObtenirClient("newName")).Returns((Client)null);
            _clientService.RenommerClient(Guid.NewGuid(), "newName");
            _serviceBDMock.Verify(s => s.ModifierClient(It.IsAny<Client>()), Times.Once);
        }
    }
}