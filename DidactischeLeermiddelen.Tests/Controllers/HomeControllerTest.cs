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
using moq;

namespace DidactischeLeermiddelen.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        private CatalogusController controller;
        private Mock<IMateriaalRepository> mockMateriaalRepository;
        private Mock<IDoelgroepRepository> mockDoelgroepRepository;
        private Mock<ILeergebiedRepository> mockLeergebiedRepository;
        MaterialenViewModel vm;
        Verlanglijst v=new Verlanglijst();

        [TestInitialize]
        public void OpzettenContext()
        {
            DummyContext context=new DummyContext();
            mockMateriaalRepository=new Mock<IMateriaalRepository>();
            mockDoelgroepRepository=new Mock<IDoelgroepRepository>();
            mockLeergebiedRepository=new Mock<ILeergebiedRepository>();
            controller=new CatalogusController(mockMateriaalRepository.Object,mockDoelgroepRepository.Object,mockLeergebiedRepository.Object);
            MateriaalViewModel m=new MateriaalViewModel(context.Bol);
            List<MateriaalViewModel> models=new List<MateriaalViewModel>();
            models.Add(m);


            vm = new MaterialenViewModel()
            {
                Materialen = models

            };

        }

        [TestMethod]
        public void IndexMethodeGeeftCatalogusWeer()
        {
            ViewResult result= controller.Index() as ViewResult;
            MaterialenViewModel vm=((MaterialenViewModel)result.Model);
            Assert.AreEqual("Wereldbol",vm.Materialen.FirstOrDefault().Naam);

        }

        [TestMethod]
        public void VoegToeAanVerlanglijstKeertTerugNaarIndexNaToevoegen()
        {
            MateriaalViewModel m = vm.Materialen.FirstOrDefault();
            RedirectToRouteResult res=controller.VoegAanVerlanglijstToe(m.ArtikelNr,m.AantalInCatalogus,v) as RedirectToRouteResult;
            Assert.AreEqual("Index",res.RouteValues["action"]);
        }
    }
}
