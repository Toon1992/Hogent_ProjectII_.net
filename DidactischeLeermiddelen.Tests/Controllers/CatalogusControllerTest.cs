using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DidactischeLeermiddelen;
using DidactischeLeermiddelen.Controllers;
using DidactischeLeermiddelen.Models.Domain;
using DidactischeLeermiddelen.Tests.Domain;
using DidactischeLeermiddelen.ViewModels;
using Moq;

namespace DidactischeLeermiddelen.Tests.Controllers
{
    [TestClass]
    public class CatalogusControllerTest
    {

        private CatalogusController controller;
        private Mock<IMateriaalRepository> mockMateriaalRepository;
        private Mock<IDoelgroepRepository> mockDoelgroepRepository;
        private Mock<ILeergebiedRepository> mockLeergebiedRepository;
        private Mock<IGebruikerRepository> mockGebruikerRepository;
        private Gebruiker gebruiker;
        private Materiaal m;
        [TestInitialize]
        public void OpzettenContext()
        {
            DummyContext context = new DummyContext();
            gebruiker = context.Toon;
            mockMateriaalRepository = new Mock<IMateriaalRepository>();
            mockDoelgroepRepository = new Mock<IDoelgroepRepository>();
            mockLeergebiedRepository = new Mock<ILeergebiedRepository>();
            mockGebruikerRepository = new Mock<IGebruikerRepository>();
            mockMateriaalRepository.Setup(t => t.FindAll()).Returns(context.Materialen);
            m = context.Encyclopedie;

            controller = new CatalogusController(mockMateriaalRepository.Object, mockDoelgroepRepository.Object, mockLeergebiedRepository.Object, mockGebruikerRepository.Object);

        }

        

        [TestMethod]
        public void VoegToeAanVerlanglijstVoegtToe()
        {
            controller.VoegAanVerlanglijstToe(m.MateriaalId, gebruiker);
            Assert.AreEqual(1, gebruiker.Verlanglijst.Materialen.Count);
            mockGebruikerRepository.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void VoegToeAanVerlanglijstRedirectoNaarIndexNaToevoegen()
        {
            RedirectToRouteResult result = controller.VoegAanVerlanglijstToe(m.MateriaalId, gebruiker) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void ToevoegenAanVerlanglijstGeeftMelding()
        {
            ViewResult res = controller.VoegAanVerlanglijstToe(m.MateriaalId, gebruiker) as ViewResult;
            controller.TempData.Add("key", "Info");
            Assert.IsTrue(controller.TempData.ContainsKey("Info"));
            mockGebruikerRepository.Verify(m => m.SaveChanges(), Times.Once);

        }

        [TestMethod]
        public void MateriaalToevoegenDatAlInVerlanglijstZitGeeftFoutmelding()
        {
            controller.VoegAanVerlanglijstToe(m.MateriaalId, gebruiker);
            controller.VoegAanVerlanglijstToe(m.MateriaalId, gebruiker);
            //controller.TempData.Add("key","message");
            Assert.IsTrue(controller.TempData.ContainsKey("Error"));
            Assert.AreEqual(1, gebruiker.Verlanglijst.Materialen.Count);
            mockGebruikerRepository.Verify(m => m.SaveChanges(), Times.Once);
        }



    }
}



