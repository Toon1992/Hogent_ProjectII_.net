using System.Linq;
using System.Web.Mvc;
using DidactischeLeermiddelen.Controllers;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Models.Domain.InterfaceRepositories;
using DidactischeLeermiddelen.Tests.Domain;
using DidactischeLeermiddelen.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DidactischeLeermiddelen.Tests.Controllers
{
    [TestClass]
    public class ReservatieControllerTest
    {
        private Mock<IMateriaalRepository> mockMateriaalRepository;
        private Mock<IGebruikerRepository> mockGebruikerRepository;
        private Mock<IReservatieRepository> mockReservatieRepository;
        private Mock<IMailServiceRepository> mailServiceRepository;
        private ReservatieController controller;
        private Gebruiker gebruiker;
        private Materiaal m;

        [TestInitialize]
        public void OpzettenContext()
        {
            DummyContext context = new DummyContext();
            gebruiker = context.Toon;
            mockMateriaalRepository = new Mock<IMateriaalRepository>();
            mockGebruikerRepository = new Mock<IGebruikerRepository>();
            mockReservatieRepository = new Mock<IReservatieRepository>();
            mailServiceRepository = new Mock<IMailServiceRepository>();
            mockMateriaalRepository.Setup(t => t.FindAll()).Returns(context.Materialen);
            m = context.Encyclopedie;

            controller = new ReservatieController(mockMateriaalRepository.Object, mockGebruikerRepository.Object, mockReservatieRepository.Object,mailServiceRepository.Object);
        }

        [TestMethod]
        public void IndexGeeftViewmodelTerugWanneerMateriaalInVerlanglijst()
        {
            gebruiker.Verlanglijst.VoegMateriaalToe(m);
            ViewResult result = controller.Index(gebruiker) as ViewResult;
            ReservatieMaterialenViewModel vm = result.Model as ReservatieMaterialenViewModel;
            Assert.AreEqual(1 , vm.Materialen.Count());
        }

        [TestMethod]
        public void IndexGeeftLegeReservatieLijstViewTerugWanneerGeenMaterialenInVerlanglijst()
        {
            ViewResult result = controller.Index(gebruiker) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("LegeReservatielijst", result.ViewName);
        }
    }
}
