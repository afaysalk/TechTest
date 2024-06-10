using Librairie.Entities;
using Librairie.Services;
using Librairie.Services.Interfaces;
using Moq;
using System;
using Xunit;

namespace LibrairieTest
{
    public class LivreServiceTests
    {
        private readonly Mock<IServiceBD> _serviceBDMock;
        private readonly LivreService _livreService;

        public LivreServiceTests()
        {
            _serviceBDMock = new Mock<IServiceBD>();
            _livreService = new LivreService(_serviceBDMock.Object);
        }

        [Fact]
        public void AcheterLivre_ShouldThrowException_WhenClientDoesNotExist() //Valider que le client existe
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns((Client)null);
            Assert.Throws<InvalidOperationException>(() => _livreService.AcheterLivre(Guid.NewGuid(), Guid.NewGuid(), 100));
        }

        [Fact]
        public void AcheterLivre_ShouldThrowException_WhenMontantIsZeroOrLess() //Valider que le montant est sup�rieur � 0
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(new Client());
            Assert.Throws<ArgumentException>(() => _livreService.AcheterLivre(Guid.NewGuid(), Guid.NewGuid(), 0));
        }

        [Fact]
        public void AcheterLivre_ShouldThrowException_WhenLivreDoesNotExist() //Valider que le livre existe
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(new Client());
            _serviceBDMock.Setup(s => s.ObtenirLivre(It.IsAny<Guid>())).Returns((Livre)null);
            Assert.Throws<InvalidOperationException>(() => _livreService.AcheterLivre(Guid.NewGuid(), Guid.NewGuid(), 100));
        }

        [Fact]
        public void AcheterLivre_ShouldThrowException_WhenLivreIsNotAvailable() //V�rifier qu'il reste au moins un exemplaire
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(new Client());
            _serviceBDMock.Setup(s => s.ObtenirLivre(It.IsAny<Guid>())).Returns(new Livre { Quantite = 0 });
            Assert.Throws<InvalidOperationException>(() => _livreService.AcheterLivre(Guid.NewGuid(), Guid.NewGuid(), 100));
        }

        [Fact]
        public void AcheterLivre_ShouldThrowException_WhenMontantIsInsufficient() //Valider que le montant est �gal ou sup�rieur � la valeur du livre
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(new Client());
            _serviceBDMock.Setup(s => s.ObtenirLivre(It.IsAny<Guid>())).Returns(new Livre { Quantite = 1, Valeur = 100 });
            Assert.Throws<InvalidOperationException>(() => _livreService.AcheterLivre(Guid.NewGuid(), Guid.NewGuid(), 50));
        }

        [Fact]
        public void AcheterLivre_ShouldReduceLivreQuantiteAndAddToClient_WhenValid() //R�duire la quantit� d'exemplaires du livre disponibles � la vente
        {
            var client = new Client();
            var livreId = Guid.NewGuid();
            var livre = new Livre { Id = livreId, Quantite = 1, Valeur = 100 };

            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            _serviceBDMock.Setup(s => s.ObtenirLivre(It.IsAny<Guid>())).Returns(livre);

            var montantRestant = _livreService.AcheterLivre(Guid.NewGuid(), livreId, 150);

            Assert.Equal(50, montantRestant);
            Assert.Equal(0, livre.Quantite);
            Assert.Equal(1, client.ListeLivreAchete[livreId]);
            _serviceBDMock.Verify(s => s.ModifierLivre(livre), Times.Once);
            _serviceBDMock.Verify(s => s.ModifierClient(client), Times.Once);
        }

        [Fact]
        public void RembourserLivre_ShouldThrowException_WhenClientDoesNotExist() //Valider que le client existe
        {
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns((Client)null);
            Assert.Throws<InvalidOperationException>(() => _livreService.RembourserLivre(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public void RembourserLivre_ShouldThrowException_WhenClientHasNotBoughtLivre()  //Valider que le client a d�j� achet� au moins un exemplaire du livre
        {
            var client = new Client();
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            Assert.Throws<InvalidOperationException>(() => _livreService.RembourserLivre(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public void RembourserLivre_ShouldThrowException_WhenLivreDoesNotExist()  //Valider que le livre existe
        {
            var client = new Client();
            client.ListeLivreAchete[Guid.NewGuid()] = 1;
            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            _serviceBDMock.Setup(s => s.ObtenirLivre(It.IsAny<Guid>())).Returns((Livre)null);
            Assert.Throws<InvalidOperationException>(() => _livreService.RembourserLivre(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public void RembourserLivre_ShouldIncreaseLivreQuantiteAndRemoveFromClient_WhenValid()  //Ajuster la quantit� d'exemplaires du livre disponibles � la vente
        {
            var client = new Client();
            var livreId = Guid.NewGuid();
            client.ListeLivreAchete[livreId] = 1;
            var livre = new Livre { Id = livreId, Quantite = 0, Valeur = 100 };

            _serviceBDMock.Setup(s => s.ObtenirClient(It.IsAny<Guid>())).Returns(client);
            _serviceBDMock.Setup(s => s.ObtenirLivre(It.IsAny<Guid>())).Returns(livre);

            var montantRembourse = _livreService.RembourserLivre(Guid.NewGuid(), livreId);

            Assert.Equal(100, montantRembourse);
            Assert.Equal(1, livre.Quantite);
            Assert.False(client.ListeLivreAchete.ContainsKey(livreId));
            _serviceBDMock.Verify(s => s.ModifierLivre(livre), Times.Once);
            _serviceBDMock.Verify(s => s.ModifierClient(client), Times.Once);
        }
    }
}