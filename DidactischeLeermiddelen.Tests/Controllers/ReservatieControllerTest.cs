using System;
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
        private DummyContext context;
        private int[] materialen;
        private int[] aantal;
        private int[] aantal2;
        private int[] aantal3;
        private string[] dagen;
        private int[] materialen2;
        [TestInitialize]
        public void OpzettenContext()
        {
            context = new DummyContext();
            gebruiker = context.Toon;
            mockMateriaalRepository = new Mock<IMateriaalRepository>();
            mockGebruikerRepository = new Mock<IGebruikerRepository>();
            mockReservatieRepository = new Mock<IReservatieRepository>();
            mailServiceRepository = new Mock<IMailServiceRepository>();
            mailServiceRepository.Setup(t => t.GeefMailTemplate("Bevestiging reservatie")).Returns(context.mailStudent);
            mockMateriaalRepository.Setup(t => t.FindAll()).Returns(context.Materialen);
            m = context.Encyclopedie;
            materialen = new[] {1};
            materialen2 = new[] {1,3};
            aantal = new[] {10};
            aantal2 = new[] {5};
            aantal3 = new[] {5, 5};
            dagen=new []{"25/03/2016"};
            mockMateriaalRepository.Setup(t => t.FindById(1)).Returns(context.Bol);
            mockMateriaalRepository.Setup(t => t.FindById(2)).Returns(context.Kaart);
            controller = new ReservatieController(mockMateriaalRepository.Object, mockGebruikerRepository.Object, mockReservatieRepository.Object,mailServiceRepository.Object);
        }

        //[TestMethod]
        //public void IndexGeeftViewmodelTerugWanneerMateriaalInVerlanglijst()
        //{
        //    gebruiker.Verlanglijst.VoegMateriaalToe(m);
        //    ViewResult result = controller.Index(gebruiker) as ViewResult;
        //    ReservatieMaterialenViewModel vm = result.Model as ReservatieMaterialenViewModel;
        //    Assert.AreEqual(1 , vm.Materialen.Count());
        //}

        [TestMethod]
        public void IndexGeeftLegeReservatieLijstViewTerugWanneerGeenMaterialenInVerlanglijst()
        {
            ViewResult result = controller.Index(gebruiker) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("LegeReservatielijst", result.ViewName);
        }

        //[TestMethod]

        //public void BlokkerenVoegtOverruledeReservatieToeInKlasseLector() //test faalt als je mails niet in commentaar zet (komt door smtpclient)
        //{
        //    controller.MaakReservatie(context.Manu, materialen, aantal, "25/03/2016", dagen);
        //    controller.MaakReservatie(context.LectorGebruiker, materialen, aantal, "25/03/2016", dagen);
            
        //    Assert.AreEqual(1,context.LectorGebruiker.OverruledeReservaties.Count);

        //}

        //[TestMethod]

        //public void BlokkerenVoegtGeenOverruledeReservatieToeInKlasseLectorAlsNogGenoegAantal() //test faalt als je mails niet in commentaar zet (komt door smtpclient)
        //{
        //    controller.MaakReservatie(context.Manu, materialen, aantal2, "25/03/2016", dagen);
        //    controller.MaakReservatie(context.LectorGebruiker, materialen, aantal2, "25/03/2016", dagen);

        //    Assert.AreEqual(0, context.LectorGebruiker.OverruledeReservaties.Count);

        //}

        //[TestMethod]

        //public void NaReserverenStudentReservatiesVerhoogd()
        //{
        //    controller.MaakReservatie(context.Manu, materialen2, aantal3, "25/03/2016", dagen);
        //    Assert.AreEqual(2,context.Manu.Reservaties.Count);
        //}


        //[TestMethod]
        //public void LectorBlokkeertMateriaalVerhoogtReservatiesLector()
        //{
        //    controller.MaakReservatie(context.LectorGebruiker, materialen2, aantal3, "25/03/2016", dagen);
        //    Assert.AreEqual(2,context.LectorGebruiker.Reservaties.Count);
        //}

        //[TestMethod]

        //public void LectorBlokkeertDeelReservatieStudentMaaktNieuweReservatieAan()
        //{
        //    int[] aantal = new[] {8};
        //    controller.MaakReservatie(context.Manu, materialen, aantal, "25/03/2016", dagen);
        //    controller.MaakReservatie(context.LectorGebruiker, materialen, aantal2, "25/03/2016", dagen);
        //    Assert.AreEqual(2,context.Manu.Reservaties.Count);
        //}




    }
}
