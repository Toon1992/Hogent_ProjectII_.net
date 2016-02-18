//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using DidactischeLeermiddelen;
//using DidactischeLeermiddelen.Controllers;
//using DidactischeLeermiddelen.Models.Domain;
//using DidactischeLeermiddelen.Tests.Domain;
//using DidactischeLeermiddelen.ViewModels;
//using Moq;

//namespace DidactischeLeermiddelen.Tests.Controllers
//{
//    [TestClass]
//    public class HomeControllerTest
//    {

//        private CatalogusController controller;
//        private Mock<IMateriaalRepository> mockMateriaalRepository;
//        private Mock<IDoelgroepRepository> mockDoelgroepRepository;
//        private Mock<ILeergebiedRepository> mockLeergebiedRepository;
//        private Mock<IGebruikerRepository> mockGebruikersRepository;
//        private DummyContext context;
//        private Gebruiker gebruiker;
//        private Verlanglijst v;

//        [TestInitialize]
//        public void OpzettenContext()
//        {
//            context = new DummyContext();
//            mockMateriaalRepository = new Mock<IMateriaalRepository>();
//            mockDoelgroepRepository = new Mock<IDoelgroepRepository>();
//            mockLeergebiedRepository = new Mock<ILeergebiedRepository>();
//            mockGebruikersRepository = new Mock<IGebruikerRepository>();
//            mockMateriaalRepository.Setup(p => p.FindAll()).Returns(context.Materialen);
//            mockDoelgroepRepository.Setup(p => p.FindAll()).Returns(context.Doelgroepen);
//            mockLeergebiedRepository.Setup(p => p.FindAll()).Returns(context.Leergebieden);
//            mockGebruikersRepository.Setup(p => p.FindByName("student")).Returns(context.Student);
//            controller = new CatalogusController(mockMateriaalRepository.Object, mockDoelgroepRepository.Object, mockLeergebiedRepository.Object, mockGebruikersRepository.Object);
//            MateriaalViewModel m = new MateriaalViewModel(context.Bol);
//            List<MateriaalViewModel> models = new List<MateriaalViewModel>();
//            models.Add(m);
//            v = new Verlanglijst();
//            v.VoegMateriaalToe(context.Bol);
//            gebruiker = context.Toon;

//        }

//        [TestMethod]
//        public void IndexMethodeGeeftCatalogusWeer()
//        {
//            ViewResult result = controller.Index() as ViewResult;
//            MaterialenViewModel vm = ((MaterialenViewModel)result.Model);
//            Assert.AreEqual("Wereldbol", vm.Materialen.FirstOrDefault().Naam);

//        }

//        [TestMethod]
//        public void VoegToeAanVerlanglijstKeertTerugNaarIndexNaToevoegen()
//        {
//            MateriaalViewModel m = vm.Materialen.FirstOrDefault();
//            RedirectToRouteResult res = controller.VoegAanVerlanglijstToe(m.ArtikelNr, context.Toon) as RedirectToRouteResult;
//            Assert.AreEqual("Index", res.RouteValues["action"]);
//        }
//    }
//}