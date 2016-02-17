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
        [TestInitialize]
        public void OpzettenContext()
        {
            DummyContext context = new DummyContext();
            gebruiker = context.Toon;
            mockMateriaalRepository = new Mock<IMateriaalRepository>();
            mockDoelgroepRepository=new Mock<IDoelgroepRepository>();
            mockLeergebiedRepository=new Mock<ILeergebiedRepository>();
            mockGebruikerRepository=new Mock<IGebruikerRepository>();
            controller = new CatalogusController(mockMateriaalRepository.Object,mockDoelgroepRepository.Object,mockLeergebiedRepository.Object,mockGebruikerRepository.Object);

        }

        [TestMethod]
        public void IndexMethodeGeeftCatalogusWeer()
        {
            
            ViewResult result = controller.Index(gebruiker) as ViewResult;
            MaterialenViewModel vm = result.Model as MaterialenViewModel;
            Assert.AreEqual(5, vm.Materialen.Count());

        }

    }
}



